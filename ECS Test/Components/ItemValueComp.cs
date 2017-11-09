using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class ItemValueComp : Component
    {
        public int ItemValue { get; set; }

        public ItemValueComp(int v)
        {
            ItemValue = v;
            CompType = Core.ComponentTypes.ItemValue;
        }
    }
}
