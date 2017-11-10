using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    class CollisionSystem : IBaseSystem
    {
        private EntityManager _entityManager;

        public CollisionSystem(EntityManager em)
        {
            _entityManager = em;
            Core.EventBus.Subscribe(Core.EventTypes.CollisionCheck, (sender, e) => OnMessage(e));
        }

        public void Update()
        {

        }

        public void OnMessage(Core.MessageEventArgs e)
        {
            switch(e.MessageType)
            {
                case Core.EventTypes.CollisionCheck:

                    Core.CollisionEventArgs details = (Core.CollisionEventArgs)e;
                    //check entity manager lookup dict
                    //string posKey = details.x.ToString() + "-" + details.y.ToString();
                    bool collision = false;
                    List<int> deathList = new List<int>();

                    //if (_entityManager.EntityPostionLookUp.ContainsKey(posKey))

                    HashSet<int> innerIDS = _entityManager.Positions[details.y, details.x];
                    foreach( int ids in innerIDS)
                    {
                        // we have something there...

                        //List<Core.Entity> entL = _entityManager.EntityPostionLookUp[posKey];
                        Components.PositionComp pc
                            = (Components.PositionComp)_entityManager.GetSingleComponentByID(ids, Core.ComponentTypes.Position);
                        
                        if (pc.X == details.x && pc.Y == details.y)
                        {
                            int checker = _entityManager.EntityBitLookUp[ids];
                            if ((checker & (int)Core.ComponentTypes.Health) > 0)
                            {
                                //Game.MessageLog.Add("FIGHT NOW");
                                collision = true;
                                // TODO more elaborate combat system....
                                Components.HealthComp hComp =
                                    (Components.HealthComp)_entityManager.GetSingleComponentByID(ids, Core.ComponentTypes.Health);

                                hComp.Health -= 1;
                                if (hComp.Health < 0)
                                {
                                    hComp.Health = 0;
                                }
                                Components.AIComp aiComp =
                                    (Components.AIComp)_entityManager.GetSingleComponentByID(ids, Core.ComponentTypes.AI);
                                aiComp.UnderAttack = true;
                                aiComp.LastBasher = details.EntRequestingMove.UID;
                                if (!aiComp.CurrentEnemies.Contains(details.EntRequestingMove.UID))
                                {
                                    aiComp.CurrentEnemies.Add(details.EntRequestingMove.UID);
                                }
                                Components.CreatureDetailsComp cdComp =
                                    (Components.CreatureDetailsComp)_entityManager.GetSingleComponentByID(ids, Core.ComponentTypes.CreatureDetails);
                                List<Components.Component> ourList = _entityManager.GetCompsByID(details.EntRequestingMove.UID);
                                Components.CreatureDetailsComp ourComp =
                                    (Components.CreatureDetailsComp)ourList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.CreatureDetails);
                                Game.MessageLog.Add($"{ourComp.PersonalName} Bashes {cdComp.PersonalName} from 3 Damage");

                                if (hComp.Health < 1)
                                {
                                    // entity is dead, collect up and kill at end
                                    deathList.Add(ids);

                                }
                            }
                        }

                        

                        //foreach(Core.Entity entities in entL)
                        //{
                        //    List<Components.Component> entComps = _entityManager.Entities[entities.UID];
                        //    Components.PositionComp pc 
                        //        = (Components.PositionComp)entComps.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Position);
                        //    if (pc.X == details.x && pc.Y == details.y)
                        //    {
                        //        // get entity we are colliding with
                        //        int entBit = _entityManager.EntityBitLookUp[entities.UID];
                                
                        //        // do they have a health component?
                        //        if ((entBit & (int)Core.ComponentTypes.Health) > 0)
                        //        {
                        //            //Game.MessageLog.Add("FIGHT NOW");
                        //            collision = true;
                        //            // TODO more elaborate combat system....
                        //            Components.HealthComp hComp =
                        //                (Components.HealthComp)entComps.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Health);
                        //            hComp.Health -= 1;
                        //            if (hComp.Health < 0)
                        //            {
                        //                hComp.Health = 0;
                        //            }
                        //            Components.AIComp aiComp = 
                        //                (Components.AIComp)entComps.FirstOrDefault(x => x.CompType == Core.ComponentTypes.AI);
                        //            aiComp.UnderAttack = true;
                        //            aiComp.LastBasher = details.EntRequestingMove.UID;
                        //            if (!aiComp.CurrentEnemies.Contains(details.EntRequestingMove.UID))
                        //            {
                        //                aiComp.CurrentEnemies.Add(details.EntRequestingMove.UID);
                        //            }
                        //            Components.CreatureDetailsComp cdComp =
                        //                (Components.CreatureDetailsComp)entComps.FirstOrDefault(x => x.CompType == Core.ComponentTypes.CreatureDetails);
                        //            List<Components.Component> ourList = _entityManager.GetCompsByID(details.EntRequestingMove.UID);
                        //            Components.CreatureDetailsComp ourComp = 
                        //                (Components.CreatureDetailsComp)ourList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.CreatureDetails);
                        //            Game.MessageLog.Add($"{ourComp.PersonalName} Bashes {cdComp.PersonalName} from 3 Damage");

                        //            if (hComp.Health < 1)
                        //            {
                        //                // entity is dead, collect up and kill at end
                        //                deathList.Add(entities.UID);
                                        
                        //            }
                        //        }
                        //    }
                        //}
                    }

                    foreach (int i in deathList)
                    {
                        Core.DeleteEntEventArgs msg = new Core.DeleteEntEventArgs(Core.EventTypes.DeleteEntity, i);
                        Core.EventBus.Publish(Core.EventTypes.DeleteEntity, msg);
                    }

                    if (!collision)
                    { 
                        // move okay
                        Core.MoveOkayEventArgs msg = new Core.MoveOkayEventArgs(Core.EventTypes.MoveOK, details.EntRequestingMove, details.x, details.y);
                        Core.EventBus.Publish(Core.EventTypes.MoveOK, msg);
                    }
                    else
                    {
                        // fight on
                        Core.NoMoveEventArgs msg 
                            = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, details.EntRequestingMove);
                        Core.EventBus.Publish(Core.EventTypes.NoMove, msg);

                    }
                    break;
            }
        }
    }
}
