using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ct
{
    /// <summary>
    /// Data source reading trades from a ByBit public data file.
    /// </summary>
    public class ByBitPublicFileDataSource : DataSource
    {
        private readonly Stream _stream;

        public ByBitPublicFileDataSource(Stream stream)
        {
            _stream = stream;
        }

        public override async IAsyncEnumerable<Message> GetMessagesAsync()
        {
            using StreamReader reader = new StreamReader(_stream, Encoding.UTF8, leaveOpen: true);
            List<string> lines = new List<string>();

            string? line = await reader.ReadLineAsync();
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.Length > 0)
                {
                    lines.Add(line);
                }
            }

            if (lines.Count > 1)
            {
                string[] firstParts = lines[0].Split(',');
                string[] lastParts = lines[^1].Split(',');
                if (firstParts.Length > 0 && lastParts.Length > 0)
                {
                    double firstStamp = double.Parse(firstParts[0], CultureInfo.InvariantCulture);
                    double lastStamp = double.Parse(lastParts[0], CultureInfo.InvariantCulture);
                    if (firstStamp > lastStamp)
                    {
                        lines.Reverse();
                    }
                }
            }
            DateTime? previousStamp = null;
            Candle? candle = null;
            double lastPrice = 0.0;

            bool hasAggregate = false;
            DateTime aggregateStamp = DateTime.MinValue;
            TradeDirection aggregateDirection = TradeDirection.Long;
            double aggregateStartPrice = 0.0;
            double aggregateEndPrice = 0.0;
            double aggregateVolume = 0.0;
            int aggregateCount = 0;

            foreach (string record in lines)
            {
                string[] parts = record.Split(',');
                if (parts.Length < 5)
                {
                    continue;
                }

                double timestampSeconds = double.Parse(parts[0], CultureInfo.InvariantCulture);
                long milliseconds = (long)(timestampSeconds * 1000.0);
                DateTime stamp = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
                double price = double.Parse(parts[4], CultureInfo.InvariantCulture);
                double volume = double.Parse(parts[3], CultureInfo.InvariantCulture);
                TradeDirection direction = parts[2] == "Buy" ? TradeDirection.Long : TradeDirection.Short;

                DateTime minuteStamp = new DateTime(stamp.Year, stamp.Month, stamp.Day, stamp.Hour, stamp.Minute, 0, DateTimeKind.Utc);

                if (candle == null)
                {
                    candle = new Candle(minuteStamp, price, price, price, price, false, TimeFrame.Minutes1, 0.0);
                    candle.AddTrade(stamp, price, volume);
                    lastPrice = price;
                }
                else
                {
                    while (minuteStamp > candle.Stamp)
                    {
                        yield return new CandleMessage(candle);
                        lastPrice = candle.Close;
                        DateTime nextMinuteStamp = candle.Stamp.AddMinutes(1);
                        if (nextMinuteStamp < minuteStamp)
                        {
                            candle = new Candle(nextMinuteStamp, lastPrice, lastPrice, lastPrice, lastPrice, false, TimeFrame.Minutes1, 0.0);
                            yield return new CandleMessage(candle);
                            lastPrice = candle.Close;
                        }
                        else
                        {
                            candle = new Candle(nextMinuteStamp, lastPrice, lastPrice, lastPrice, lastPrice, false, TimeFrame.Minutes1, 0.0);
                        }
                    }

                    candle.AddTrade(stamp, price, volume);
                    lastPrice = price;
                }

                if (!hasAggregate)
                {
                    aggregateStamp = stamp;
                    aggregateDirection = direction;
                    aggregateStartPrice = price;
                    aggregateEndPrice = price;
                    aggregateVolume = volume;
                    aggregateCount = 1;
                    hasAggregate = true;
                    continue;
                }

                if (stamp == aggregateStamp && direction == aggregateDirection)
                {
                    aggregateEndPrice = price;
                    aggregateVolume += volume;
                    aggregateCount += 1;
                    continue;
                }

                if (previousStamp.HasValue)
                {
                    TimeSpan gap = aggregateStamp - previousStamp.Value;
                    int seconds = (int)gap.TotalSeconds;
                    for (int i = 1; i < seconds; i++)
                    {
                        DateTime timeStamp = previousStamp.Value.AddSeconds(i);
                        yield return new TimeMessage(timeStamp);
                    }
                }

                yield return new TradeMessage(aggregateStamp, aggregateStartPrice, aggregateEndPrice, aggregateVolume, aggregateDirection, aggregateCount);
                yield return new CandleMessage(candle);
                previousStamp = aggregateStamp;

                aggregateStamp = stamp;
                aggregateDirection = direction;
                aggregateStartPrice = price;
                aggregateEndPrice = price;
                aggregateVolume = volume;
                aggregateCount = 1;
            }

            if (hasAggregate)
            {
                if (previousStamp.HasValue)
                {
                    TimeSpan gap = aggregateStamp - previousStamp.Value;
                    int seconds = (int)gap.TotalSeconds;
                    for (int i = 1; i < seconds; i++)
                    {
                        DateTime timeStamp = previousStamp.Value.AddSeconds(i);
                        yield return new TimeMessage(timeStamp);
                    }
                }

                yield return new TradeMessage(aggregateStamp, aggregateStartPrice, aggregateEndPrice, aggregateVolume, aggregateDirection, aggregateCount);
                yield return new CandleMessage(candle);
                previousStamp = aggregateStamp;
            }

            if (candle != null)
            {
                yield return new CandleMessage(candle);
            }

            if (previousStamp.HasValue)
            {
                yield return new EndOfDataMessage(previousStamp.Value);
            }
        }
    }
}
