using ElpisOpcServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElpisOpcServer.SocketService.Net.Formatters
{
    public interface IPacketWriter
    {
        int Length { get; }

        void Clear();
        byte[] Encode(MessagePacket value);
        byte[] GetBytes();
        void Reset(int size);
    }
}
