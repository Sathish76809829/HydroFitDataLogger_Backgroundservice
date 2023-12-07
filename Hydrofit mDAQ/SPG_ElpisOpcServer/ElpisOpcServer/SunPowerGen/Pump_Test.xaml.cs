using Elpis.Windows.OPC.Server;
using ElpisOpcServer.Configurations;
using ElpisOpcServer.Models;
using ElpisOpcServer.SocketService.Net;
using ElpisOpcServer.SocketService.SocketServer;
using ElpisOpcServer.SunPowerGen;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Extensions.Options;
using Modbus;
using OPCEngine.View_Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Linq;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Elpis.Embedded.SignalValueConverter.Interfaces;
using Elpis.Embedded.SignalValueConverter.ValueConverters;
using Elpis.Embedded.SignalValueConverter.Enums;
using Microsoft.Extensions.Logging;
using ElpisOpcServer.ReportTemplete;
using LiveCharts.Defaults;
using ElpisOpcServer.Diagnostics;



////DataType Code commented because of Hydrofit 
/// <summary>
/// 
/// </summary>
namespace ElpisOpcServer.SunPowerGen
{
    /// <summary>
    /// Interaction logic for Pump_Test.xaml
    /// </summary>
    public partial class Pump_Test : UserControl
    {
        BitmapImage starticon = new BitmapImage();
        BitmapImage stopicon = new BitmapImage();
        private DispatcherTimer dispatcherTimer;
       
        public static DispatcherTimer Testdurationtimer = new DispatcherTimer();
        public Stopwatch TestdurationStopwatch = new Stopwatch();
        public static DispatcherTimer Dequedispatcher = new DispatcherTimer();
        public static DispatcherTimer AutoReportGeneration = new DispatcherTimer();
        string currentTime = string.Empty;
       
        private bool isInitialPressureSet;
        private int startAddressPos;
        private int retryCount;
        private static string connectorName;
        private string deviceName;
        public static string channelConfigJsonString;
        public static string StartStopCmd;
        public static string UpdateCmd;
        public static List<Tag> selectedTagNames = new List<Tag>();
        

        public static List<Tuple<string, Tag>> seletedItems = new List<Tuple<string, Tag>>();
        private List<string> allTags = new List<string>();
        private int dataCounts;
        public static List<string> Chart1xAxisParaValue { get; set; }
        public static List<string> Chart1xAxisTimeValue { get; set; }
        public static List<string> Chart2xAxisParaValue { get; set; }
        public List<string> Chart2xAxisTimeValue { get; set; }
        public Func<double, string> YFormatter { get; set; }
        private byte slaveId { get; set; }
        public SeriesCollection chart1SeriesCollections { get; private set; }
        public SeriesCollection chart2SeriesCollections { get; private set; }
        public SeriesCollection chart1XaxisSeriesCollection { get; private set; }
        public static AxesCollection Chart1YAxisCollection = new AxesCollection();
        public static AxesCollection Chart1XAxisCollection = new AxesCollection();

        public static AxesCollection Chart2YAxisCollection = new AxesCollection();
        public static AxesCollection Chart2XAxisCollection = new AxesCollection();
        public Axis YAxis { get; private set; }
        public Axis XAxisPara { get; private set; }
        public Axis XAxisTime { get; private set; }
        Brush[] graphStrokes = new Brush[] { Brushes.DarkOrange, Brushes.DarkGreen, Brushes.DarkBlue, Brushes.Brown, Brushes.Red, Brushes.DarkCyan, Brushes.DarkMagenta, Brushes.DarkOliveGreen, Brushes.DarkOrange, Brushes.DarkSalmon };
        //private string formulaPara1;
        //private string formulaPara2;
        public Stopwatch stopwatch = new Stopwatch();

        public static Dictionary<string, List<string>> tempTable = new Dictionary<string, List<string>>();
        public static SocketConnection _connection;/*=new SocketConnection();*/
        public static int packetlength;
        private int time = 0;
        private static readonly Elpis.Embedded.SignalValueConverter.Interfaces.IValueConverter valueConverter = new SignalValueConverter();
      

        public double PressureReadTime { get; }
        public static ObservableCollection<Tag> TagsCollection { get; /*private*/ set; }
        
        public LineSeries PumpSeries { get; private set; }
        public AxesCollection AxisYCollection { get; }
        public static List<string> Chart1YaxisParaList = new List<string>();
        public static List<string> Chart2YaxisParaList = new List<string>();
        public static Pump_Test PumpTest { get; private set; }
        public string dequeuedata = null;
        OpenLoopExcelReport openLoopExcel = new OpenLoopExcelReport();
        
        public List<TagConfigaration> tagConfigaration;
        public static List<string[]> OpenExcelTableData = new List<string[]>();
        private static readonly ILogger _logger;

        static Pump_Test()
        {
            _logger = new Logger();
           

        }
        public Pump_Test()
        {

            InitializeComponent();
            
            PumpTest = this;
            btnStart.IsEnabled = false;
            DataContext = this;
            if (HomePage.PumpTestInformation == null)
                HomePage.PumpTestInformation = new PumpTestInformation();
            HomePage.PumpTestInformation.TestName = TestType.PumpTest;
            HomePage.PumpTestInformation.TableData.Add("Time", new Dictionary<string, string>());
            HomePage.PumpTestInformation.TableParameterList.Add("Time");
            //HomePage.PumpTestInformation.TableData = new Dictionary<string, List<string>>();
            connectorName = HomePage.SelectedConnector;
            deviceName = HomePage.SelectedDevice;
            #region Configuration load from File
            TagsCollection = Helper.GetTagsCollection(TestType.PumpTest, connectorName, deviceName);
            #endregion Configuration load from File
            HomePage.PumpTestInformation.TagInformation = TagsCollection;
            if (TagsCollection != null)
            {
                foreach (var item in TagsCollection)
                {
                    allTags.Add(item.TagName + " (" + item.Units + ")");
                }
                // lstAllTags.ItemsSource = allTags;
            }

            this.gridCeritificateInfo.DataContext = HomePage.PumpTestInformation;

            PressureReadTime = double.Parse(ConfigurationManager.AppSettings["PressureReadInterval"].ToString());
            starticon.BeginInit();
            starticon.UriSource = new Uri("/ElpisOpcServer;component/Images/starticon.png", UriKind.Relative);
            starticon.EndInit();
            stopicon.BeginInit();
            stopicon.UriSource = new Uri("/ElpisOpcServer;component/Images/stopicon.png", UriKind.Relative);
            stopicon.EndInit();
            dispatcherTimer = new DispatcherTimer();
           
            HomePage.DeviceObject = new DeviceBase();

            #region Dispatchers of PumpTest_Test
            #region Old DataRecevie DispatcherTimer
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Convert.ToDouble(HomePage.DeviceObject.SamplingRate));
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            #endregion Old DataRecevie DispatcherTimer
            #region  Hydrofit test duration
            Testdurationtimer.Tick += timer_Tick;
            Testdurationtimer.Interval = TimeSpan.FromSeconds(10);
            #endregion
            #region DequeDispatcher Graph Rendering
            Dequedispatcher.Tick += Dequedispatcher_Tick;
            Dequedispatcher.Interval = TimeSpan.FromMilliseconds(Convert.ToDouble(HomePage.DeviceObject.SamplingRate));
            //Dequedispatcher.Interval = TimeSpan.FromMilliseconds(500);
            #endregion DequeDispatcher Graph Rendering
            #region Auto ReportGeneration
            AutoReportGeneration.Tick += AutoReportGeneration_Tick;
            var tcpdevice = SunPowerGenMainPage.DeviceObject as TcpSocketDevice;
            AutoReportGeneration.Interval = TimeSpan.FromSeconds(Convert.ToDouble(tcpdevice.AutoModeTime));
            #endregion AutoRepor tGeneration

            #endregion Dispatchers of PumpTest_Test

        }





        private void AutoReportGeneration_Tick(object sender, EventArgs e)
        {
            btnGenerateReport_Click(sender, null);

        }

        private void Dequedispatcher_Tick(object sender, EventArgs e)
        {
            while (!SocketConnection.concurrentQueue.IsEmpty)
            {
                SocketConnection.concurrentQueue.TryDequeue(out dequeuedata);
                Debug.Print($"Dequedata:{dequeuedata}:{DateTime.Now.Millisecond}");
                Debug.Print($"$$$$$**Dequedata from the que**$$$$$");
                Tcp_DataReceived(dequeuedata);

            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (TestdurationStopwatch.IsRunning)
            {

                TimeSpan ts = TestdurationStopwatch.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Hours, ts.Minutes, ts.Seconds);
                // TbCountDown.Text = currentTime;
                LblCountDown.Content = currentTime;
            }

        }
      

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            //ReadDeviceData();


        }




        private async void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PumpTestTypeUpdate();

                if (lblStartStop.Content.ToString() == "Connect")
                {


                    bool isValid = ValidateInputs();
                    if (isValid)
                    {

                        if (rbtn_blockA.IsChecked == true || rbtn_blockB.IsChecked == true)
                        {

                            var tcpdevice = SunPowerGenMainPage.DeviceObject as TcpSocketDevice;
                            #region Chart1 Graph
                            if (chart1cmbXAxis.SelectedItem != null && chart2cmbXAxis.SelectedItem == null)
                            {
                                if (Chart1YaxisParaList.Count > 0)
                                {
                                    if (selectedTagNames.Count > 0)
                                    {
                                        connectorName = HomePage.SelectedConnector;
                                        deviceName = HomePage.SelectedDevice;
                                        
                                        if (chart1SeriesCollections == null)
                                        {
                                            if (!string.IsNullOrEmpty(connectorName) && !string.IsNullOrEmpty(deviceName))
                                            {
                                                this.IsHitTestVisible = false;
                                                this.Cursor = Cursors.Wait;
                                                lblStartStop.Content = "DisConnect";
                                                imgStartStop.Source = stopicon;
                                                btnStartStop.ToolTip = "DisConnect";
                                                //  tbxDeviceStatus.Text = "";

                                                //seletedItems = new List<Tuple<string, Tag>>();
                                                Chart1xAxisParaValue = new List<string>();
                                                Chart1xAxisTimeValue = new List<string>();
                                                if (TagsCollection != null && TagsCollection.Count > 0)
                                                {

                                                    #region SelectedItems_TagCollection
                                                    //foreach (var tag in TagsCollection)
                                                    //{
                                                    //    foreach (var seletedItem in selectedTagNames)
                                                    //    {
                                                    //        if (seletedItem.TagName.ToLower().Contains(tag.TagName.ToLower()))
                                                    //        {
                                                    //            seletedItems.Add(new Tuple<string, Tag>(seletedItem.TagName, tag));
                                                    //        }
                                                    //    }
                                                    //}
                                                    #endregion SelectedItems_TagCollection
                                                    //SelectedItems_TagCollection();
                                                    
                                                    //ExtratConfigPacket();
                                                    
                                                    if (seletedItems != null)
                                                    {
                                                       await ConnectDeviceAsync(ElpisOPCServerMainWindow.TcpSocketClient.IsConnected);
                                                        this.IsHitTestVisible = true;
                                                        this.IsHitTestVisible = true;
                                                        if (tbxDeviceStatus.Text == "Connected")
                                                        {
                                                            //UpdateConfigCommand();
                                                           
                                                            ElpisOPCServerMainWindow.homePage.LblDeviceIDValue.Content = tcpdevice.DeviceId;
                                                            ElpisOPCServerMainWindow.homePage.PumpTestDisableInputs(true);

                                                            btnReset.IsEnabled = false;
                                                            


                                                            ElpisOPCServerMainWindow.homePage.ReportTab.IsEnabled = true;
                                                            ElpisOPCServerMainWindow.homePage.txtFilePath.IsEnabled = false;

                                                            chart1SeriesCollections = new SeriesCollection();
                                                            chart1.AxisY.Clear();


                                                            var chart1Xaxis = seletedItems.Find(item => item.Item1 == chart1cmbXAxis.SelectedItem.ToString());
                                                            #region old chart x-axis working code
                                                            //plot chart 1 xAxis
                                                            try
                                                            {
                                                                if (chart1Xaxis != null)
                                                                {
                                                                    chart1.AxisX.Clear();
                                                                    //Chart1XAxisCollection = new AxesCollection();
                                                                    // var XAxisPara = new Axis { Title = string.Format(chart1Xaxis.Item1 + "( Time )") };
                                                                    //var XAxisPara = new Axis { Title = $"{chart1Xaxis.Item1}{"("} {chart1Xaxis.Item2.Units}{")"}" };
                                                                    var XAxisPara = new Axis { Title = string.Format("{0}({1})", chart1Xaxis.Item1, chart1Xaxis.Item2.Units), MinValue = chart1Xaxis.Item2.MinValue, MaxValue = chart1Xaxis.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = chart1Xaxis.Item2.Divisions }/*, LabelsRotation = 90 */};
                                                                    XAxisPara.LabelFormatter = value => (value).ToString("F2");
                                                                    XAxisPara.Foreground = Brushes.Black;
                                                                    Chart1XAxisCollection.Add(XAxisPara);
                                                                    chart1.AxisX = Chart1XAxisCollection;
                                                                }
                                                            }
                                                            catch (Exception)
                                                            {

                                                                throw;
                                                            }
                                                            #endregion old chart x-axis working code

                                                            //plot yaxis of chart1
                                                            #region old chart y-axis working code
                                                            if (Chart1YaxisParaList != null)
                                                            {
                                                                foreach (var item in Chart1YaxisParaList)
                                                                {
                                                                    var yAxis = seletedItems.Find(q => q.Item1 == item);
                                                                    if (yAxis != null)
                                                                    {

                                                                        var PumpSeries = new LineSeries
                                                                        {

                                                                            //Title = string.Format("{0}{1}", yAxis.Item1, yAxis.Item2.Units),
                                                                            Title = yAxis.Item1,
                                                                            Values = new ChartValues<ObservablePoint>(),
                                                                            PointGeometrySize = 5,
                                                                            ScalesYAt = chart1SeriesCollections.Count,
                                                                            ScalesXAt = 0,
                                                                            Fill = Brushes.Transparent
                                                                        };
                                                                        //YAxis = new Axis { Title = tupleItem.Item1, MinValue = tupleItem.Item2.MinValue, MaxValue = tupleItem.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = tupleItem.Item2.MaxValue / 10 } };
                                                                        var YAxis = new Axis { Title = string.Format("{0}({1})", yAxis.Item1, yAxis.Item2.Units), MinValue = yAxis.Item2.MinValue, MaxValue = yAxis.Item2.MaxValue, ToolTip = FrameworkElement.ToolTipProperty, Separator = new LiveCharts.Wpf.Separator() { Step = yAxis.Item2.Divisions }, };
                                                                        //var YAxis = new Axis { Title = string.Format("{0}{1}", yAxis.Item1, yAxis.Item2.TagId), MinValue = yAxis.Item2.MinValue, MaxValue = yAxis.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = yAxis.Item2.Divisions } };
                                                                        //YAxis = new Axis { Title = tupleItem.Item1};
                                                                        YAxis.LabelFormatter = value => (value).ToString("F2");

                                                                        YAxis.Foreground = Brushes.Black;
                                                                        Chart1YAxisCollection.Add(YAxis);
                                                                        chart1.AxisY = Chart1YAxisCollection;

                                                                        chart1SeriesCollections.Add(PumpSeries);
                                                                    }
                                                                }


                                                            }
                                                            #endregion old chart y-axis working code
                                                            ////plot yaxis of chart2
                                                            #region old code
                                                            //foreach (var tupleItem in seletedItems)
                                                            //{
                                                            //    #region chart1 graph plot
                                                            //    //if (chart1chkXaxis.IsChecked == true && tupleItem.Item1 == chart1cmbXAxis.SelectedItem.ToString())
                                                            //    /*rbtn_blockA.IsChecked == true||rbtn_blockB.IsChecked==true &&
                                                            //    if (tupleItem.Item1 == chart1cmbXAxis.SelectedItem.ToString())
                                                            //    {
                                                            //        chart1.AxisX.Clear();
                                                            //        XAxisPara = new Axis { Title = string.Format(tupleItem.Item1 + "( Time )") };
                                                            //        XAxisPara.LabelFormatter = value => (value).ToString();
                                                            //        XAxisPara.Foreground = Brushes.Black;
                                                            //        XAxisCollection.Add(XAxisPara);
                                                            //        chart1.AxisX = XAxisCollection;

                                                            //        //XAxisTime = new Axis { Title = "Time" };
                                                            //        //XAxisPara.LabelFormatter = value =>value.ToString("HH: mm:ss tt");
                                                            //        //XAxisCollection.Add(XAxisTime);

                                                            //        //pumpTestChart.AxisX = XAxisCollection;


                                                            //    }
                                                            //    // TODO HYDROFIT
                                                            //    //if(tupleItem.Item1 != chart1cmbXAxis.SelectedItem.ToString())
                                                            //    ////if (chart1chkXaxis.IsChecked == false)
                                                            //    //{
                                                            //    //    chart1.AxisX.Clear();
                                                            //    //    XAxisTime = new Axis { Title = "Time" };
                                                            //    //    XAxisTime.Foreground = Brushes.Black;
                                                            //    //    chart1.AxisX.Add(XAxisTime);

                                                            //    //}
                                                            //    //if(tupleItem.Item1 != chart1cmbXAxis.SelectedItem.ToString())
                                                            //    //if (chart1chkXaxis.IsChecked == true)
                                                            //    if (chart1cmbXAxis.SelectedItem.ToString() != null)
                                                            //    {
                                                            //        foreach (var item in Chart1YaxisParaList)
                                                            //        {
                                                            //            if (item == tupleItem.Item1)
                                                            //            {
                                                            //                PumpSeries = new LineSeries
                                                            //                {
                                                            //                    Title = tupleItem.Item1,
                                                            //                    Values = new ChartValues<double>(),
                                                            //                    PointGeometrySize = 5,
                                                            //                    ScalesYAt = chart1SeriesCollections.Count,
                                                            //                    ScalesXAt = 0,
                                                            //                    Fill = Brushes.Transparent
                                                            //                };
                                                            //                //YAxis = new Axis { Title = tupleItem.Item1, MinValue = tupleItem.Item2.MinValue, MaxValue = tupleItem.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = tupleItem.Item2.MaxValue / 10 } };
                                                            //                YAxis = new Axis { Title = tupleItem.Item1, MinValue = tupleItem.Item2.MinValue, MaxValue = tupleItem.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = tupleItem.Item2.Divisions } };
                                                            //                //YAxis = new Axis { Title = tupleItem.Item1};
                                                            //                YAxis.LabelFormatter = value => (value).ToString("F2");
                                                            //                YAxis.Foreground = Brushes.Black;
                                                            //                YAxisCollection.Add(YAxis);
                                                            //                chart1.AxisY = YAxisCollection;

                                                            //                chart1SeriesCollections.Add(PumpSeries);
                                                            //            }
                                                            //        }

                                                            //    }
                                                            //    #endregion chart1 grph plot

                                                            //    #region chart2 grph plot

                                                            //    if (chart2cmbXAxis.SelectedItem.ToString() != null)
                                                            //    {
                                                            //        foreach (var item in Chart2YaxisParaList)
                                                            //        {
                                                            //            if (item == tupleItem.Item1)
                                                            //            {
                                                            //                PumpSeries = new LineSeries
                                                            //                {
                                                            //                    Title = tupleItem.Item1,
                                                            //                    Values = new ChartValues<double>(),
                                                            //                    PointGeometrySize = 5,
                                                            //                    ScalesYAt = chart2SeriesCollections.Count,
                                                            //                    ScalesXAt = 0,
                                                            //                    Fill = Brushes.Transparent


                                                            //                };
                                                            //                //YAxis = new Axis { Title = tupleItem.Item1, MinValue = tupleItem.Item2.MinValue, MaxValue = tupleItem.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = tupleItem.Item2.MaxValue / 10 } };
                                                            //                YAxis = new Axis { Title = tupleItem.Item1, MinValue = tupleItem.Item2.MinValue, MaxValue = tupleItem.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = tupleItem.Item2.Divisions } };
                                                            //                //YAxis = new Axis { Title = tupleItem.Item1};
                                                            //                YAxis.LabelFormatter = value => (value).ToString("F2");
                                                            //                YAxis.Foreground = Brushes.Black;
                                                            //                YAxisCollection.Add(YAxis);
                                                            //                chart2.AxisY = YAxisCollection;

                                                            //                chart2SeriesCollections.Add(PumpSeries);
                                                            //            }
                                                            //        }

                                                            //    }
                                                            //    #endregion chart2 grph plot
                                                            //    // TODO HYDROFIT
                                                            //    // else if (chart1chkXaxis.IsChecked == false)
                                                            //    //{
                                                            //    //    PumpSeries = new LineSeries
                                                            //    //    {
                                                            //    //        Title = tupleItem.Item1,
                                                            //    //        Values = new ChartValues<double>(),
                                                            //    //        PointGeometrySize = 5,
                                                            //    //        ScalesYAt = SeriesCollections.Count,
                                                            //    //        ScalesXAt = 0,
                                                            //    //        Fill = Brushes.Transparent

                                                            //    //    };
                                                            //    //    //YAxis = new Axis { Title = tupleItem.Item1, MinValue = tupleItem.Item2.MinValue, MaxValue = tupleItem.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = tupleItem.Item2.MaxValue / 10 } };
                                                            //    //    YAxis = new Axis { Title = tupleItem.Item1, MinValue = tupleItem.Item2.MinValue, MaxValue = tupleItem.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = tupleItem.Item2.Divisions } };
                                                            //    //    // YAxis = new Axis { Title = tupleItem.Item1 };
                                                            //    //    YAxis.LabelFormatter = value => (value).ToString("F2");
                                                            //    //    YAxis.Foreground = Brushes.Black;
                                                            //    //    YAxisCollection.Add(YAxis);
                                                            //    //    chart1.AxisY = YAxisCollection;
                                                            //    //    SeriesCollections.Add(PumpSeries);
                                                            //    //}




                                                            //}

                                                            //if (isformualParaSelect)
                                                            //{
                                                            //PumpSeries = new LineSeries
                                                            //{
                                                            //    Title = "Computed Value",
                                                            //    Values = new ChartValues<double>(),
                                                            //    PointGeometrySize = 5,
                                                            //    ScalesYAt = SeriesCollections.Count,
                                                            //    ScalesXAt = 0,

                                                            //};
                                                            //YAxis = new Axis { Title = "Computed Value" };
                                                            //YAxis.LabelFormatter = value => (value + 100.00).ToString("N2");
                                                            //YAxisCollection.Add(YAxis);
                                                            //pumpTestChart.AxisY = YAxisCollection;
                                                            //SeriesCollections.Add(PumpSeries);
                                                            //}
                                                            #endregion
                                                            chart1.Series = chart1SeriesCollections;
                                                            //TO DO FOR HYDROFIT
                                                            chart1.LegendLocation = LiveCharts.LegendLocation.Top;
                                                            SunPowerGenMainPage.isTestRunning = true;
                                                            TriggerPLC(true);
                                                            //stopwatch.Start();
                                                           // dispatcherTimer.Start();
                                                            //Testdurationtimer.Start();

                                                            HomePage.PumpTestInformation.TestDateTime = DateTime.Now.ToString();
                                                            dataCounts = 0;
                                                            blbStateOFF.Visibility = Visibility.Hidden;
                                                            blbStateON.Visibility = Visibility.Visible;
                                                            //ElpisOPCServerMainWindow.homePage.PumpExpanderCertificate.IsExpanded = false;
                                                            configGrid.IsEnabled = false;
                                                            // SelectionGrid.IsEnabled = false;
                                                            //ReadDeviceData();
                                                            #region Tcpsocketdevice-implementation
                                                            if (SunPowerGenMainPage.DeviceObject.DeviceType == DeviceType.TcpSocketDevice)
                                                            {
                                                                //if (TcpServer.ServerResponse.Contains("getsignallist"))
                                                                //{
                                                                //    string str = Helper.formatConfigData(seletedItems);
                                                                //    Helper.SendConfigPacketToServer(str, TcpServer.client);
                                                                //}
                                                                //if(TcpServer.ServerResponse.Contains("200400000001"))
                                                                //{
                                                                //    StartStopcommand(true/* ref resp*/);
                                                                //}
                                                                //dispatcherTimer.Start();
                                                                //timer.Start();
                                                                //ReadDeviceData();
                                                            }
                                                            #endregion Tcpsocketdevice-implementation
                                                        }
                                                        else
                                                        {
                                                            lblStartStop.Content = "Connect";
                                                            imgStartStop.Source = starticon;
                                                            btnStartStop.ToolTip = "Connect";
                                                            this.IsHitTestVisible = true;
                                                            blbStateOFF.Visibility = Visibility.Visible;
                                                            blbStateON.Visibility = Visibility.Hidden;
                                                            //ElpisOPCServerMainWindow.homePage.PumpExpanderCertificate.IsExpanded = true;
                                                            configGrid.IsEnabled = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        StopTest();
                                                        MessageBox.Show("Configuration file having the invalid Tag Names", "HYD Reporting Tool", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                    }

                                                }
                                                else
                                                {
                                                    StopTest();
                                                    MessageBox.Show("Configuration file having the invalid Tag Names", "HYD Reporting Tool", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                }
                                            }
                                            else
                                            {
                                                StopTest();
                                                MessageBox.Show("Please create tags in configuration section.", "HYD Reporting Tool", MessageBoxButton.OK, MessageBoxImage.Warning);
                                            }
                                        }
                                        else
                                        {
                                            MessageBoxResult messageOption = MessageBox.Show("Please reset all fields by clicking reset button, and start new Data Recording.", "HYD Report Tool", MessageBoxButton.OK, MessageBoxImage.Question);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("please select the Parameters", "HYD Report Tool", MessageBoxButton.OK, MessageBoxImage.Question);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("please select the chart 1 y-axis Parameters", "HYD Report Tool", MessageBoxButton.OK, MessageBoxImage.Question);
                                }


                            }
                            #endregion chart1 Graph
                            #region Chart2 graph
                            else if (chart1cmbXAxis.SelectedItem == null && chart2cmbXAxis.SelectedItem != null)
                            {
                                if (Chart2YaxisParaList.Count > 0)
                                {
                                    if (selectedTagNames.Count > 0)
                                    {
                                        connectorName = HomePage.SelectedConnector;
                                        deviceName = HomePage.SelectedDevice;
                                        if (chart2SeriesCollections == null)
                                        {
                                            if (!string.IsNullOrEmpty(connectorName) && !string.IsNullOrEmpty(deviceName))
                                            {
                                                this.IsHitTestVisible = false;
                                                this.Cursor = Cursors.Wait;
                                                lblStartStop.Content = "DisConnect";
                                                imgStartStop.Source = stopicon;
                                                btnStartStop.ToolTip = "DisConnect";
                                                //  tbxDeviceStatus.Text = "";

                                               // seletedItems = new List<Tuple<string, Tag>>();
                                                Chart2xAxisParaValue = new List<string>();
                                                Chart2xAxisTimeValue = new List<string>();
                                                if (TagsCollection != null && TagsCollection.Count > 0)
                                                {
                                                    #region SelectedItems_TagCollection
                                                    //foreach (var tag in TagsCollection)
                                                    //{
                                                    //    foreach (var seletedItem in selectedTagNames)
                                                    //    {
                                                    //        if (seletedItem.TagName.ToLower().Contains(tag.TagName.ToLower()))
                                                    //        {
                                                    //            seletedItems.Add(new Tuple<string, Tag>(seletedItem.TagName, tag));
                                                    //        }
                                                    //    }
                                                    //}
                                                    #endregion SelectedItems_TagCollection
                                                    //SelectedItems_TagCollection();
                                                    //ExtratConfigPacket();
                                                   // _connection = new SocketConnection();
                                                    if (seletedItems != null)
                                                    {
                                                        await ConnectDeviceAsync(ElpisOPCServerMainWindow.TcpSocketClient.IsConnected);
                                                        this.IsHitTestVisible = true;
                                                        this.IsHitTestVisible = true;
                                                        if (tbxDeviceStatus.Text == "Connected")
                                                        {
                                                            ElpisOPCServerMainWindow.homePage.LblDeviceIDValue.Content = tcpdevice.DeviceId;
                                                            ElpisOPCServerMainWindow.homePage.PumpTestDisableInputs(true);
                                                            //ElpisOPCServerMainWindow.homePage.btnReset.IsEnabled = false;
                                                            //ElpisOPCServerMainWindow.pump_Test.btnReset.IsEnabled = false;

                                                            btnReset.IsEnabled = false;
                                                            //btnGenerateReport.IsEnabled = false;
                                                            //ElpisOPCServerMainWindow.pump_Test.btnReset.IsEnabled = false;
                                                            //ElpisOPCServerMainWindow.pump_Test.btnGenerateReport.IsEnabled = false;

                                                            ElpisOPCServerMainWindow.homePage.ReportTab.IsEnabled = true;
                                                            ElpisOPCServerMainWindow.homePage.txtFilePath.IsEnabled = false;
                                                            //ElpisOPCServerMainWindow.homePage.PumpExpanderCertificate.IsExpanded = true;

                                                            chart2SeriesCollections = new SeriesCollection();
                                                            // chart2SeriesCollections = new SeriesCollection();
                                                            chart2.AxisY.Clear();
                                                            var chart2Xaxis = seletedItems.Find(item => item.Item1 == chart2cmbXAxis.SelectedItem.ToString());
                                                            //plot chart2 xaxis
                                                            if (chart2Xaxis != null)
                                                            {
                                                                chart2.AxisX.Clear();
                                                                //Chart2XAxisCollection = new AxesCollection();
                                                                //var XAxisPara = new Axis { Title = string.Format(chart2Xaxis.Item1 + "( Time )") };
                                                                var XAxisPara = new Axis { Title = string.Format("{0}({1})", chart2Xaxis.Item1, chart2Xaxis.Item2.Units), MinValue = chart2Xaxis.Item2.MinValue, MaxValue = chart2Xaxis.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = chart2Xaxis.Item2.Divisions }/*,LabelsRotation=90*/ };
                                                                XAxisPara.LabelFormatter = value => (value).ToString("F2");

                                                                XAxisPara.Foreground = Brushes.Black;
                                                                Chart2XAxisCollection.Add(XAxisPara);
                                                                chart2.AxisX = Chart2XAxisCollection;
                                                            }

                                                            if (Chart2YaxisParaList != null)
                                                            {
                                                                // Chart2YAxisCollection = new AxesCollection();
                                                                foreach (var item in Chart2YaxisParaList)
                                                                {
                                                                    var yAxis = seletedItems.Find(q => q.Item1 == item);
                                                                    if (yAxis != null)
                                                                    {
                                                                        var PumpSeries = new LineSeries
                                                                        {
                                                                            Title = yAxis.Item1,
                                                                            Values = new ChartValues<ObservablePoint>(),
                                                                            PointGeometrySize = 5,
                                                                            ScalesYAt = chart2SeriesCollections.Count,
                                                                            ScalesXAt = 0,
                                                                            Fill = Brushes.Transparent
                                                                        };
                                                                        //YAxis = new Axis { Title = tupleItem.Item1, MinValue = tupleItem.Item2.MinValue, MaxValue = tupleItem.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = tupleItem.Item2.MaxValue / 10 } };
                                                                        var YAxis = new Axis { Title = string.Format("{0}({1})", yAxis.Item1, yAxis.Item2.Units), MinValue = yAxis.Item2.MinValue, MaxValue = yAxis.Item2.MaxValue, Separator = new LiveCharts.Wpf.Separator() { Step = yAxis.Item2.Divisions } };
                                                                        //YAxis = new Axis { Title = tupleItem.Item1};
                                                                        YAxis.LabelFormatter = value => (value).ToString("F2");
                                                                        YAxis.Foreground = Brushes.Black;
                                                                        Chart2YAxisCollection.Add(YAxis);
                                                                        chart2.AxisY = Chart2YAxisCollection;

                                                                        chart2SeriesCollections.Add(PumpSeries);
                                                                    }

                                                                }
                                                            }
                                                            chart2.Series = chart2SeriesCollections;
                                                            chart2.LegendLocation = LiveCharts.LegendLocation.Top;
                                                            SunPowerGenMainPage.isTestRunning = true;
                                                            TriggerPLC(true);
                                                            //stopwatch.Start();
                                                            //dispatcherTimer.Start();
                                                            //Testdurationtimer.Start();

                                                            HomePage.PumpTestInformation.TestDateTime = DateTime.Now.ToString();
                                                            dataCounts = 0;
                                                            blbStateOFF.Visibility = Visibility.Hidden;
                                                            blbStateON.Visibility = Visibility.Visible;
                                                            //ElpisOPCServerMainWindow.homePage.PumpExpanderCertificate.IsExpanded = false;
                                                            configGrid.IsEnabled = false;

                                                        }
                                                        else
                                                        {
                                                            lblStartStop.Content = "Connect";
                                                            imgStartStop.Source = starticon;
                                                            btnStartStop.ToolTip = "Connect";
                                                            this.IsHitTestVisible = true;
                                                            blbStateOFF.Visibility = Visibility.Visible;
                                                            blbStateON.Visibility = Visibility.Hidden;
                                                            //ElpisOPCServerMainWindow.homePage.PumpExpanderCertificate.IsExpanded = true;
                                                            configGrid.IsEnabled = true;

                                                        }
                                                    }
                                                    else
                                                    {
                                                        StopTest();
                                                        MessageBox.Show("Configuration file having the invalid Tag Names", "HYD Reporting Tool", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                    }

                                                }
                                                else
                                                {
                                                    StopTest();
                                                    MessageBox.Show("Configuration file having the invalid Tag Names", "HYD Reporting Tool", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                }
                                            }
                                            else
                                            {
                                                StopTest();
                                                MessageBox.Show("Please create tags in configuration section.", "HYD Reporting Tool", MessageBoxButton.OK, MessageBoxImage.Warning);
                                            }
                                        }
                                        else
                                        {
                                            MessageBoxResult messageOption = MessageBox.Show("Please reset all fields by clicking reset button, and start new Data Recording.", "HYD Report Tool", MessageBoxButton.OK, MessageBoxImage.Question);

                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("please select the Parameters", "HYD Report Tool", MessageBoxButton.OK, MessageBoxImage.Question);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("please select the chart 2 y-axis Parameters", "HYD Report Tool", MessageBoxButton.OK, MessageBoxImage.Question);
                                }
                            }
                            else if (chart1cmbXAxis.SelectedItem != null && chart2cmbXAxis.SelectedItem != null)
                            {
                                MessageBox.Show("Please select any one chart while testing the ClosedLoop PumpTest.");
                            }
                            #endregion Chart2 graph
                        }




                      
                    }

                    else
                    {
                        MessageBox.Show("Please Fill Configuration Details.");
                    }
                }
                else
                {
                    StopTest();

                }
                this.Cursor = Cursors.Arrow;
            }

            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                ElpisServer.Addlogs("Report Tool", "Start Button-Pump Test", ex.Message, LogStatus.Error);
                StopTest();
            }
        }

        public static void SelectedItems_TagCollection()
        {
            if(seletedItems.Count>0)
            {
                seletedItems.Clear();
            }
            
            foreach (var tag in TagsCollection)
            {
                foreach (var seletedItem in selectedTagNames)
                {
                    if (seletedItem.TagName.ToLower().Contains(tag.TagName.ToLower()))
                    {
                        seletedItems.Add(new Tuple<string, Tag>(seletedItem.TagName, tag));
                    }
                }
            }
        }

        private void PumpTestTypeUpdate()
        {
            if (rbtn_blockA.IsChecked == true)
            {
                HomePage.HomePageUIControl.txtPumpType.Text = "Open Loop";
                HomePage.HomePageUIControl.txtPumpType.FontWeight = FontWeights.Bold;
                HomePage.HomePageUIControl.txtPumpType.Foreground = Brushes.DarkGreen;


            }
            else
            {
                HomePage.HomePageUIControl.txtPumpType.Text = "Closed Loop";
                HomePage.HomePageUIControl.txtPumpType.FontWeight = FontWeights.Bold;
                HomePage.HomePageUIControl.txtPumpType.Foreground = Brushes.DarkGreen;
            }
        }

        #region HydFit StartCommand
        private void TcpStartCommand()
        {
            string data = null;
            var tcpdevice = SunPowerGenMainPage.DeviceObject as TcpSocketDevice;
            int startvalue = 1;
            foreach (var item in TagsCollection)
            {
                if (item.Address.ToLower() == "start-stop")
                {
                    data = $"{item.TagId};{item.Address};{tcpdevice.DeviceId};{startvalue}";
                    //HydFit_001s16;start/stop;HydFit_001;1

                }
            }

            StartStopCmd = System.Text.Json.JsonSerializer.Serialize(data);
            packetlength = StartStopCmd.Length;
        }
        #endregion HydFit StartCommand

        #region HydFit StopCommand
        private void TcpStopCommand()
        {
            string data = null;
            var tcpdevice = SunPowerGenMainPage.DeviceObject as TcpSocketDevice;
            int stopvalue = 0;
            foreach (var item in TagsCollection)
            {
                if (item.Address.ToLower() == "start-stop")
                {
                    data = $"{item.TagId};{item.Address};{tcpdevice.DeviceId};{stopvalue}";
                    //HydFit_001s16;start/stop;HydFit_001;0

                }
            }

            StartStopCmd = System.Text.Json.JsonSerializer.Serialize(data);
            packetlength = StartStopCmd.Length;
        }
        #endregion HydFit StopCommand
        public void UpdateConfigCommand()
        {
            string data = null;
            var tcpdevice = SunPowerGenMainPage.DeviceObject as TcpSocketDevice;
            int UpdateValue = 1;
            foreach (var item in TagsCollection)
            {
                if (item.Address.ToLower() == "upd-signal")
                {
                    //HydFit_001s16;start/stop;HydFit_001;1
                    data = $"{item.TagId};{item.Address};{tcpdevice.DeviceId};{UpdateValue}";
                }
            }

            UpdateCmd = System.Text.Json.JsonSerializer.Serialize(data);
            packetlength = UpdateCmd.Length;
        }
        #region ConfigurationPacket
        public static void ExtratConfigPacket()
        {
            channelConfigJsonString = string.Empty;
            channelConfigJsonString = Helper.StringformatConfigData(seletedItems);
            _logger.LogInfo("ExtractConfigPacket :" + channelConfigJsonString);
            packetlength = channelConfigJsonString.Length;
        }
        #endregion ConfigurationPacket
        internal void ResetPumpTest()
        {
            ElpisOPCServerMainWindow.homePage.PumpTestDisableInputs(false);
            HomePage.PumpTestInformation = null;
            HomePage.PumpTestInformation = new PumpTestInformation();
            this.gridCeritificateInfo.DataContext = HomePage.PumpTestInformation;
            ElpisOPCServerMainWindow.homePage.PumpGridMain.DataContext = HomePage.PumpTestInformation;
            HomePage.PumpTestInformation.IsTestStarted = true;
            blbStateOFF.Visibility = Visibility.Visible;
            blbStateON.Visibility = Visibility.Hidden;
            //SelectionGrid.IsEnabled = true;
            //tablePara.ItemsSource = null;
            chart1cmbXAxis.Items.Clear();
            chart1cmbXAxis.ItemsSource = null;
            chart1YaxisPara.ItemsSource = null;
            Chart1YaxisParaList.Clear();
            chart1YaxisPara.Items.Clear();

            chart2cmbXAxis.Items.Clear();
            chart2cmbXAxis.ItemsSource = null;
            chart2YaxisPara.ItemsSource = null;
            Chart2YaxisParaList.Clear();
            chart2YaxisPara.Items.Clear();
            configGrid.IsEnabled = true;
            if (stopwatch != null)
            {
                stopwatch.Reset();
            }

            rbtnCsv.IsChecked = false;
            rbtnExcel.IsChecked = false;
            // Helper.Indexcount = 0;
            TestdurationStopwatch.Reset();
            // TbCountDown.Text = "00:00:00";
            LblCountDown.Content = "00:00:00";

            HomePage.HomePageUIControl.txtPumpType.Text = "None";

            HomePage.PumpTestInformation.TableData.Add("Time", new Dictionary<string, string>());
            HomePage.PumpTestInformation.TableParameterList.Add("Time");
            connectorName = HomePage.SelectedConnector;
            deviceName = HomePage.SelectedDevice;
            TagsCollection = Helper.GetTagsCollection(TestType.PumpTest, connectorName, deviceName);
            HomePage.PumpTestInformation.TagInformation = TagsCollection;

            ResetTestParametersData();

            if (TagsCollection != null)
            {
                foreach (var item in TagsCollection)
                {

                    allTags.Add(item.TagName + " (" + item.Units + ")");
                }
                //FOR HYDROFIT  CHANGES
                //lstAllTags.ItemsSource = allTags;

                this.gridCeritificateInfo.DataContext = HomePage.PumpTestInformation;
            }
            if (lblStartStop.Content.ToString() == "Stop Record")
                StopTest();
            if (chart1.Series != null && chart1.Series.Count > 0)
            {
                chart1.Series.Clear();
                chart1SeriesCollections.Clear();
                chart1SeriesCollections = null;
                Chart1xAxisParaValue.Clear();
                Chart1xAxisTimeValue.Clear();
                chart1.AxisY.Clear();
                chart1.AxisX.Clear();

            }
            if (chart2.Series != null && chart2.Series.Count > 0)
            {
                chart2.Series.Clear();
                chart2SeriesCollections.Clear();
                chart2SeriesCollections = null;
                Chart2xAxisParaValue.Clear();
                Chart2xAxisTimeValue.Clear();
                chart2.AxisY.Clear();
                chart2.AxisX.Clear();

            }
            for (int i = ChartView1.Children.Count - 1; i > 0; i--)
            {
                ChartView1.Children.Remove(ChartView1.Children[i]);

            }
            for (int i = ChartView2.Children.Count - 1; i > 0; i--)
            {
                ChartView2.Children.Remove(ChartView2.Children[i]);

            }
            this.DataContext = this;

        }

        private void ResetTestParametersData()
        {
            txtTestparameterCurrent.Text = string.Empty;
            txtTestparameterDrain.Text = string.Empty;
            txtTestparameterFlow.Text = string.Empty;
            txtTestparameterPressure.Text = string.Empty;
            txtTestparameterRPM.Text = string.Empty;
            txtTestparameterVolumetricEfficiency.Text = string.Empty;
        }

        private bool ValidateInputs()
        {
            bool isValid = false;
            try
            {
                isValid = true;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.PumpJobNumber) || string.IsNullOrEmpty(HomePage.PumpTestInformation.PumpJobNumber))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.PumpReportNumber) || string.IsNullOrEmpty(HomePage.PumpTestInformation.PumpReportNumber))
                    isValid = isValid && false;
                //TO Do for Hydro fit by sathish
                //if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.PumpSPGSerialNo) || string.IsNullOrEmpty(HomePage.PumpTestInformation.PumpSPGSerialNo))
                //    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.EqipCustomerName) || string.IsNullOrEmpty(HomePage.PumpTestInformation.EqipCustomerName))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.EquipManufacturer) || string.IsNullOrEmpty(HomePage.PumpTestInformation.EquipManufacturer))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.EquipModelNo) || string.IsNullOrEmpty(HomePage.PumpTestInformation.EquipModelNo))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.EquipType) || string.IsNullOrEmpty(HomePage.PumpTestInformation.EquipType))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.EquipControlType) || string.IsNullOrEmpty(HomePage.PumpTestInformation.EquipControlType))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.EquipPumpType) || string.IsNullOrEmpty(HomePage.PumpTestInformation.EquipPumpType))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.EquipSerialNo) || string.IsNullOrEmpty(HomePage.PumpTestInformation.EquipSerialNo))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.TestManufacture) || string.IsNullOrEmpty(HomePage.PumpTestInformation.TestManufacture))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.TestSerialNo) || string.IsNullOrEmpty(HomePage.PumpTestInformation.TestSerialNo))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.TestType) || string.IsNullOrEmpty(HomePage.PumpTestInformation.TestType))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.TestRange) || string.IsNullOrEmpty(HomePage.PumpTestInformation.TestRange))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.TestedBy) || string.IsNullOrEmpty(HomePage.PumpTestInformation.TestedBy))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.WitnessedBy) || string.IsNullOrEmpty(HomePage.PumpTestInformation.WitnessedBy))
                    isValid = isValid && false;
                if (string.IsNullOrWhiteSpace(HomePage.PumpTestInformation.ApprovedBy) || string.IsNullOrEmpty(HomePage.PumpTestInformation.ApprovedBy))
                    isValid = isValid && false;

            }
            catch (Exception ex)
            {
                isValid = false;
            }
            finally
            {
                string reportNumber = HomePage.PumpTestInformation.PumpReportNumber;
                this.gridCeritificateInfo.DataContext = null;
                ElpisOPCServerMainWindow.homePage.PumpGridMain.DataContext = null;
                HomePage.PumpTestInformation.PumpReportNumber = reportNumber;
                this.gridCeritificateInfo.DataContext = HomePage.PumpTestInformation;
                ElpisOPCServerMainWindow.homePage.PumpGridMain.DataContext = HomePage.PumpTestInformation;
            }
            if (isValid)
            {
                return true;
            }
            //ElpisOPCServerMainWindow.homePage.PumpExpanderCertificate.IsExpanded = false;
            else
                //ElpisOPCServerMainWindow.homePage.PumpExpanderCertificate.IsExpanded = true;
                return isValid;
        }

        //private void ConnectDevice()
        public async Task ConnectDeviceAsync(bool isConnected)
        {
            try
            {
               // bool isConnected = true;
                //isConnected = await _connection.StartClient();
                // isConnected = Helper.ConnectingDevice(isConnected, TagsCollection);
                if (isConnected)
                {
                    tbxDeviceStatus.Text = "Connected";
                    _logger.LogInfo("Client_Connected");
                    tbxDeviceStatus.Foreground = Brushes.DarkGreen;
                    startAddressPos = 0;
                    retryCount = 0;
                }
                else
                {
                    tbxDeviceStatus.Text = "Not Connected";
                    _logger.LogWarning("Client_NotConnected");
                    tbxDeviceStatus.Foreground = Brushes.Red;
                }

            }

            catch (Exception)
            {
                tbxDeviceStatus.Text = "Not Connected";
                tbxDeviceStatus.Foreground = Brushes.Red;
            }
        }

        private void TriggerPLC(bool v)
        {
        }

        private void ReadDeviceData()
        {
            try
            {
                #region Old Device TCp Socket connection 
                if (SunPowerGenMainPage.ModbusTcpMaster != null || SunPowerGenMainPage.ModbusSerialPortMaster != null || SunPowerGenMainPage.ABEthernetClient != null || SunPowerGenMainPage.DeviceTcpSocketClient != null && retryCount <= 3)
                {
                    #region Modbus Ethernet
                    if (SunPowerGenMainPage.DeviceObject.DeviceType == DeviceType.ModbusEthernet)
                    {
                        var val = "0";
                        dataCounts++;
                        foreach (var tupleItem in seletedItems)
                        {
                            if (tupleItem.Item2.Address.ToString().Length >= 5)
                            {

                                /// DataType Code commented because of Hydrofit 
                                /// 

                                //try
                                //{
                                //    if ((tupleItem.Item2.Address.ToString()[0]).ToString() == "3")
                                //    {
                                //        val = Helper.ReadDeviceInputRegisterValue(ushort.Parse(tupleItem.Item2.Address.ToString()), Elpis.Windows.OPC.Server.DataType.Short, DeviceType.ModbusEthernet, startAddressPos, slaveId);
                                //    }
                                //    else if ((tupleItem.Item2.Address.ToString()[0]).ToString() == "4")
                                //    {
                                //        val = Helper.ReadDeviceHoldingRegisterValue(ushort.Parse(tupleItem.Item2.Address.ToString()), Elpis.Windows.OPC.Server.DataType.Short, DeviceType.ModbusEthernet, startAddressPos, slaveId);
                                //    }
                                //}
                                //catch (SlaveException)
                                //{
                                //    startAddressPos = 1;
                                //    retryCount++;
                                //}
                                //catch (Exception ex)
                                //{
                                //    ElpisServer.Addlogs("All", "SPG Reporting Tool-Pump Test", ex.Message, LogStatus.Error);
                                //    StopTest();
                                //}
                            }
                            else
                            {

                                val = SunPowerGenMainPage.ModbusTcpMaster.ReadHoldingRegisters(ushort.Parse(tupleItem.Item2.Address), 1)[0].ToString();


                            }
                            UpdateData(val, tupleItem);
                        }

                    }
                    #endregion Modbus Ethernet

                    #region Serial Device
                    else if (SunPowerGenMainPage.DeviceObject.DeviceType == DeviceType.ModbusSerial)
                    {
                        var val = "0";
                        dataCounts++;
                        foreach (var tupleItem in seletedItems)
                        {
                            if (tupleItem.Item2.Address.ToString().Length >= 5)
                            {


                                /// DataType Code commented because of Hydrofit 
                                /// 

                                //try
                                //{
                                //    if ((tupleItem.Item2.Address.ToString()[0]).ToString() == "1")
                                //    {
                                //        val = Helper.ReadDeviceCoilsRegisterValue(ushort.Parse(tupleItem.Item2.Address.ToString()), DeviceType.ModbusSerial, slaveId);
                                //    }
                                //    else if ((tupleItem.Item2.Address.ToString()[0]).ToString() == "2")
                                //    {
                                //        val = Helper.ReadDeviceDiscreteInputRegisterValue(ushort.Parse(tupleItem.Item2.Address.ToString()), DeviceType.ModbusSerial, slaveId);
                                //    }
                                //    else if ((tupleItem.Item2.Address.ToString()[0]).ToString() == "3")
                                //    {
                                //        val = Helper.ReadDeviceInputRegisterValue(ushort.Parse(tupleItem.Item2.Address.ToString()), Elpis.Windows.OPC.Server.DataType.Short, DeviceType.ModbusSerial, 0, slaveId);
                                //    }
                                //    else if ((tupleItem.Item2.Address.ToString()[0]).ToString() == "4")
                                //    {
                                //        val = Helper.ReadDeviceHoldingRegisterValue(ushort.Parse(tupleItem.Item2.Address.ToString()), Elpis.Windows.OPC.Server.DataType.Short, DeviceType.ModbusSerial, 0, slaveId);
                                //    }

                                //}
                                //catch (SlaveException)
                                //{
                                //    startAddressPos = 1;
                                //    retryCount++;
                                //}
                                //catch (Exception ex)
                                //{
                                //    ElpisServer.Addlogs("All", "SPG Reporting Tool-Pump Test", ex.Message, LogStatus.Information);
                                //    StopTest();
                                //}
                            }
                            else
                            {

                                val = SunPowerGenMainPage.ModbusSerialPortMaster.ReadHoldingRegisters(slaveId, ushort.Parse(tupleItem.Item2.Address.ToString()), 1)[0].ToString();


                            }
                            UpdateData(val, tupleItem);
                        }


                    }
                    #endregion Serial Device

                    #region AB Micrologix Ethernet
                    else if (SunPowerGenMainPage.DeviceObject.DeviceType == DeviceType.ABMicroLogixEthernet)
                    {
                        if (Helper.MappedTagList != null)
                        {
                            var val = "0";
                            dataCounts++;
                            foreach (var item in Helper.MappedTagList)
                            {
                                foreach (var Selecteditem in seletedItems)
                                {
                                    if (item.Key.ToLower().Contains(Selecteditem.Item1.ToLower()))
                                    {
                                        val = Helper.ReadEthernetIPDevice(item.Value.Item1, item.Value.Item2);
                                        if (val == null)
                                        {
                                            MessageBox.Show(string.Format("Please, check Pump test device connection for One of the following reason:\n1.Device is disconnected.\n2.Processor Selection Mismatch. \n \n Check Tag: \" {0} \" for one of the following reason \n1.Invalid Tag Address.\n2.Mismatch the Tag DataType.", Selecteditem.Item1), "SPG Report Tool", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                            SunPowerGenMainPage.ABEthernetClient.Dispose();
                                            tbxDeviceStatus.Text = "Not Connected";
                                            tbxDeviceStatus.Foreground = Brushes.Red;
                                            StopTest();
                                            return;

                                        }
                                        UpdateData(val, Selecteditem);
                                    }

                                }

                                #region Old code commented by pravin
                                //if (!string.IsNullOrEmpty(HomePage.PumpTestInformation.Flow) && !string.IsNullOrEmpty(HomePage.PumpTestInformation.Pressure) && !string.IsNullOrEmpty(HomePage.PumpTestInformation.CylinderMovement))
                                //{
                                //    if (item.Key.ToLower().Contains("flow"))
                                //        HomePage.PumpTestInformation.Flow = Helper.ReadEthernetIPDevice(item.Value.Item1, item.Value.Item2);
                                //    if (HomePage.PumpTestInformation.Flow == null)
                                //    {
                                //        SunPowerGenMainPage.ABEthernetClient.Dispose();
                                //        tbxDeviceStatus.Text = "Not Connected";
                                //        tbxDeviceStatus.Foreground = Brushes.Red;
                                //        StopTest();
                                //        MessageBox.Show("Please, check slip stick test device connection. One of the following are reason:\n1.Device is disconnected.\n2.Invalid Tag Address.\n3.Mismatch the Tag DataType.\n4.Processor Selection Mismatch.", "SPG Report Tool", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                //        return;

                                //    }
                                //    else if (item.Key.ToLower().Contains("pressure"))
                                //    {
                                //        if (HomePage.PumpTestInformation.Pressure == null)
                                //        {
                                //            SunPowerGenMainPage.ABEthernetClient.Dispose();
                                //            tbxDeviceStatus.Text = "Not Connected";
                                //            tbxDeviceStatus.Foreground = Brushes.Red;
                                //            StopTest();
                                //            MessageBox.Show("Please, check slip stick test device connection. One of the following are reason:\n1.Device is disconnected.\n2.Invalid Tag Address.\n3.Mismatch the Tag DataType.\n4.Processor Selection Mismatch.", "SPG Report Tool", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                //            return;

                                //        }

                                //        HomePage.PumpTestInformation.Pressure = Helper.ReadEthernetIPDevice(item.Value.Item1, item.Value.Item2);
                                //        if (!isInitialPressureSet)
                                //        {
                                //            isInitialPressureSet = true;
                                //        }


                                //    }
                                //    else if (item.Key.ToLower().Contains("cylindermovement"))
                                //    {
                                //        HomePage.PumpTestInformation.CylinderMovement = Helper.ReadEthernetIPDevice(item.Value.Item1, item.Value.Item2);
                                //        if (HomePage.PumpTestInformation.CylinderMovement == null)
                                //        {
                                //            SunPowerGenMainPage.ABEthernetClient.Dispose();
                                //            tbxDeviceStatus.Text = "Not Connected";
                                //            tbxDeviceStatus.Foreground = Brushes.Red;
                                //            StopTest();
                                //            MessageBox.Show("Please, check slip stick test device connection. One of the following are reason:\n1.Device is disconnected.\n2.Invalid Tag Address.\n3.Mismatch the Tag DataType.\n4.Processor Selection Mismatch.", "SPG Report Tool", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                //            return;

                                //        }
                                //    }

                                //}


                                //if (string.IsNullOrEmpty(txtFlow.Text) || string.IsNullOrEmpty(txtCurrentPressure.Text) || string.IsNullOrEmpty(txtCylinderMovement.Text))
                                //{
                                //    if (PressureSeries.Values.Count > 0)
                                //    {
                                //        HomePage.PumpTestInformation.Pressure = PressureSeries.Values[PressureSeries.Values.Count - 1].ToString();

                                //    }
                                //    else
                                //    {
                                //        HomePage.PumpTestInformation.Pressure = "0";
                                //        HomePage.PumpTestInformation.PressureAfterFirstCylinderMovement = "0";
                                //        HomePage.PumpTestInformation.CylinderMovement = "0";
                                //        HomePage.PumpTestInformation.InitialCylinderMovement = "0";
                                //        HomePage.PumpTestInformation.CylinderFirstMovement = "0";
                                //        HomePage.PumpTestInformation.Flow = "0";
                                //    }
                                //    //SunPowerGenMainPage.ABEthernetClient.Dispose();
                                //    //tbxDeviceStatus.Text = "Not Connected";
                                //    //tbxDeviceStatus.Foreground = Brushes.Red;
                                //    //MessageBox.Show("Please, check slip stick test device conne
                                //    //StopTest();ction. One of the following are reason:\n1.Device is disconnected.\n2.Invalid Tag Address.\n3.Mismatch the Tag DataType.\n4.Processor Selection Mismatch.", "SPG Report Tool", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                //    //return;
                                //}
                                #endregion Old code commented by pravin
                            }
                        }

                    }
                    #endregion AB Micrologix Ethernet                       
                    #region TcpSocket Device 
                    else if (SunPowerGenMainPage.DeviceObject.DeviceType == DeviceType.TcpSocketDevice)
                    {
                        string stream = Helper.TcpDeviceData(SunPowerGenMainPage.DeviceTcpSocketClient);
                        Tcp_DataReceived(stream);
                    }
                    #endregion TcpSocket Device
                    //PressureSeries.Values.Add(double.Parse(txtCurrentPressure.Text.ToString()));
                    //pressureXAxis.Labels = CylinderMovementLabels;
                    //DataContext = this;

                }
                #region TcpSocketServer Device
                //else if (/*_connection.socket.Connected*/)
                //{
                //    if (!SocketConnection.concurrentQueue.IsEmpty)
                //    {
                //        SocketConnection.concurrentQueue.TryDequeue(out dequeuedata);
                //        Debug.Print($"{"Dequedata"}:{dequeuedata}:{DateTime.Now.Millisecond}");
                //        Debug.Print($"{"**Dequedata from the que**"}");
                //        //Tcp_DataReceived(dequeuedata);
                //    }
                //    else
                //    {
                //        Debug.Print($"{DateTime.Now.Millisecond}:{SocketConnection.concurrentQueue.Count},{"que is empty"}");
                //    }
                //    string stream = Helper.TcpDeviceData(TcpServer.client);


                //}
                #endregion TcpSocketServer Device
                else
                {
                    StopTest();
                    ElpisServer.Addlogs("All", "SPG Reporting Tool-Pump Test", string.Format("Retry Count:{0}", retryCount), LogStatus.Information);
                    MessageBox.Show("Problem in connecting device, please check it.", "SPG Reporting Tool", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                #endregion Old Device TCp Socket connection 


            }
            catch (Exception exe)
            {
                ElpisServer.Addlogs("All", "SPG Reporting Tool-Pump Test", string.Format("Error in Read value.{0}.", exe.Message), LogStatus.Error);
                StopTest();
            }
        }



        public static List<string> TabledataArray;
        public static int count = 0;
        public static double VolumetricEfficency;
        public static void Tcp_DataReceived(string obj)
        {
            TabledataArray = new List<string>();
            double ConstantFlow = Convert.ToDouble(HomePage.HomePageUIControl.txtFlowEfficency.Text);
            // Parsing the Data 
            List<parsedDeviceData> parsedData = Helper.ParsedDeviceDatas(obj);
            _logger.LogInfo("-------------->>>>> Tcp_DataReceived() "+DateTime.Now.Millisecond);
            
            Application.Current.Dispatcher.Invoke(() =>
            {

                if (parsedData != null && parsedData.Any())
                {
                    var deviceInfo = DeviceFactory.GetDevice(SunPowerGenMainPage.DeviceObject) as TcpSocketDevice;

                    foreach (var item in seletedItems)
                    {
                        var dataObj = parsedData.FirstOrDefault(e => e.SignalId == item.Item2.TagId);

                        int index = parsedData.IndexOf(dataObj);


                        if (item.Item2.Units.ToString() == Units.Rpm.ToString())
                        {
                            #region code to check


                            #region Adding the  converted data to the File
                            string AdcPhydata = string.Format("{0},{1},{2}", "ConvertFormulaValues", Convert.ToDouble(dataObj.DataValue), Convert.ToDouble(dataObj.DataValue));
                            Helper.AdcAndPhysicalQuantity(AdcPhydata);
                            #endregion Adding the  converted data to the File
                            UpdateData(parsedData, item);
                            double oldRpm = 0;
                            if (oldRpm < Convert.ToDouble(dataObj.DataValue))
                            {
                                oldRpm = Convert.ToDouble(dataObj.DataValue);
                                PumpTest.txtTestparameterRPM.Text = oldRpm.ToString();
                            }
                            //TabledataArray.Add(dataObj.DataValue.ToString());
                           
                            
                            #endregion code to check

                            #region Working Code
                            //var value = deviceData.Where(e => e.signalId == item.Item2.TagId).ToList();

                            //if (value.Count > 0)
                            //{
                            //    Debug.Print($"{item.Item2.TagId}={value[0].value}");
                            //    string AdcPhydata = string.Format("{0},{1},{2}", "ConvertFormulaValues", value[0].value, value[0].value);
                            //    Helper.AdcAndPhysicalQuantity(AdcPhydata);
                            //    UpdateData(value[0].value, item);

                            //}
                            #endregion Working Code
                        }
                        else if (item.Item2.Units.ToString() == Units.Ams.ToString())
                        {
                            #region code to check
                            var realvalue = dataObj.DataValue;
                            dataObj.DataValue = Helper.Calculate2AmsValue(Convert.ToInt32(AdcComputationConstant.SensorMin_Current), Convert.ToInt32(AdcComputationConstant.SensorMax_Current), Convert.ToInt32(AdcComputationConstant.MinConstant_Current), Convert.ToInt32(AdcComputationConstant.MaxConstant_Current), Convert.ToDouble(dataObj.DataValue));
                            parsedData[index].DataValue = dataObj.DataValue;

                            #region Adding the  converted data to the File
                            string AdcPhydata = string.Format("{0},{1},{2}", "ConvertFormulaValues", realvalue, dataObj.DataValue);
                            Helper.AdcAndPhysicalQuantity(AdcPhydata);
                            #endregion Adding the  converted data to the File

                           
                            //TabledataArray.Add(dataObj.DataValue.ToString());
                            UpdateData(parsedData, item);
                            double oldAms = 0;
                            if (oldAms < Convert.ToDouble(dataObj.DataValue))
                            {
                                oldAms = Convert.ToDouble(dataObj.DataValue);
                                PumpTest.txtTestparameterCurrent.Text = oldAms.ToString();
                            }
                            #endregion code to check
                            #region Working Code
                            //var value = deviceData.Where(e => e.signalId == item.Item2.TagId).ToList();

                            //if (value.Count > 0)
                            //{

                            //double TwoAmswidgetvalue = Helper.Calculate2AmsValue(TwoAmssensorMin, TwoAmssensorMax, TwoAmsminConstant, TwoAmsmaxConstant, Convert.ToDouble(value[0].value));
                            //double TwoAmswidgetvalue1 = Helper.Calculate2AmsValue(Convert.ToInt32(AmsComputationInput.AmsSensorMin), Convert.ToInt32(AmsComputationInput.AmsSensorMax), Convert.ToInt32(AmsComputationInput.AmsMinConstant), Convert.ToInt32(AmsComputationInput.AmsMaxConstant), Convert.ToDouble(dataObj.value));

                            //    string AdcPhydata = string.Format("{0},{1},{2}", "ConvertFormulaValues", value[0].value, TwoAmswidgetvalue);
                            //    Helper.AdcAndPhysicalQuantity(AdcPhydata);
                            //    UpdateData(TwoAmswidgetvalue.ToString(), item);

                            //    #region TestParameter MAX Value 

                            //    //if(TwoAmswidgetvalueOld<TwoAmswidgetvalue)
                            //    //{
                            //    //    TwoAmswidgetvalueOld = TwoAmswidgetvalue;
                            //    //    PumpTest.txtTestparameterCurrent.Text = TwoAmswidgetvalueOld.ToString();
                            //    //}
                            //    #endregion TestParameter MAX Value
                            //}
                            #endregion Working Code
                        }
                        else if (item.Item2.Units.ToString() == Units.bar.ToString())
                        {
                            #region code to check
                            //var dataObj = parsedData.FirstOrDefault(e => e.SignalId == item.Item2.TagId);
                            var realvalue = dataObj.DataValue;
                            dataObj.DataValue = valueConverter.GetConvertedValue(Convert.ToDouble(dataObj.DataValue), Convert.ToInt32(AdcComputationConstant.SensorMin_PressureFlow), Convert.ToInt32(AdcComputationConstant.SensorMax_PressureFlow), item.Item2.MinValue, item.Item2.MaxValue, Convert.ToInt32(AdcComputationConstant.MinConstant_PressureFlow), Convert.ToInt32(AdcComputationConstant.MaxConstant_PressureFlow), SignalType.InputSignal);
                            parsedData[index].DataValue = dataObj.DataValue;
                            #region Adding the  converted data to the File
                            string AdcPhydata = string.Format("{0},{1},{2}", "ConvertFormulaValues", realvalue, dataObj.DataValue);
                            Helper.AdcAndPhysicalQuantity(AdcPhydata);
                            #endregion Adding the  converted data to the File
                           

                           // TabledataArray.Add(dataObj.DataValue.ToString());
                            UpdateData(parsedData, item);
                            
                            double oldPressure = 0;
                            if (oldPressure < Convert.ToDouble(dataObj.DataValue))
                            {
                                oldPressure = Convert.ToDouble(dataObj.DataValue);
                                PumpTest.txtTestparameterPressure.Text = oldPressure.ToString();
                            }
                            #endregion code to check
                        }
                        else
                        {
                            #region code to check
                            
                            var realvalue = dataObj.DataValue;
                            dataObj.DataValue = valueConverter.GetConvertedValue(dataObj.DataValue, Convert.ToInt32(AdcComputationConstant.SensorMin_PressureFlow), Convert.ToInt32(AdcComputationConstant.SensorMax_PressureFlow), item.Item2.MinValue, item.Item2.MaxValue, Convert.ToInt32(AdcComputationConstant.MinConstant_PressureFlow), Convert.ToInt32(AdcComputationConstant.MaxConstant_PressureFlow), SignalType.InputSignal);
                            parsedData[index].DataValue = dataObj.DataValue;

                            #region Adding the  converted data to the File
                            string AdcPhydata = string.Format("{0},{1},{2}", "ConvertFormulaValues", realvalue, dataObj.DataValue);
                            Helper.AdcAndPhysicalQuantity(AdcPhydata);
                            #endregion Adding the  converted data to the File

                            //TabledataArray.Add(dataObj.DataValue.ToString());
                            double oldFlow = 0;
                            if (oldFlow < dataObj.DataValue)
                            {
                                oldFlow = dataObj.DataValue;
                                PumpTest.txtTestparameterFlow.Text = oldFlow.ToString();

                            }

                            #region Volumetric Efficency Calculation
                            VolumetricEfficency = Helper.CalVolumetricEfficency(dataObj.DataValue, ConstantFlow);
                            PumpTest.txtTestparameterVolumetricEfficiency.Text = VolumetricEfficency.ToString();
                            #endregion Volumetric Efficency Calculation

                            UpdateData(parsedData, item);


                            #endregion code to check

                            #region Working Code
                            //var value = deviceData.Where(e => e.signalId == item.Item2.TagId).ToList();

                            //if (value.Count > 0)
                            //{

                            //    double Graph_widget_Value = valueConverter.GetConvertedValue(Convert.ToDouble(value[0].value), AdcsensorMin, AdcsensorMax, item.Item2.MinValue, item.Item2.MaxValue, AdcminConstant, AdcmaxConstant, SignalType.InputSignal);

                            //    string AdcPhydata = string.Format("{0},{1},{2}", "ConvertFormulaValues", value[0].value, Graph_widget_Value);
                            //    Helper.AdcAndPhysicalQuantity(AdcPhydata);
                            //    UpdateData(Graph_widget_Value.ToString(), item);
                            //}
                            #endregion Working Code
                        }
                        // UpdateData(parsedData, item);

                    }
                    TabledataArray.Add(VolumetricEfficency.ToString());
                    Helper.AdcAndPhysicalQuantity($"<<<<<<<<<<<<<<----------------------------->>>>>>>>>>>>>>>>>>>>>>>>>>>{DateTime.Now.ToString()}:" + count++);
                   //_logger.LogInfo("-------------->>>>> Exit fron the  Tcp_DataReceived()");
                    _logger.LogInfo("-------------->>>>> Exit from the  Tcp_DataReceived() " + DateTime.Now.Millisecond);

                    #region Working Old Code

                    //if (deviceData.Count > 0)
                    //{
                    //    foreach (var item in seletedItems)
                    //    {

                    //        if (item.Item2.Units.ToString() != "Rpm")
                    //        {
                    //            #region code to check
                    //            //var dataObj = deviceData.FirstOrDefault(e => e.signalId == item.Item2.TagId);

                    //            //dataObj.value = valueConverter.GetConvertedValue(Convert.ToDouble(dataObj.value), AdcsensorMin, AdcsensorMax, item.Item2.MinValue, item.Item2.MaxValue, AdcminConstant, AdcmaxConstant, SignalType.InputSignal).ToString();
                    //            //UpdateData(dataObj.value, item);
                    //            #endregion code to check



                    //            var value = deviceData.Where(e => e.signalId == item.Item2.TagId).ToList();

                    //            if (value.Count > 0)
                    //            {

                    //                double Graph_widget_Value = valueConverter.GetConvertedValue(Convert.ToDouble(value[0].value), AdcsensorMin, AdcsensorMax, item.Item2.MinValue, item.Item2.MaxValue, AdcminConstant, AdcmaxConstant, SignalType.InputSignal);

                    //                string AdcPhydata = string.Format("{0},{1},{2}", "ConvertFormulaValues", value[0].value, Graph_widget_Value);
                    //                Helper.AdcAndPhysicalQuantity(AdcPhydata);
                    //                UpdateData(Graph_widget_Value.ToString(), item);
                    //            }
                    //        }
                    //        else if (item.Item2.Units.ToString() == "Ams")
                    //        {

                    //            var value = deviceData.Where(e => e.signalId == item.Item2.TagId).ToList();

                    //            if (value.Count > 0)
                    //            {

                    //                double TwoAmswidgetvalue = Helper.Calculate2AmsValue(TwoAmssensorMin, TwoAmssensorMax, TwoAmsminConstant, TwoAmsmaxConstant, Convert.ToDouble(value[0].value));

                    //                string AdcPhydata = string.Format("{0},{1},{2}", "ConvertFormulaValues", value[0].value, TwoAmswidgetvalue);
                    //                Helper.AdcAndPhysicalQuantity(AdcPhydata);
                    //                UpdateData(TwoAmswidgetvalue.ToString(), item);

                    //                #region TestParameter MAX Value 

                    //                //if(TwoAmswidgetvalueOld<TwoAmswidgetvalue)
                    //                //{
                    //                //    TwoAmswidgetvalueOld = TwoAmswidgetvalue;
                    //                //    PumpTest.txtTestparameterCurrent.Text = TwoAmswidgetvalueOld.ToString();
                    //                //}
                    //                #endregion TestParameter MAX Value
                    //            }

                    //        }
                    //        else
                    //        {
                    //            var value = deviceData.Where(e => e.signalId == item.Item2.TagId).ToList();

                    //            if (value.Count > 0)
                    //            {
                    //                Debug.Print($"{item.Item2.TagId}={value[0].value}");
                    //                string AdcPhydata = string.Format("{0},{1},{2}", "ConvertFormulaValues", value[0].value, value[0].value);
                    //                Helper.AdcAndPhysicalQuantity(AdcPhydata);
                    //                UpdateData(value[0].value, item);

                    //            }
                    //        }



                    //    }
                    //    Helper.AdcAndPhysicalQuantity($"<<<<<<<<<<<<<<----------------------------->>>>>>>>>>>>>>>>>>>>>>>>>>>{DateTime.Now.ToString()}:" + count++);
                    //}
                    #endregion Working Old Code


                }
                // PumpTest.OpenExcelTableData.Add()
            });

            OpenExcelTableData.Add(TabledataArray.ToArray());

        }
        #region DataParse Convertion
        private static void DataParse(string deviceData, Tuple<string, Tag> item)
        {


            Pump_Test pump = new Pump_Test();
            string convertedData = string.Empty;
            switch (item.Item2.DataType)
            {
                /// DataType Code commented because of Hydrofit  
                /// 

                //case DataType.Boolean:
                //    convertedData = Convert.ToString(Convert.ToInt64(deviceData) > 0 ? 1 : 0);
                //    break;
                //case DataType.Double:
                //    convertedData = Convert.ToString(Convert.ToDouble(deviceData));
                //    break;
                //case DataType.Float:
                //    convertedData = Convert.ToString(Convert.ToDecimal(deviceData));
                //    break;
                //case DataType.Integer:

                //    //Convert.ToString(Math.Round( Convert.ToInt32((deviceData))));
                //    convertedData = Convert.ToString(Convert.ToDouble(deviceData));
                //    //convertedData = Convert.ToString(Convert.ToInt32(deviceData));
                //    break;
                //case DataType.Short:
                //    convertedData = Convert.ToString(Convert.ToDouble(deviceData));

                //    break;
                //case DataType.String:
                //    convertedData = Convert.ToString(deviceData);
                //    break;
                //default:
                //    break;
            }


            // if (!string.IsNullOrEmpty(convertedData))
            //pump.UpdateData(convertedData, item);
        }
        #endregion DataParse Convertion
       
        internal void ConnectorTypeChanged()
        {
            connectorName = HomePage.SelectedConnector;
            deviceName = HomePage.SelectedDevice;
            TagsCollection = Helper.GetTagsCollection(TestType.PumpTest, connectorName, deviceName);

            if (TagsCollection != null)
            {
                bool checkpara = allTags.SequenceEqual(TagsCollection.Select(x => x.TagName).ToList<string>());
                if (TagsCollection.Count != allTags.Count || (!checkpara))
                {
                    HomePage.PumpTestInformation.TagInformation = TagsCollection;

                    //if (lstAllTags.Items.Count > 0)
                    //{
                    //    allTags.Clear();
                    //    lstAllTags.ItemsSource = null;
                    //}
                    //if (lstSelectedTags.Items.Count > 0)
                    //{
                    //    selectedTagNames.Clear();
                    //    lstSelectedTags.ItemsSource = null;
                    //}
                    if (tempTable.Count > 0)
                    {
                        tempTable.Clear();
                    }
                    if (HomePage.PumpTestInformation.TableData.Count > 0)
                    {
                        HomePage.PumpTestInformation.TableData.Clear();
                    }
                    if (HomePage.PumpTestInformation.TableParameterList.Count > 0)
                    {

                        HomePage.PumpTestInformation.TableParameterList.Clear();
                    }

                    HomePage.PumpTestInformation.TableData.Add("Time", new Dictionary<string, string>());
                    HomePage.PumpTestInformation.TableParameterList.Add("Time");
                    foreach (var item in TagsCollection)
                    {
                        if (!allTags.Contains(item.TagName + " (" + item.Units + ")"))
                            allTags.Add(item.TagName + " (" + item.Units + ")");
                    }
                    // lstAllTags.ItemsSource = allTags;

                    this.gridCeritificateInfo.DataContext = HomePage.PumpTestInformation;
                }
            }
            else
            {
                HomePage.PumpTestInformation.TagInformation = null;

                //if (lstAllTags.Items.Count > 0)
                //{
                //    allTags.Clear();
                //    lstAllTags.ItemsSource = null;
                //}
                //if (lstSelectedTags.Items.Count > 0)
                //{
                //    selectedTagNames.Clear();
                //    lstSelectedTags.ItemsSource = null;
                //}
                if (tempTable.Count > 0)
                {
                    tempTable.Clear();
                }
                if (HomePage.PumpTestInformation.TableData.Count > 0)
                {
                    HomePage.PumpTestInformation.TableData.Clear();
                }
                if (HomePage.PumpTestInformation.TableParameterList.Count > 0)
                {

                    HomePage.PumpTestInformation.TableParameterList.Clear();
                }
                HomePage.PumpTestInformation.TableData.Add("Time", new Dictionary<string, string>());
                HomePage.PumpTestInformation.TableParameterList.Add("Time");
                this.gridCeritificateInfo.DataContext = HomePage.PumpTestInformation;

            }
        }
        /// <summary>
        /// this Update function will  update the data based on the recieveTask  in socket connection class
        /// </summary>
        /// <param name="val"></param>
        /// <param name="tupleItem"></param>
        /// 

        #region Old Graph Plotting
        private static void UpdateData(string val, Tuple<string, Tag> tupleItem)
        {
            //TimeSpan ts = stopwatch.Elapsed;

            try
            {

                var selectedxAxis = tempTable.FirstOrDefault(e => e.Key == tupleItem.Item1);
                if (selectedxAxis.Key != null)
                    selectedxAxis.Value.Add(val);
                #region Indudial chart ploting
                if (PumpTest.rbtn_blockA.IsChecked == true || PumpTest.rbtn_blockB.IsChecked == true)
                {

                    #region selection 1 block update the chart 2
                    //else if (chart2cmbXAxis!=null)
                    //{
                    //    if (tupleItem.Item1 == PumpTest.chart2cmbXAxis.SelectedItem.ToString())
                    //    {
                    //        //TimeSpan ts = stopwatch.Elapsed;
                    //        Chart2xAxisParaValue.Add(val/*string.Format(val + " (" + string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds) + " sec)")*/);
                    //        var XAxis = PumpTest.chart2.AxisX.FirstOrDefault(e => e.Title == string.Format("{0}({1})", tupleItem.Item1, tupleItem.Item2.Units)/*string.Format(PumpTest.chart2cmbXAxis.SelectedItem.ToString() + "( Time )")*/);
                    //        XAxis.Labels = Chart2xAxisParaValue;

                    //    }

                    //    else if (PumpTest.chart2.Series.FirstOrDefault(e => e.Title == tupleItem.Item1) != null)
                    //    {
                    //        var ser = PumpTest.chart2.Series.First(e => e.Title == tupleItem.Item1);
                    //        if (ser != null)
                    //        {
                    //            ser.Values.Add(double.Parse(val));
                    //        }
                    //    }
                    //}
                    #endregion selection 1 block update the chart 2
                    if (PumpTest.chart1cmbXAxis.SelectedItem != null)
                    {
                        double valueX = -1;
                        if (tupleItem.Item1 == PumpTest.chart1cmbXAxis.SelectedItem.ToString() /*&& chart2cmbvalue==null||tupleItem.Item1==chart2cmbvalue&& chart1cmbvalue==null*/)
                        {

                            valueX = Math.Round(Convert.ToDouble(val), 2);
                            var XAxis = PumpTest.chart1.AxisX[0]/*string.Format(PumpTest.chart1cmbXAxis.SelectedItem.ToString() + "( Time )")*/;
                           

                        }
                        else if (PumpTest.chart1.Series.Any(e => e.Title == tupleItem.Item1))
                        {
                            PumpTest
                                .chart1
                                .Series
                                .FirstOrDefault(series => series.Title == tupleItem.Item1)
                                .Values
                                .AddRange(new ChartValues<ObservablePoint>
                                {
                                    new ObservablePoint
                                    {
                                        X = valueX,
                                        Y = 0
                                    }
                                });




                            var ser = PumpTest.chart1.Series.First(e => e.Title == /*string.Format("{0}{1}", tupleItem.Item1, tupleItem.Item2.TagId)*/tupleItem.Item1);
                            ser.Values.Add(double.Parse(val));

                           
                        }

                    }
                    #region Chart2 DataUpdate
                    else
                    {
                        if (tupleItem.Item1 == PumpTest.chart2cmbXAxis.SelectedItem.ToString())
                        {
                            //TimeSpan ts = stopwatch.Elapsed;
                            decimal result = decimal.Parse(val);
                            Chart2xAxisParaValue.Add(decimal.Round(result, 2, MidpointRounding.AwayFromZero).ToString()/*string.Format(val + " (" + string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds) + " sec)")*/);
                            // Chart2xAxisParaValue.Add(val/*string.Format(val + " (" + string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds) + " sec)") */);

                            var XAxis = PumpTest.chart2.AxisX.FirstOrDefault(e => e.Title == string.Format("{0}({1})", tupleItem.Item1, tupleItem.Item2.Units)/*string.Format(PumpTest.chart2cmbXAxis.SelectedItem.ToString() + "( Time )")*/);
                            XAxis.Labels = Chart2xAxisParaValue;
                            
                        }

                        else if (PumpTest.chart2.Series.FirstOrDefault(e => e.Title == tupleItem.Item1) != null)
                        {
                            var ser = PumpTest.chart2.Series.First(e => e.Title == tupleItem.Item1);
                            if (ser != null)
                            {

                                decimal result = decimal.Parse(val);
                                
                                ser.Values.Add(double.Parse(val));
                               
                            }
                           
                        }

                    }

                    #endregion Chart2 DataUpdate
                }
                else if (PumpTest.rbtn_blockA.IsChecked == false && PumpTest.rbtn_blockB.IsChecked == true)
                {
                    //if (tupleItem.Item1 == PumpTest.chart2cmbXAxis.SelectedItem.ToString())
                    //{
                    //    //TimeSpan ts = stopwatch.Elapsed;
                    //    decimal result = decimal.Parse(val);
                    //    Chart2xAxisParaValue.Add(decimal.Round(result, 2, MidpointRounding.AwayFromZero).ToString()/*string.Format(val + " (" + string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds) + " sec)")*/);
                    //    // Chart2xAxisParaValue.Add(val/*string.Format(val + " (" + string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds) + " sec)") */);

                    //    var XAxis = PumpTest.chart2.AxisX.FirstOrDefault(e => e.Title == string.Format("{0}({1})", tupleItem.Item1, tupleItem.Item2.Units)/*string.Format(PumpTest.chart2cmbXAxis.SelectedItem.ToString() + "( Time )")*/);
                    //    XAxis.Labels = Chart2xAxisParaValue;

                    //}

                    //else if (PumpTest.chart2.Series.FirstOrDefault(e => e.Title == tupleItem.Item1) != null)
                    //{
                    //    var ser = PumpTest.chart2.Series.First(e => e.Title == tupleItem.Item1);
                    //    if (ser != null)
                    //    {

                    //        decimal result = decimal.Parse(val);
                    //        ser.Values.Add(decimal.Round(result, 2, MidpointRounding.AwayFromZero));
                    //        //ser.Values.Add(double.Parse(val));
                    //        //ser.Values.Add(double.Parse(val));
                    //    }
                    //}

                }
                #endregion  Indudial chart ploting
                /// sathish done radion button  to checkbox changed for hydrofit
                #region Multiple chart selection
                //else if (PumpTest.rbtn_blockA.IsChecked == true || PumpTest.rbtn_blockB.IsChecked == true)
                //{
                //    if (PumpTest.chart1cmbXAxis.SelectedItem != null && tupleItem.Item1 == PumpTest.chart1cmbXAxis.SelectedItem.ToString())
                //    {
                //        //TimeSpan ts = stopwatch.Elapsed;
                //        //Chart1xAxisParaValue.Add(string.Format(val + " (" + string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds) + " sec)"));
                //        //var XAxis = PumpTest.chart1.AxisX.FirstOrDefault(e => e.Title == string.Format(PumpTest.chart1cmbXAxis.SelectedItem.ToString() + "( Time )"));

                //        Chart1xAxisParaValue.Add(val/*string.Format(val + " (" + string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds) + " sec)") */);
                //        var XAxis = PumpTest.chart1.AxisX.FirstOrDefault(e => e.Title == string.Format("{0}({1})", tupleItem.Item1, tupleItem.Item2.Units)/*string.Format(PumpTest.chart1cmbXAxis.SelectedItem.ToString() + "( Time )")*/);
                //        XAxis.Labels = Chart1xAxisParaValue;

                //    }
                //    #region changeing the Xaxis to lineseries
                //    //if (tupleItem.Item1 == PumpTest.chart1cmbXAxis.SelectedItem.ToString())
                //    //{
                //    //    var ser = PumpTest.chart1.Series.First(e => e.Title == /*string.Format("{0}{1}", tupleItem.Item1, tupleItem.Item2.TagId)*/tupleItem.Item1);
                //    //    if (ser != null)
                //    //    {
                //    //        ser.Values.Add(double.Parse(val));
                //    //    }
                //    //}
                //    #endregion changeing the Xaxis to lineseries
                //    // TO DO FOR Chart2Xaxis 
                //    else if (PumpTest.chart2cmbXAxis.SelectedItem != null && tupleItem.Item1 == PumpTest.chart2cmbXAxis.SelectedItem.ToString())
                //    {
                //        //TimeSpan ts = stopwatch.Elapsed;
                //        Chart2xAxisParaValue.Add(val/*string.Format(val + " (" + string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds) + " sec)")*/);
                //        var XAxis = PumpTest.chart2.AxisX.FirstOrDefault(e => e.Title == string.Format("{0}({1})", tupleItem.Item1, tupleItem.Item2.Units)/*string.Format(PumpTest.chart2cmbXAxis.SelectedItem.ToString() + "( Time )")*/);
                //        XAxis.Labels = Chart2xAxisParaValue;

                //    }

                //    #region Old Code for y axis
                //    //else if (chart1.Series.First(e => e.Title == tupleItem.Item1) != null)
                //    //{
                //    //    // here we need to check the the selected tupleitem is related to chart1 or chart2 
                //    //    var ser = chart1.Series.First(e => e.Title == tupleItem.Item1);
                //    //    //var ser1 = chart2.Series.First(e => e.Title == tupleItem.Item1);
                //    //    if (ser != null)
                //    //    {
                //    //        ser.Values.Add(double.Parse(val));
                //    //    }

                //    //}
                //    //else if (chart2.Series.First(e => e.Title == tupleItem.Item1) != null)
                //    //{
                //    //    var ser = chart2.Series.First(e => e.Title == tupleItem.Item1);
                //    //    if (ser != null)
                //    //    {
                //    //        ser.Values.Add(double.Parse(val));
                //    //    }
                //    //}
                //    #endregion Old Code for y axis

                //    #region chart1 & chart2 YAxis 

                //    //else if (PumpTest.chart1.Series.FirstOrDefault(e => e.Title == string.Format("{0}{1}", tupleItem.Item1, tupleItem.Item2.TagId)) != null)
                //    else if (PumpTest.chart1.Series.FirstOrDefault(e => e.Title == tupleItem.Item1) != null)
                //    {
                //        var ser = PumpTest.chart1.Series.First(e => e.Title == /*string.Format("{0}{1}", tupleItem.Item1, tupleItem.Item2.TagId)*/tupleItem.Item1);
                //        if (ser != null)
                //        {
                //            ser.Values.Add(double.Parse(val));
                //        }
                //    }
                //    else if (PumpTest.chart2.Series.FirstOrDefault(e => e.Title == tupleItem.Item1) != null)
                //    {
                //        var ser = PumpTest.chart2.Series.First(e => e.Title == tupleItem.Item1);
                //        if (ser != null)
                //        {
                //            ser.Values.Add(double.Parse(val));
                //        }
                //    }


                //    #endregion chart1 & chart2 YAxis
                //    #region chart code
                //    //else if (tupleItem.Item1 != chart1cmbXAxis.SelectedItem.ToString() && tupleItem.Item1 != chart2cmbXAxis.SelectedItem.ToString())
                //    //{
                //    //    var ser = chart1.Series.First(e => e.Title == tupleItem.Item1);
                //    //    if (ser != null)
                //    //    {
                //    //        ser.Values.Add(double.Parse(val));
                //    //    }
                //    //    var ser1 = chart2.Series.First(e => e.Title == tupleItem.Item1);
                //    //    else
                //    //    {

                //    //        if (ser1 != null)
                //    //        {
                //    //            ser.Values.Add(double.Parse(val));
                //    //        }
                //    //    }
                //    //}
                //    #endregion chart code
                //}
                //else
                //{
                //    //TimeSpan ts = stopwatch.Elapsed;
                //    //xAxisTimeValue.Add(DateTime.Now.ToString("HH:mm:ss tt"));
                //    //Chart1xAxisTimeValue.Add(string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds + " sec"));
                //    //var XAxis = PumpTest.chart1.AxisX.First(e => e.Title == "Time");
                //    //XAxis.Labels = Chart1xAxisTimeValue;
                //    //var ser = PumpTest.chart1.Series.First(e => e.Title == tupleItem.Item1);
                //    //ser.Values.Add(double.Parse(val));
                //}
                #endregion Multiple chart selection
                //}
                //else
                //{
                //    //MessageBox.Show("Testdurationtimer is stop not in running state");
                //}


                #region Formula perpose
                //if (chkFormula.IsChecked == true && cmbFirstPara.SelectedItem != null && cmbSecondPara.SelectedItem != null)
                //{
                //    if (tupleItem.Item1 == cmbFirstPara.SelectedItem.ToString())
                //    {
                //        formulaPara1 = val;
                //    }
                //    if (tupleItem.Item1 == cmbSecondPara.SelectedItem.ToString())
                //    {
                //        formulaPara2 = val; ;
                //    }
                //    if (formulaPara1 != null && formulaPara2 != null)
                //    {
                //        var para = pumpTestChart.Series.FirstOrDefault(e => e.Title == "Computed Value");
                //        var calVal = (Convert.ToDouble(formulaPara1) * Convert.ToDouble(formulaPara2)) / 180;
                //        para.Values.Add(calVal);
                //    }
                //}
                #endregion Formula perpose
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            //////TODO: check this logic
            //xAxis.Add(dataCounts.ToString());
            //pressureXAxis.Labels = xAxis;

            //if (HomePage.PumpTestInformation.TableData.Keys.Contains(tupleItem.Item1))
            //{

            //    var para = HomePage.PumpTestInformation.TableData.FirstOrDefault(e => e.Key == tupleItem.Item1);
            //    para.Value.Add(val);
            //}
            //else
            //{
            //   HomePage.PumpTestInformation.TableData.Add(tupleItem.Item1,new List<string> {val});

            //}

        }
        #endregion Old Graph Plotting
        public static double xAxisDataValue = 0;
        static double Xvalue = 0;
        static double  Yvalue = 0;
        private static void UpdateData(IEnumerable<parsedDeviceData> dataList, Tuple<string, Tag> tupleItem)
        {
            
            _logger.LogInfo("Enter  ==========>>>>>>>> Update the chart()");

            foreach (var item in dataList)
            {
                try
                {
                    
                    #region Indudial chart ploting
                    if (PumpTest.rbtn_blockA.IsChecked == true || PumpTest.rbtn_blockB.IsChecked == true)
                    {

                        #region selection 1 block update the chart 2
                        //else if (chart2cmbXAxis!=null)
                        //{
                        //    if (tupleItem.Item1 == PumpTest.chart2cmbXAxis.SelectedItem.ToString())
                        //    {
                        //        //TimeSpan ts = stopwatch.Elapsed;
                        //        Chart2xAxisParaValue.Add(val/*string.Format(val + " (" + string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds) + " sec)")*/);
                        //        var XAxis = PumpTest.chart2.AxisX.FirstOrDefault(e => e.Title == string.Format("{0}({1})", tupleItem.Item1, tupleItem.Item2.Units)/*string.Format(PumpTest.chart2cmbXAxis.SelectedItem.ToString() + "( Time )")*/);
                        //        XAxis.Labels = Chart2xAxisParaValue;

                        //    }

                        //    else if (PumpTest.chart2.Series.FirstOrDefault(e => e.Title == tupleItem.Item1) != null)
                        //    {
                        //        var ser = PumpTest.chart2.Series.First(e => e.Title == tupleItem.Item1);
                        //        if (ser != null)
                        //        {
                        //            ser.Values.Add(double.Parse(val));
                        //        }
                        //    }
                        //}
                        #endregion selection 1 block update the chart 2
                        if (PumpTest.chart1cmbXAxis.SelectedItem != null)
                        {

                            if (tupleItem.Item1 == PumpTest.chart1cmbXAxis.SelectedItem.ToString() && item.SignalId == tupleItem.Item2.TagId /*&& chart2cmbvalue==null||tupleItem.Item1==chart2cmbvalue&& chart1cmbvalue==null*/)
                            {
                                xAxisDataValue = Math.Round(item.DataValue, 2);
                                TabledataArray.Add(item.DataValue.ToString());

                            }
                            else if (PumpTest.chart1.Series.Any(e => e.Title == tupleItem.Item1) && item.SignalId == tupleItem.Item2.TagId)
                            {
                                PumpTest
                                    .chart1
                                    .Series
                                    .FirstOrDefault(series => series.Title == tupleItem.Item1)
                                    .Values
                                    .AddRange(new ChartValues<ObservablePoint>
                                    {
                                    new ObservablePoint
                                    {
                                        X = xAxisDataValue,
                                        Y = Math.Round(item.DataValue, 2)
                                        //X=Xvalue++,
                                        //Y=Yvalue++

                                    }
                                    });
                                TabledataArray.Add(item.DataValue.ToString());
                                #region working Code
                                _logger.LogInfo($"X :{xAxisDataValue}, Y :{item.DataValue}");
                                // 08-11-23;
                                //var ser = PumpTest.chart1.Series.First(e => e.Title == /*string.Format("{0}{1}", tupleItem.Item1, tupleItem.Item2.TagId)*/tupleItem.Item1);
                                //ser.Values.Add(double.Parse(val));


                                #endregion working Code
                            }

                        }
                        else if(PumpTest.chart2cmbXAxis.SelectedItem !=null)
                        {
                            #region New Graph plotting
                           

                            if (tupleItem.Item1 == PumpTest.chart2cmbXAxis.SelectedItem.ToString() && item.SignalId == tupleItem.Item2.TagId)
                            {
                                xAxisDataValue = Math.Round(item.DataValue, 2);
                                TabledataArray.Add(item.DataValue.ToString());
                            }
                            else if (PumpTest.chart2.Series.Any(e => e.Title == tupleItem.Item1) && item.SignalId == tupleItem.Item2.TagId)
                            {
                                PumpTest
                                    .chart2
                                    .Series
                                    .FirstOrDefault(series => series.Title == tupleItem.Item1)
                                    .Values
                                    .AddRange(new ChartValues<ObservablePoint>
                                    {
                                    new ObservablePoint
                                    {
                                        X = xAxisDataValue,
                                        Y = Math.Round(item.DataValue, 2)
                                    }
                                    });
                                TabledataArray.Add(item.DataValue.ToString());
                                _logger.LogInfo($"X :{xAxisDataValue}, Y :{item.DataValue}" );
                            }

                            
                            #endregion New Graph plotting
                        }
                        #region Chart2 DataUpdate
                        //else
                        //{
                        //    if (tupleItem.Item1 == PumpTest.chart2cmbXAxis.SelectedItem.ToString())
                        //    {
                        //        //TimeSpan ts = stopwatch.Elapsed;
                        //        decimal result = decimal.Parse(val);
                        //        Chart2xAxisParaValue.Add(decimal.Round(result, 2, MidpointRounding.AwayFromZero).ToString()/*string.Format(val + " (" + string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds) + " sec)")*/);
                        //        // Chart2xAxisParaValue.Add(val/*string.Format(val + " (" + string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds) + " sec)") */);

                        //        var XAxis = PumpTest.chart2.AxisX.FirstOrDefault(e => e.Title == string.Format("{0}({1})", tupleItem.Item1, tupleItem.Item2.Units)/*string.Format(PumpTest.chart2cmbXAxis.SelectedItem.ToString() + "( Time )")*/);
                        //        XAxis.Labels = Chart2xAxisParaValue;
                        //       
                        //    }

                        //    else if (PumpTest.chart2.Series.FirstOrDefault(e => e.Title == tupleItem.Item1) != null)
                        //    {
                        //        var ser = PumpTest.chart2.Series.First(e => e.Title == tupleItem.Item1);
                        //        if (ser != null)
                        //        {

                        //            decimal result = decimal.Parse(val);
                        //            //ser.Values.Add(decimal.Round(result, 2, MidpointRounding.AwayFromZero));
                        //            ser.Values.Add(double.Parse(val));
                        //            //ser.Values.Add(double.Parse(val));
                        //           
                        //        }
                        //        
                        //    }

                        //}

                        #endregion Chart2 DataUpdate
                        
                    }
                  
                    #endregion  Indudial chart ploting
                  
                }
                catch (Exception ex)
                {
                    _logger.LogError("UpdateData Exception() :" + ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }

            _logger.LogInfo("Exit  ==========>>>>>>>>   Update the chart()");


        }


        private void StopTest()
        {
            string deviceres = string.Empty;
            try
            {
                if (SocketConnection.socket.Connected == true)
                {
                    //ElpisOPCServerMainWindow.TcpSocketClient.DisconnectConnection();
                    //_connection.DisconnectConnection();

                    _logger.LogInfo($"Client_DisConnected:{SocketConnection.socket.RemoteEndPoint}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }



            lblStartStop.Content = "Connect";
            TglAutoMode.IsEnabled = true;
            imgStartStop.Source = starticon;
            btnStartStop.ToolTip = "Connect";
            //dispatcherTimer.Stop();
            stopwatch.Stop();
            Testdurationtimer.Stop();
            SunPowerGenMainPage.isTestRunning = false;
            this.IsHitTestVisible = true;
            //ElpisOPCServerMainWindow.homePage.btnReset.IsEnabled = true;
            /*ElpisOPCServerMainWindow.pump_Test.*/
            btnReset.IsEnabled = true;
            /*ElpisOPCServerMainWindow.pump_Test.*/
            btnGenerateReport.IsEnabled = true;
            //ElpisOPCServerMainWindow.homePage.PumpExpanderCertificate.IsExpanded = true;
            tbxDeviceStatus.Text = "DisConnected";
            tbxDeviceStatus.Foreground = Brushes.Red;
            btnStart.IsEnabled = false;
            blbStateOFF.Visibility = Visibility.Visible;
            blbStateON.Visibility = Visibility.Hidden;
            TriggerPLC(false);
            btnStart.Content = "StartTest";
            HomePage.HomePageUIControl.txtPumpType.Text = "None";
            //GenerateCSVDataFile();
            //StartStopcommand(false/*,ref deviceres*/);

        }

        private void GenerateCSVDataFile()
        {
            try
            {
                #region OLd code
                //PumpReportGeneration reportGeneration = new PumpReportGeneration();
                //ObservableCollection<LineSeries> lineSeriesList = null;
                //ObservableCollection<List<string>> labelCollection = null;
                //HomePage.PumpTestInformation.SeriesCounts = pumpTestChart.Series.Count;
                //HomePage.PumpTestInformation.SelectedXaxis = pumpTestChart.AxisX.FirstOrDefault().Title;
                //PumpTestInformation testData = HomePage.PumpTestInformation;
                //if (SeriesCollections != null)
                //{

                //    LineSeries series1;
                //    lineSeriesList = new ObservableCollection<LineSeries>();
                //    labelCollection = new ObservableCollection<List<string>>() { pumpTestChart.AxisX.FirstOrDefault().Labels.ToList() };
                //    foreach (var item in pumpTestChart.Series)
                //    {
                //        series1 = new LineSeries() { Values = item.Values, Title = item.Title, LabelPoint = item.LabelPoint, Foreground = Brushes.Black };
                //        lineSeriesList.Add(series1);
                //    }

                // reportGeneration.GenerateCSVFile(TestType.PumpTest, testData, lineSeriesList, labelCollection);
                //}
                #endregion OLd code
                //new code


                PumpReportGeneration reportGeneration = new PumpReportGeneration();
                ObservableCollection<LineSeries> lineSeriesList = null;
                // ObservableCollection<List<string>> labelCollection = null;
                if (chart1.Series != null)
                {
                    HomePage.PumpTestInformation.SeriesCounts = chart1.Series.Count;
                }
                #region Chart2 SeriesCollection
                else if (chart2.Series != null)
                {
                    HomePage.PumpTestInformation.SeriesCounts = chart2.Series.Count;
                }
                #endregion Chart2 SeriesCollection

                HomePage.PumpTestInformation.SelectedXaxis = chart1.AxisX.FirstOrDefault().Title;
                // PumpTestInformation testData = HomePage.PumpTestInformation;
                if (chart1SeriesCollections != null)
                {

                    LineSeries series1;
                    lineSeriesList = new ObservableCollection<LineSeries>();

                    HomePage.PumpTestInformation.LabelCollection = new ObservableCollection<List<string>>()
                    {
                        //chart1.AxisX.FirstOrDefault().Labels.ToList()
                       //chart1.AxisX.FirstOrDefault().Labels.ToList()
                    };
                    foreach (var item in chart1.Series)
                    {
                        series1 = new LineSeries() { Values = item.Values, Title = item.Title, LabelPoint = item.LabelPoint, Foreground = Brushes.Black };
                        lineSeriesList.Add(series1);
                    }

                    HomePage.PumpTestInformation.LineSeriesList = lineSeriesList;

                    // reportGeneration.GenerateCSVFile(TestType.PumpTest, testData, lineSeriesList, labelCollection);
                }
                #region Chart2 Graph Series collection 
                else if (chart2SeriesCollections != null)
                {
                    LineSeries series1;
                    lineSeriesList = new ObservableCollection<LineSeries>();

                    HomePage.PumpTestInformation.LabelCollection = new ObservableCollection<List<string>>()
                    {
                        //chart2.AxisX.FirstOrDefault().Labels.ToList()
                    };
                    foreach (var item in chart2.Series)
                    {
                        series1 = new LineSeries() { Values = item.Values, Title = item.Title, LabelPoint = item.LabelPoint, Foreground = Brushes.Black };
                        lineSeriesList.Add(series1);
                    }

                    HomePage.PumpTestInformation.LineSeriesList = lineSeriesList;
                }
                #endregion Chart2 Graph Series collection 
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        private void lstAllTags_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void lstSelectedTags_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        public void generateButtonClicked()
        {
            string pdfchart1 = "PdfPumpChart1.png";
            string pdfchart2 = "PdfPumpChart2.png";
            ConvertXamltoImage(ChartView1, (int)ChartView1.ActualWidth, (int)ChartView1.ActualHeight, pdfchart1);
            ConvertXamltoImage2(ChartView2, (int)ChartView2.ActualWidth, (int)ChartView2.ActualHeight, pdfchart2);
        }

        public void HydgenerateButtonClicked()
        {
            string excelchart1 = "ExcelPumpChart1.png";
            string excelchart2 = "ExcelPumpChart2.png";
            ConvertXamltoImage(ChartView1, (int)ChartView1.ActualWidth, (int)ChartView1.ActualHeight,excelchart1);
            ConvertXamltoImage2(ChartView2, (int)ChartView2.ActualWidth, (int)ChartView2.ActualHeight,excelchart2);
        }
        #region SubcribebtnClick
        //private void OnUnsubBtnClick(object sender, RoutedEventArgs e)
        //{

        //    if (lstSelectedTags.SelectedItems != null)
        //    {

        //        for (int i = lstSelectedTags.SelectedItems.Count; i > 0; i--)
        //        {
        //            //IPublishers publisher = publisher;//PublisherSettingsPropertyGrid.SelectedObject as IPublishers;
        //            selectedTagNames.Remove(lstSelectedTags.SelectedItems[i - 1].ToString());
        //            tempTable.Remove(lstAllTags.SelectedItems[i - 1].ToString());
        //        }
        //        lstSelectedTags.ItemsSource = null;
        //       // tablePara.ItemsSource = null;
        //        chart1cmbXAxis.ItemsSource = null;
        //        //cmbYAxis.ItemsSource = null;
        //       // cmbFirstPara.ItemsSource = null;
        //       // cmbSecondPara.ItemsSource = null;
        //        lstAllTags.ItemsSource = allTags.Except(selectedTagNames).ToList();
        //        lstSelectedTags.ItemsSource = selectedTagNames;
        //       // tablePara.ItemsSource = selectedTagNames;
        //        chart1cmbXAxis.ItemsSource = selectedTagNames;
        //        //cmbYAxis.ItemsSource = selectedTagNames;
        //      // cmbFirstPara.ItemsSource = selectedTagNames;
        //       // cmbSecondPara.ItemsSource = selectedTagNames;
        //    }

        //}
        #endregion SubcribebtnClick
       
        private void TablePara_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ChkPrameters_Checked(object sender, RoutedEventArgs e)
        {
            var da = e.Source as CheckBox;

            HomePage.PumpTestInformation.TableParameterList.Add(da.Content.ToString());
            HomePage.PumpTestInformation.TableData.Add(da.Content.ToString(), new Dictionary<string, string>());

            
        }

        private void ParamterComboLabel()
        {
            StringBuilder label = new StringBuilder();
            foreach (var item in HomePage.PumpTestInformation.TableParameterList)
            {
                label.Append(item);
                label.Append(',');
            }
            // tablePara.Text = label.ToString().TrimEnd(new char[] { ',' });
        }

        private void ChkPrameters_Unchecked(object sender, RoutedEventArgs e)
        {
            var da = e.Source as CheckBox;
            HomePage.PumpTestInformation.TableParameterList.Remove(da.Content.ToString());
            HomePage.PumpTestInformation.TableData.Remove(da.Content.ToString());
            // ParamterComboLabel();
        }

        #region Chart_DataClick Points
        private void PumpTestChart1_DataClick(object sender, ChartPoint chartPoint)
        {

            Point point = Mouse.GetPosition(ChartView1);
            var xVal = chart1.AxisX.FirstOrDefault().Labels[Convert.ToInt32(chartPoint.X)];
            
            var xAxisVal = string.Empty;
            var xAxisTime = string.Empty;
            var time = string.Empty;
            //sathish radion button to checkbox changed for hydrofit
            if (rbtn_blockA.IsChecked == true && chart1cmbXAxis.SelectedItem != null)
            {
                xAxisVal = string.Format("{0} : {1}", chart1cmbXAxis.SelectedItem.ToString(), xVal.Split('(')[0]);
                //time = xVal.Split('(')[1].Replace('(', ' ').Replace(')', ' ');
                //xAxisTime = string.Format("{0} : {1}", "Time", time);
            }
            //sathish radion button  to checkbox changed for hydrofit
            else if (rbtn_blockB.IsChecked == true && chart2cmbXAxis.SelectedItem != null)
            {
                xAxisVal = string.Format("{0} : {1}", chart2cmbXAxis.SelectedItem.ToString(), xVal.Split('(')[0]);
                // time = xVal.Split('(')[1].Replace('(', ' ').Replace(')', ' ');
                //xAxisTime = string.Format("{0} : {1}", "Time", time);
            }
            else if (rbtn_blockA.IsChecked == false || rbtn_blockB.IsChecked == false)
            {
                time = xVal;
                xAxisTime = string.Format("{0} : {1}", "Time", time);
            }
            //else if (rbtn_blockA.IsChecked == false || rbtn_blockB.IsChecked == false)
            //{
            //    time = xVal;
            //    xAxisTime = string.Format("{0} : {1}", "Time", time);
            //}

            int index = -1;

            var tableTimepara = HomePage.PumpTestInformation.TableData.FirstOrDefault(e => e.Key == "Time");
            if (tableTimepara.Key != null)
            {               // we use here Dictionary concept.
                index = tableTimepara.Value.Count;
                tableTimepara.Value["Time" + "_" + index] = time;

            }

            //Collecting selected points for table
            foreach (var key in tempTable.Keys)
            {
                var tablepara = HomePage.PumpTestInformation.TableData.FirstOrDefault(e => e.Key == key);
                if (tablepara.Key != null)
                {
                    index = tablepara.Value.Count;
                    tablepara.Value[key + "_" + index] = tempTable.FirstOrDefault(e => e.Key == tablepara.Key).Value[(int)chartPoint.X].ToString();

                }
            }


            CustomTooltipModel customTooltipModel = new CustomTooltipModel()
            {
                SeriesName = chartPoint.SeriesView.Title,
                SeriesValue = chartPoint.Y.ToString(),
                YAxisValue = string.Format("{0} : {1}", chartPoint.SeriesView.Title, chartPoint.Y.ToString()),
                XAxisValue = xAxisVal,
                // xAxisTime = xAxisTime,
                CustomNote = "gfdgdfg",
                Index = index
            };

            var va = HomePage.PumpTestInformation.TableData;


            HomePage.PumpTestInformation.CustomTooltip = customTooltipModel;
            //canvasView.DataContext = HomePage.PumpTestInformation.CustomTooltip;
            //Canvas.SetTop(canvasView, (chartPoint.ChartLocation.Y - 150));
            //Canvas.SetLeft(canvasView, (chartPoint.ChartLocation.X - 300));
            //CustomToolTip.Visibility = Visibility.Visible;
            //canvasView.DataContext = HomePage.PumpTestInformation.CustomTooltip;
            //createView(chartPoint.ChartLocation.Y, chartPoint.ChartLocation.X);
            var mousePoint = Mouse.GetPosition(ChartView1);
            string actualVal = string.Format("{0} : {1}", chartPoint.SeriesView.Title, chartPoint.Y.ToString());
            pumpCustomTooltip pumpCustomTooltip = new pumpCustomTooltip(mousePoint.Y, mousePoint.X, customTooltipModel);
            //DraggabalePopup draggabalePopup = new DraggabalePopup();
            //draggabalePopup.IsOpen = true;
            //pumpCustomTooltip.CustomToolTip.Visibility = Visibility.Visible;
            //pumpCustomTooltip.canvasView.DataContext = HomePage.PumpTestInformation.CustomTooltip;
            ChartView1.Children.Add(pumpCustomTooltip);
        }
        private void PumpTestChart2_DataClick(object sender, ChartPoint chartPoint)
        {
            Point point = Mouse.GetPosition(ChartView2);
            var xVal = chart2.AxisX.FirstOrDefault().Labels[Convert.ToInt32(chartPoint.X)];
            var xAxisVal = string.Empty;
            var xAxisTime = string.Empty;
            var time = string.Empty;
            //sathish radion button to checkbox changed for hydrofit
            if (rbtn_blockA.IsChecked == true && chart1cmbXAxis.SelectedItem != null)
            {
                xAxisVal = string.Format("{0} : {1}", chart2cmbXAxis.SelectedItem.ToString(), xVal.Split('(')[0]);
                //time = xVal.Split('(')[1].Replace('(', ' ').Replace(')', ' ');
                // xAxisTime = string.Format("{0} : {1}", "Time", time);
            }
            //sathish radion button  to checkbox changed for hydrofit
            else if (rbtn_blockB.IsChecked == true && chart2cmbXAxis.SelectedItem != null)
            {
                xAxisVal = string.Format("{0} : {1}", chart2cmbXAxis.SelectedItem.ToString(), xVal.Split('(')[0]);
                //time = xVal.Split('(')[1].Replace('(', ' ').Replace(')', ' ');
                // xAxisTime = string.Format("{0} : {1}", "Time", time);
            }
            else if (rbtn_blockA.IsChecked == false || rbtn_blockB.IsChecked == false)
            {
                time = xVal;
                xAxisTime = string.Format("{0} : {1}", "Time", time);
            }
            //else if (rbtn_blockA.IsChecked == false || rbtn_blockB.IsChecked == false)
            //{
            //    time = xVal;
            //    xAxisTime = string.Format("{0} : {1}", "Time", time);
            //}

            int index = -1;

            var tableTimepara = HomePage.PumpTestInformation.TableData.FirstOrDefault(e => e.Key == "Time");
            if (tableTimepara.Key != null)
            {               // we use here Dictionary concept.
                index = tableTimepara.Value.Count;
                tableTimepara.Value["Time" + "_" + index] = time;

            }

            //Collecting selected points for table
            foreach (var key in tempTable.Keys)
            {
                var tablepara = HomePage.PumpTestInformation.TableData.FirstOrDefault(e => e.Key == key);
                if (tablepara.Key != null)
                {
                    index = tablepara.Value.Count;
                    tablepara.Value[key + "_" + index] = tempTable.FirstOrDefault(e => e.Key == tablepara.Key).Value[(int)chartPoint.X].ToString();

                }
            }


            CustomTooltipModel customTooltipModel = new CustomTooltipModel()
            {
                SeriesName = chartPoint.SeriesView.Title,
                SeriesValue = chartPoint.Y.ToString(),
                YAxisValue = string.Format("{0} : {1}", chartPoint.SeriesView.Title, chartPoint.Y.ToString()),
                XAxisValue = xAxisVal,
                //xAxisTime = xAxisTime,
                CustomNote = "gfdgdfg",
                Index = index
            };

            var va = HomePage.PumpTestInformation.TableData;


            HomePage.PumpTestInformation.CustomTooltip = customTooltipModel;
            //canvasView.DataContext = HomePage.PumpTestInformation.CustomTooltip;
            //Canvas.SetTop(canvasView, (chartPoint.ChartLocation.Y - 150));
            //Canvas.SetLeft(canvasView, (chartPoint.ChartLocation.X - 300));
            //CustomToolTip.Visibility = Visibility.Visible;
            //canvasView.DataContext = HomePage.PumpTestInformation.CustomTooltip;
            //createView(chartPoint.ChartLocation.Y, chartPoint.ChartLocation.X);
            var mousePoint = Mouse.GetPosition(ChartView2);
            string actualVal = string.Format("{0} : {1}", chartPoint.SeriesView.Title, chartPoint.Y.ToString());
            pumpCustomTooltip pumpCustomTooltip = new pumpCustomTooltip(mousePoint.Y, mousePoint.X, customTooltipModel);
            //DraggabalePopup draggabalePopup = new DraggabalePopup();
            //draggabalePopup.IsOpen = true;
            //pumpCustomTooltip.CustomToolTip.Visibility = Visibility.Visible;
            //pumpCustomTooltip.canvasView.DataContext = HomePage.PumpTestInformation.CustomTooltip;
            ChartView2.Children.Add(pumpCustomTooltip);
        }
        #endregion Chart_DataClick Points
        private void createView(double y, double x)
        {
            Canvas canvas = new Canvas();
            StackPanel stackPanel = new StackPanel();
            stackPanel.Name = "canvasView";
            // stackPanel.Background = (Brush)ColorConverter.ConvertFromString("#FFF0E6DC");
            stackPanel.Width = 195;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            StackPanel childStack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            Binding binding = new Binding()
            {
                Source = HomePage.PumpTestInformation.CustomTooltip.SeriesName + " : " + HomePage.PumpTestInformation.CustomTooltip.SeriesValue,

            };

            Label label = new Label();
            BindingOperations.SetBinding(label, Label.ContentProperty, binding);
            childStack.Children.Add(label);
            stackPanel.Children.Add(childStack);
            Canvas.SetTop(stackPanel, (y - 150));
            Canvas.SetTop(stackPanel, (x - 300));
            canvas.Children.Add(stackPanel);
            Canvas.SetZIndex(stackPanel, 1);
        }

        public void ConvertXamltoImage(UIElement visual, int Width, int Height, string chart1)
        {
            //UIElement visual = XamlReader.Load(System.Xml.XmlReader.Create(new StringReader(XamlString))) as UIElement;

            RenderTargetBitmap bmpCopied = new RenderTargetBitmap(Width, Height, 92, 92, PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(visual);
                Rect rect = new Rect(new Point(), new Size(Width, Height));
                rect.Offset(10, 10);
                dc.DrawRectangle(vb, null, rect);

            }

            bmpCopied.Render(dv);

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmpCopied));
            


            using (var stream = File.Create(string.Format("{0}\\{1}", Directory.GetCurrentDirectory(),chart1)))
            {
                
                png.Save(stream);
                
            }

            //using (var stream = File.Create(string.Format("{0}\\PumpChart1.png", Directory.GetCurrentDirectory())))
            //{
            //   // stream.
            //    png.Save(stream);
            //    // stream.Close();
            //}

        }
        public void ConvertXamltoImage2(UIElement visual, int Width, int Height, string chart2)
        {
            //UIElement visual = XamlReader.Load(System.Xml.XmlReader.Create(new StringReader(XamlString))) as UIElement;

            RenderTargetBitmap bmpCopied = new RenderTargetBitmap(Width, Height, 92, 92, PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(visual);
                Rect rect = new Rect(new Point(), new Size(Width, Height));
                rect.Offset(10, 10);
                dc.DrawRectangle(vb, null, rect);

            }

            bmpCopied.Render(dv);

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmpCopied));

            using (var stream = File.Create(string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), chart2)))
            {
                
                png.Save(stream);
                
            }
            //using (var stream = File.Create(string.Format("{0}\\PumpChart2.png", Directory.GetCurrentDirectory())))
            //{
            //    png.Save(stream);
            //}

        }
        private void ConfigExpander_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void ConfigExpander_Collapsed(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectorTypeChanged();
        }

        private void chart1ChkYaxisPrameters_Checked(object sender, RoutedEventArgs e)
        {
            //var da = e.Source as CheckBox;
            //Chart1YaxisParaList.Add(da.Content.ToString());
            var da = e.Source as CheckBox;

            Chart1YaxisParaList.Add(da.Content.ToString());
            chart2cmbXAxis.Items.Clear();
            foreach (var item in chart1YaxisPara.Items)
            {
                if (!Chart1YaxisParaList.Contains(item))
                {
                    chart2cmbXAxis.Items.Add(item);
                }
            }




        }

        private void chart1ChkYaxisPrameters_Unchecked(object sender, RoutedEventArgs e)
        {
            var da = e.Source as CheckBox;
            // Chart1YaxisParaList.Remove(da.Content.ToString());

            Chart1YaxisParaList.Remove(da.Content.ToString());
            chart2cmbXAxis.Items.Clear();
            foreach (var item in chart1YaxisPara.Items)
            {
                if (!Chart1YaxisParaList.Contains(item))
                {
                    chart2cmbXAxis.Items.Add(item);
                }
            }


        }
        private void chart2ChkYaxisPrameters_Checked(object sender, RoutedEventArgs e)
        {
            //var da = e.Source as CheckBox;
            //Chart1YaxisParaList.Add(da.Content.ToString());
            var da = e.Source as CheckBox;
            Chart2YaxisParaList.Add(da.Content.ToString());
        }

        private void chart2ChkYaxisPrameters_Unchecked(object sender, RoutedEventArgs e)
        {
            var da = e.Source as CheckBox;
            Chart2YaxisParaList.Remove(da.Content.ToString());
           

        }

        private void chart1cmbXAxis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if (chart1cmbXAxis.SelectedItem != null)
                {
                    chart1.AxisX.Add(new Axis
                    {
                        Title = chart1cmbXAxis.SelectedItem.ToString()
                    });
                    Chart1YaxisParaList.Clear();
                    if (chart1YaxisPara.Items.Count > 0)
                        chart1YaxisPara.Items.Clear();
                    //foreach (var tag in TagsCollection)
                    //{
                    foreach (var seletedItem in selectedTagNames)
                    {
                        if (seletedItem.TagName != chart1cmbXAxis.SelectedItem.ToString())
                        {

                            chart1YaxisPara.Items.Add(seletedItem.TagName);
                        }
                    }

                    //}
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void chart2CmbXAxis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if (chart2cmbXAxis.SelectedItem != null)
                {
                    Chart2YaxisParaList.Clear();
                    if (chart2YaxisPara.Items.Count > 0)
                        chart2YaxisPara.Items.Clear();
                    //foreach (var tag in TagsCollection)
                    //{
                    foreach (string item in chart2cmbXAxis.Items)
                    {
                        if (item != chart2cmbXAxis.SelectedItem.ToString())
                        {

                            chart2YaxisPara.Items.Add(item);

                        }
                    }

                    //}
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void txtTimer_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnSaveOperation_Click(object sender, RoutedEventArgs e)
        {

        }


        private void rbtn_blockA_Checked(object sender, RoutedEventArgs e)
        {
            selectedTagNames.Clear();
            chart1cmbXAxis.Items.Clear();
            chart2cmbXAxis.Items.Clear();
            Chart1YaxisParaList.Clear();
            chart1YaxisPara.Items.Clear();
            Chart2YaxisParaList.Clear();
            chart2YaxisPara.Items.Clear();
            #region RadioButton Function Commented

            
            #endregion RadioButton Function Commented
        }

        private void chart1YaxisPara_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void chart1_DataClick(object sender, ChartPoint chartPoint)
        {

        }


      
        
        #region Decimal to a hexadecimal string
        // Convert the decimal integer to a hexadecimal string
        public static string DecimalToHexTwosComplement(int decimalValue)
        {
            // Determine the number of bits needed to represent the input value in 2's complement form
            int numBits = (int)Math.Ceiling(Math.Log(Math.Abs(decimalValue), 2)) + 1;

            // Convert the decimal value to its 2's complement binary representation
            int binaryValue = decimalValue < 0 ? (1 << numBits) + decimalValue : decimalValue;

            // Convert the binary value to a hexadecimal string
            string hexValue = binaryValue.ToString("X4");

            return hexValue;
        }
        #endregion Decimal to a hexadecimal string
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            time = 0;
            ElpisOPCServerMainWindow.homePage.DisableInputs(false);
            ElpisOPCServerMainWindow.homePage.BtnCommentBox.IsChecked = false;
            ElpisOPCServerMainWindow.homePage.txtCommentBox.Text = "";
            ElpisOPCServerMainWindow.homePage.txtCommnetUpdateMessage.Text = "";
            ElpisOPCServerMainWindow.homePage.BtnTestDetails.IsChecked = false;
            ElpisOPCServerMainWindow.homePage.txtTestDetailsBox.Text = "";
            ElpisOPCServerMainWindow.homePage.txtTestDetailsMessage.Text = "";
            //StrokeTestWindow.ResetStrokeTest();
            HomePage.HoldMidPositionTestWindow.ResetHoldMidPosition();
            //SlipStickTestWindow.ResetSlipStickTest();
            rbtn_blockA.IsChecked = false;
            rbtn_blockB.IsChecked = false;
            HomePage.PumpTestWindow.ResetPumpTest();

            if (ElpisOPCServerMainWindow.homePage.tabControlTest.SelectedIndex == 0)
            {
                HomePage.strokeTestInfo.IsTestStarted = false;
                ElpisOPCServerMainWindow.homePage.gridMain.DataContext = HomePage.strokeTestInfo;
                HomePage.strokeTestInfo.IsTestStarted = true;


            }
            else if (ElpisOPCServerMainWindow.homePage.tabControlTest.SelectedIndex == 1)
            {
                if (HomePage.HoldMidPositionTestWindow.rb_LineA.IsChecked == true)
                {
                    HomePage.holdMidPositionLineAInfo.IsTestStarted = false;
                    ElpisOPCServerMainWindow.homePage.gridMain.DataContext = HomePage.holdMidPositionLineAInfo;
                    HomePage.holdMidPositionLineAInfo.IsTestStarted = true;

                }
                else if (HomePage.HoldMidPositionTestWindow.rb_LineB.IsChecked == true)
                {
                    HomePage.holdMidPositionLineBinfo.IsTestStarted = false;
                    ElpisOPCServerMainWindow.homePage.gridMain.DataContext = HomePage.holdMidPositionLineBinfo;
                    HomePage.holdMidPositionLineBinfo.IsTestStarted = true;
                    ElpisOPCServerMainWindow.homePage.txtStatus.Text = "";
                }
            }
            else if (ElpisOPCServerMainWindow.homePage.tabControlTest.SelectedIndex == 2)
            {
                HomePage.slipStickTestInformation.IsTestStarted = false;
                ElpisOPCServerMainWindow.homePage.gridMain.DataContext = HomePage.slipStickTestInformation;
                HomePage.slipStickTestInformation.IsTestStarted = true;
            }

            else if (ElpisOPCServerMainWindow.homePage.tabControlTest.SelectedIndex == 3)
            {
                HomePage.PumpTestInformation.IsTestStarted = false;
                ElpisOPCServerMainWindow.homePage.gridMain.DataContext = HomePage.PumpTestInformation;
                HomePage.PumpTestInformation.IsTestStarted = true;
            }

            //ElpisOPCServerMainWindow.homePage.panelStatusMessage.Visibility = Visibility.Hidden;
            /*ElpisOPCServerMainWindow.pump_Test.*/
            panelStatusMessage.Visibility = Visibility.Hidden;
            ElpisOPCServerMainWindow.homePage.ReportTab.IsEnabled = true;
            ElpisOPCServerMainWindow.homePage.txtFilePath.IsEnabled = true;
        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            
            GenerateCSVDataFile();
            ElpisOPCServerMainWindow.homePage.Generatereport();
            openLoopExcel.ExcelReportGenerateReport();
            //excelReport.MSExcelReportGeneration();
        }

        private void rbtn_BlockA_Checked(object sender, RoutedEventArgs e)
        {

            selectedTagNames.Clear();
            chart1cmbXAxis.Items.Clear();
            chart2cmbXAxis.Items.Clear();
            Chart1YaxisParaList.Clear();
            chart1YaxisPara.Items.Clear();
            Chart2YaxisParaList.Clear();
            chart2YaxisPara.Items.Clear();


            #region chart selection Combo box  old code
            //if (chk_BlockA.IsChecked == true && chk_BlockB.IsChecked == false)
            //{

            //    var tags = HomePage.PumpTestInformation.TagInformation;
            //    var block1Tags = tags.Where(a => a.BlockType == BlockTypes.Block1);
            //    foreach (var item in block1Tags)
            //    {

            //        if (item.Address != "upd-signal" && item.Address != "start-stop")
            //        {
            //            selectedTagNames.Add(item);
            //            chart1cmbXAxis.Items.Add(item.TagName);
            //        }


            //    }
            //}
            //else if (chk_BlockA.IsChecked == false && chk_BlockB.IsChecked == true)
            //{
            //    var tags = HomePage.PumpTestInformation.TagInformation;
            //    var block2Tags = tags.Where(a => a.BlockType == BlockTypes.Block2);
            //    foreach (var item in block2Tags)
            //    {
            //        if (item.Address != "upd-signal" && item.Address != "start-stop")
            //        {

            //            selectedTagNames.Add(item);
            //            chart1cmbXAxis.Items.Add(item.TagName);

            //        }
            //    }

            //}
            #endregion chart selection Combo box  old code
            #region Load all channels to chart1 and chart2 x combobox
            if (rbtn_blockA.IsChecked == true && rbtn_blockB.IsChecked == false)
            {
                #region Configuration Of Selected  parameters 
                //var tags = HomePage.PumpTestInformation.TagInformation;
                //var block1Tags = tags.Where(a => a.BlockType == BlockTypes.Block1);
                //foreach (var item in block1Tags)
                //{

                //    if (item.Address != "upd-signal" && item.Address != "start-stop")
                //    {
                //        selectedTagNames.Add(item);
                //        chart1cmbXAxis.Items.Add(item.TagName);
                //        chart2cmbXAxis.Items.Add(item.TagName);
                //    }


                //}
                #endregion  Configuration Of Selected  parameters 
                Chart2groupbox.IsEnabled = false;
                var tags = HomePage.PumpTestInformation.TagInformation;
                //var block1Tags = tags.Where(a => a.BlockType == BlockTypes.Block1);
                foreach (var item in tags)
                {

                    if (item.Address != "upd-signal" && item.Address != "start-stop")
                    {
                        selectedTagNames.Add(item);
                        chart1cmbXAxis.Items.Add(item.TagName);
                        chart2cmbXAxis.Items.Add(item.TagName);
                    }


                }
            }
            #endregion Load all channels to chart1 and chart2 x combobox
            else if (rbtn_blockA.IsChecked == false && rbtn_blockB.IsChecked == true)
            {
                #region Configuration of only B block
                //var tags = HomePage.PumpTestInformation.TagInformation;
                //var block2Tags = tags.Where(a => a.BlockType == BlockTypes.Block2);
                //foreach (var item in block2Tags)
                //{
                //    if (item.Address != "upd-signal" && item.Address != "start-stop")
                //    {

                //        selectedTagNames.Add(item);
                //        chart1cmbXAxis.Items.Add(item.TagName);
                //        chart2cmbXAxis.Items.Add(item.TagName);

                //    }
                //}
                #endregion Configuration of only B block
                Chart2groupbox.IsEnabled = true;
                var tags = HomePage.PumpTestInformation.TagInformation;
                //var block2Tags = tags.Where(a => a.BlockType == BlockTypes.Block2);
                foreach (var item in tags)
                {
                    if (item.Address != "upd-signal" && item.Address != "start-stop")
                    {

                        selectedTagNames.Add(item);
                        chart1cmbXAxis.Items.Add(item.TagName);
                        chart2cmbXAxis.Items.Add(item.TagName);

                    }
                }
            }
            else if (rbtn_blockA.IsChecked == true && rbtn_blockB.IsChecked == true)
            {
                selectedTagNames.Clear();
                chart1cmbXAxis.Items.Clear();
                chart2cmbXAxis.Items.Clear();
                Chart1YaxisParaList.Clear();
                chart1YaxisPara.Items.Clear();
                Chart2YaxisParaList.Clear();
                chart2YaxisPara.Items.Clear();
                var tags = HomePage.PumpTestInformation.TagInformation;
                foreach (var item in tags)
                {
                    if (item.Address != "upd_signal" && item.Address != "start-stop")
                    {
                        selectedTagNames.Add(item);
                        chart1cmbXAxis.Items.Add(item.TagName);
                        chart2cmbXAxis.Items.Add(item.TagName);
                    }

                }
            }

        }
        public static void configPacketWithSelectedTagNames()
        {
            if(selectedTagNames.Count>0)
            {
                selectedTagNames.Clear();
            }
            var tags = HomePage.PumpTestInformation.TagInformation;
            foreach (var item in tags)
            {
                if (item.Address != "upd-signal" && item.Address != "start-stop")
                {
                    selectedTagNames.Add(item);
                }
            }
            SelectedItems_TagCollection();
        }
       
        private void rbtn_BlockA_Unchecked(object sender, RoutedEventArgs e)
        {

            if (rbtn_blockA.IsChecked == true && rbtn_blockB.IsChecked == false)
            {

                var tags = HomePage.PumpTestInformation.TagInformation;
                var block2Tags = tags.Where(a => a.BlockType == BlockTypes.Block2);

                foreach (var item in block2Tags)
                {
                    selectedTagNames.Remove(item);
                    chart1cmbXAxis.Items.Remove(item.TagName);
                    chart1YaxisPara.Items.Clear();
                    chart2cmbXAxis.Items.Clear();
                    chart2YaxisPara.Items.Clear();
                }
                Chart2groupbox.IsEnabled = false;
            }
            else if (rbtn_blockA.IsChecked == false && rbtn_blockB.IsChecked == true)
            {
                var tags = HomePage.PumpTestInformation.TagInformation;
                var block1Tags = tags.Where(a => a.BlockType == BlockTypes.Block1);
                foreach (var item in block1Tags)
                {
                    selectedTagNames.Remove(item);
                    chart1cmbXAxis.Items.Remove(item.TagName);
                    chart1YaxisPara.Items.Clear();
                    chart2cmbXAxis.Items.Clear();
                    chart2YaxisPara.Items.Clear();
                }
                Chart2groupbox.IsEnabled = true;
            }
            else if (rbtn_blockA.IsChecked == false && rbtn_blockB.IsChecked == false)
            {
                selectedTagNames.Clear();
                chart1cmbXAxis.Items.Clear();
                chart2cmbXAxis.Items.Clear();
                Chart1YaxisParaList.Clear();
                chart1YaxisPara.Items.Clear();
                Chart2YaxisParaList.Clear();
                chart2YaxisPara.Items.Clear();
                
            }
        }
        #region AutoMode Implementation
        private void TglAutoMode_Checked(object sender, RoutedEventArgs e)
        {

            //if (lblStartStop.Content.ToString() == "Connect")
            //{
            if (TglAutoMode.IsChecked == true)
            {
                #region AutoMode ReportGeneration
                AutoReportGeneration.Start();
                #endregion AutoMode ReportGeneration
                #region  AutoMode Test Code
                //if (chart1cmbXAxis.SelectedItem != null && chart2cmbXAxis.SelectedItem != null)
                //{
                //    AutoTimer.Start();
                //    btnStartStop.IsEnabled = false;
                //    btnReset.IsEnabled = false;
                //    btnplayback.IsEnabled = false;
                //    btnGenerateReport.IsEnabled = false;
                //    btnStartStop_Click(sender, e);
                //}
                //else
                //{
                //    TglAutoMode.IsChecked = false;
                //    TglAutoMode_Unchecked(sender, e);
                //}
                #endregion AutoMode Test Code
            }
            //}
            //else
            //{
            //    MessageBox.Show("Manual mode is running please turn off the manual mode");
            //    TglAutoMode.IsChecked = false;
            //}

        }



        private void TglAutoMode_Unchecked(object sender, RoutedEventArgs e)
        {

            if (TglAutoMode.IsChecked == false)
            {
                #region AutoMode ReportGeneration
                AutoReportGeneration.Stop();
                #endregion AutoMode ReportGeneration
                #region  AutoMode Test Code
                //AutoTimer.Stop();
                //btnStartStop.IsEnabled = true;
                //btnReset.IsEnabled = true;
                //btnplayback.IsEnabled = true;
                //btnGenerateReport.IsEnabled = true;
                //if (lblStartStop.Content.ToString() != "Start Record" && TglAutoMode.IsChecked == true)
                //{
                //    StopTest();
                //}
                #endregion AutoMode Test Code
            }
        }
        #endregion AutoMode Implementation

        public void DataLogging(string Data)
        {
            //string json = "[{ \"device\": \"HydFit_001>2023-03-23 08:06:43<3\", \"SignalDataList\": [ \"Hydfit_001s0>30151.000000\", \"Hydfit_001s1>30057.000000\", \"Hydfit_001s2>30139.000000\", \"Hydfit_001s3>30357.000000\", \"Hydfit_001s4>30464.000000\", \"Hydfit_001s5>30321.000000\", \"Hydfit_001s6>30115.000000\", \"Hydfit_001s7>30226.000000\", \"Hydfit_001s8>30245.000000\", \"Hydfit_001s9>30144.000000\", \"Hydfit_001s10>2.000000\", \"Hydfit_001s11>30213.000000\", \"Hydfit_001s12>30213.000000\", \"Hydfit_001s13>339\", \"Hydfit_001s14>679\", \"Hydfit_001s15>0.000000\" ] }]";
            if (rbtnCsv.IsChecked == true)
            {
                #region Create the File data
                //// Deserialize the JSON data
                //var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LogDataItem>>(Data);

                //// Get the current timestamp
                //var currentTimestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

                //string fileName = $"data_{currentTimestamp}.csv";

                //// Open the file for appending or create a new file
                //using (var writer = new StreamWriter(fileName, append: true))
                //{
                //    // Write the headers if the file is newly created
                //    if (!File.Exists(fileName))
                //    {
                //        writer.WriteLine("DeviceId,TimeStamp,SignalId,DataValue");
                //    }

                //    //Write the data rows
                //    foreach (var item in data)
                //    {
                //        var deviceId = item.device.Split('>')[0];
                //        var timestamp = item.device.Split('>')[1].Split('<')[0];

                //        for (int i = 0; i < item.SignalDataList.Count; i++)
                //        {
                //            var signalData = item.SignalDataList[i];
                //            var signalId = signalData.Split('>')[0];
                //            var dataValue = signalData.Split('>')[1];

                //            var rowData = new List<string>
                //                    {
                //                         deviceId,
                //                         DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("M/d/yyyy H:mm"),
                //                         signalId,
                //                         dataValue
                //                    };

                //            writer.WriteLine(string.Join(",", rowData));
                //        }
                //    }
                //}
                #endregion Create the File data
                #region cretae file with based on Size
                // Deserialize JSON data
                //var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LogDataItem>>(Data);

                //// Set the threshold file size (2MB)
                //const long maxSizeBytes = 2 * 1024 * 1024;

                //// Initialize variables
                //var currentTimestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                //var fileCount = 1;
                //var fileSize = 0L;

                //// Create a subfolder for data logs if it doesn't exist
                //string logsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataLogs");
                //Directory.CreateDirectory(logsFolderPath);

                //// Iterate through data and write to files
                //foreach (var item in data)
                //{
                //    var deviceId = item.device.Split('>')[0];
                //    var timestamp = item.device.Split('>')[1].Split('<')[0];

                //    for (int i = 0; i < item.SignalDataList.Count; i++)
                //    {
                //        var signalData = item.SignalDataList[i];
                //        var signalId = signalData.Split('>')[0];
                //        var dataValue = signalData.Split('>')[1];

                //        var rowData = new List<string>
                //        {
                //            deviceId,
                //            DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("M/d/yyyy H:mm"),
                //            signalId,
                //            dataValue
                //        };

                //        var row = string.Join(",", rowData);
                //        fileSize += row.Length; // Calculate current row size

                //        // Check if the current file exceeds the size threshold
                //        if (fileSize > maxSizeBytes)
                //        {
                //            // Create a new file
                //            currentTimestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                //            fileCount++;
                //            fileSize = 0; // Reset the file size counter
                //        }

                //        string fileName = Path.Combine(logsFolderPath, $"data_{currentTimestamp}_part{fileCount}.csv");

                //        using (var writer = new StreamWriter(fileName, append: true))
                //        {
                //            // Write the headers if the file is newly created
                //            if (new FileInfo(fileName).Length == 0)
                //            {
                //                writer.WriteLine("DeviceId,TimeStamp,SignalId,DataValue");
                //            }

                //            writer.WriteLine(row);
                //        }
                //    }
                #endregion cretae file with based on Size

                #region create file every 3 min once
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LogDataItem>>(Data);

                // Initialize variables
                var lastFileCreationTime = DateTime.MinValue;
                var fileCount = 1;

                // Create a subfolder for data logs if it doesn't exist
                string logsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataLogs");
                Directory.CreateDirectory(logsFolderPath);

                // Iterate through data and write to files
                foreach (var item in data)
                {
                    var deviceId = item.device.Split('>')[0];
                    var timestamp = item.device.Split('>')[1].Split('<')[0];

                    for (int i = 0; i < item.SignalDataList.Count; i++)
                    {
                        var signalData = item.SignalDataList[i];
                        var signalId = signalData.Split('>')[0];
                        var dataValue = signalData.Split('>')[1];

                        var rowData = new List<string>
                           {
                               deviceId,
                               DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("M/d/yyyy H:mm"),
                               signalId,
                               dataValue
                           };

                        var row = string.Join(",", rowData);

                        DateTime now = DateTime.Now;

                        // Check if it's time to create a new file (every 3 minutes)
                        if ((now - lastFileCreationTime).TotalMinutes >= 3)
                        {
                            // Create a new file
                            lastFileCreationTime = now;
                            fileCount++;
                        }

                        //while (stopwatch.Elapsed.TotalMinutes < 2)
                        //{

                        //}
                        string currentTimestamp = lastFileCreationTime.ToString("yyyy-MM-dd-HH-mm-ss");
                        string fileName = Path.Combine(logsFolderPath, $"data_{currentTimestamp}_part{fileCount}.csv");

                        using (var writer = new StreamWriter(fileName, append: true))
                        {
                            // Write the headers if the file is newly created
                            if (!File.Exists(fileName))
                            {
                                writer.WriteLine("DeviceId,TimeStamp,SignalId,DataValue");
                            }

                            writer.WriteLine(row);
                        }
                    }
                }

                #endregion create file every 3 min once
                #region filesize creation
                //var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LogDataItem>>(Data);

                //// Initialize variables
                //var lastFileCreationTime = DateTime.MinValue;
                //var fileCount = 1;
                //var maxFileSizeBytes = 2 * 1024 * 1024; // 2MB in bytes

                //// Create a subfolder for data logs if it doesn't exist
                //string logsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataLogs");
                //Directory.CreateDirectory(logsFolderPath);

                //// Iterate through data and write to files
                //foreach (var item in data)
                //{
                //    var deviceId = item.device.Split('>')[0];
                //    var timestamp = item.device.Split('>')[1].Split('<')[0];

                //    for (int i = 0; i < item.SignalDataList.Count; i++)
                //    {
                //        var signalData = item.SignalDataList[i];
                //        var signalId = signalData.Split('>')[0];
                //        var dataValue = signalData.Split('>')[1];

                //        var rowData = new List<string>
                //        {
                //            deviceId,
                //            DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("M/d/yyyy H:mm"),
                //            signalId,
                //            dataValue
                //        };

                //        var row = string.Join(",", rowData);

                //        string currentTimestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                //        string fileName = Path.Combine(logsFolderPath, $"data_{currentTimestamp}_part{fileCount}.csv");

                //        using (var writer = new StreamWriter(fileName, append: true))
                //        {
                //            // Write the headers if the file is newly created
                //            if (new FileInfo(fileName).Length == 0)
                //            {
                //                writer.WriteLine("DeviceId,TimeStamp,SignalId,DataValue");
                //            }

                //            writer.WriteLine(row);

                //            // Check if the file size exceeds 2MB
                //            if (new FileInfo(fileName).Length > maxFileSizeBytes)
                //            {
                //                // Create a new file
                //                fileCount++;
                //            }
                //        }
                //    }
                //}

                #endregion FileSize creation

            }
            else if (rbtnExcel.IsChecked == true)
            {
                #region By Using ExcelWork Book
                //var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LogDataItem>>(json);

                //// Get the current timestamp
                //var currentTimestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");

                //string fileName = $"data_{currentTimestamp}.xlsx";

                //using (var workbook = new XLWorkbook())
                //{
                //    var worksheet = workbook.Worksheets.Add("Data");

                //    // Write the headers
                //    worksheet.Cell(1, 1).Value = "DeviceId";
                //    worksheet.Cell(1, 2).Value = "TimeStamp";
                //    worksheet.Cell(1, 3).Value = "SignalId";
                //    worksheet.Cell(1, 4).Value = "DataValue";

                //    int row = 2;

                //    // Write the data rows
                //    foreach (var item in data)
                //    {
                //        var deviceId = item.device.Split('>')[0];
                //        var timestamp = item.device.Split('>')[1].Split('<')[0];

                //        for (int i = 0; i < item.SignalDataList.Count; i++)
                //        {
                //            var signalData = item.SignalDataList[i];
                //            var signalId = signalData.Split('>')[0];
                //            var dataValue = signalData.Split('>')[1];

                //            // Write the data values to the worksheet
                //            worksheet.Cell(row, 1).Value = deviceId;
                //            worksheet.Cell(row, 2).Value = DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("M/d/yyyy H:mm");
                //            worksheet.Cell(row, 3).Value = signalId;
                //            worksheet.Cell(row, 4).Value = dataValue;
                //            row++;
                //        }
                //    }

                //   // Save the workbook to a file
                //    workbook.SaveAs(fileName);
                //}

                //MessageBox.Show("Data logged successfully!");
                #endregion By Using ExcelWork Book
                #region working excel
                //var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LogDataItem>>(Data);

                ////Get the current timestamp
                //var currentTimestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

                //string fileName = $"data_{currentTimestamp}.xlsx";

                //// Open the file for appending or create a new file
                //using (var writer = new StreamWriter(fileName, append: true))
                //{
                //    //Write the headers if the file is newly created
                //    if (!File.Exists(fileName))
                //    {
                //        writer.WriteLine("DeviceId,TimeStamp,SignalId,DataValue");
                //    }

                //    // Write the data rows
                //    foreach (var item in data)
                //    {
                //        var deviceId = item.device.Split('>')[0];
                //        var timestamp = item.device.Split('>')[1].Split('<')[0];

                //        for (int i = 0; i < item.SignalDataList.Count; i++)
                //        {
                //            var signalData = item.SignalDataList[i];
                //            var signalId = signalData.Split('>')[0];
                //            var dataValue = signalData.Split('>')[1];

                //            var rowData = new List<string>
                //            {
                //                 deviceId,
                //                 DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("M/d/yyyy H:mm"),
                //                 signalId,
                //                 dataValue
                //          };

                //            writer.WriteLine(string.Join(",", rowData));
                //        }
                //    }
                //}
                #endregion working excel
                #region create file every 3 min once
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LogDataItem>>(Data);

                // Initialize variables
                var lastFileCreationTime = DateTime.MinValue;
                var fileCount = 1;

                // Create a subfolder for data logs if it doesn't exist
                string logsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExceldataLogs");
                Directory.CreateDirectory(logsFolderPath);

                // Iterate through data and write to files
                foreach (var item in data)
                {
                    var deviceId = item.device.Split('>')[0];
                    var timestamp = item.device.Split('>')[1].Split('<')[0];

                    for (int i = 0; i < item.SignalDataList.Count; i++)
                    {
                        var signalData = item.SignalDataList[i];
                        var signalId = signalData.Split('>')[0];
                        var dataValue = signalData.Split('>')[1];

                        var rowData = new List<string>
                        {
                            deviceId,
                            DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("M/d/yyyy H:mm"),
                            signalId,
                            dataValue
                        };

                        var row = string.Join(",", rowData);

                        DateTime now = DateTime.Now;

                        // Check if it's time to create a new file (every 3 minutes)
                        if ((now - lastFileCreationTime).TotalMinutes >= 3)
                        {
                            // Create a new file
                            lastFileCreationTime = now;
                            fileCount++;
                        }

                        string currentTimestamp = lastFileCreationTime.ToString("yyyy-MM-dd-HH-mm-ss");
                        string fileName = Path.Combine(logsFolderPath, $"data_{currentTimestamp}_part{fileCount}.xlsx");

                        using (var writer = new StreamWriter(fileName, append: true))
                        {
                            // Write the headers if the file is newly created
                            if (new FileInfo(fileName).Length == 0)
                            {
                                writer.WriteLine("DeviceId,TimeStamp,SignalId,DataValue");
                            }

                            writer.WriteLine(row);
                        }
                    }
                }

                #endregion create file every 3 min once
            }

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (btnStart.Content.ToString() == "StartTest")
            {

                
                Dequedispatcher.Start();
               
                TestdurationStopwatch.Start();
                Testdurationtimer.Start();

                btnStart.Content = "stopTest";

                TcpStartCommand();

            }
            else if (btnStart.Content.ToString() == "stopTest")
            {
                if (TestdurationStopwatch.IsRunning)
                {
                    TestdurationStopwatch.Stop();
                }
                
                Testdurationtimer.Stop();
                Dequedispatcher.Stop();
                btnStart.Content = "StartTest";
                TcpStopCommand();
                
                
            }
            
            _ = ElpisOPCServerMainWindow.TcpSocketClient.SendSignalListDataToDeviceAsync(StartStopCmd);
            //Task.Factory.StartNew(() => ElpisOPCServerMainWindow.TcpSocketClient.SendSignalListDataToDeviceAsync(StartStopCmd).ConfigureAwait(false));
            _logger.LogInfo("Start/Stop cmd : " + StartStopCmd);
        }

        #region old Deque Pattern
        //private void worker_DoWork(object sender, DoWorkEventArgs e)
        //{


        //    while (!SocketConnection.concurrentQueue.IsEmpty)
        //    {
        //        SocketConnection.concurrentQueue.TryDequeue(out dequeuedata);
        //        Debug.Print($"Dequedata:{dequeuedata}:{DateTime.Now.Millisecond}");
        //        Debug.Print($"$$$$$**Dequedata from the que**$$$$$");
        //        Tcp_DataReceived(dequeuedata);

        //    }

        //}
        #endregion old Deque Pattern
        public void btnStartTestEnable()
        {
            
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { PumpTest.btnStart.IsEnabled = true; });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            
           

        }
    }


}

