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
        private List<int> _entsToDelete;

        public GarbageSystem(SchedulingSystem shed, EntityManager em)
        {
            _shedSystem = shed;
            _entityManager = em;
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
                    _shedSystem.Remove(_entityManager.JustEntities[msg.entID]);
                    _entityManager.RemoveEntity(msg.entID);
                    break;

            }
        }
    }
}
