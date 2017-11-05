using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    class SystemManager
    {
        //blic static RenderSystem RenderSystem { get; set; }
        public static EntityManager EntityManager { get; private set; }

        public SystemManager(EntityManager em)
        {
            EntityManager = em;
        }

        public void Update()
        {
        }
    }
}
