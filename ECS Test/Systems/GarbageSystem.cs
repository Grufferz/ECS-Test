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

            }
        }
    }
}
