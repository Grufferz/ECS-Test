using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class InventoryComp : Component
    {
        public List<int> Inventory;

        public InventoryComp()
        {
            CompType = Core.ComponentTypes.Inventory;

            Inventory = new List<int>();

        }
    }
}
