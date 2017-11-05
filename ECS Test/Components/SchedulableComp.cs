using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class SchedulableComp : Component
    {
        public int Time;

        public SchedulableComp(int t)
        {
            Time = t;
            CompType = Core.ComponentTypes.Schedulable;
        }
    }
}
