using ElpisOpcServer.SunPowerGen;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace ElpisOpcServer.ReportTemplete
{
    public class OpenLoopExcelReport
    {
        
       
        public string cellIndex;
        public int count = 0;
        string imageFile1;
        public string GraphimageFile1;
        public string GraphimageFile2;
        Image graph1;
        ExcelPicture pic1;
        ExcelPackage excel;
        public void ExcelReportGenerateReport()
        {
            HomePage.PumpTestWindow.HydgenerateButtonClicked();
            PumpTestInformation pumpTestData = HomePage.PumpTestInformation;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using (/*ExcelPackage*/ excel = new ExcelPackage())
                {
                    //createing the sheets in workbook
                    excel.Workbook.Worksheets.Add("worksheet1");
                    excel.Workbook.Worksheets.Add("worksheet2");
                    excel.Workbook.Worksheets.Add("worksheet3");
                    var workSheet = excel.Workbook.Worksheets["worksheet1"];
                    var worksheet2 = excel.Workbook.Worksheets["worksheet2"];
                    #region Report Header 
                    // Elpis Logo insert
                    #region Elpis Logo 
                    workSheet.Cells["A1:D4"].Merge = true;
                    imageFile1 = string.Format("{0}\\elpis logo white.png", Directory.GetCurrentDirectory());
                    //var Img = Image.FromFile(@"E:\Sathish.b\Testing Code\HydroFit V.1\RadioButton\New folder\HydrofitDataLogger v.1.3\Hydrofit mDAQ\SPG_ElpisOpcServer\ElpisOpcServer\Images\elpis logo white.png");
                    var Img = Image.FromFile(imageFile1);
                    var pic = workSheet.Drawings.AddPicture("LogoPicture", Img);
                    pic.SetPosition(1, 0, 0, 0);
                    pic.SetSize(200, 50);
                    workSheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    #endregion ELpis Logo


                    workSheet.Cells["E1:J4"].Merge = true;
                    workSheet.Cells["E1"].Value = "Pump/Motor Test Report";
                    workSheet.Cells["E1"].Style.Font.Bold = true;
                    workSheet.Cells["E1"].Style.Font.Size = 24;
                    workSheet.Cells["E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["E1"].Style.Font.Color.SetColor(Color.Black);

                    #region Test Type
                    workSheet.Cells["K1:L1"].Merge = true;
                    workSheet.Cells["K1"].Value = "Test Type";
                    workSheet.Cells["K1"].Style.Font.Bold = true;
                    workSheet.Cells["K1"].Style.Font.Size = 14;
                    workSheet.Cells["K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["K1"].Style.Font.Color.SetColor(Color.Black);

                    workSheet.Cells["K2:L2"].Merge = true;
                    workSheet.Cells["K2"].Value = HomePage.HomePageUIControl.txtPumpType.Text.ToString();
                    workSheet.Cells["K2"].Style.Font.Bold = true;
                    workSheet.Cells["K2"].Style.Font.Size = 14;
                    workSheet.Cells["K2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["K2"].Style.Font.Color.SetColor(Color.Red);

                    workSheet.Cells["K3:L3"].Merge = true;
                    workSheet.Cells["K3"].Value = "Date";
                    workSheet.Cells["K3"].Style.Font.Bold = true;
                    workSheet.Cells["K3"].Style.Font.Size = 14;
                    workSheet.Cells["K3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["K3"].Style.Font.Color.SetColor(Color.Black);

                    workSheet.Cells["K4:L4"].Merge = true;
                    workSheet.Cells["K4"].Value = DateTime.Now.ToString();
                    workSheet.Cells["K4"].Style.Font.Bold = true;
                    workSheet.Cells["K4"].Style.Font.Size = 14;
                    workSheet.Cells["K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["K4"].Style.Font.Color.SetColor(Color.Black);
                    #endregion Test Type
                    #endregion Report Header

                    #region General Table Details
                    workSheet.Cells["A5:F5"].Merge = true;
                    workSheet.Cells["A5"].Value = "General Details";
                    workSheet.Cells["A5"].Style.Font.Bold = true;
                    workSheet.Cells["A5"].Style.Font.Size = 14;
                    workSheet.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["A5"].Style.Font.Color.SetColor(Color.Black);



                    workSheet.Cells["G5:L5"].Merge = true;
                    workSheet.Cells["G5"].Value = "Test Parameters";
                    workSheet.Cells["G5"].Style.Font.Bold = true;
                    workSheet.Cells["G5"].Style.Font.Size = 14;
                    workSheet.Cells["G5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["G5"].Style.Font.Color.SetColor(Color.Black);
                    #endregion General Table Details

                    #region General Table Details A Coloum

                    workSheet.Cells["A6:C6"].Merge = true;
                    workSheet.Cells["A6"].Value = "Customer";
                    cellIndex = "A6";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["D6:F6"].Merge = true;
                    workSheet.Cells["D6"].Value = pumpTestData.EqipCustomerName;
                    cellIndex = "D6";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["A7:C7"].Merge = true;
                    workSheet.Cells["A7"].Value = "Manufacturer";
                    cellIndex = "A7";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["D7:F7"].Merge = true;
                    workSheet.Cells["D7"].Value = pumpTestData.EquipManufacturer;
                    cellIndex = "D7";
                    CellStyle(workSheet, cellIndex);


                    workSheet.Cells["A8:C8"].Merge = true;
                    workSheet.Cells["A8"].Value = "Type";
                    cellIndex = "A8";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["D8:F8"].Merge = true;
                    workSheet.Cells["D8"].Value = pumpTestData.EquipType;
                    cellIndex = "D8";
                    CellStyle(workSheet, cellIndex);


                    workSheet.Cells["A9:C9"].Merge = true;
                    workSheet.Cells["A9"].Value = "Model No";
                    cellIndex = "A9";
                    CellStyle(workSheet, cellIndex);


                    workSheet.Cells["D9:F9"].Merge = true;
                    workSheet.Cells["D9"].Value = pumpTestData.EquipModelNo;
                    cellIndex = "D9";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["A10:C10"].Merge = true;
                    workSheet.Cells["A10"].Value = "Serial No";
                    cellIndex = "A10";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["D10:F10"].Merge = true;
                    workSheet.Cells["D10"].Value = pumpTestData.TestSerialNo;
                    cellIndex = "D10";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["A11:C11"].Merge = true;
                    workSheet.Cells["A11"].Value = "Pump Type";
                    cellIndex = "A11";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["D11:F11"].Merge = true;
                    workSheet.Cells["D11"].Value = pumpTestData.EquipPumpType;
                    cellIndex = "D11";
                    CellStyle(workSheet, cellIndex);


                    workSheet.Cells["A12:C12"].Merge = true;
                    workSheet.Cells["A12"].Value = "Range";
                    cellIndex = "A12";
                    CellStyle(workSheet, cellIndex);


                    workSheet.Cells["D12:F12"].Merge = true;
                    workSheet.Cells["D12"].Value = pumpTestData.TestRange;
                    cellIndex = "D12";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["A13:C13"].Merge = true;
                    workSheet.Cells["A13"].Value = "Power";
                    cellIndex = "A13";
                    CellStyle(workSheet, cellIndex);


                    workSheet.Cells["D13:F13"].Merge = true;
                    workSheet.Cells["D13"].Value = HomePage.HomePageUIControl.txtPower.Text;
                    cellIndex = "D13";
                    CellStyle(workSheet, cellIndex);
                    #endregion General Table Details 


                    #region TestParameter Details A Coloum


                    workSheet.Cells["G6:I6"].Merge = true;
                    workSheet.Cells["G6"].Value = "Flow(LPM)";
                    cellIndex = "G6";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["J6:L6"].Merge = true;
                    workSheet.Cells["J6"].Value = Pump_Test.PumpTest.txtTestparameterFlow.Text;
                    cellIndex = "J6";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["G7:I7"].Merge = true;
                    workSheet.Cells["G7"].Value = "Current(AMP)";
                    cellIndex = "G7";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["J7:L7"].Merge = true;
                    workSheet.Cells["J7"].Value = Pump_Test.PumpTest.txtTestparameterCurrent.Text;
                    cellIndex = "J7";
                    CellStyle(workSheet, cellIndex);


                    workSheet.Cells["G8:I8"].Merge = true;
                    workSheet.Cells["G8"].Value = "Drain(LPM)";
                    cellIndex = "G8";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["J8:L8"].Merge = true;
                    workSheet.Cells["J8"].Value = Pump_Test.PumpTest.txtTestparameterDrain.Text;
                    cellIndex = "J8";
                    CellStyle(workSheet, cellIndex);


                    workSheet.Cells["G9:I9"].Merge = true;
                    workSheet.Cells["G9"].Value = "Volumetric(η) ";
                    cellIndex = "G9";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["J9:L9"].Merge = true;
                    workSheet.Cells["J9"].Value = Pump_Test.PumpTest.txtTestparameterVolumetricEfficiency.Text;
                    CellStyle(workSheet, cellIndex);


                    workSheet.Cells["G10:I10"].Merge = true;
                    workSheet.Cells["G10"].Value = "RPM";
                    cellIndex = "G10";
                    CellStyle(workSheet, cellIndex);


                    workSheet.Cells["J10:L10"].Merge = true;
                    workSheet.Cells["J10"].Value = Pump_Test.PumpTest.txtTestparameterRPM.Text;
                    cellIndex = "J10";
                    CellStyle(workSheet, cellIndex);


                    workSheet.Cells["G11:I11"].Merge = true;
                    workSheet.Cells["G11"].Value = "Pressure(Bar)";
                    cellIndex = "G11";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["J11:L11"].Merge = true;
                    workSheet.Cells["J11"].Value = Pump_Test.PumpTest.txtTestparameterPressure.Text;
                    cellIndex = "J11";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["G12:I12"].Merge = true;
                    workSheet.Cells["G12"].Value = "Variable Parameter";
                    cellIndex = "G12";
                    CellStyle(workSheet, cellIndex);

                    workSheet.Cells["J12:L12"].Merge = true;
                    workSheet.Cells["J12"].Value = "";
                    cellIndex = "J12";
                    CellStyle(workSheet, cellIndex);

                    #endregion TestParameter Details 



                    #region Chart Assigning to the Cell
                    // worksheet2.Cells["A13:L35"].Merge = true;
                    // //var Img1 = System.Drawing.Image.FromFile(@"E:\Sathish.b\Testing Code\HydroFit V.1\RadioButton\New folder\HydrofitDataLogger v.1.3\Hydrofit mDAQ\SPG_ElpisOpcServer\ElpisOpcServer\Images\elpis logo white.png");
                    // string imageFile1 = string.Format("{0}\\PumpChart1.png", Directory.GetCurrentDirectory());
                    // Image graph1 = Image.FromFile(imageFile1);

                    // //Image graph1 = Image.FromFile(PumpReportGeneration.imageFile1);
                    // var pic1 = worksheet2.Drawings.AddPicture("OpenLoopChart", graph1);
                    // pic1.SetPosition(13, 0, 0, 0);

                    //// pic1.SetSize(1000, 650);
                    // worksheet2.Cells["A13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    #endregion Chart Assigning to the Cell

                    #region Signal Headers
                    if (Pump_Test.PumpTest.chart1cmbXAxis.SelectedItem != null)
                    {
                        
                        #region Chart Assigning to the Cell
                        worksheet2.Cells["A13:L35"].Merge = true;
                        #region OldCode Graph Adding
                        
                        GraphimageFile1 = string.Format("{0}\\ExcelPumpChart1.png", Directory.GetCurrentDirectory());

                       

                        graph1 = Image.FromFile(GraphimageFile1);
                        pic1 = worksheet2.Drawings.AddPicture("OpenLoopChart", graph1);
                        #endregion OldCode Graph Adding

                       // pic1 = worksheet2.Drawings.AddPicture("OpenLoopChart",PumpReportGeneration.ExcelGraph1);

                        pic1.SetPosition(13, 0, 0, 0);
                        
                        // pic1.SetSize(1000, 650);
                        worksheet2.Cells["A13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        #endregion Chart Assigning to the Cell

                     
                        #region Chart1 Data Collection
                        IList<string[]> headerData = new List<string[]>(1);
                        if(Pump_Test.Chart1YaxisParaList.Contains("Volumetric Efficency(η)"))
                        {
                            Pump_Test.Chart1YaxisParaList.Remove("Volumetric Efficency(η)");
                        }
                        Pump_Test.Chart1YaxisParaList.Add("Volumetric Efficency(η)");
                        int Arraysize = Pump_Test.Chart1YaxisParaList.Count + 1;
                        string[] headerArray = new string[Arraysize];
                        for (int index = 0; index < headerArray.Length; index++)
                        {
                            int Chart1YaxisParaListindex = index - 1;

                            if (index == 0)
                            {
                                headerArray[index] = Pump_Test.Chart1XAxisCollection.First().Title;
                            }
                            else
                            {
                                headerArray[index] = Pump_Test.Chart1YaxisParaList[Chart1YaxisParaListindex];
                            }


                        }

                        headerData.Add(headerArray);

                        // Creation of header in selected sheet
                        string HeaderRange = "A15:" + char.ConvertFromUtf32(headerData[0].Length + 64) + "15";
                        workSheet.Cells[HeaderRange].LoadFromArrays(headerData);
                        //string HeaderRange = "A14:" + char.ConvertFromUtf32(stringArray[0].Length + 64) + "14";
                        //workSheet.Cells[HeaderRange].LoadFromCollection(stringArray[1]);
                        // set the Styles for the HeaderText
                        workSheet.Cells[HeaderRange].Style.Font.Bold = true;
                        workSheet.Cells[HeaderRange].Style.Font.Size = 14;
                        workSheet.Cells[HeaderRange].Style.Font.Color.SetColor(Color.Blue);
                        #endregion Chart1 Data Collection
                        
                    }
                    else
                    {
                        #region Chart Assigning to the Cell
                        worksheet2.Cells["A13:L35"].Merge = true;
                        //var Img1 = System.Drawing.Image.FromFile(@"E:\Sathish.b\Testing Code\HydroFit V.1\RadioButton\New folder\HydrofitDataLogger v.1.3\Hydrofit mDAQ\SPG_ElpisOpcServer\ElpisOpcServer\Images\elpis logo white.png");
                        //imageFile1 = string.Format("{0}\\PumpChart2.png", Directory.GetCurrentDirectory());
                        //graph1 = Image.FromFile(imageFile1);
                        GraphimageFile2 = string.Format("{0}\\ExcelPumpChart2.png", Directory.GetCurrentDirectory());
                        graph1 = Image.FromFile(GraphimageFile2);
                        //Image graph1 = Image.FromFile(PumpReportGeneration.imageFile1);
                        pic1 = worksheet2.Drawings.AddPicture("CloseLoopChart", graph1);
                        pic1.SetPosition(13, 0, 0, 0);

                        // pic1.SetSize(1000, 650);
                        worksheet2.Cells["A13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        #endregion Chart Assigning to the Cell
                        #region Chart2 DataCollection
                        IList<string[]> headerData = new List<string[]>(1);
                        if (Pump_Test.Chart2YaxisParaList.Contains("Volumetric Efficency(η)"))
                        {
                            Pump_Test.Chart2YaxisParaList.Remove("Volumetric Efficency(η)");
                        }
                        Pump_Test.Chart2YaxisParaList.Add("Volumetric Efficency(η)");
                        int Arraysize = Pump_Test.Chart2YaxisParaList.Count + 1;
                        string[] headerArray = new string[Arraysize];
                        for (int index = 0; index < headerArray.Length; index++)
                        {
                            int Chart1YaxisParaListindex = index - 1;

                            if (index == 0)
                            {
                                headerArray[index] = Pump_Test.Chart2XAxisCollection.First().Title;
                            }
                            else
                            {
                                headerArray[index] = Pump_Test.Chart2YaxisParaList[Chart1YaxisParaListindex];
                            }


                        }

                        headerData.Add(headerArray);

                        // Creation of header in selected sheet
                        string HeaderRange = "A14:" + char.ConvertFromUtf32(headerData[0].Length + 64) + "14";
                        workSheet.Cells[HeaderRange].LoadFromArrays(headerData);
                        //string HeaderRange = "A14:" + char.ConvertFromUtf32(stringArray[0].Length + 64) + "14";
                        //workSheet.Cells[HeaderRange].LoadFromCollection(stringArray[1]);
                        // set the Styles for the HeaderText
                        workSheet.Cells[HeaderRange].Style.Font.Bold = true;
                        workSheet.Cells[HeaderRange].Style.Font.Size = 14;
                        workSheet.Cells[HeaderRange].Style.Font.Color.SetColor(Color.Blue);
                        #endregion Chart2 DataCollection
                    }
                    #endregion Signal Headers
                    //List<string[]> headerRow = new List<string[]>()
                    //{

                    //    new string[]{ "PT_B1_ch0", "PT_B1_ch1", "FM_B1_ch2", "FM_B1_ch3", "FM_B1_ch4", "PCDFM_B1_ch5", "PTEP_B1_ch6", "PTEP_B1_ch7", "PT_B2_ch8" }
                    //};
                    //Custome Cell Assigine the Data to cell
                    //workSheet.Cells["F1"].Value = "My Name Is Ramya";

                    #region CellData Load to the File 
                    //var CellData = new List<object[]>()
                    //{

                    //    new object[]{"25",60,59,15},
                    //    new object[]{"Sah","vdsfv","gfyd","dgff"},
                    //    new object[]{"hdhf",256,true,"ghf"}
                    //};
                    //var data = new object[]
                    //{
                    //    "sathish",256,true,"sayan"
                    //};
                    //CellData.Add(data);
                    workSheet.Cells[16, 1].LoadFromArrays(Pump_Test.OpenExcelTableData);
                    Pump_Test.OpenExcelTableData.Clear();
                    #endregion CellData Load to the File
                    #region Footer
                    // to set the Footer for the excel


                    worksheet2.Cells["A37:C37"].Merge = true;
                    worksheet2.Cells["A37"].Value = "Tested By";
                    worksheet2.Cells["A37"].Style.Font.Bold = true;
                    worksheet2.Cells["A37"].Style.Font.Size = 14;
                    worksheet2.Cells["A37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells["A37"].Style.Font.Color.SetColor(Color.Black);
                    worksheet2.Cells["A37"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells["A37"].Style.Fill.BackgroundColor.SetColor(Color.Orange);


                    worksheet2.Cells["A38:C39"].Merge = true;
                    worksheet2.Cells["A38"].Value = pumpTestData.TestedBy;
                    worksheet2.Cells["A38"].Style.Font.Bold = true;
                    worksheet2.Cells["A38"].Style.Font.Size = 10;
                    worksheet2.Cells["A38"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells["A38"].Style.Font.Color.SetColor(Color.Black);


                    worksheet2.Cells["D37:F37"].Merge = true;
                    worksheet2.Cells["D37"].Value = "Witnessed By";
                    worksheet2.Cells["D37"].Style.Font.Bold = true;
                    worksheet2.Cells["D37"].Style.Font.Size = 14;
                    worksheet2.Cells["D37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells["D37"].Style.Font.Color.SetColor(Color.Black);
                    worksheet2.Cells["D37"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells["D37"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    worksheet2.Cells["D38:F39"].Merge = true;
                    worksheet2.Cells["D38"].Value = pumpTestData.WitnessedBy;
                    worksheet2.Cells["D38"].Style.Font.Bold = true;
                    worksheet2.Cells["D38"].Style.Font.Size = 10;
                    worksheet2.Cells["D38"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells["D38"].Style.Font.Color.SetColor(Color.Black);

                    worksheet2.Cells["G37:I37"].Merge = true;
                    worksheet2.Cells["G37"].Value = "Approved By";
                    worksheet2.Cells["G37"].Style.Font.Bold = true;
                    worksheet2.Cells["G37"].Style.Font.Size = 14;
                    worksheet2.Cells["G37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells["G37"].Style.Font.Color.SetColor(Color.Black);
                    worksheet2.Cells["G37"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells["G37"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    worksheet2.Cells["G38:I39"].Merge = true;
                    worksheet2.Cells["G38"].Value = pumpTestData.ApprovedBy;
                    worksheet2.Cells["G38"].Style.Font.Bold = true;
                    worksheet2.Cells["G38"].Style.Font.Size = 10;
                    worksheet2.Cells["G38"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells["G38"].Style.Font.Color.SetColor(Color.Black);

                    worksheet2.Cells["J37:L37"].Merge = true;
                    worksheet2.Cells["J37"].Value = "Company seal / stamp";
                    worksheet2.Cells["J37"].Style.Font.Bold = true;
                    worksheet2.Cells["J37"].Style.Font.Size = 14;
                    worksheet2.Cells["J37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells["J37"].Style.Font.Color.SetColor(Color.Black);
                    worksheet2.Cells["J37"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet2.Cells["J37"].Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    worksheet2.Cells["J38:L39"].Merge = true;
                    worksheet2.Cells["J38"].Value = "";
                    worksheet2.Cells["J38"].Style.Font.Bold = true;
                    worksheet2.Cells["J38"].Style.Font.Size = 10;
                    worksheet2.Cells["J38"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet2.Cells["J38"].Style.Font.Color.SetColor(Color.Black);


                    #endregion Footer
                    //Pump_Test.OpenExcelTableData
                    #region File Directory Creation
                    string folderName = "ExcelReport"; // Replace with your desired folder name

                    // Combine the current directory path with the folder name
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    // Create the directory if it doesn't exist
                    Directory.CreateDirectory(folderPath);

                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    FileInfo excelFile = new FileInfo(Path.Combine(folderPath, $"{pumpTestData.PumpReportNumber}_{count++}{HomePage.HomePageUIControl.txtPumpType.Text}_{timestamp}.xlsx"));

                    excel.SaveAs(excelFile);
                    
                    
                    //MessageBox.Show("Program Execute Successfully");
                    #endregion File Directory Creation
                }
                //File.Delete(GraphimageFile1);
                //File.Delete(GraphimageFile2);
            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Dispose of Image object
                if (graph1 != null)
                {
                    graph1.Dispose();
                }

                // Dispose of Excel objects
                if (pic1 != null)
                {
                    pic1.Dispose();
                }

                if (excel != null)
                {
                    excel.Dispose();
                }
                // Clean up image files after adding them to the table
                if (GraphimageFile1!=null&& GraphimageFile2!=null)
                {
                    File.Delete(GraphimageFile1);
                    File.Delete(GraphimageFile2);
                }

            }

        }
        public static void CellStyle(ExcelWorksheet workSheet, string cellIndex)
        {
            workSheet.Cells[cellIndex].Style.Font.Bold = true;
            workSheet.Cells[cellIndex].Style.Font.Size = 10;
            workSheet.Cells[cellIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[cellIndex].Style.Font.Color.SetColor(Color.Black);
        }
    }
}
