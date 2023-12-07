using ElpisOpcServer.SocketService.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElpisOpcServer.Models
{
    public class MessagePacket
    {
        public MessageType Type { get; set; }

        /// <summary>
        /// If message is not delivered it will retain for some interval
        /// </summary>
        public bool Retain { get; set; }

        public ushort Id { get; set; }

        /// <summary>
        /// Quality of the deliver
        /// 0 - for none
        /// 1 - for only ack
        /// 2 - for ack and retry
        /// </summary>
        public DeliveryQuality Quality { get; set; }

        //public JToken Message { get; set; }
        public JsonElement Message { get; set; }
    }
}
