﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class FurnitureComp : Component
    {

        public FurnitureComp()
        {
            CompType = Core.ComponentTypes.Furniture;
        }
    }
}
