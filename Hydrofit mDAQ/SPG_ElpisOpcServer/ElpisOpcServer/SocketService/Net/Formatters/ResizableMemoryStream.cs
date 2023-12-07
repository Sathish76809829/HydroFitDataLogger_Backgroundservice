using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElpisOpcServer.SocketService.Net.Formatters
{
    public class ResizableMemoryStream : MemoryStream
    {
        public ResizableMemoryStream()
        {
        }

        public ResizableMemoryStream(int size)
            : base(size)
        {
        }

        public override int Capacity
        {
            get => (int)this.Length;
            set
            {
                if (value < this.Length)
                    return;

                byte[] newBuffer = new byte[value];
                this.Read(newBuffer, 0, (int)this.Length);
                this.SetLength(0);
                this.Write(newBuffer, 0, (int)this.Length);
            }
        }

        public void EnsureCapacity(int sizeHint)
        {
            if (sizeHint <= 0)
                return;

            if (this.Capacity < sizeHint)
                this.Capacity = sizeHint;
        }
    }
}
