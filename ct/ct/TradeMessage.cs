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

        public TradeMessage(DateTime stamp, double startPrice, double endPrice, double volume) : base(stamp)
        {

        }

        public override MessageKind Kind => MessageKind.Trade;
    }
}
