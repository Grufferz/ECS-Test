using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class InventoryAddEventArgs : MessageEventArgs
    {
        public int EntPickingUp { get; set; }
        public int PickedUpObject { get; set; }

        public InventoryAddEventArgs(EventTypes t, int epu, int puo) : base(t)
        {
            EntPickingUp = epu;
            PickedUpObject = puo;
        }
    }
}
