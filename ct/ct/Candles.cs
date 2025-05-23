using System;
using System.Collections.Generic;

namespace ct
{
    public class Candles
    {
        private readonly TimeFrame _baseTimeFrame;
        public TimeFrame BaseTimeFrame => _baseTimeFrame;

        private readonly SortedDictionary<DateTime, Candle> _candles;

        public Candles(TimeFrame baseTimeFrame)
        {
            _baseTimeFrame = baseTimeFrame;
            _candles = new SortedDictionary<DateTime, Candle>();
        }

        private static DateTime Align(DateTime stamp, TimeFrame timeFrame)
        {
            long totalSeconds = (long)(stamp - DateTime.UnixEpoch).TotalSeconds;
            int seconds = (int)timeFrame;
            long alignedSeconds = (totalSeconds / seconds) * seconds;
            return DateTime.UnixEpoch.AddSeconds(alignedSeconds);
        }

        public void AddCandle(Candle candle)
        {
            if (candle.TimeFrame != _baseTimeFrame)
            {
                throw new ArgumentException("Invalid candle timeframe.");
            }
            _candles[candle.Stamp] = candle;
        }

        public void AddTrade(DateTime stamp, double price, double volume)
        {
            DateTime aligned = Align(stamp, _baseTimeFrame);
            if (!_candles.TryGetValue(aligned, out Candle? candle))
            {
                candle = new Candle(aligned, price, price, price, price, true, _baseTimeFrame, 0.0);
                _candles[aligned] = candle;
            }
            candle.AddTrade(stamp, price, volume);
        }

        private double GetLastClose(DateTime stamp)
        {
            double close = 0.0;
            foreach (KeyValuePair<DateTime, Candle> pair in _candles)
            {
                if (pair.Key >= stamp)
                {
                    break;
                }
                close = pair.Value.Close;
            }
            return close;
        }

        private Candle CreateEmpty(DateTime stamp, double price)
        {
            return new Candle(stamp, price, price, price, price, true, _baseTimeFrame, 0.0);
        }

        public IEnumerable<Candle> Enumerate(TimeFrame timeFrame, DateTime from, DateTime to)
        {
            if ((int)timeFrame < (int)_baseTimeFrame)
            {
                throw new ArgumentException("Requested timeframe is smaller than base.");
            }
            int baseSeconds = (int)_baseTimeFrame;
            int requestSeconds = (int)timeFrame;
            if (requestSeconds % baseSeconds != 0)
            {
                throw new ArgumentException("Timeframe must be multiple of base timeframe.");
            }
            int ratio = requestSeconds / baseSeconds;
            DateTime start = Align(from, _baseTimeFrame);
            DateTime end = Align(to, _baseTimeFrame);
            if (end < to)
            {
                end = end.AddSeconds(baseSeconds);
            }

            double lastClose = GetLastClose(start);
            List<Candle> baseList = new List<Candle>();
            for (DateTime stamp = start; stamp < end; stamp = stamp.AddSeconds(baseSeconds))
            {
                if (_candles.TryGetValue(stamp, out Candle? candle))
                {
                    baseList.Add(candle);
                    lastClose = candle.Close;
                }
                else
                {
                    Candle empty = CreateEmpty(stamp, lastClose);
                    baseList.Add(empty);
                }
            }

            for (int i = 0; i < baseList.Count; i += ratio)
            {
                DateTime stamp = baseList[i].Stamp;
                double open = baseList[i].Open;
                double close = baseList[Math.Min(i + ratio - 1, baseList.Count - 1)].Close;
                double high = open;
                double low = open;
                DateTime highTime = baseList[i].Stamp;
                DateTime lowTime = baseList[i].Stamp;
                double volume = 0.0;

                for (int j = 0; j < ratio && i + j < baseList.Count; j++)
                {
                    Candle c = baseList[i + j];
                    volume += c.Volume;
                    if (c.High > high)
                    {
                        high = c.High;
                        highTime = c.FirstHighTime ?? c.Stamp;
                    }
                    if (c.Low < low)
                    {
                        low = c.Low;
                        lowTime = c.FirstLowTime ?? c.Stamp;
                    }
                }

                bool highBeforeLow = highTime <= lowTime;
                Candle aggregated = new Candle(stamp, open, high, low, close, highBeforeLow, timeFrame, volume);
                yield return aggregated;
            }
        }
    }
}
