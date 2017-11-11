using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    class GarbageSystem : IBaseSystem
    {
        private SchedulingSystem _shedSystem;
        private EntityManager _entityManager;
        private Core.DungeonMap _dungeonMap;
        private List<int> _entsToDelete;

        public GarbageSystem(SchedulingSystem shed, EntityManager em, Core.DungeonMap dm)
        {
            _shedSystem = shed;
            _entityManager = em;
            _dungeonMap = dm;
            _entsToDelete = new List<int>();
            Core.EventBus.Subscribe(Core.EventTypes.DeleteEntity, (sender, e) => OnMessage(e));
            Core.EventBus.Subscribe(Core.EventTypes.DeadEntity, (sender, e) => OnMessage(e));
        }

        public void Update()
        {
            foreach (int ent in _entsToDelete)
            {
                _entityManager.RemoveEntity(ent);
            }
        }

        public void OnMessage(Core.MessageEventArgs e)
        {
            switch( e.MessageType )
            {
                case Core.EventTypes.DeleteEntity:
                    Core.DeleteEntEventArgs msg = (Core.DeleteEntEventArgs)e;

                    //_entsToDelete.Add(msg.entID);
                    // set cell walkable
                    List<Components.Component> compList = _entityManager.GetCompsByID(msg.entID);
                    Components.PositionComp pc 
                        = (Components.PositionComp)compList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Position);
                    _dungeonMap.SetIsWalkable(pc.X, pc.Y, true);

                    _shedSystem.Remove(_entityManager.JustEntities[msg.entID]);
                    _entityManager.RemoveEntity(msg.entID);
                    break;

                case Core.EventTypes.DeadEntity:

                    // leave corpse of entity

                    Core.DeleteEntEventArgs dM = (Core.DeleteEntEventArgs)e;
                    int entID = dM.entID;

                    Components.PositionComp posC = (Components.PositionComp)_entityManager.GetSingleComponentByID(entID, Core.ComponentTypes.Position);
                    _dungeonMap.SetIsWalkable(posC.X, posC.Y, true);

                    Components.RenderComp rendC = (Components.RenderComp)_entityManager.GetSingleComponentByID(entID, Core.ComponentTypes.Render);
                    rendC.Glyph = '%';
                    rendC.Colour = RLNET.RLColor.Red;

                    _shedSystem.Remove(_entityManager.JustEntities[entID]);

                    _entityManager.RemoveCompFromEnt(entID, Core.ComponentTypes.Health);
                    _entityManager.RemoveCompFromEnt(entID, Core.ComponentTypes.Actor);

                    _entityManager.AddFurnitureToEnt(entID);

                    // drop inventory
                    Components.InventoryComp invC = (Components.InventoryComp)_entityManager.GetSingleComponentByID(entID, Core.ComponentTypes.Inventory);
                    foreach(int droppedID in invC.Inventory)
                    {
                        Components.CollectableComp cc = (Components.CollectableComp)_entityManager.GetSingleComponentByID(droppedID, Core.ComponentTypes.Collectable);
                        // only drop treasure for the moment
                        //TODO - drop other items apart from treasure
                        if (cc.Treasure)
                        {
                            _entityManager.AddPositionToEnt(droppedID, posC.X, posC.Y);
                            cc.Active = true;
                        }
                    }

                    break;

            }
        }
    }
}
