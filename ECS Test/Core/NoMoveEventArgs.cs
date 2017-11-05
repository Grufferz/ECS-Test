using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    public class NoMoveEventArgs : MessageEventArgs
    {
        public Entity EntNotMoving { get; set; }

        public NoMoveEventArgs(EventTypes t, Entity e) : base(t)
        {
            MessageType = t;
            EntNotMoving = e;
        }
    }
}
