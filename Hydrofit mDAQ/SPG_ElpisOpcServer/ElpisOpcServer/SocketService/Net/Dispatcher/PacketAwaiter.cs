using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElpisOpcServer.SocketService.Net.Dispatcher
{
    public class PacketAwaiter : IDisposable
    {
        private readonly TaskCompletionSource<bool> _taskCompletionSource;
        public readonly ushort MessageId;

        private readonly PacketDispatcher _dispatcher;
        public PacketAwaiter(ushort messageId, PacketDispatcher dispatcher)
        {
            MessageId = messageId;
            _dispatcher = dispatcher;
            _taskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.None);
        }
        public async Task<bool> WaitOneAsync(System.TimeSpan timeout)
        {
            using (var timeoutToken = new CancellationTokenSource(timeout))
            {
                using (timeoutToken.Token.Register(() => Fail()))
                {
                    var res = await _taskCompletionSource.Task.ConfigureAwait(false);
                    return res;
                }
            }
        }
        public void Complete()
        {
            _taskCompletionSource.TrySetResult(true);
        }
        public void Cancel()
        {
            _taskCompletionSource.TrySetCanceled();
        }
        void Fail()
        {
            _taskCompletionSource.TrySetResult(false);
        }

        public void Dispose()
        {
            _dispatcher.RemoveAwaiter(this);
        }
    }
}
