using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class UseableComp : Component
    {
        public bool CanUse;

        public UseableComp()
        {
            CanUse = true;
        }
    }
}
