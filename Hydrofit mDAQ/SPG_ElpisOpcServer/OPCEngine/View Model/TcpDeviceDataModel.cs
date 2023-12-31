﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCEngine.View_Model
{
    public class DeviceDataModel
    {
        public string device { get; set; }
        //public DateTime timeStamp { get; set; }
        public string[] SignalDataList { get; set; }
    }

    public class parsedDeviceData
    {
        //public string deviceId { get; set; }
        public string SignalId { get; set; }
        //public string signalName { get; set; }
        public double DataValue { get; set; }
        //public string timeStamp { get; set; }
        //public string datatype { get; set; }
    }
    public class SignalData
    {
        public string Device { get; set; }
        public List<string> SignalDataList { get; set; }
    }



}
