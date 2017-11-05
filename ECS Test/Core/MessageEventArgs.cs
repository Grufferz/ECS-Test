using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    public class MessageEventArgs : EventArgs
    {
        public EventTypes MessageType { get; set; }

        public MessageEventArgs(EventTypes t)
        {
            MessageType = t;
        }
    }
}
