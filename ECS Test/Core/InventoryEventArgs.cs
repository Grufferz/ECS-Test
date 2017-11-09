using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    public class InventoryEventArgs : MessageEventArgs
    {
        Entity EntPickingUp { get; set; }
        Entity PickedUpObject { get; set; }

        public InventoryEventArgs(EventTypes t, Entity epu, Entity puo) : base(t)
        {
            EntPickingUp = epu;
            PickedUpObject = puo;
        }
    }
}
