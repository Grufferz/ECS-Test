using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class AIReqMessageEventArgs : MessageEventArgs
    {
        public Entity entRequestingMove { get; set; }

        public AIReqMessageEventArgs(EventTypes t, Entity ent)
            : base(t)
        {
            entRequestingMove = ent;
        }
    }
}
