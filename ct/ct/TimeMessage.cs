using System;

namespace ct
{
    /// <summary>
    /// Message emitted periodically when no other messages are available.
    /// </summary>
    public class TimeMessage : Message
    {
        public TimeMessage(DateTime stamp) : base(stamp)
        {
        }

        public override MessageKind Kind => MessageKind.Time;
    }
}
