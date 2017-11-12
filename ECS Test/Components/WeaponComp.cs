using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class WeaponComp : Component
    {
        public string DamageBase { get; set; }
        public Types.WeaponDmgTypes DmgType { get; set; }
        public bool Magic { get; set; }
        public string Name { get; set; }
        public int MaxDmg { get; set; }

        public WeaponComp(string dmg, Types.WeaponDmgTypes t, bool mag, string name, int maxDmg)
        {
            CompType = Core.ComponentTypes.Weapon;
            Magic = mag;
            Name = name;
            DamageBase = dmg;
            DmgType = t;
            MaxDmg = maxDmg;
        }
    }
}
