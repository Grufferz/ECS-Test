using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    class UseSystem : IBaseSystem
    {
        private EntityManager _entManager;

        public UseSystem(EntityManager em)
        {
            _entManager = em;
            Core.EventBus.Subscribe(Core.EventTypes.Use, (sender, e) => OnMessage(e));
        }

        public void Update()
        {

        }

        public void OnMessage(Core.MessageEventArgs e)
        {
            switch (e.MessageType)
            {
                case Core.EventTypes.Use:
                    Core.UseEventArgs args = (Core.UseEventArgs)e;
                    // position based?
                    if (args.PositionBased)
                    {
                        Core.Entity entityUsing = args.EntityUsing;
                        Components.PositionComp posComp
                            = (Components.PositionComp)_entManager.GetSingleComponentByID(entityUsing.UID, Core.ComponentTypes.Position);
                        int xPos = posComp.X;
                        int yPos = posComp.Y;

                        string posLU = xPos.ToString() + "-" + yPos.ToString();
                        if (_entManager.EntityPostionLookUp.TryGetValue(posLU, out List<Core.Entity> entry))
                        {
                            foreach( Core.Entity li in entry )
                            {
                                int useableLU = (int)Core.ComponentTypes.Useable;
                                int stairsLU = (int)Core.ComponentTypes.Stairs;
                                int entBit = _entManager.EntityBitLookUp[li.UID];
                                if ((useableLU & entBit) > 0)
                                {
                                    // is it a stair?
                                    if ((entBit & stairsLU) > 0)
                                    {
                                        // yay!  it's a stair....
                                        Components.StairComp stairComp 
                                            = (Components.StairComp) _entManager.GetSingleComponentByID(li.UID, Core.ComponentTypes.Stairs);
                                        if (stairComp.isUp)
                                        {
                                            // go up
                                        }
                                        else
                                        {
                                            //go down
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}
