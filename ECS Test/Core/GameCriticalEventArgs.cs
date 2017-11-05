using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class GameCriticalEventArgs : MessageEventArgs
    {
        public GameEvents gameEvent;

        public GameCriticalEventArgs(EventTypes t, GameEvents ev) : base(t)
        {
            gameEvent = ev;
        }
    }
}
