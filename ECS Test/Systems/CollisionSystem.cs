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
                    string posKey = details.x.ToString() + "-" + details.y.ToString();
                    bool collision = false;
                    if (_entityManager.EntityPostionLookUp.ContainsKey(posKey))
                    {
                        // we have something there...

                        List<Core.Entity> entL = _entityManager.EntityPostionLookUp[posKey];

                        foreach(Core.Entity entities in entL)
                        {
                            List<Components.Component> entComps = _entityManager.Entities[entities.UID];
                            Components.PositionComp pc 
                                = (Components.PositionComp)entComps.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Position);
                            if (pc.X == details.x && pc.Y == details.y)
                            {
                                //Components.MonterStatsComp msComp =
                                //    (Components.MonterStatsComp)entComps.FirstOrDefault(x => x.CompType == Core.ComponentTypes.MonsterStats);

                                int entBit = _entityManager.EntityBitLookUp[entities.UID];
                                // TODO fight this monster....
                                if ((entBit & (int)Core.ComponentTypes.Health) > 0)
                                {
                                    //FIGHT - do nothing
                                    Game.MessageLog.Add("FIGHT NOW");
                                    collision = true;
                                }
                            }
                        }

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

                    }
                    break;
            }
        }
    }
}
