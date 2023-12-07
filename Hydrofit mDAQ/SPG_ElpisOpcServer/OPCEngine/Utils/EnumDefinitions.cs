#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion Namespaces

#region OPCEngine namespace
namespace Elpis.Windows.OPC.Server
{

    #region ConnectionType Enum
    /// <summary>
    /// ConnectionType Enum
    /// </summary>
    public enum ConnectionType
    {
        Ethernet,
        Port,
        None
    }

    #endregion End Of ConnectionType Enum

    #region DataAndAccessType Enum
    /// <summary>
    ///  DataAndAccessType Enum
    /// </summary>
    public enum DataAndAccessType
    {
        Read,
        Write,
        ReadWrite,
        Integer,
        Boolean,
        String,
        ReplaceWithZero,
        UnModified,
        IPAddress,
        NetWorkCard,
        TCP_IP,
        UDP

    }
    #endregion End Of DataAndAccessType Enum
    #region Units Enum
    public enum Units
    {
        //MPa,
        bar,
        lpm,
        Ams,
        Rpm
    }
    #endregion Units Enum
    #region DataType Enum
    /// <summary>
    /// DataType Enum
    /// </summary>
    public enum DataType
    {
        //Boolean,
        //Integer,
        //Short,
        //Double,
        //String

        // Array = 0x2000,

        //  Byte = 0x11,
        //  Char = 0x12,
        //  Currency = 6,
        //  DataObject = 13,
        // Date = 7,
        // Decimal = 14,

        //  Empty = 0,
        //  Error = 10,



       //Boolean = 11,
       // Double = 5,
        Float,
        Integer = 3,
        //Short = 2,
        //String = 8,


        // Long = 20,
        // Null = 1,
        //Object = 9,
       
        // Single = 4,
        
        // UserDefinedType = 0x24,
        // Variant = 12

    }
    #endregion End Of DataType Enum
    #region HydroFitDataType Enum
    public enum HydDatatype
    {
        Float,
        Integer = 3,
    }
    #endregion EndOf HydroFitDataType Enum
    #region Blocktype
    public enum BlockTypes
    {
        // None,
        Select,
        Block1,
        Block2
    }
    #endregion BlockType
    #region DataAccess Enum
    /// <summary>
    /// DataType Enum
    /// </summary>
    public enum DataAccess
    {
        Read,
        Write,
        ReadWrite
    }
    #endregion End Of DataAccess Enum

    #region ControlType Enum
    /// <summary>
    ///  ControlType Enum
    /// </summary>
    public enum ControlType
    {
        TextBlock,
        TextBox,
        ComboBox,
    }
    #endregion End Of ControlType Enum

    public enum ReturnStatus
    {
        Success,
        Fail
    }

    public enum ConnectorType
    {

        ModbusEthernet = 0,
        ModbusSerial = 1,
        ABMicroLogixEthernet = 2,
        ABControlLogix = 3,
        ABCompactLogix = 4,
        TcpSocket = 5

    }

    public enum InvalidFloatValues
    {
        ReplaceWithZero,
        UnModified
    }

    //public enum NetworkAdaptor //TODO: Need to delete --Done
    //{
    //    IPAddress,
    //    NetWorkCard
    //}

    public enum DeviceType
    {
        ModbusEthernet,
        ModbusSerial,
        ABControlLogix,
        ABCompactLogix,
        ABMicroLogixEthernet,
        TcpSocketDevice
    }

    public enum AccessPermissions
    {
        sdaReadAccess,
        sdaWriteAccess
    }

    public enum IPType { TCP_IP, UDP }

    public enum LogStatus
    {
        Information,
        Error,
        Warning
    }

    public enum ImageStatus
    {
        InfoImage,
        ErrorImage,
        WarningImage
    }


    public enum EnumDataType
    {
        Bool = 1,
        Byte = 2,
        SByte = 3,
        Int16 = 4,
        UInt16 = 5,
        Int32 = 6,
        UInt32 = 7,
        Int64 = 8,
        UInt64 = 9,
        Float32 = 10,
        Float64 = 11,
        String = 12
    }
    public enum ModbusAddressType
    {
        Coil = 000001,
        DiscreteInput = 100001,
        InputRegister = 300001,
        HoldingRegister = 400001
    }

    public enum Baudrate
    {
        B_9600 = 9600
    }
    public enum AllenBbadleyModel
    {
        //LGX,
        //SLC,
        //PLC5,
        //micro800
        ControlLogix,
        CompactLogix,
        MicroLogix,
        Micro800,
        PLC5
    }

    public enum ChannelType
    {
        ADC,
        RPM,
        _2AMS

    }
    public enum ChannelAddress
    {
        ch0,
        ch1,
        ch2,
        ch3,
        ch4,
        ch5,
        ch6,
        ch7,
        ch8,
        ch9,
        ch10,
        ch11,
        ch12,
        ch13,
        ch14,
        ch15,
        start_stop

    }
    public enum RPM
    {
       
        ch13,
        ch14


    }
    public enum _2AMS
    {
        ch15

    }
    public enum AllowedSamplingRates
    {
        Rate100 = 100,
        Rate200 = 200,
        Rate300 = 300,
        Rate400 = 400,
        Rate500 = 500
    }
    public enum AdcComputationConstant
    {
        MinConstant_PressureFlow = 0,
        MaxConstant_PressureFlow = 32767,
        MinConstant_Current = 8163,
        MaxConstant_Current = 28555,
        SensorMin_PressureFlow = 4,
        SensorMax_PressureFlow = 20,
        SensorMin_Current = 100,
        SensorMax_Current = 1796,

        



    }
}

#endregion OPCEngine namespace

