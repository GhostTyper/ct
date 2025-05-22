using System;

namespace ct
{
    public enum TimeFrame
    {
        Seconds1 = 1,
        Seconds2 = 2,
        Seconds3 = 3,
        Seconds4 = 4,
        Seconds5 = 5,
        Seconds6 = 6,
        Seconds10 = 10,
        Seconds12 = 12,
        Seconds15 = 15,
        Seconds20 = 20,
        Seconds30 = 30,

        Minutes1 = 60,
        Minutes2 = 120,
        Minutes3 = 180,
        Minutes4 = 240,
        Minutes5 = 300,
        Minutes6 = 360,
        Minutes10 = 600,
        Minutes12 = 720,
        Minutes15 = 900,
        Minutes20 = 1200,
        Minutes30 = 1800,

        Hours1 = 3600,
        Hours2 = 7200,
        Hours3 = 10800,
        Hours4 = 14400,
        Hours6 = 21600,
        Hours8 = 28800,
        Hours12 = 43200,
        Days1 = 86400,
        Days2 = 172800,
        Days3 = 259200,
        Days4 = 345600,
        Days5 = 432000,
        Days6 = 518400,
        Weeks1 = 604800,
        Weeks2 = 1209600,
        Weeks3 = 1814400,
        Weeks4 = 2419200
    }

    public class Candle
    {
        private readonly DateTime _stamp;
        public DateTime Stamp => _stamp;

        private double _open;
        public double Open => _open;

        private double _high;
        public double High => _high;

        private double _low;
        public double Low => _low;

        private double _close;
        public double Close => _close;

        private bool _highBeforeLow;
        public bool HighBeforeLow => _highBeforeLow;

        public TimeFrame TimeFrame { get; }

        private double _volume;
        public double Volume => _volume;

        private DateTime? _firstHighTime;
        private DateTime? _firstLowTime;

        public Candle(DateTime stamp, double open, double high, double low, double close, bool highBeforeLow, TimeFrame timeFrame, double volume)
        {
            _stamp = stamp;
            _open = open;
            _high = high;
            _low = low;
            _close = close;
            _highBeforeLow = highBeforeLow;
            TimeFrame = timeFrame;
            _volume = volume;
        }

        public void AddTrade(DateTime stamp, double price, double volume)
        {
            if (_volume == 0)
            {
                _open = price;
                _high = price;
                _low = price;
                _firstHighTime = stamp;
                _firstLowTime = stamp;
            }
            else
            {
                if (price > _high)
                {
                    _high = price;
                    if (!_firstHighTime.HasValue)
                    {
                        _firstHighTime = stamp;
                    }
                }
                if (price < _low)
                {
                    _low = price;
                    if (!_firstLowTime.HasValue)
                    {
                        _firstLowTime = stamp;
                    }
                }
            }

            _close = price;
            _volume += volume;

            if (_firstHighTime.HasValue && _firstLowTime.HasValue)
            {
                _highBeforeLow = _firstHighTime.Value <= _firstLowTime.Value;
            }
            else if (_firstHighTime.HasValue)
            {
                _highBeforeLow = true;
            }
            else
            {
                _highBeforeLow = false;
            }
        }
    }
}
