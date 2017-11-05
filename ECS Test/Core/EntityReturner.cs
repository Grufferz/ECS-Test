using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class EntityReturner
    {
        public int LookUpBit { get; private set; }
        public List<Components.Component> ComponentList;

        public EntityReturner(int bit, List<Components.Component> compList)
        {
            LookUpBit = bit;
            ComponentList = compList;
        }
    }
}
