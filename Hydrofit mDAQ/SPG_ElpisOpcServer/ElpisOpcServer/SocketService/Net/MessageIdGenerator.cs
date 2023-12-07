using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElpisOpcServer.SocketService.Net
{
    public class MessageIdGenerator
    {
        readonly object _syncRoot = new object();
        ushort _value;

        public void Reset()
        {
            lock (_syncRoot)
            {
                _value = 0;
            }
        }
        public ushort GetNextPacketIdentifier()
        {
            lock (_syncRoot)
            {
                _value++;

                if (_value == 0)
                {
                    //id should never be 0.
                    _value = 1;
                }

                return _value;
            }
        }
    }
}
