using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElpisOpcServer.Extensions
{
    public static class SocketExtensions
    {
        public static Task<bool> ConnectAsync(this Socket self,EndPoint endPoint,int timeout)
        {
            return Task.Run(() =>
            {
                var clientDone = new ManualResetEvent(false);
                var arg = new SocketAsyncEventArgs
                {
                    RemoteEndPoint = endPoint
                };
                var connected = false;
                arg.Completed += (s, e) =>
                  {
                      connected = e.SocketError == SocketError.Success;
                      clientDone.Set();
                  };
                self.ConnectAsync(arg);
                clientDone.Reset();
                clientDone.WaitOne(timeout);
                return connected;
            });
        }
    }
}
