using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class MonterStatsComp : Component
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; } 

        public MonterStatsComp()
        {
            Health = RogueSharp.DiceNotation.Dice.Roll("2d5");
            MaxHealth = Health;
            CompType = Core.ComponentTypes.MonsterStats;
        }
    }
}
