using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElpisOpcServer.SocketService.Net.Formatters
{
    public class ByteReader : IPacketReader
    {
        public byte[] CreateHeader()
        {
            return new byte[6];
        }

        public MessageHeader ReadHeader(byte[] header)
        {
            MessageHeader res = default;
            res.Type = (MessageType)header[0];
            res.Length = (header[1]) | ((header[2]) << 8);
            res.Id = (ushort)(header[5] | ((header[6]) << 8));
            return res;
        }

        public ushort ReadUInt16(byte[] value, int index)
        {
            return (ushort)((value[index]) | ((value[index + 1]) << 8));
        }
    }
}
