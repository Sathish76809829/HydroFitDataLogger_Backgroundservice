using Elpis.Windows.OPC.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Elpis.Windows.OPC.Server
{
    [DisplayName("TcpSocket Device")]
    [Serializable()]
    public class TcpSocketDevice : DeviceBase
    {
        #region Constructor
        /// <summary>
        /// Serialization constructor
        /// </summary> 
        /// 
       // private readonly TcpListener listener;
        public TcpSocketDevice() : base()
        {
            
        }
        //public TcpSocketDevice(string ipAddress, ushort port)
        //{
        //    this.port = port;
        //    listener = new TcpListener(ipAddress, port);
        //}

        public TcpSocketDevice(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
        #endregion Constructor

        #region Properties

        private string ipAddress;

        [Description("Specify the IP address of the object. The valid IP address is something between integers like 0-255.0-255.0-255.0-255"), DisplayName("IP Address *"), PropertyOrder(3)]

        public string IPAddress
        {
            get
            {
                return ipAddress;
            }
            set
            {
                ipAddress = value;
                OnPropertyChanged("IPAddress");
            }
        }
        //private IPAddress ipAddress;

        //[Description("Specify the IP address of the object. The valid IP address is something between integers like 0-255.0-255.0-255.0-255"), DisplayName("IP Address *"), PropertyOrder(3)]

        //public IPAddress IPAddress
        //{
        //    get
        //    {
        //        return ipAddress;
        //    }
        //    set
        //    {
        //        ipAddress = value;
        //        OnPropertyChanged("IPAddress");
        //    }
        //}

        private ushort port;

        [Description("Specify the port number of the device.The valid range is 0 to 65,535."), DisplayName("Port Number *"), PropertyOrder(4)]

        public ushort Port
        {
            get
            {
                return port;
            }
            set
            {
                try
                {
                    if (value >= ushort.MinValue && value <= ushort.MaxValue)
                    {
                        port = value;
                    }
                    else
                    {
                        MessageBox.Show("Check the port number");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Check the port number");
                }
                OnPropertyChanged("Port");
            }
        }

        private string deviceId;
        [DisplayName("Device Id *"), Description("Specify the DeviceId"),PropertyOrder(3)]
        public string DeviceId
        {
            get
            {
                return deviceId;
            }
            set
            {
                deviceId = value;
                OnPropertyChanged("DeviceId");
            }
        }
        private string boardip;
        [Description("Specify the identity of the BoardIp."), DisplayName("Board Ip*"), Browsable(false),PropertyOrder(6)]
        public string BoardIP
        {
            get { return boardip; }
            set { boardip = value; }
        }
        private string boardgateway;
        [Description("Specify the identity of the BoardGateWay."), DisplayName("BoardGateWay*"), Browsable(false), PropertyOrder(7)]
        public string BoardGateWay
        {
            get { return boardgateway; }
            set { boardgateway = value; }
        }
        private string serialno;
        [Description("Specify the identity of the SerialNo."), DisplayName("SerialNo*"), Browsable(false), PropertyOrder(8)]
        public string SerialNo
        {
            get { return serialno; }
            set { serialno = value; }
        }
        private int autoModeTime;
        [Description("Specify the AutoMode TestRun Time in minutes(Min))."), DisplayName("Automodetime(Min)*"), PropertyOrder(9)]
        public int AutoModeTime
        {
            get { return autoModeTime; }
            set { autoModeTime = value; }
        }
        private int _stepCount;
        [Description("Specify the StepCount to capture the data points need to capture Valid Range is 1-4)."), Browsable(false), DisplayName("StepCount*")]
        public int StepCount
        {
            get { return _stepCount; }
            set 
            {
                try
                {
                    if (value <= 4 && value > 0)
                    {
                        _stepCount = value;
                    }
                    else
                    {
                        MessageBox.Show("Check Valid range it should be(1-4)");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Check Valid range it should be(1-4)");
                }
                OnPropertyChanged("StepCount"); 
            }
        }

        #endregion Properties
    }
}
