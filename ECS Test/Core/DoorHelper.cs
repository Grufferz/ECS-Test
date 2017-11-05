using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class DoorHelper
    {
        public bool IsOpen { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public DoorHelper(int xp, int yp, bool open)
        {
            X = xp;
            Y = yp;
            IsOpen = open;
        }
    }
}
