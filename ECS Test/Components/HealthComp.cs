using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class HealthComp : Component
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; }

        public HealthComp(int hp)
        {
            //Health = RogueSharp.DiceNotation.Dice.Roll("2d5");
            Health = hp;
            MaxHealth = Health;
            CompType = Core.ComponentTypes.Health;
        }
    }
}