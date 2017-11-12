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
        public bool UnderAttack { get; set; }
        public HashSet<int> CurrentEnemies { get; set; }
        public int LastBasher { get; set; }
        public bool Fleeing { get; set; }
        public int FleeingFrom { get; set; }
        public int FleeCounter { get; set; }
        public List<int> ItemsAlreadySeen { get; set; }
        public Core.DungeonMap DMap { get; }
        public Types.AITypes AiType { get; set; }
        public Dictionary<int, RogueSharp.Point> TreasureMemory { get; set; }
        public RogueSharp.Point HomeSpot { get; set; }

        public AIComp(Core.DungeonMap m, Types.AITypes aiT, RogueSharp.Point startPoint)
        {
            HasAI = true;
            CompType = Core.ComponentTypes.AI;
            DMap = m;
            Target = new RogueSharp.Point();
            PathToTarget = null;
            GotPath = false;
            AtTarget = false;
            TurnsSinceMove = 0;
            Greedy = (RogueSharp.DiceNotation.Dice.Roll("1d10") < 4);
            UnderAttack = false;
            CurrentEnemies = new HashSet<int>();
            LastBasher = 0;
            Fleeing = false;
            FleeCounter = 0;
            ItemsAlreadySeen = new List<int>();
            AiType = aiT;

            AiState = Core.AIStates.Wandering;
            TreasureMemory = new Dictionary<int, RogueSharp.Point>();
        }
    }
}
