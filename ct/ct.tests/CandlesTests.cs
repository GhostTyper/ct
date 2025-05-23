using System;
using System.Collections.Generic;
using Xunit;

namespace ct.tests
{
    public class CandlesTests
    {
        [Fact]
        public void AddCandleStoresData()
        {
            DateTime stamp = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Candles candles = new Candles(TimeFrame.Minutes1);
            Candle candle = new Candle(stamp, 1.0, 1.0, 1.0, 1.0, true, TimeFrame.Minutes1, 1.0);
            candles.AddCandle(candle);

            List<Candle> result = new List<Candle>(candles.Enumerate(TimeFrame.Minutes1, stamp, stamp.AddMinutes(1)));
            Assert.Single(result);
            Assert.Equal(1.0, result[0].Open);
            Assert.Equal(1.0, result[0].Close);
        }

        [Fact]
        public void AddTradeAggregatesCorrectly()
        {
            DateTime stamp = new DateTime(2024, 1, 1, 0, 5, 30, DateTimeKind.Utc);
            Candles candles = new Candles(TimeFrame.Minutes1);
            candles.AddTrade(stamp, 10.0, 1.0);
            candles.AddTrade(stamp.AddSeconds(10), 12.0, 1.0);

            List<Candle> result = new List<Candle>(candles.Enumerate(TimeFrame.Minutes1, new DateTime(2024, 1, 1, 0, 5, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 6, 0, DateTimeKind.Utc)));
            Assert.Single(result);
            Assert.Equal(10.0, result[0].Open);
            Assert.Equal(12.0, result[0].Close);
            Assert.Equal(12.0, result[0].High);
            Assert.Equal(10.0, result[0].Low);
            Assert.Equal(2.0, result[0].Volume);
        }

        [Fact]
        public void EnumerateAggregatesLargerTimeFrame()
        {
            DateTime start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Candles candles = new Candles(TimeFrame.Minutes1);
            for (int i = 0; i < 60; i++)
            {
                Candle candle = new Candle(start.AddMinutes(i), i, i, i, i, true, TimeFrame.Minutes1, 1.0);
                candles.AddCandle(candle);
            }

            List<Candle> result = new List<Candle>(candles.Enumerate(TimeFrame.Minutes15, start, start.AddHours(1)));
            Assert.Equal(4, result.Count);
            Assert.Equal(0.0, result[0].Open);
            Assert.Equal(14.0, result[0].Close);
            Assert.Equal(15.0, result[1].Open);
            Assert.Equal(29.0, result[1].Close);
            Assert.Equal(59.0, result[3].Close);
            double volume = 0.0;
            foreach (Candle c in result)
            {
                volume += c.Volume;
            }
            Assert.Equal(60.0, volume);
        }

        [Fact]
        public void EnumerateSmallerTimeFrameThrows()
        {
            Candles candles = new Candles(TimeFrame.Minutes5);
            Assert.Throws<ArgumentException>(() => new List<Candle>(candles.Enumerate(TimeFrame.Minutes1, DateTime.UnixEpoch, DateTime.UnixEpoch.AddMinutes(5))));
        }

        [Fact]
        public void ForeachAggregatesToFiveMinutes()
        {
            DateTime start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Candles candles = new Candles(TimeFrame.Minutes1);
            for (int i = 0; i < 10; i++)
            {
                DateTime stamp = start.AddMinutes(i).AddSeconds(10);
                candles.AddTrade(stamp, i + 1.0, 1.0);
            }

            List<Candle> result = new List<Candle>();
            foreach (Candle candle in candles.Enumerate(TimeFrame.Minutes5, start, start.AddMinutes(10)))
            {
                result.Add(candle);
            }

            Assert.Equal(2, result.Count);
            Assert.Equal(1.0, result[0].Open);
            Assert.Equal(5.0, result[0].Close);
            Assert.Equal(6.0, result[1].Open);
            Assert.Equal(10.0, result[1].Close);
            Assert.Equal(5.0, result[0].Volume);
            Assert.Equal(5.0, result[1].Volume);
        }
    }
}
