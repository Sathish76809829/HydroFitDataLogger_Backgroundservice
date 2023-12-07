using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElpisOpcServer.Models
{
    /// <summary>
    /// Socket response for sending message
    /// </summary>
    public enum SocketResponse
    {
        Success = 1,
        NotReachable = 2,
        FailedToPublish = 4
    }
}
