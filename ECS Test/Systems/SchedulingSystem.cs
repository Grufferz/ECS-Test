using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    class SchedulingSystem
    {
        private int _time;
        private readonly SortedDictionary<int, List<Core.Entity>> _scheduleables;

        public SchedulingSystem()
        {
            _time = 0;
            _scheduleables = new SortedDictionary<int, List<Core.Entity>>();
        }

        public void Add( Core.Entity schedulable, int speed)
        {

            int key = _time + speed;
            if (!_scheduleables.ContainsKey( key ))
            {
                _scheduleables.Add(key, new List<Core.Entity>());
            }
            _scheduleables[key].Add(schedulable);
        }

        public void Remove(Core.Entity scheduleable)
        {
            KeyValuePair<int, List<Core.Entity>> schedulabelListFound
                = new KeyValuePair<int, List<Core.Entity>>(-1, null);

            foreach( var schedulableList in _scheduleables )
            {
                if ( schedulableList.Value.Contains( scheduleable ))
                {
                    schedulabelListFound = schedulableList;
                    break;
                }
            }
            if (schedulabelListFound.Value != null )
            {
                schedulabelListFound.Value.Remove(scheduleable);
                if ( schedulabelListFound.Value.Count <= 0 )
                {
                    _scheduleables.Remove(schedulabelListFound.Key);
                }
            }
        }

        public Core.Entity Get()
        {
            var firstSchedulableGroup = _scheduleables.First();
            var firstSchedulable = firstSchedulableGroup.Value.First();
            Remove(firstSchedulable);
            _time = firstSchedulableGroup.Key;
            return firstSchedulable;
        }

        public int GetTime()
        {
            return _time;
        }

        public void Clear()
        {
            _time = 0;
            _scheduleables.Clear();
        }

        public bool  IsEmpty()
        {
            return _scheduleables.Count < 1;
        }
    }
}
