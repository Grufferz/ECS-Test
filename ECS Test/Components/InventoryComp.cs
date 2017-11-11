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
        public List<int> Treasure;
        public int ValueCarried;
        public int CurrentWeapon;

        public InventoryComp()
        {
            CompType = Core.ComponentTypes.Inventory;
            ValueCarried = 0;
            CurrentWeapon = 0;
            Inventory = new List<int>();
            Treasure = new List<int>();

        }
    }
}
