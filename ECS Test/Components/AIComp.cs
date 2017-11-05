using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class AIComp : Component
    {
        public bool HasAI;
        public RogueSharp.Point Target { get; set; }
        public RogueSharp.Path PathToTarget { get; set; }
        public bool GotPath { get; set; }
        public bool AtTarget { get; set; }
        public int TurnsSinceMove { get; set;  }
        public bool Greedy { get; set; }
        public Core.AIStates AiState { get; set; }

        public AIComp()
        {
            HasAI = true;
            CompType = Core.ComponentTypes.AI;

            Target = new RogueSharp.Point();
            PathToTarget = null;
            GotPath = false;
            AtTarget = false;
            TurnsSinceMove = 0;
            Greedy = (RogueSharp.DiceNotation.Dice.Roll("1d10") > 4);
            AiState = Core.AIStates.Sleeping;
        }
    }
}
