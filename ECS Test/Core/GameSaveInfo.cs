using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class GameSaveInfo
    {
        public int RandomSeed { get; private set; }
        public int MapLevel { get; private set; }

        public GameSaveInfo(int seed, int level)
        {
            RandomSeed = seed;
            MapLevel = level;
        }
    }
}
