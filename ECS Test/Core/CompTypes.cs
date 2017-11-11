using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    public enum ComponentTypes
    {
        Position = 1,
        Render = 2,
        MonsterStats = 4, 
        Health = 8,
        Attributes = 16, 
        AI = 32,
        Schedulable = 64,
        Stairs = 128,
        Useable = 256,
        ItemValue = 512,
        CreatureDetails = 1024,
        Magic = 2048,
        Door = 4096,
        Collectable = 8192,
        Inventory = 16384,
        Actor = 32768,
        Weapon = 65536,
        Furniture = 131072
    }
}
