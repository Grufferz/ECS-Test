using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class RenderComp : Component
    {
        public char Glyph { get; set; }
        public RLNET.RLColor Colour { get; set; }

        public RenderComp(char c, RLNET.RLColor color)
        {
            CompType = Core.ComponentTypes.Render;
            Glyph = c;
            Colour = color;
        }
    }
}
