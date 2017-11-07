using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    public enum EventTypes
    {
        ActionReqMove = 1,
        ActionCompleteMove = 2,
        KeyPressed = 3,
        AIRequest = 4,
        GameCritical = 5,
        NoCollison = 6,
        Collision = 7,
        CollisionCheck = 8,
        MoveOK = 9,
        Use = 10, 
        DirectMove = 11,
        NoMove = 12,
        DeleteEntity = 13
    }
}
