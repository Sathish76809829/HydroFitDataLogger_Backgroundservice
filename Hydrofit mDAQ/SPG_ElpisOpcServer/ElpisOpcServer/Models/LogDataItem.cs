using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElpisOpcServer.Models
{
    public class LogDataItem
    {
        public string device { get; set; }
        public List<string> SignalDataList { get; set; }
    }

}
