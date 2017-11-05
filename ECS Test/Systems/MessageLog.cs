using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    public class MessageLog
    {
        private static readonly int _maxLines = 9;
        private readonly Queue<string> _lines;

        public MessageLog()
        {
            _lines = new Queue<string>();
        }

        public void Add( string message )
        {
            _lines.Enqueue(message);

            if ( _lines.Count > _maxLines )
            {
                _lines.Dequeue();
            }
        }

        public void Draw( RLNET.RLConsole console )
        {
            console.Clear();
            string[] lines = _lines.ToArray();
            for ( int i = 0; i < lines.Length; i++)
            {
                console.Print(1, i + 1, lines[i], RLNET.RLColor.White);
            }
        }
    }
}
