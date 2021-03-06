﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    class InventorySystem : IBaseSystem
    {
        private EntityManager _entityManager;

        public InventorySystem(EntityManager em)
        {
            _entityManager = em;

            Core.EventBus.Subscribe(Core.EventTypes.InventoryAdd, (sender, e) => OnMessage(e));
        }

        public void Update()
        {

        }

        public void OnMessage(Core.MessageEventArgs e)
        {
            switch(e.MessageType)
            {
                case Core.EventTypes.InventoryAdd:

                    Core.InventoryAddEventArgs msg = (Core.InventoryAddEventArgs)e;

                    int entPickingUp = msg.EntPickingUp;
                    int pickedUpObj = msg.PickedUpObject;

                    List<Components.Component> pickedUpObjComps = _entityManager.GetCompsByID(pickedUpObj);
                    List<Components.Component> entComps = _entityManager.GetCompsByID(entPickingUp);

                    Components.InventoryComp invComp 
                        = (Components.InventoryComp) _entityManager.GetSingleComponentByID(entPickingUp, Core.ComponentTypes.Inventory);

                    if (invComp != null)
                    {

                        Components.PositionComp posComp =
                            (Components.PositionComp)pickedUpObjComps.Find(x => x.CompType == Core.ComponentTypes.Position);
                        Components.CollectableComp collectComp =
                            (Components.CollectableComp)pickedUpObjComps.Find(x => x.CompType == Core.ComponentTypes.Collectable);
                        Components.ItemValueComp vC = (Components.ItemValueComp)pickedUpObjComps.Find(x => x.CompType == Core.ComponentTypes.ItemValue);

                        //Components.Component c = _entityManager.GetSingleComponentByID(collectComp.)

                        bool hasValue = (vC != null);

                        if (collectComp.Active)
                        {

                            //is it treasure?
                            if (collectComp.Treasure)
                            {
                                //TODO stackable treasure

                                if (vC != null)
                                {
                                    invComp.ValueCarried += vC.ItemValue;
                                }

                                // remove position comps from picked up object
                                _entityManager.RemoveCompFromEnt(pickedUpObj, Core.ComponentTypes.Position);
                                collectComp.Active = false;
                                _entityManager.RemoveEntFromPosition(posComp.X, posComp.Y, pickedUpObj);

                                invComp.Treasure.Add(pickedUpObj);

                            }
                            else
                            {
                                // is it stackable? - not currently used
                                //TODO stackable collectables
                                if (collectComp.Stackable)
                                {

                                }
                                else
                                {
                                    Game.MessageLog.Add("picking up item");
                                    // System.Threading.Thread.Sleep(1000);
                                    if (posComp != null)
                                    {
                                        _entityManager.RemoveEntFromPosition(posComp.X, posComp.Y, pickedUpObj);
                                        _entityManager.RemoveCompFromEnt(pickedUpObj, Core.ComponentTypes.Position);
                                        pickedUpObjComps.Remove(posComp);
                                    }
                                    invComp.Inventory.Add(pickedUpObj);

                                }
                            }

                        }
                    }
                    break;
            }
        }
    }
}
