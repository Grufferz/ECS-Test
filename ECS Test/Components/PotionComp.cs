using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class PotionComp : Component
    {
        public Types.PotionTypes PotionType { get; set; }

        public PotionComp(Types.PotionTypes potionT)
        {
            PotionType = potionT;
        }
    }
}
