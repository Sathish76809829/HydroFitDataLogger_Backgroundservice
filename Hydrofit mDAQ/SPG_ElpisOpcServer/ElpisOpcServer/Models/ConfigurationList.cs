using Elpis.Windows.OPC.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElpisOpcServer.Models
{
    public static class ConfigurationList
    {
        //public static ObservableCollection<Tag> TagConfigurationList
        //{
        //    //get
        //    //{
        //    //    return  new ObservableCollection<Tag>()
        //    //    {
        //    //        new Tag()
        //    //        {
        //    //            TagName="PT_B1_ch0_Block1",
        //    //            SignalId="HydFit_001s0",
        //    //            Address="ch0",
        //    //            Units=Units.bar,
        //    //            BlockType=BlockTypes.Block1,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(0),
        //    //            MaxValue=Convert.ToInt32(40),
        //    //            Divisions=Convert.ToInt32(4)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="PT_B1_ch1_Block1",
        //    //            SignalId="HydFit_001s1",
        //    //            Address="ch1",
        //    //            Units=Units.bar,
        //    //            BlockType=BlockTypes.Block1,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(0),
        //    //            MaxValue=Convert.ToInt32(40),
        //    //            Divisions=Convert.ToInt32(4)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="FM_B1_ch2_Block1",
        //    //            SignalId="HydFit_001s2",
        //    //            Address="ch2",
        //    //            Units=Units.lpm,
        //    //            BlockType=BlockTypes.Block1,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(3),
        //    //            MaxValue=Convert.ToInt32(300),
        //    //            Divisions=Convert.ToInt32(30)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="FM_B1_ch3_Block1",
        //    //            SignalId="HydFit_001s3",
        //    //            Address="ch3",
        //    //            Units=Units.lpm,
        //    //            BlockType=BlockTypes.Block1,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(3),
        //    //            MaxValue=Convert.ToInt32(300),
        //    //            Divisions=Convert.ToInt32(30)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="FM_B1_ch4_Block1",
        //    //            SignalId="HydFit_001s4",
        //    //            Address="ch4",
        //    //            Units=Units.lpm,
        //    //            BlockType=BlockTypes.Block1,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(3),
        //    //            MaxValue=Convert.ToInt32(300),
        //    //            Divisions=Convert.ToInt32(30)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="PCDFM_B1_ch5_Block1",
        //    //            SignalId="HydFit_001s5",
        //    //            Address="ch5",
        //    //            Units=Units.lpm,
        //    //            BlockType=BlockTypes.Block1,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(0),
        //    //            MaxValue=Convert.ToInt32(150),
        //    //            Divisions=Convert.ToInt32(15)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="PTEP_B1_ch6_Block1",
        //    //            SignalId="HydFit_001s6",
        //    //            Address="ch6",
        //    //            Units=Units.bar,
        //    //            BlockType=BlockTypes.Block1,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(0),
        //    //            MaxValue=Convert.ToInt32(6),
        //    //            Divisions=Convert.ToInt32(1)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="PTEP_B1_ch7_Block1",
        //    //            SignalId="HydFit_001s7",
        //    //            Address="ch7",
        //    //            Units=Units.bar,
        //    //            BlockType=BlockTypes.Block1,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(0),
        //    //            MaxValue=Convert.ToInt32(6),
        //    //            Divisions=Convert.ToInt32(1)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="PT_B2_ch8_Block2",
        //    //            SignalId="HydFit_001s8",
        //    //            Address="ch8",
        //    //            Units=Units.bar,
        //    //            BlockType=BlockTypes.Block2,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(0),
        //    //            MaxValue=Convert.ToInt32(40),
        //    //            Divisions=Convert.ToInt32(4)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="PT_B2_ch9_Block2",
        //    //            SignalId="HydFit_001s9",
        //    //            Address="ch9",
        //    //            Units=Units.bar,
        //    //            BlockType=BlockTypes.Block2,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(0),
        //    //            MaxValue=Convert.ToInt32(40),
        //    //            Divisions=Convert.ToInt32(4)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="FM_B2_ch10_Block2",
        //    //            SignalId="HydFit_001s10",
        //    //            Address="ch10",
        //    //            Units=Units.lpm,
        //    //            BlockType=BlockTypes.Block2,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(3),
        //    //            MaxValue=Convert.ToInt32(300),
        //    //            Divisions=Convert.ToInt32(30)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="BPTM_B2_ch11_Block2",
        //    //            SignalId="HydFit_001s11",
        //    //            Address="ch11",
        //    //            Units=Units.lpm,
        //    //            BlockType=BlockTypes.Block2,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(0),
        //    //            MaxValue=Convert.ToInt32(40),
        //    //            Divisions=Convert.ToInt32(4)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="MCDFM_B2_ch12_Block2",
        //    //            SignalId="HydFit_001s12",
        //    //            Address="ch12",
        //    //            Units=Units.lpm,
        //    //            BlockType=BlockTypes.Block2,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(0),
        //    //            MaxValue=Convert.ToInt32(150),
        //    //            Divisions=Convert.ToInt32(15)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="RPM_P_B2_ch13_Block2",
        //    //            SignalId="HydFit_001s13",
        //    //            Address="ch13",
        //    //            Units=Units.Rpm,
        //    //            BlockType=BlockTypes.Block2,
        //    //            DataType=DataType.Integer,
        //    //            MinValue=Convert.ToInt32(200),
        //    //            MaxValue=Convert.ToInt32(1800),
        //    //            Divisions=Convert.ToInt32(180)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="RPM_M_B2_ch14_Block2",
        //    //            SignalId="HydFit_001s14",
        //    //            Address="ch14",
        //    //            Units=Units.Rpm,
        //    //            BlockType=BlockTypes.Block2,
        //    //            DataType=DataType.Integer,
        //    //            MinValue=Convert.ToInt32(200),
        //    //            MaxValue=Convert.ToInt32(1800),
        //    //            Divisions=Convert.ToInt32(180)
        //    //        },
        //    //        new Tag()
        //    //        {
        //    //            TagName="Temp_B2_ch15_Block2",
        //    //            SignalId="HydFit_001s15",
        //    //            Address="ch15",
        //    //            Units=Units.Ams,
        //    //            BlockType=BlockTypes.Block2,
        //    //            DataType=DataType.Float,
        //    //            MinValue=Convert.ToInt32(0),
        //    //            MaxValue=Convert.ToInt32(2000),
        //    //            Divisions=Convert.ToInt32(200)
        //    //        }
        //    //    };

        //    //}
        //}
    }
}
