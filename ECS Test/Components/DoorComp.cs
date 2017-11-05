using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class DoorComp : Component
    {
        public bool IsOpen;
        public bool IsLocked;

        public DoorComp(bool o, bool l)
        {
            IsOpen = o;
            IsLocked = l;
            CompType = Core.ComponentTypes.AI;
        }
    }
}
