using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    public class Entity
    {
        public int UID { get; set; }

        public Entity( int id )
        {
            UID = id;
        }
    }
}
