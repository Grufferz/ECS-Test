using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class UseEventArgs : MessageEventArgs
    {

        public bool InventoryBased;
        public bool PositionBased;
        public Entity EntityUsing;

        public UseEventArgs(EventTypes t, bool invBased, bool posBased, Entity ent) : base(t)
        {
            InventoryBased = invBased;
            PositionBased = posBased;
            EntityUsing = ent;
        }
    }
}
