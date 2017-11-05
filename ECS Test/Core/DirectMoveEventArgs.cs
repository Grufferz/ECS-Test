using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class DirectMoveEventArgs : MessageEventArgs
    {
        public Entity EntRequestingMove { get; set; }
        public RogueSharp.Point PointToMoveTo { get; set; }

        public DirectMoveEventArgs(EventTypes t, Entity ent, RogueSharp.Point p)
            : base(t)
        {
            MessageType = t;
            EntRequestingMove = ent;
            PointToMoveTo = p;
        }
    }
}
