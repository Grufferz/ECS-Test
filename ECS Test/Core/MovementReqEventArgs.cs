using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class MovementReqEventArgs : MessageEventArgs
    {
        public Entity EntRequestingMove { get; set; }
        public Directions Direction { get; set; }

        public MovementReqEventArgs(EventTypes t, Entity ent, Directions d)
            : base(t)
        {
            MessageType = t;
            EntRequestingMove = ent;
            Direction = d;
        }
    }
}
