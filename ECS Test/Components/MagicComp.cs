using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class MagicComp : Component
    {
        public int Magic { get; set; }
        public int MaxMagic { get; set; }

        public MagicComp(int m)
        {
            CompType = Core.ComponentTypes.Magic;

            Magic = m;
            MaxMagic = m;
        }
    }
}
