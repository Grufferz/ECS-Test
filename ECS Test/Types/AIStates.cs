using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    public enum AIStates
    {
        Sleeping = 1,
        Waiting = 2,
        Aggressive = 3,
        Wandering = 4,
        WanderingPickingUp = 5,
        Fleeing = 6
    }
}
