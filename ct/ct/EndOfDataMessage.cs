using System;

namespace ct
{
    /// <summary>
    /// Message indicating that no further messages are available from the data source.
    /// </summary>
    public class EndOfDataMessage : Message
    {
        public EndOfDataMessage(DateTime stamp) : base(stamp)
        {
        }

        public override MessageKind Kind => MessageKind.EndOfData;
    }
}
