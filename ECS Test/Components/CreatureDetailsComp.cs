using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class CreatureDetailsComp : Component
    {
        public string Name { get; set; }
        public Types.CreatureTypes CreatureType { get; set; }
        public string PersonalName { get; set; }
        public bool Undead { get; set; }

        public CreatureDetailsComp(string n, string pn, Types.CreatureTypes t)
        {
            CompType = Core.ComponentTypes.CreatureDetails;

            Name = n;
            PersonalName = pn;
            CreatureType = t;
            
            if (CreatureType == Types.CreatureTypes.Zombie)
            {
                Undead = true;
            }
            else
            {
                Undead = false;
            }
        }
    }
}
