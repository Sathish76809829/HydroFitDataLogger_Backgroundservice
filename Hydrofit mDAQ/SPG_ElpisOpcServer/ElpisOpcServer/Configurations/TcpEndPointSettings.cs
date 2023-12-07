using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ElpisOpcServer.Configurations
{
    public class TcpEndPointSettings
    {
        public bool Enabled { get; set; }

        /// <summary>
        /// Keep Alive Interval in seconds
        /// </summary>
        public int KeepAliveInterval { get; } = 120;
        /// <summary>
        /// Listen Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Listen Address
        /// </summary>
        public string IPv6 { get; set; }

        /// <summary>
        /// Listen Port
        /// </summary>
        public int Port { get; set; } = 5008;

        public int SendTimeout { get; set; } = 30000;

        /// <summary>
        /// Read IPv4
        /// </summary>
        /// <returns></returns>
        public bool TryReadAddress(out IPAddress address)
        {
            if (Address == "*")
            {
                address = IPAddress.Any;
                return true;
            }

            if (Address == "localhost")
            {
                address = IPAddress.Loopback;
                return true;
            }

            if (Address == "disable")
            {
                address = IPAddress.None;
                return true;
            }

            if (IPAddress.TryParse(Address, out var ip))
            {
                address = ip;
                return true;
            }

            throw new Exception($"Could not parse IP address: {Address}");
        }
    }
}
