using ElpisOpcServer.SunPowerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using excel= Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ElpisOpcServer.ReportTemplete
{
    public class ExcelReport
    {
        int filecount = 0;
        excel.Application ExcelApp;
        excel.Workbook ExcelWorkBook = null;
        excel.Worksheet workSheet1 = null;
        excel.Worksheet workSheet2 = null;
        public ExcelReport()
        {
            
        }
        
        public void MSExcelReportGeneration()
        {
            ExcelApp = new Application();
            ExcelWorkBook = ExcelApp.Workbooks.Add();
            try
            {
                workSheet2 = ExcelWorkBook.Worksheets.Add();
                workSheet2.Name = "Worksheet2";

                workSheet1 = ExcelWorkBook.Worksheets.Add();
                workSheet1.Name = "Worksheet1";


                excel.Range Logo = workSheet1.get_Range("A1", "D4");
                Logo.Merge();
                //C:\Users\Sathish\Downloads\ExcelReportGeneration\ExcelReportGeneration\Images\elpis logo white.png
                string imageFile1 = string.Format("{0}\\elpis logo white.png", Directory.GetCurrentDirectory());
                //Shape picture = workSheet1.Shapes.AddPicture(imageFile1, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                excel.Shape picture = workSheet1.Shapes.AddPicture(imageFile1, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 190, 50);
                Logo.HorizontalAlignment = excel.XlHAlign.xlHAlignCenter;

                

                #region Header Details 

                excel.Range heading = workSheet1.get_Range("E1", "J4");
                heading.Merge();
                heading.Font.Bold = true;
                heading.Font.Size = 24;
                heading.HorizontalAlignment = excel.XlHAlign.xlHAlignCenter;
                heading.Font.Color = ColorTranslator.ToOle(System.Drawing.Color.Red);
                heading.Value = "Pump/Motor Test Report";
                #endregion Header Details values

                #region General Details Lables
                excel.Range generalDetails = workSheet1.get_Range("A5", "F5");
                generalDetails.Merge();
                generalDetails.Font.Bold = true;
                generalDetails.HorizontalAlignment = excel.XlHAlign.xlHAlignCenter;
                generalDetails.Font.Size = 14;
                generalDetails.Value = "General Details";

                excel.Range testParameters = workSheet1.get_Range("G5", "L5");
                testParameters.Merge();
                testParameters.Font.Bold = true;
                testParameters.HorizontalAlignment = excel.XlHAlign.xlHAlignCenter;
                testParameters.Font.Size = 14;
                testParameters.Value = "Test Parameters";

                excel.Range lblcustomer = workSheet1.get_Range("A6", "C6");
                LblFontStye(lblcustomer);
                lblcustomer.Value = "Customer";

                excel.Range lblmanufacture = workSheet1.get_Range("A7", "C7");
                LblFontStye(lblmanufacture);
                lblmanufacture.Value = "Manufacturer";

                excel.Range lbltype = workSheet1.get_Range("A8", "C8");
                LblFontStye(lbltype);
                lbltype.Value = "Type";


                excel.Range lblModelNo = workSheet1.get_Range("A9", "C9");
                LblFontStye(lblModelNo);
                lblModelNo.Value = "Model No";

                excel.Range lblSerialNo = workSheet1.get_Range("A10", "C10");
                LblFontStye(lblSerialNo);
                lblSerialNo.Value = "Serial No";

               
                excel.Range lblPumpType = workSheet1.get_Range("A11", "C11");
                LblFontStye(lblPumpType);
                lblPumpType.Value = "Pump Type";

                excel.Range lblRange = workSheet1.get_Range("A12", "C12");
                LblFontStye(lblRange);
                lblRange.Value = "Range";

                excel.Range lblPower = workSheet1.get_Range("A13", "C13");
                LblFontStye(lblPower);
                lblPower.Value = "Power";

               
                #endregion General Details Lables

                #region General Details values
                excel.Range txtelpis = workSheet1.get_Range("D6", "F6");
                txtFontStye(txtelpis);
                txtelpis.Value = "Elpis";

                excel.Range txthydrofit = workSheet1.get_Range("D7", "F7");
                txtFontStye(txthydrofit);
                txthydrofit.Value = "Hydrofit";

                excel.Range txtSensorType = workSheet1.get_Range("D8", "F8");
                txtFontStye(txtSensorType);
                txtSensorType.Value = "SensorType";



                excel.Range txtModel = workSheet1.get_Range("D9", "F9");
                txtFontStye(txtModel);
                txtModel.Value = "Hyd_2";


                excel.Range txtSerialNo = workSheet1.get_Range("D10", "F10");
                txtFontStye(txtSerialNo);
                txtSerialNo.Value = "220000318";



                excel.Range txtHYD_test = workSheet1.get_Range("D11", "F11");
                txtFontStye(txtHYD_test);
                txtHYD_test.Value = "HYD_test";

                

                excel.Range txtrange = workSheet1.get_Range("D12", "F12");
                txtFontStye(txtrange);
                txtrange.Value = "0-450 BAR";

                excel.Range txtpower = workSheet1.get_Range("D13", "F13");
                txtFontStye(txtpower);
                #endregion General Details values

                #region TestParameters Details Lables
                excel.Range lblFlow = workSheet1.get_Range("G6", "I6");
                LblFontStye(lblFlow);
                lblFlow.Value = "Flow(LPM)";


                excel.Range lblCurrent = workSheet1.get_Range("J6", "L6");
                LblFontStye(lblCurrent);
                lblCurrent.Value = "Current(AMP)";

                excel.Range lblDrain = workSheet1.get_Range("G7", "I7");
                LblFontStye(lblDrain);
                lblDrain.Value = "Drain(LPM)";

                excel.Range lblVolumetric = workSheet1.get_Range("G8", "I8");
                LblFontStye(lblVolumetric);
                lblVolumetric.Value = "Volumetric(\u03B7)";

                excel.Range lblrpm = workSheet1.get_Range("G9", "I9");
                LblFontStye(lblrpm);
                lblrpm.Value = "RPM";

                excel.Range lblpressure = workSheet1.get_Range("G10", "I10");
                LblFontStye(lblpressure);
                lblpressure.Value = "Pressure";

                excel.Range lblvariableparameter = workSheet1.get_Range("G11", "I11");
                LblFontStye(lblvariableparameter);
                lblvariableparameter.Value = "Variable Parameter";

                #endregion TestParameters Details Lables

                #region TestParameters Details values
                excel.Range txtvariableparameter = workSheet1.get_Range("J12", "L12");
                txtFontStye(txtvariableparameter);
                txtvariableparameter.Value = "";

                excel.Range txtFlow = workSheet1.get_Range("J6", "L6");
                txtFontStye(txtFlow);
                txtFlow.Value = "0";

                excel.Range txtCurrent = workSheet1.get_Range("J7", "L7");
                txtFontStye(txtCurrent);
                txtCurrent.Value = "0";

                excel.Range txtDrain = workSheet1.get_Range("J8", "L8");
                txtFontStye(txtDrain);
                txtDrain.Value = "0";

                excel.Range txtVolumetric = workSheet1.get_Range("J9", "L9");
                txtFontStye(txtVolumetric);
                txtVolumetric.Value = "0";

                excel.Range txtrpm = workSheet1.get_Range("J10", "L10");
                txtFontStye(txtrpm);
                txtrpm.Value = "0";

                excel.Range txtpressure = workSheet1.get_Range("J11", "L11");
                txtFontStye(txtpressure);
                txtpressure.Value = "0";
                #endregion TestParameters Details values


               

                //string filename = $"E:\\ReportExcelDocument{filecount++}_{DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss")}.xlsx";
                #region File Directory Creation
                string folderName = "MsExcelReport"; // Replace with your desired folder name

                // Combine the current directory path with the folder name
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                // Create the directory if it doesn't exist
                Directory.CreateDirectory(folderPath);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                FileInfo excelFile = new FileInfo(Path.Combine(folderPath, $"Excerpeort_{filecount++}_{timestamp}.xlsx"));


                //string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                //// Create the File Name 
                //FileInfo excelFile = new FileInfo($"{pumpTestData.PumpReportNumber}_{count++}{HomePage.HomePageUIControl.txtPumpType.Text.ToString()}_{timestamp}.xlsx");

                //// Save the ExcelWork book as custom name
                //excel.SaveAs(excelFile);


                #endregion File Directory Creation  
                ExcelWorkBook.SaveAs(excelFile);

                ExcelWorkBook.Close();

                ExcelApp.Quit();

                Marshal.ReleaseComObject(workSheet1);

                Marshal.ReleaseComObject(ExcelWorkBook);

                Marshal.ReleaseComObject(ExcelApp);

            }

            catch (Exception exHandle)

            {
                System.Windows.MessageBox.Show("Exception: " + exHandle.Message);

            }

            //finally

            //{
            //    foreach (Process process in Process.GetProcessesByName("Excel"))
            //     process.Kill();

            //}
        }

        private static void LblFontStye(excel.Range txtvariableparameter)
        {
            txtvariableparameter.Merge();
            txtvariableparameter.Font.Bold = true;
            txtvariableparameter.Font.Size = 11;
            txtvariableparameter.HorizontalAlignment = excel.XlHAlign.xlHAlignCenter;
            txtvariableparameter.VerticalAlignment = excel.XlVAlign.xlVAlignCenter;
        }
        private static void txtFontStye(excel.Range txtvariableparameter)
        {
            txtvariableparameter.Merge();
            txtvariableparameter.Font.Bold = true;
            txtvariableparameter.Font.Size = 11;
            txtvariableparameter.HorizontalAlignment = excel.XlHAlign.xlHAlignCenter;
            txtvariableparameter.VerticalAlignment = excel.XlVAlign.xlVAlignCenter;
        }
    }
}
