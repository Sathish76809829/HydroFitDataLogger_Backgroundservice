﻿using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElpisOpcServer.SunPowerGen
{
    public class PumpTestInformation : TestInformation, IDataErrorInfo, INotifyPropertyChanged
    {

        private string _EquipCustomerName = "HydroFit";// string.Empty;
        private string _pumpJobNumber = "Pump_001";//string.Empty;
        private TestType testName;
        private string pumpReportNumber = "Test001";//string.Empty;
        private string testDateTime;
        private bool isTestStarted = false;
        private string reportStatus = "";
        public string plctrigger = "0";
        private string _PumpTestStatus = "OFF";
        private string plcTriggerTagData = "";
        private string timeInterval = "00:00:00:000";
        private ObservableCollection<Elpis.Windows.OPC.Server.Tag> _tagInformation;
        private string _equipManufacturer = "HydroFit";// string.Empty;
        private string _equipType = "SensorType";//string.Empty;
        private string _equipModelNo = "Hyd_2";//string.Empty;
        private string _testManufacture = "HYDROFIT ";//ConfigurationSettings.AppSettings["Manufacturer"];
        private string _testType = "HYDFIT PUMP BENCH"; //ConfigurationSettings.AppSettings["Type"];
        private string _testSerialNo = "220000318";// ConfigurationSettings.AppSettings["Serial No"];
        private string _testRange ="0-450 BAR";//ConfigurationSettings.AppSettings["Range"];
        private string _testedBy = "sathish";//string.Empty;
        private string _witnessedBy = "Sneha";//string.Empty;
        private string _approvedBy = "Venu";//string.Empty;
        private string pumpReportName;
        private int _seriesCounts;
        private string _pumpSPGSerialNo = "HYd_003";//string.Empty;
        private string _equipSerialNo = "HYd_Eqp1";//string.Empty;
        private string _equipControlType="Hyd_Pump";//string.Empty;
        private string _equipPumpType="HYD_test";//string.Empty;
        private List<string> _tableParameterList = new List<string>();
        private string _selectedXaxis;
        private CustomTooltipModel _customTooltip;
        private ObservableCollection<LineSeries> _lineSeriesList;
        private ObservableCollection<List<string>> _labelCollection;
        private Dictionary<string, Dictionary<string, string>> _tableData = new Dictionary<string, Dictionary<string,string>>();

        //private Dictionary<string, List<string>> _tableData;

        public ObservableCollection<Elpis.Windows.OPC.Server.Tag> TagInformation
        {
            get { return _tagInformation; }
            set
            {
                _tagInformation = value;
                OnPropertyChanged(nameof(TagInformation));
            }
        }


        public ObservableCollection<LineSeries> LineSeriesList
        {
            get { return _lineSeriesList; }
            set
            {
                _lineSeriesList = value;
                OnPropertyChanged(nameof(LineSeriesList));
            }
        }

        public ObservableCollection<List<string>> LabelCollection
        {
            get { return _labelCollection; }
            set
            {
                _labelCollection = value;
                OnPropertyChanged(nameof(LabelCollection));
            }
        }

        public Dictionary<string, Dictionary<string, string>> TableData
        {
            get { return _tableData; }
            set
            {
                _tableData = value;
                OnPropertyChanged(nameof(TableData));
            }
        }

        public List<string> TableParameterList
        {
            get { return _tableParameterList; }
            set
            {
                _tableParameterList = value;
                OnPropertyChanged(nameof(TableParameterList));
            }
        }

        public string SelectedXaxis
        {
            get { return _selectedXaxis; }
            set
            {
                _selectedXaxis = value;
                OnPropertyChanged(nameof(SelectedXaxis));
            }
        }




        public string PumpSPGSerialNo
        {
            get { return _pumpSPGSerialNo; }
            set
            {
                _pumpSPGSerialNo = value;
                OnPropertyChanged(nameof(PumpSPGSerialNo));
            }
        }


        public string EquipSerialNo
        {
            get { return _equipSerialNo; }
            set
            {
                _equipSerialNo = value;
                OnPropertyChanged(nameof(EquipSerialNo));
            }
        }

        public string EquipControlType
        {
            get { return _equipControlType; }
            set
            {
                _equipControlType = value;
                OnPropertyChanged(nameof(EquipControlType));
            }
        }

        public string EquipPumpType
        {
            get { return _equipPumpType; }
            set
            {
                _equipPumpType = value;
                OnPropertyChanged(nameof(EquipPumpType));
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        public string ReportStatus
        {
            get { return reportStatus; }
            set
            {
                reportStatus = value;
                OnPropertyChanged("reportStatus");
            }
        }

        public string TimeInterval
        {
            get { return timeInterval; }
            set
            {
                timeInterval = value;
                OnPropertyChanged("TimeInterval");
            }
        }


        public string PlcTriggerTagData
        {
            get { return plcTriggerTagData; }
            set
            {
                plcTriggerTagData = value;
                OnPropertyChanged("PlcTriggerTagData");
            }
        }

        public string PumpTestStatus
        {
            get { return _PumpTestStatus; }
            set
            {
                _PumpTestStatus = value;
                OnPropertyChanged(nameof(PumpTestStatus));
            }
        }


        public int SeriesCounts
        {
            get { return _seriesCounts; }
            set
            {
                _seriesCounts = value;
                OnPropertyChanged(nameof(SeriesCounts));
            }
        }

        public string EqipCustomerName
        {
            get { return _EquipCustomerName; }
            set
            {
                _EquipCustomerName = value;
                OnPropertyChanged(nameof(EqipCustomerName));
            }
        }

        public string EquipManufacturer
        {
            get { return _equipManufacturer; }
            set
            {
                _equipManufacturer = value;
                OnPropertyChanged(nameof(EquipManufacturer));
            }
        }

        public string EquipType
        {
            get { return _equipType; }
            set
            {
                _equipType = value;
                OnPropertyChanged(nameof(EquipType));
            }
        }

        public string EquipModelNo
        {
            get { return _equipModelNo; }
            set
            {
                _equipModelNo = value;
                OnPropertyChanged(nameof(EquipModelNo));
            }
        }

        public string TestManufacture
        {
            get { return _testManufacture; }
            set
            {
                _testManufacture = value;
                OnPropertyChanged(nameof(TestManufacture));
            }
        }

        public string TestType
        {
            get { return _testType; }
            set
            {
                _testType = value;
                OnPropertyChanged(nameof(TestType));
            }
        }

        public string TestSerialNo
        {
            get { return _testSerialNo; }
            set
            {
                _testSerialNo = value;
                OnPropertyChanged(nameof(TestSerialNo));
            }
        }

        public string TestRange
        {
            get { return _testRange; }
            set
            {
                _testRange = value;
                OnPropertyChanged(nameof(TestRange));

            }
        }
        public string TestedBy
        {
            get { return _testedBy; }
            set
            {
                _testedBy = value;
                OnPropertyChanged(nameof(TestedBy));
            }
        }
        public string WitnessedBy
        {
            get { return _witnessedBy; }
            set
            {
                _witnessedBy = value;
                OnPropertyChanged(nameof(WitnessedBy));
            }
        }
        public string ApprovedBy
        {
            get { return _approvedBy; }
            set
            {
                _approvedBy = value;
                OnPropertyChanged(nameof(ApprovedBy));
            }
        }

            
        public string PumpJobNumber
        {
            get { return _pumpJobNumber; }
            set
            {
                _pumpJobNumber = value;
                OnPropertyChanged(nameof(PumpJobNumber));
               
            }
        }

        public TestType TestName
        {
            get { return testName; }
            set
            {
                testName = value;
                OnPropertyChanged("TestName");
            }
        }
        public string PumpReportNumber
        {
            get { return pumpReportNumber; }
            set
            {
                pumpReportNumber = value;
                OnPropertyChanged(nameof(PumpReportNumber));
               
            }
        }

        public string PumpReportName
        {
            get { return pumpReportName; }
            set
            {
                pumpReportName = value;
                OnPropertyChanged(nameof(PumpReportName));
            }
        }

        public bool IsTestStarted
        {
            get { return isTestStarted; }
            set { isTestStarted = value; OnPropertyChanged("IsTestStarted"); }
        }
        public string TestDateTime
        {
            get { return testDateTime; }
            set
            {
                testDateTime = value;
                OnPropertyChanged("TestDateTime");
            }
        }






        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                return Validate(columnName);
            }
        }


        public string PLCTrigger
        {
            get
            {
                return plctrigger;
            }
            set
            {
                plctrigger = value;
                OnPropertyChanged("plctrigger");
            }
        }

        //public Dictionary<string, List<string>> TableData
        //{
        //    get { return _tableData; }
        //    set
        //    {
        //        _tableData = value;
        //        OnPropertyChanged(nameof(TableData));
        //    }
        //}



        private string Validate(string propertyName)
        {
            // Return error message if there is error on else return empty or null string
            string validationMessage = string.Empty;
            if (isTestStarted)
            {
                switch (propertyName)
                {
                    case "PumpJobNumber":
                        if (string.IsNullOrWhiteSpace(PumpJobNumber) || string.IsNullOrEmpty(PumpJobNumber))
                            validationMessage = "Required";
                        break;
                    case "PumpReportNumber":
                        if (string.IsNullOrWhiteSpace(PumpReportNumber) || string.IsNullOrEmpty(PumpReportNumber))
                            validationMessage = "Required";
                        break;
                    case "PumpSPGSerialNo":
                        if (string.IsNullOrWhiteSpace(PumpSPGSerialNo) || string.IsNullOrEmpty(PumpSPGSerialNo))
                            validationMessage = "Required";
                        break;
                    case "EqipCustomerName":
                        if (string.IsNullOrWhiteSpace(EqipCustomerName) || string.IsNullOrEmpty(EqipCustomerName))
                            validationMessage = "Required";
                        break;
                    case "EquipManufacturer":
                        if (string.IsNullOrWhiteSpace(EquipManufacturer) || string.IsNullOrEmpty(EquipManufacturer))
                            validationMessage = "Required";
                        break;
                    case "EquipModelNo":
                        if (string.IsNullOrWhiteSpace(EquipModelNo) || string.IsNullOrEmpty(EquipModelNo))
                            validationMessage = "Required";
                        break;
                    case "EquipType":
                        if (string.IsNullOrWhiteSpace(EquipType) || string.IsNullOrEmpty(EquipType))
                            validationMessage = "Required";
                        break;
                    case "EquipControlType":
                        if (string.IsNullOrWhiteSpace(EquipControlType) || string.IsNullOrEmpty(EquipControlType))
                            validationMessage = "Required";
                        break;
                    case "EquipPumpType":
                        if (string.IsNullOrWhiteSpace(EquipPumpType) || string.IsNullOrEmpty(EquipPumpType))
                            validationMessage = "Required";
                        break;
                    case "EquipSerialNo":
                        if (string.IsNullOrWhiteSpace(EquipSerialNo) || string.IsNullOrEmpty(EquipSerialNo))
                            validationMessage = "Required";
                        break;
                    case "TestManufacture":
                        if (string.IsNullOrWhiteSpace(TestManufacture) || string.IsNullOrEmpty(TestManufacture))
                            validationMessage = "Required";
                        break;
                    case "TestType":
                        if (string.IsNullOrWhiteSpace(TestType) || string.IsNullOrEmpty(TestType))
                            validationMessage = "Required";
                        break;
                    case "TestSerialNo":
                        if (string.IsNullOrWhiteSpace(TestSerialNo) || string.IsNullOrEmpty(TestSerialNo))
                            validationMessage = "Required";
                        break;
                    case "TestRange":
                        if (string.IsNullOrWhiteSpace(TestRange) || string.IsNullOrEmpty(TestRange))
                            validationMessage = "Required";
                        break;
                    case "TestedBy":
                        if (string.IsNullOrWhiteSpace(TestedBy) || string.IsNullOrEmpty(TestedBy))
                            validationMessage = "Required";
                        break;
                    case "WitnessedBy":
                        if (string.IsNullOrWhiteSpace(WitnessedBy) || string.IsNullOrEmpty(WitnessedBy))
                            validationMessage = "Required";
                        break;
                    case "ApprovedBy":
                        if (string.IsNullOrWhiteSpace(ApprovedBy) || string.IsNullOrEmpty(ApprovedBy))
                            validationMessage = "Required";
                        break;


                }
            }
            return validationMessage;
        }

        public CustomTooltipModel CustomTooltip
        {
            get { return _customTooltip; }
            set
            {
                _customTooltip = value;
                OnPropertyChanged(nameof(CustomTooltip));
            }
        }

    }

    public class CustomTooltipModel
    {
        public string SeriesName { get; set; }
        public string SeriesValue { get; set; }
        public string CustomNote { get; set; }
        public string YAxisValue { get; set; }
        public string XAxisValue { get; set; }
        public string xAxisTime { get; set; }
        public int Index { get; set; }
    }
   
}

