using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    class CommandSystem : IBaseSystem
    {
        public CommandSystem()
        {
            Core.EventBus.Subscribe(Core.EventTypes.KeyPressed, (sender, e) => OnMessage(e));
        }

        public void Update()
        {

        }

        public void OnMessage(Core.MessageEventArgs e)
        {
            Game.MessageLog.Add($"MESSAGE REC");

            if (e.MessageType == Core.EventTypes.KeyPressed)
            {
                Core.KeyPressEventArgs msg = (Core.KeyPressEventArgs)e;
                switch(msg.keyPress.Key)
                {
                    case RLNET.RLKey.Up:

                        break;
                    case RLNET.RLKey.Down:

                        break;
                    case RLNET.RLKey.Left:

                        break;
                    case RLNET.RLKey.Right:

                        break;
                    case RLNET.RLKey.Escape:
                        Core.GameCriticalEventArgs gce 
                            = new Core.GameCriticalEventArgs(Core.EventTypes.GameCritical, Core.GameEvents.QUIT);
                        Core.EventBus.Publish(Core.EventTypes.GameCritical, gce);
                        break;

                }
            }
        }

        //protected virtual void OnKeyPressed(Core.MessageEventArgs e)
        //{
        //    OnMessageRec?.Invoke(this, e);
        //}
    }
}
