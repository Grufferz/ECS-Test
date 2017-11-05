using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class PositionComp : Component
    {
        public int X { get; set; }
        public int Y { get; set; }

        public PositionComp(int xp, int yp)
        {
            X = xp;
            Y = yp;
            CompType = Core.ComponentTypes.Position;
        }
    }
}
