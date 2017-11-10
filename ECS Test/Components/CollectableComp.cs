using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class CollectableComp : Component
    {

        public bool Stackable;
        public int Quantity;
        public bool Treasure;
        public int RdmIdent;
        public Types.ItemTypes ItemType;

        public CollectableComp(int quant, bool stack, bool treas, Types.ItemTypes it, int r)
        {
            CompType = Core.ComponentTypes.Collectable;
            Treasure = treas;
            Quantity = quant;
            Stackable = stack;
            ItemType = it;
            RdmIdent = r;
        }
    }
}
