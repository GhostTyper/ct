using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct
{
    public class TradeMessage : Message
    {
        public readonly double StartPrice;
        public readonly double EndPrice;
        public readonly double Volume;
        public readonly TradeDirection Direction;

        private int _count;
        public int Count => _count;

        public TradeMessage(DateTime stamp, double startPrice, double endPrice, double volume, TradeDirection direction) : this(stamp, startPrice, endPrice, volume, direction, 1)
        {
        }

        public TradeMessage(DateTime stamp, double startPrice, double endPrice, double volume, TradeDirection direction, int count) : base(stamp)
        {
            StartPrice = startPrice;
            EndPrice = endPrice;
            Direction = direction;
            Volume = volume;
            _count = count;
        }

        public override MessageKind Kind => MessageKind.Trade;

        public override string ToString()
        {
            return $"Trade {Stamp:O} start={StartPrice} end={EndPrice} volume={Volume} direction={Direction} count={Count}";
        }
    }
}
