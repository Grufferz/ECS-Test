using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class StairComp : Component
    {
        public bool isUp;

        public StairComp(bool up)
        {
            isUp = up;
        }
    }
}
