using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class WeaponComp : Component
    {
        public int DamageBase { get; set; }
        public Types.WeaponDmgTypes DmgType { get; set; }
        public bool Magic;
        public string Name;

        public WeaponComp(int dmg, Types.WeaponDmgTypes t, bool mag, string name)
        {
            CompType = Core.ComponentTypes.Weapon;
            Magic = mag;
            Name = name;
            DamageBase = dmg;
            DmgType = t;
        }
    }
}
