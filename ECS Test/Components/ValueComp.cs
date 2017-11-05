using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class ValueComp : Component
    {
        public int Value;

        public ValueComp(int v)
        {
            Value = v;
        }
    }
}
