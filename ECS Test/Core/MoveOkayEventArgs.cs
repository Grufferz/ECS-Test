using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class MoveOkayEventArgs : MessageEventArgs
    {
        public Entity EntRequestingMove;
        public int newX;
        public int newY;

        public MoveOkayEventArgs(EventTypes e, Entity movingEnt, int xPos, int yPos ) : base(e)
        {
            EntRequestingMove = movingEnt;
            newX = xPos;
            newY = yPos;
        }
    }
}
