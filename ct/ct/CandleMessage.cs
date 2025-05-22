using System;

namespace ct
{
    public class CandleMessage : Message
    {
        public Candle Candle { get; }

        public CandleMessage(Candle candle) : base(candle.Stamp)
        {
            Candle = candle;
        }

        public override MessageKind Kind => MessageKind.Candle;

        public override string ToString()
        {
            return $"Candle {Candle.Stamp:O} O={Candle.Open} H={Candle.High} L={Candle.Low} C={Candle.Close} V={Candle.Volume}";
        }
    }
}
