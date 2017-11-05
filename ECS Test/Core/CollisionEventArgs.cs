using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class CollisionEventArgs : MessageEventArgs
    {
        public Entity EntRequestingMove { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public CollisionEventArgs(EventTypes t, Entity movingEnt, int xPos, int yPos) : base(t)
        {
            EntRequestingMove = movingEnt;
            x = xPos;
            y = yPos;
        }
    }
}
