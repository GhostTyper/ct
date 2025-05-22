using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct
{
    public class Message
    {
        public readonly DateTime Stamp;

        public Message(DateTime stamp)
        {
            Stamp = stamp;
        }

        public virtual MessageKind Kind => MessageKind.Trade;
    }
}
