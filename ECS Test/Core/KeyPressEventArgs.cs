using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class KeyPressEventArgs : MessageEventArgs
    {
        public RLNET.RLKeyPress keyPress;

        public KeyPressEventArgs(EventTypes t, RLNET.RLKeyPress kp) : base(t)
        {
            keyPress = kp;
        }
    }
}
