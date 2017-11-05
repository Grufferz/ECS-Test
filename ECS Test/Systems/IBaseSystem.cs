using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    public interface IBaseSystem
    {
        void Update();
        void OnMessage( Core.MessageEventArgs e);
    }
}
