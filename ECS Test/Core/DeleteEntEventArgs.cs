using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class DeleteEntEventArgs : MessageEventArgs
    {
        public int entID;

        public DeleteEntEventArgs(EventTypes t, int ent) : base(t)
        {
            entID = ent;
        }
    }
}
