using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    public static class EventBus
    {
        private static Dictionary<EventTypes, List<EventHandler<MessageEventArgs>>> handlers =
            new Dictionary<EventTypes, List<EventHandler<MessageEventArgs>>>();

        public static void Publish(EventTypes eventType, MessageEventArgs e)
        {
            Publish(null, eventType, e);
        }

        public static void Publish(object sender, EventTypes eventType, MessageEventArgs e)
        {
            foreach( EventHandler<MessageEventArgs> handler in handlers[eventType])
            {
                handler(sender, e);
            }
        }

        public static void Subscribe(EventTypes evT, EventHandler<MessageEventArgs> handler)
        {
            if (!handlers.ContainsKey(evT))
            {
                handlers.Add(evT, new List<EventHandler<MessageEventArgs>>());
            }
            handlers[evT].Add(handler);
        }
    }
}
