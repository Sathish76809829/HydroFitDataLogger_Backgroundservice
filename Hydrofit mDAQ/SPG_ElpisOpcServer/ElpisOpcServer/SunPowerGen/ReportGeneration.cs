﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LiveCharts;
using LiveCharts.Wpf;
using System.Configuration;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Elpis.Windows.OPC.Server;
using System.Collections.ObjectModel;

namespace ElpisOpcServer.SunPowerGen
{
    /// <summary>
    /// This Class Generates CSV, PDF file.
    /// </summary>
    public class ReportGeneration
    {
        #region Properties
        internal static string FilePrefix = ConfigurationManager.AppSettings["ReportPrefix"].ToString(); //ConfigurationSettings.AppSettings.Get["ReportPrefix"].ToString();
        internal static string ReportLocation { get; set; }/// = string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReportLocation"].ToString()) ? Directory.GetCurrentDirectory() : ConfigurationManager.AppSettings["ReportLocation"].ToString();
        internal static ObservableCollection<SeriesCollection> seriesCollection { get; set; }
        internal static ObservableCollection<List<string>> labelCollection { get; set; }
        #endregion

        public ReportGeneration()
        {
            if (string.IsNullOrEmpty(ReportLocation))
            {
                ReportLocation = ConfigurationManager.AppSettings["ReportLocation"].ToString();
                if (string.IsNullOrEmpty(ReportLocation) || !(Directory.Exists(ReportLocation)))
                {
                    ReportLocation = string.Format("{0}\\Reports", Directory.GetCurrentDirectory());
                }
            }

        }

        #region PDF File  related operation
        /// <summary>
        /// It Generates a PDF report file.
        /// </summary>
        /// <param name="testType"></param>
        /// <param name="testInfo"></param>
        /// <param name="seriesCollection"></param>
        /// <param name="labelCollection"></param>
        /// <param name="noofCyclesCompleted"></param>
        public void GeneratePDFReport(TestType testType, object testInfo, ObservableCollection<SeriesCollection> seriesCollection, ObservableCollection<List<string>> labelCollection, bool createNew = false, string noofCyclesCompleted = null)
        {
            //string pdfFileName = null;
            //int fileCount = 0;
            //Document pdfDoc = null;
            //try
            //{

            //    #region stroke test
            //    if (testType == TestType.StrokeTest)
            //    {
            //        StrokeTestInformation testInformation = testInfo as StrokeTestInformation;
            //        pdfFileName = string.Format(@"{0}\{2}\{3}\{1}_{2} _{3}_CylinderTest_001_{4}.pdf", ReportLocation, FilePrefix, testInformation.JobNumber, testInformation.CylinderNumber, (testInformation.TestDateTime.Replace(':', '_')).Replace('/', '-')); // ConfigurationSettings.AppSettings.Get("PdfReportLocation").ToString()
            //        Directory.CreateDirectory(string.Format(@"{0}\{1}\{2}", ReportLocation, testInformation.JobNumber, testInformation.CylinderNumber));
            //        pdfDoc = new Document(PageSize.A4, 10, 10, 30, 30);
            //        //pdfDoc.AddHeader("Cylinder Test Report", "Test Report");
            //        if (File.Exists(pdfFileName))
            //        {
            //            if (!createNew) //messageOption == MessageBoxResult.Yes)
            //            {
            //                try
            //                {
            //                    File.Delete(pdfFileName);
            //                }
            //                catch (Exception ex)
            //                {
            //                    //MessageBox.Show(ex.Message, "SPG Reporting Tool", MessageBoxButton.OK, MessageBoxImage.Error);
            //                    throw ex;
            //                }
            //            }
            //            else
            //            {
            //                while (File.Exists(pdfFileName))
            //                {
            //                    fileCount++;
            //                    if (fileCount == 1)
            //                        pdfFileName = string.Concat(pdfFileName.Substring(0, pdfFileName.LastIndexOf('.')), '(', fileCount, ')', ".pdf");
            //                    else
            //                        pdfFileName = string.Concat(pdfFileName.Substring(0, pdfFileName.LastIndexOf('(')), '(', fileCount, ')', ".pdf");
            //                }
            //            }
            //        }

            //        using (PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, new FileStream(pdfFileName, System.IO.FileMode.Create)))
            //        {
            //            PDFHeaderFooter headerFooter = new PDFHeaderFooter();
            //            headerFooter.testInformation = testInformation;
            //            pdfWriter.PageEvent = headerFooter;
            //            pdfDoc.Open();

            //            //This adds the header on top of page.

            //            //AddHeadertoCertificate(testInformation, pdfDoc,pdfWriter);
            //            PdfPTable table = new PdfPTable(1);
            //            table.AddCell(new PdfPCell() { MinimumHeight = 40f, Border = Rectangle.NO_BORDER });
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);

            //            AddCeritificateInformation(testInformation, pdfDoc);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);

            //            AddCylinderInformation(testInformation, pdfDoc);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);


            //            var content = pdfWriter.DirectContent;
            //            var pageBorderRect = new Rectangle(pdfDoc.PageSize);
            //            pageBorderRect.Left += pdfDoc.LeftMargin + 15;
            //            pageBorderRect.Right -= pdfDoc.RightMargin + 15;
            //            pageBorderRect.Top -= pdfDoc.TopMargin - 2;
            //            pageBorderRect.Bottom += pdfDoc.BottomMargin - 2;

            //            content.SetColorStroke(BaseColor.ORANGE);
            //            content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
            //            content.Stroke();


            //            pdfDoc.NewPage();

            //            table = new PdfPTable(1);
            //            table.AddCell(new PdfPCell() { MinimumHeight = 10f, Border = Rectangle.NO_BORDER });

            //            pdfDoc.Add(table);

            //            PdfPTable t1 = new PdfPTable(1);
            //            Font f1 = new Font(Font.FontFamily.HELVETICA, 15f, Font.BOLD, BaseColor.BLACK);
            //            Chunk c1 = new Chunk("STROKE TEST REPORT", f1);
            //            PdfPCell cell = new PdfPCell(new Paragraph(c1)) { Border = Rectangle.NO_BORDER, PaddingTop = 10f, PaddingBottom = 10f, HorizontalAlignment = Element.ALIGN_CENTER };
            //            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            //            t1.AddCell(cell);
            //            pdfDoc.Add(t1);
            //            pdfDoc.Add(table);

            //            AddReportNumber(testInformation, pdfDoc);
            //            pdfDoc.Add(table);

            //            AddTestParameterInformation(testInformation, pdfDoc);
            //            pdfDoc.Add(table);

            //            AddMaximumAllowablePewssure(testInformation, pdfDoc);
            //            pdfDoc.Add(table);

            //            AddGraphstoCertificate(seriesCollection, labelCollection, pdfDoc, TestType.StrokeTest);
            //            pdfDoc.Add(table);

            //            AddNumberofCycles(pdfDoc, noofCyclesCompleted);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);
            //            pdfDoc.Add(table);

            //            content = pdfWriter.DirectContent;
            //            pageBorderRect = new Rectangle(pdfDoc.PageSize);

            //            pageBorderRect.Left += pdfDoc.LeftMargin + 15;
            //            pageBorderRect.Right -= pdfDoc.RightMargin + 15;
            //            pageBorderRect.Top -= pdfDoc.TopMargin - 2;
            //            pageBorderRect.Bottom += pdfDoc.BottomMargin - 2;

            //            content.SetColorStroke(BaseColor.YELLOW);
            //            content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
            //            content.Stroke();

            //            pdfDoc.Close();
            //        }
            //    }
            //    #endregion

            //    #region Hold or Mid Position Test
            //    else if (testType == TestType.HoldMidPositionTest)
            //    {
            //        Hold_MidPositionLineATestInformation testInformation = testInfo as Hold_MidPositionLineATestInformation;
            //        pdfFileName = string.Format(@"{0}\{2}\{3}\{1}_{2} _HoldMidPositionTest_002_{4}.pdf", ReportLocation, FilePrefix, testInformation.JobNumber, testInformation.CylinderNumber, (testInformation.TestDateTime.Replace(':', '_')).Replace('/', '-')); // ConfigurationSettings.AppSettings.Get("PdfReportLocation").ToString()
            //        Directory.CreateDirectory(string.Format(@"{0}\{1}", ReportLocation, testInformation.JobNumber));
            //        pdfDoc = new Document(PageSize.A4, 5, 5, 40, 35);
            //        while (File.Exists(pdfFileName))
            //        {
            //            fileCount++;
            //            if (fileCount == 1)
            //                pdfFileName = string.Concat(pdfFileName.Substring(0, pdfFileName.LastIndexOf('.')), '(', fileCount, ')', ".pdf");
            //            else
            //                pdfFileName = string.Concat(pdfFileName.Substring(0, pdfFileName.LastIndexOf('(')), '(', fileCount, ')', ".pdf");
            //        }
            //        using (PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, new FileStream(pdfFileName, System.IO.FileMode.Append)))
            //        {
            //            pdfDoc.Open();
            //            Paragraph para = new Paragraph("");
            //            pdfDoc.Add(para);
            //            //This adds the header on top of page.
            //            AddHeadertoCertificate(testInformation, pdfDoc, pdfWriter);
            //            AddCeritificateInformation(testInformation, pdfDoc);
            //            AddCylinderInformation(testInformation, pdfDoc);
            //            AddTestParameterInformation(testInformation, pdfDoc, "LineA");
            //            AddMaximumAllowablePewssure(testInformation, pdfDoc);
            //            AddGraphstoCertificate(seriesCollection, labelCollection, pdfDoc, TestType.HoldMidPositionTest);
            //            AddTestStatus(pdfDoc, testInformation.TestStatus);
            //            pdfDoc.Close();
            //        }
            //    }
            //    #endregion

            //    #region Slip Stick Test
            //    else if (testType == TestType.SlipStickTest)
            //    {
            //        Slip_StickTestInformation testInformation = testInfo as Slip_StickTestInformation;
            //        pdfFileName = string.Format(@"{0}\{2}\{3}\{1}_{2} SlipStickTest_003_{4}.pdf", ReportLocation, FilePrefix, testInformation.JobNumber, testInformation.CylinderNumber, (testInformation.TestDateTime.Replace(':', '_')).Replace('/', '-')); // ConfigurationSettings.AppSettings.Get("PdfReportLocation").ToString()
            //        Directory.CreateDirectory(string.Format(@"{0}\{1}\{2}", ReportLocation, testInformation.JobNumber, testInformation.CylinderNumber));
            //        pdfDoc = new Document(PageSize.A4, 5, 5, 40, 35);

            //        while (File.Exists(pdfFileName))
            //        {
            //            fileCount++;
            //            if (fileCount == 1)
            //                pdfFileName = string.Concat(pdfFileName.Substring(0, pdfFileName.LastIndexOf('.')), '(', fileCount, ')', ".pdf");
            //            else
            //                pdfFileName = string.Concat(pdfFileName.Substring(0, pdfFileName.LastIndexOf('(')), '(', fileCount, ')', ".pdf");
            //        }


            //        using (PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, new FileStream(pdfFileName, System.IO.FileMode.Create)))
            //        {
            //            pdfDoc.Open();
            //            Paragraph para = new Paragraph("");
            //            pdfDoc.Add(para);
            //            //This adds the header on top of page.
            //            AddHeadertoCertificate(testInformation, pdfDoc, pdfWriter);
            //            PdfPTable table = new PdfPTable(1);
            //            table.AddCell(new PdfPCell() { MinimumHeight = 10f, Border = Rectangle.NO_BORDER });
            //            pdfDoc.Add(table);
            //            AddCeritificateInformation(testInformation, pdfDoc);
            //            pdfDoc.Add(table);
            //            AddCylinderInformation(testInformation, pdfDoc);
            //            pdfDoc.Add(table);
            //            pdfDoc.NewPage();
            //            PdfPTable t1 = new PdfPTable(1);
            //            Font f1 = new Font(Font.FontFamily.TIMES_ROMAN, 17.0f, Font.BOLD, BaseColor.BLACK);
            //            Chunk c1 = new Chunk("SlipStick Test Report", f1);

            //            PdfPCell cell = new PdfPCell(new Paragraph(c1)) { PaddingTop = 5f, PaddingBottom = 5f, VerticalAlignment = 1 };
            //            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            //            t1.AddCell(cell);
            //            pdfDoc.Add(t1);
            //            AddTestParameterInformation(testInformation, pdfDoc);
            //            pdfDoc.Add(table);

            //            AddGraphstoCertificate(seriesCollection, labelCollection, pdfDoc, TestType.SlipStickTest);
            //            //pdfDoc.Add(table);
            //            //AddNumberofCycles(pdfDoc, noofCyclesCompleted);

            //            pdfDoc.Close();
            //        }
            //    }
            //    #endregion
            //    //MessageBox.Show("Report file created in following path:\n" + pdfFileName, "SPG Report Tool", MessageBoxButton.OK);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("File Path:" + pdfFileName + "\n" + ex.Message);
            //    throw ex;
            //}
            //finally
            //{
            //    if (pdfDoc != null && pdfDoc.IsOpen())
            //        pdfDoc.Close();

            //}

        }

        /// <summary>
        /// Adds the Report number, Date time to documnet.
        /// </summary>
        /// <param name="testInfo"></param>
        /// <param name="pdfDoc"></param>
        private void AddReportNumber(dynamic testInfo, Document pdfDoc)
        {
            try
            {
                Font arial_Heading = FontFactory.GetFont("Arial", 15, BaseColor.BLACK);
                Font arial_Content = FontFactory.GetFont("Arial", 11.5f, BaseColor.BLACK);

                PdfPTable table = new PdfPTable(2);
                table.SetWidths(new float[] { 1, 2 });
                table.WidthPercentage = 85;
                PdfPCell cell = new PdfPCell(new Paragraph("Test Details", arial_Heading)) { BackgroundColor = BaseColor.LIGHT_GRAY, PaddingTop = 5, PaddingBottom = 5, VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER }; //Border = Rectangle.NO_BORDER, //new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD, BaseColor.BLACK)
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                Chunk c = new Chunk("Report Number", arial_Content);// new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c)) { PaddingTop = 5, PaddingBottom = 5 };// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.ReportNumber.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 };// Border = Rectangle.NO_BORDER, //new Font(Font.FontFamily.HELVETICA, 12f, Font.ITALIC, BaseColor.BLACK)
                table.AddCell(cell);

                c = new Chunk("Test DataTime", arial_Content);
                cell = new PdfPCell(new Paragraph(c)) { PaddingTop = 5, PaddingBottom = 5 };// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.TestDateTime.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 }; //Border = Rectangle.NO_BORDER,
                table.AddCell(cell);

                PdfPCell cell1 = new PdfPCell(table);
                cell1.AddElement(table);
                PdfPTable table1 = new PdfPTable(1);
                table1.AddCell(cell1);
                pdfDoc.Add(table1);
                // MessageBox.Show("Test Parameters Added");
            }
            catch (Exception)
            {

            }

        }
        //int fileCount = 0;
        public void GeneratePDFReport(string jobNumber, string customerName, string cylinderNumber, bool createNew)
        {

            string pdfFileName = null;
            Document pdfDoc = null;
            try
            {
               
                pdfFileName = string.Format(@"{0}\{2}_{4}\{3}\{1}_{2}_{3}_CylinderTest.pdf", ReportLocation, FilePrefix, jobNumber , cylinderNumber, customerName);
                Directory.CreateDirectory(string.Format(@"{0}\{1}_{3}\{2}", ReportLocation, jobNumber, cylinderNumber, customerName));
                pdfDoc = new Document(PageSize.A3, 10f, 10f,95f, 8f);// 5, 5, 40, 35);// 10f, 10f, 140f, 10f);

                if (File.Exists(pdfFileName))
                {
                    MessageBoxResult messageOption = MessageBox.Show("Certificate with same name already exists.\nDo you want replace it?", "SPG Report Tool", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (/*!createNew*/ messageOption==MessageBoxResult.Yes)
                    {
                        try
                        {
                            File.Delete(pdfFileName);
                            using (PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, new FileStream(pdfFileName, System.IO.FileMode.Create)))
                            {
                                Font arial = FontFactory.GetFont("Arial", 28, BaseColor.BLACK);

                                PDFHeaderFooter headerFooter = new PDFHeaderFooter();
                                ITextEvents itext = new ITextEvents();

                                dynamic testInfo = null;
                                string[] file = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*.csv");
                                if (file.Length > 0)
                                {
                                    testInfo = Helper.GetTestInformation(file[0], TestType.StrokeTest) as StrokeTestInformation;

                                    headerFooter.testInformation = testInfo;
                                    itext.testInformation = testInfo;
                                    pdfWriter.PageEvent = itext;//headerFooter;
                                    pdfDoc.Open();
                                    Paragraph para = new Paragraph("");
                                    pdfDoc.Add(para);
                                    PdfPTable table = new PdfPTable(1);
                                    table.AddCell(new PdfPCell() { MinimumHeight = 10f, Border = Rectangle.NO_BORDER });
                                    string[] dataFiles = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*.csv");
                                    //for (int file = 0; file < dataFiles.Length; file++)
                                    //{
                                    //    // return;
                                    // string fileName = dataFiles[file];
                                    string fileName = string.Empty;
                                    for (int i = 1; i <= 3; i++)
                                    {
                                        if (i == 1)
                                        {
                                            file = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*stroketest*.csv");
                                            ObservableCollection<LineSeries> lineSeriesCollection = null;
                                            // ObservableCollection<List<string>> linelabelCollection = null;
                                            //ObservableCollection < SeriesCollection > seriesCollection1= null;
                                            if (file.Length > 0)
                                            {
                                                if (file.Length > 2)
                                                {
                                                    fileName = file[file.Length - 2];
                                                }
                                                else
                                                {
                                                    fileName = file[0];
                                                }
                                                if (!string.IsNullOrEmpty(fileName))
                                                {
                                                    StrokeTestInformation testInformation = Helper.GetTestInformation(fileName, TestType.StrokeTest) as StrokeTestInformation;
                                                    lineSeriesCollection = Helper.GetSeriesCollection(fileName, 4, TestType.StrokeTest);
                                                    lineSeriesCollection[0].ScalesYAt = 0;
                                                    lineSeriesCollection[1].ScalesYAt = 1;
                                                    lineSeriesCollection[2].ScalesYAt = 0;
                                                    lineSeriesCollection[3].ScalesYAt = 1;
                                                    if (lineSeriesCollection.Count == 4)
                                                        seriesCollection = new ObservableCollection<SeriesCollection> { new SeriesCollection() { lineSeriesCollection[0], lineSeriesCollection[1] }, new SeriesCollection() { lineSeriesCollection[2], lineSeriesCollection[3] } };
                                                    labelCollection = Helper.GetLabelCollection(fileName, 4);
                                                    table = new PdfPTable(1);
                                                    table.AddCell(new PdfPCell() { MinimumHeight = 10f, Border = Rectangle.NO_BORDER });
                                                    pdfDoc.Add(table);

                                                    AddCylinderInformation(testInformation, pdfDoc);
                                                    pdfDoc.Add(table);

                                                    PdfPTable t1 = new PdfPTable(1);
                                                    Font f1 = new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD, BaseColor.BLACK);
                                                    Chunk c1 = new Chunk("STROKE TEST REPORT", f1);
                                                    PdfPCell cell = new PdfPCell(new Paragraph(c1)) { Border = Rectangle.NO_BORDER, PaddingTop = 5f, PaddingBottom = 5f, VerticalAlignment = Element.ALIGN_CENTER };
                                                    cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                                    t1.AddCell(cell);
                                                    pdfDoc.Add(t1);
                                                    pdfDoc.Add(table);

                                                    AddReportNumber(testInformation, pdfDoc);
                                                    pdfDoc.Add(table);

                                                    AddTestParameterInformation(testInformation, pdfDoc);
                                                    pdfDoc.Add(table);

                                                    AddMaximumAllowablePewssure(testInformation, pdfDoc);
                                                    pdfDoc.Add(table);

                                                    AddGraphstoCertificate(seriesCollection, labelCollection, pdfDoc, TestType.StrokeTest); //(seriesCollection, labelCollection, pdfDoc);
                                                    pdfDoc.Add(table);


                                                    //AddNumberofCycles(pdfDoc, testInformation.NoofCyclesCompleted.ToString());
                                                    //pdfDoc.Add(table);
                                                    //pdfDoc.Add(table);
                                                    if (HomePage.strokeTestInfo.Comment!="")
                                                    {
                                                        AddCommnetBox(pdfDoc, HomePage.strokeTestInfo.Comment);
                                                        pdfDoc.Add(table);
                                                        pdfDoc.Add(table);
                                                    }

                                                    var content = pdfWriter.DirectContent;
                                                    var pageBorderRect = new Rectangle(pdfDoc.PageSize);

                                                    pageBorderRect.Left += pdfDoc.LeftMargin + 15;
                                                    pageBorderRect.Right -= pdfDoc.RightMargin + 15;
                                                    pageBorderRect.Top -= pdfDoc.TopMargin - 2;
                                                    pageBorderRect.Bottom += pdfDoc.BottomMargin - 2;

                                                    content.SetColorStroke(BaseColor.DARK_GRAY);
                                                    content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
                                                    content.Stroke();
                                                    //ElpisOPCServerMainWindow.homePage.txtStatusMessage.Text = HomePage.strokeTestInfo.ReportStatus = "Report Generated Successfully.";
                                                    ElpisOPCServerMainWindow.pump_Test.txtStatusMessage.Text = HomePage.strokeTestInfo.ReportStatus = "Report Generated Successfully.";

                                                }
                                                fileName = string.Empty;
                                            }
                                        }

                                        if (i == 2)
                                        {
                                            bool isPageAdded = false;
                                            file = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*hmpatest*.csv");
                                            ObservableCollection<LineSeries> lineSeriesCollection = null;
                                            // ObservableCollection<List<string>> linelabelCollection = null;
                                            if (file.Length > 0)
                                            {
                                                if (file.Length > 2)
                                                {
                                                    fileName = file[file.Length - 2];
                                                }
                                                else
                                                {
                                                    fileName = file[0];
                                                }
                                                if (!string.IsNullOrEmpty(fileName))
                                                {
                                                    pdfDoc.NewPage();
                                                    isPageAdded = true;
                                                    Hold_MidPositionLineATestInformation testInformation = Helper.GetTestInformation(fileName, TestType.HoldMidPositionTest) as Hold_MidPositionLineATestInformation;
                                                    lineSeriesCollection = Helper.GetSeriesCollection(fileName, 2, TestType.HoldMidPositionTest);
                                                    lineSeriesCollection[1].ScalesYAt = 1;
                                                    if (lineSeriesCollection.Count == 2)
                                                        seriesCollection = new ObservableCollection<SeriesCollection> { new SeriesCollection() { lineSeriesCollection[0], lineSeriesCollection[1] } };
                                                    labelCollection = Helper.GetLabelCollection(fileName, 2);

                                                    PdfPTable t1 = new PdfPTable(1);
                                                    Font f1 = new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD, BaseColor.BLACK);
                                                    Chunk c1 = new Chunk("HOLD/MID POSITION TEST REPORT LINE A", f1);
                                                    PdfPCell cell = new PdfPCell(new Paragraph(c1)) { Border = Rectangle.NO_BORDER, PaddingTop = 5f, PaddingBottom = 5f, VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER };

                                                    t1.AddCell(cell);
                                                    pdfDoc.Add(t1);


                                                    AddReportNumber(testInformation, pdfDoc);
                                                    pdfDoc.Add(table);

                                                    AddTestParameterInformation(testInformation, pdfDoc, "LineA");
                                                    pdfDoc.Add(table);
                                                    AddGraphstoCertificate(seriesCollection, labelCollection, pdfDoc, TestType.HoldMidPositionTest);

                                                    AddTestStatus(pdfDoc, testInformation.TestStatusA.ToString());
                                                    if (HomePage.holdMidPositionLineAInfo.Comment != "")
                                                    {
                                                        AddCommnetBox(pdfDoc, HomePage.holdMidPositionLineAInfo.Comment);
                                                    }



                                                    var content = pdfWriter.DirectContent;
                                                    var pageBorderRect = new Rectangle(pdfDoc.PageSize);

                                                    pageBorderRect.Left += pdfDoc.LeftMargin + 15;
                                                    pageBorderRect.Right -= pdfDoc.RightMargin + 15;
                                                    pageBorderRect.Top -= pdfDoc.TopMargin - 2;
                                                    pageBorderRect.Bottom += pdfDoc.BottomMargin - 2;

                                                    content.SetColorStroke(BaseColor.DARK_GRAY);
                                                    content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
                                                    content.Stroke();
                                                    //ElpisOPCServerMainWindow.homePage.txtStatusMessage.Text = HomePage.holdMidPositionLineAInfo.ReportStatus = "Report Generated Successfully.";
                                                    ElpisOPCServerMainWindow.pump_Test.txtStatusMessage.Text = HomePage.holdMidPositionLineAInfo.ReportStatus = "Report Generated Successfully.";
                                                    fileName = string.Empty;
                                                }
                                            }

                                            file = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*hmpbtest*.csv");
                                            if (file.Length > 0)
                                            {
                                                if (file.Length > 2)
                                                {
                                                    fileName = file[file.Length - 2];
                                                }
                                                else
                                                {
                                                    fileName = file[0];
                                                }
                                                if (!string.IsNullOrEmpty(fileName))
                                                {
                                                    // if (!isPageAdded)
                                                    pdfDoc.NewPage();
                                                    PdfPTable t1 = new PdfPTable(1);
                                                    Font f1 = new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD, BaseColor.BLACK);
                                                    Chunk c1 = new Chunk("HOLD/MID POSITION TEST REPORT LINE B", f1);
                                                    PdfPCell cell = new PdfPCell(new Paragraph(c1)) { Border = Rectangle.NO_BORDER, PaddingTop = 5f, PaddingBottom = 5f, VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER };

                                                    t1.AddCell(cell);
                                                    pdfDoc.Add(t1);
                                                    Hold_MidPositionLineBTestInformation testInformation = Helper.GetTestInformation(fileName, TestType.HoldMidPositionLineBTest) as Hold_MidPositionLineBTestInformation;
                                                    lineSeriesCollection = Helper.GetSeriesCollection(fileName, 2, TestType.HoldMidPositionLineBTest);
                                                    lineSeriesCollection[1].ScalesYAt = 1;
                                                    if (lineSeriesCollection.Count == 2)
                                                        seriesCollection = new ObservableCollection<SeriesCollection> { new SeriesCollection() { lineSeriesCollection[0], lineSeriesCollection[1] } };
                                                    labelCollection = Helper.GetLabelCollection(fileName, 2);

                                                    AddReportNumber(testInformation, pdfDoc);
                                                    pdfDoc.Add(table);
                                                    AddTestParameterInformation(testInformation, pdfDoc, "LineB");
                                                    pdfDoc.Add(table);
                                                    AddGraphstoCertificate(seriesCollection, labelCollection, pdfDoc, TestType.HoldMidPositionLineBTest);
                                                    AddTestStatus(pdfDoc, testInformation.TestStatusB.ToString());
                                                    pdfDoc.Add(table);
                                                    pdfDoc.Add(table);
                                                    if (HomePage.holdMidPositionLineBinfo.Comment != "")
                                                    {
                                                        AddCommnetBox(pdfDoc, HomePage.holdMidPositionLineBinfo.Comment);
                                                        pdfDoc.Add(table);
                                                        pdfDoc.Add(table);
                                                    }
                                                    var content = pdfWriter.DirectContent;
                                                    var pageBorderRect = new Rectangle(pdfDoc.PageSize);

                                                    pageBorderRect.Left += pdfDoc.LeftMargin + 15;
                                                    pageBorderRect.Right -= pdfDoc.RightMargin + 15;
                                                    pageBorderRect.Top -= pdfDoc.TopMargin - 2;
                                                    pageBorderRect.Bottom += pdfDoc.BottomMargin - 2;

                                                    content.SetColorStroke(BaseColor.DARK_GRAY);
                                                    content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
                                                    content.Stroke();
                                                    /*ElpisOPCServerMainWindow.homePage.txtStatusMessage.Text = HomePage.holdMidPositionLineAInfo.ReportStatus = "Report Generated Successfully.";*/
                                                    ElpisOPCServerMainWindow.pump_Test.txtStatusMessage.Text = HomePage.holdMidPositionLineAInfo.ReportStatus = "Report Generated Successfully.";



                                                }
                                                fileName = string.Empty;
                                            }
                                        }
                                        if (i == 3)
                                        {


                                            file = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*slipsticktest*.csv");
                                            ObservableCollection<LineSeries> lineSeriesCollection = null;
                                            // ObservableCollection<List<string>> linelabelCollection = null;
                                            if (file.Length > 0)
                                            {
                                                if (file.Length > 2)
                                                {
                                                    fileName = file[file.Length - 2];
                                                }
                                                else
                                                {
                                                    fileName = file[0];
                                                }
                                                if (!string.IsNullOrEmpty(fileName))
                                                {
                                                    pdfDoc.NewPage();
                                                    PdfPTable t1 = new PdfPTable(1);
                                                    Font f1 = new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD, BaseColor.BLACK);
                                                    Chunk c1 = new Chunk("SLIP/STICK TEST REPORT", f1);
                                                    PdfPCell cell = new PdfPCell(new Paragraph(c1)) { Border = Rectangle.NO_BORDER, PaddingTop = 5f, PaddingBottom = 5f, VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER };

                                                    t1.AddCell(cell);
                                                    pdfDoc.Add(t1);


                                                    Slip_StickTestInformation testInformation = Helper.GetTestInformation(fileName, TestType.SlipStickTest) as Slip_StickTestInformation;

                                                    AddReportNumber(testInformation, pdfDoc);
                                                    pdfDoc.Add(table);
                                                    AddTestParameterInformation(testInformation, pdfDoc);
                                                    //pdfDoc.Add(table);
                                                    lineSeriesCollection = Helper.GetSeriesCollection(fileName, 1, TestType.SlipStickTest);


                                                    if (lineSeriesCollection.Count == 1)
                                                        seriesCollection = new ObservableCollection<SeriesCollection> { new SeriesCollection() { lineSeriesCollection[0] } };
                                                    labelCollection = Helper.GetLabelCollection(fileName, 1);
                                                    //foreach (var item in labelCollection)
                                                    //{
                                                    //    if (Convert.ToInt16( item[0])>0)
                                                    //    {
                                                    //        lineSeriesCollection[0].LabelPoint = Point => "" + Point.Y;
                                                    //    }
                                                    //}

                                                    //pdfDoc.Add(table);
                                                    AddGraphstoCertificate(seriesCollection, labelCollection, pdfDoc, TestType.SlipStickTest);
                                                    pdfDoc.Add(table);
                                                    if (HomePage.slipStickTestInformation.Comment != "")
                                                    {
                                                        AddCommnetBox(pdfDoc, HomePage.slipStickTestInformation.Comment);
                                                        pdfDoc.Add(table);
                                                        pdfDoc.Add(table);
                                                    }

                                                    var content = pdfWriter.DirectContent;
                                                    var pageBorderRect = new Rectangle(pdfDoc.PageSize);

                                                    pageBorderRect.Left += pdfDoc.LeftMargin + 15;
                                                    pageBorderRect.Right -= pdfDoc.RightMargin + 15;
                                                    pageBorderRect.Top -= pdfDoc.TopMargin - 2;
                                                    pageBorderRect.Bottom += pdfDoc.BottomMargin - 2;

                                                    content.SetColorStroke(BaseColor.DARK_GRAY);
                                                    content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
                                                    content.Stroke();
                                                    //ElpisOPCServerMainWindow.homePage.txtStatusMessage.Text = HomePage.slipStickTestInformation.ReportStatus = "Report Generated Successfully.";
                                                    ElpisOPCServerMainWindow.pump_Test.txtStatusMessage.Text = HomePage.slipStickTestInformation.ReportStatus = "Report Generated Successfully.";

                                                }
                                                fileName = string.Empty;
                                            }
                                        }
                                    }



                                    //  }
                                    pdfDoc.Close();
                                    
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("File is Open");
                            //ElpisOPCServerMainWindow.homePage.txtStatusMessage.Text = "Pdf File is Open";
                            ElpisOPCServerMainWindow.pump_Test.txtStatusMessage.Text = "Pdf File is Open";
                        }
                    }
                    else
                    {
                        //ElpisOPCServerMainWindow.homePage.txtStatusMessage.Text = "";
                        ElpisOPCServerMainWindow.pump_Test.txtStatusMessage.Text = "";
                        ////16/10/2018
                        //while (File.Exists(pdfFileName))
                        //{
                        //    fileCount++;
                        //    if (fileCount == 1)
                        //        pdfFileName = string.Concat(pdfFileName.Substring(0, pdfFileName.LastIndexOf('.')), '(', fileCount, ')', ".pdf");
                        //    else
                        //        pdfFileName = string.Concat(pdfFileName.Substring(0, pdfFileName.LastIndexOf('(')), '(', fileCount, ')', ".pdf");
                        //}
                        ///16/10/2018
                    }
                }
                else {

                    using (PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, new FileStream(pdfFileName, System.IO.FileMode.Create)))
                    {
                        Font arial = FontFactory.GetFont("Arial", 28, BaseColor.BLACK);

                        PDFHeaderFooter headerFooter = new PDFHeaderFooter();
                        ITextEvents itext = new ITextEvents();

                        dynamic testInfo = null;
                        string[] file = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*.csv");
                        if (file.Length > 0)
                        {
                            testInfo = Helper.GetTestInformation(file[0], TestType.StrokeTest) as StrokeTestInformation;

                            headerFooter.testInformation = testInfo;
                            itext.testInformation = testInfo;
                            pdfWriter.PageEvent = itext;//headerFooter;
                            pdfDoc.Open();
                            Paragraph para = new Paragraph("");
                            pdfDoc.Add(para);
                            PdfPTable table = new PdfPTable(1);
                            table.AddCell(new PdfPCell() { MinimumHeight = 10f, Border = Rectangle.NO_BORDER });
                            string[] dataFiles = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*.csv");
                            //for (int file = 0; file < dataFiles.Length; file++)
                            //{
                            //    // return;
                            // string fileName = dataFiles[file];
                            string fileName = string.Empty;
                            for (int i = 1; i <= 3; i++)
                            {
                                if (i == 1)
                                {
                                    file = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*stroketest*.csv");
                                    ObservableCollection<LineSeries> lineSeriesCollection = null;
                                    // ObservableCollection<List<string>> linelabelCollection = null;
                                    //ObservableCollection < SeriesCollection > seriesCollection1= null;
                                    if (file.Length > 0)
                                    {
                                        if (file.Length > 2)
                                        {
                                            fileName = file[file.Length - 2];
                                        }
                                        else
                                        {
                                            fileName = file[0];
                                        }
                                        if (!string.IsNullOrEmpty(fileName))
                                        {
                                            StrokeTestInformation testInformation = Helper.GetTestInformation(fileName, TestType.StrokeTest) as StrokeTestInformation;
                                            lineSeriesCollection = Helper.GetSeriesCollection(fileName, 4, TestType.StrokeTest);
                                            lineSeriesCollection[0].ScalesYAt = 0;
                                            lineSeriesCollection[1].ScalesYAt = 1;
                                            lineSeriesCollection[2].ScalesYAt = 0;
                                            lineSeriesCollection[3].ScalesYAt = 1;
                                            if (lineSeriesCollection.Count == 4)
                                                seriesCollection = new ObservableCollection<SeriesCollection> { new SeriesCollection() { lineSeriesCollection[0], lineSeriesCollection[1] }, new SeriesCollection() { lineSeriesCollection[2], lineSeriesCollection[3] } };
                                            labelCollection = Helper.GetLabelCollection(fileName, 4);
                                            table = new PdfPTable(1);
                                            table.AddCell(new PdfPCell() { MinimumHeight = 10f, Border = Rectangle.NO_BORDER });
                                            pdfDoc.Add(table);

                                            AddCylinderInformation(testInformation, pdfDoc);
                                            pdfDoc.Add(table);

                                            PdfPTable t1 = new PdfPTable(1);
                                            Font f1 = new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD, BaseColor.BLACK);
                                            Chunk c1 = new Chunk("STROKE TEST REPORT", f1);
                                            PdfPCell cell = new PdfPCell(new Paragraph(c1)) { Border = Rectangle.NO_BORDER, PaddingTop = 5f, PaddingBottom = 5f, VerticalAlignment = Element.ALIGN_CENTER };
                                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                            t1.AddCell(cell);
                                            pdfDoc.Add(t1);
                                            pdfDoc.Add(table);

                                            AddReportNumber(testInformation, pdfDoc);
                                            pdfDoc.Add(table);

                                            AddTestParameterInformation(testInformation, pdfDoc);
                                            pdfDoc.Add(table);

                                            AddMaximumAllowablePewssure(testInformation, pdfDoc);
                                            pdfDoc.Add(table);

                                            AddGraphstoCertificate(seriesCollection, labelCollection, pdfDoc, TestType.StrokeTest); //(seriesCollection, labelCollection, pdfDoc);
                                            pdfDoc.Add(table);


                                            //AddNumberofCycles(pdfDoc, testInformation.NoofCyclesCompleted.ToString());
                                            //pdfDoc.Add(table);
                                            //pdfDoc.Add(table);
                                            if (HomePage.strokeTestInfo.Comment != "")
                                            {
                                                AddCommnetBox(pdfDoc, HomePage.strokeTestInfo.Comment);
                                                pdfDoc.Add(table);
                                                pdfDoc.Add(table);
                                            }
                                            var content = pdfWriter.DirectContent;
                                            var pageBorderRect = new Rectangle(pdfDoc.PageSize);

                                            pageBorderRect.Left += pdfDoc.LeftMargin + 15;
                                            pageBorderRect.Right -= pdfDoc.RightMargin + 15;
                                            pageBorderRect.Top -= pdfDoc.TopMargin - 2;
                                            pageBorderRect.Bottom += pdfDoc.BottomMargin - 2;

                                            content.SetColorStroke(BaseColor.DARK_GRAY);
                                            content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
                                            content.Stroke();
                                            HomePage.strokeTestInfo.ReportStatus = "Reports Generated Successfully.";

                                        }
                                        fileName = string.Empty;
                                    }
                                }

                                if (i == 2)
                                {
                                    bool isPageAdded = false;
                                    file = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*hmpatest*.csv");
                                    ObservableCollection<LineSeries> lineSeriesCollection = null;
                                    // ObservableCollection<List<string>> linelabelCollection = null;
                                    if (file.Length > 0)
                                    {
                                        if (file.Length > 2)
                                        {
                                            fileName = file[file.Length - 2];
                                        }
                                        else
                                        {
                                            fileName = file[0];
                                        }
                                        if (!string.IsNullOrEmpty(fileName))
                                        {
                                            pdfDoc.NewPage();
                                            isPageAdded = true;
                                            Hold_MidPositionLineATestInformation testInformation = Helper.GetTestInformation(fileName, TestType.HoldMidPositionTest) as Hold_MidPositionLineATestInformation;
                                            lineSeriesCollection = Helper.GetSeriesCollection(fileName, 2, TestType.HoldMidPositionTest);
                                            lineSeriesCollection[1].ScalesYAt = 1;
                                            if (lineSeriesCollection.Count == 2)
                                                seriesCollection = new ObservableCollection<SeriesCollection> { new SeriesCollection() { lineSeriesCollection[0], lineSeriesCollection[1] } };
                                            labelCollection = Helper.GetLabelCollection(fileName, 2);

                                            PdfPTable t1 = new PdfPTable(1);
                                            Font f1 = new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD, BaseColor.BLACK);
                                            Chunk c1 = new Chunk("HOLD/MID POSITION TEST REPORT LINE A", f1);
                                            PdfPCell cell = new PdfPCell(new Paragraph(c1)) { Border = Rectangle.NO_BORDER, PaddingTop = 5f, PaddingBottom = 5f, VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER };

                                            t1.AddCell(cell);
                                            pdfDoc.Add(t1);


                                            AddReportNumber(testInformation, pdfDoc);
                                            pdfDoc.Add(table);

                                            AddTestParameterInformation(testInformation, pdfDoc, "LineA");
                                            pdfDoc.Add(table);
                                            AddGraphstoCertificate(seriesCollection, labelCollection, pdfDoc, TestType.HoldMidPositionTest);

                                            AddTestStatus(pdfDoc, testInformation.TestStatusA.ToString());
                                            if (HomePage.holdMidPositionLineAInfo.Comment != "")
                                            {
                                                AddCommnetBox(pdfDoc, HomePage.holdMidPositionLineAInfo.Comment);
                                                pdfDoc.Add(table);
                                                pdfDoc.Add(table);

                                            }
                                            var content = pdfWriter.DirectContent;
                                            var pageBorderRect = new Rectangle(pdfDoc.PageSize);

                                            pageBorderRect.Left += pdfDoc.LeftMargin + 15;
                                            pageBorderRect.Right -= pdfDoc.RightMargin + 15;
                                            pageBorderRect.Top -= pdfDoc.TopMargin - 2;
                                            pageBorderRect.Bottom += pdfDoc.BottomMargin - 2;

                                            content.SetColorStroke(BaseColor.DARK_GRAY);
                                            content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
                                            content.Stroke();
                                            HomePage.holdMidPositionLineAInfo.ReportStatus = "Reports Generated Successfully.";
                                            fileName = string.Empty;
                                        }
                                    }

                                    file = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*hmpbtest*.csv");
                                    if (file.Length > 0)
                                    {
                                        if (file.Length > 2)
                                        {
                                            fileName = file[file.Length - 2];
                                        }
                                        else
                                        {
                                            fileName = file[0];
                                        }
                                        if (!string.IsNullOrEmpty(fileName))
                                        {
                                            // if (!isPageAdded)
                                            pdfDoc.NewPage();
                                            PdfPTable t1 = new PdfPTable(1);
                                            Font f1 = new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD, BaseColor.BLACK);
                                            Chunk c1 = new Chunk("HOLD/MID POSITION TEST REPORT LINE B", f1);
                                            PdfPCell cell = new PdfPCell(new Paragraph(c1)) { Border = Rectangle.NO_BORDER, PaddingTop = 5f, PaddingBottom = 5f, VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER };

                                            t1.AddCell(cell);
                                            pdfDoc.Add(t1);
                                            Hold_MidPositionLineBTestInformation testInformation = Helper.GetTestInformation(fileName, TestType.HoldMidPositionLineBTest) as Hold_MidPositionLineBTestInformation;
                                            lineSeriesCollection = Helper.GetSeriesCollection(fileName, 2, TestType.HoldMidPositionLineBTest);
                                            lineSeriesCollection[1].ScalesYAt = 1;
                                            if (lineSeriesCollection.Count == 2)
                                                seriesCollection = new ObservableCollection<SeriesCollection> { new SeriesCollection() { lineSeriesCollection[0], lineSeriesCollection[1] } };
                                            labelCollection = Helper.GetLabelCollection(fileName, 2);

                                            AddReportNumber(testInformation, pdfDoc);
                                            pdfDoc.Add(table);
                                            AddTestParameterInformation(testInformation, pdfDoc, "LineB");
                                            pdfDoc.Add(table);
                                            AddGraphstoCertificate(seriesCollection, labelCollection, pdfDoc, TestType.HoldMidPositionLineBTest);
                                            AddTestStatus(pdfDoc, testInformation.TestStatusB.ToString());
                                            pdfDoc.Add(table);
                                            pdfDoc.Add(table);
                                            if (HomePage.holdMidPositionLineBinfo.Comment != "")
                                            {
                                                AddCommnetBox(pdfDoc, HomePage.holdMidPositionLineBinfo.Comment);
                                                pdfDoc.Add(table);
                                                pdfDoc.Add(table);
                                            }
                                            var content = pdfWriter.DirectContent;
                                            var pageBorderRect = new Rectangle(pdfDoc.PageSize);

                                            pageBorderRect.Left += pdfDoc.LeftMargin + 15;
                                            pageBorderRect.Right -= pdfDoc.RightMargin + 15;
                                            pageBorderRect.Top -= pdfDoc.TopMargin - 2;
                                            pageBorderRect.Bottom += pdfDoc.BottomMargin - 2;

                                            content.SetColorStroke(BaseColor.DARK_GRAY);
                                            content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
                                            content.Stroke();
                                            HomePage.holdMidPositionLineAInfo.ReportStatus = "Reports Generated Successfully.";



                                        }
                                        fileName = string.Empty;
                                    }
                                }
                                if (i == 3)
                                {


                                    file = Directory.GetFiles(string.Format("{0}\\{1}_{2}\\{3}", ReportLocation, jobNumber, customerName, cylinderNumber), "*slipsticktest*.csv");
                                    ObservableCollection<LineSeries> lineSeriesCollection = null;
                                    // ObservableCollection<List<string>> linelabelCollection = null;
                                    if (file.Length > 0)
                                    {
                                        if (file.Length > 2)
                                        {
                                            fileName = file[file.Length - 2];
                                        }
                                        else
                                        {
                                            fileName = file[0];
                                        }
                                        if (!string.IsNullOrEmpty(fileName))
                                        {
                                            pdfDoc.NewPage();
                                            PdfPTable t1 = new PdfPTable(1);
                                            Font f1 = new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD, BaseColor.BLACK);
                                            Chunk c1 = new Chunk("SLIP/STICK TEST REPORT", f1);
                                            PdfPCell cell = new PdfPCell(new Paragraph(c1)) { Border = Rectangle.NO_BORDER, PaddingTop = 5f, PaddingBottom = 5f, VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER };

                                            t1.AddCell(cell);
                                            pdfDoc.Add(t1);


                                            Slip_StickTestInformation testInformation = Helper.GetTestInformation(fileName, TestType.SlipStickTest) as Slip_StickTestInformation;

                                            AddReportNumber(testInformation, pdfDoc);
                                            pdfDoc.Add(table);
                                            AddTestParameterInformation(testInformation, pdfDoc);
                                            //pdfDoc.Add(table);
                                            lineSeriesCollection = Helper.GetSeriesCollection(fileName, 1, TestType.SlipStickTest);


                                            if (lineSeriesCollection.Count == 1)
                                                seriesCollection = new ObservableCollection<SeriesCollection> { new SeriesCollection() { lineSeriesCollection[0] } };
                                            labelCollection = Helper.GetLabelCollection(fileName, 1);
                                            //foreach (var item in labelCollection)
                                            //{
                                            //    if (Convert.ToInt16( item[0])>0)
                                            //    {
                                            //        lineSeriesCollection[0].LabelPoint = Point => "" + Point.Y;
                                            //    }
                                            //}

                                            //pdfDoc.Add(table);
                                            AddGraphstoCertificate(seriesCollection, labelCollection, pdfDoc, TestType.SlipStickTest);
                                            pdfDoc.Add(table);
                                            if (HomePage.slipStickTestInformation.Comment != "")
                                            {
                                                AddCommnetBox(pdfDoc, HomePage.slipStickTestInformation.Comment);
                                                pdfDoc.Add(table);
                                                pdfDoc.Add(table);
                                            }
                                            var content = pdfWriter.DirectContent;
                                            var pageBorderRect = new Rectangle(pdfDoc.PageSize);

                                            pageBorderRect.Left += pdfDoc.LeftMargin + 15;
                                            pageBorderRect.Right -= pdfDoc.RightMargin + 15;
                                            pageBorderRect.Top -= pdfDoc.TopMargin - 2;
                                            pageBorderRect.Bottom += pdfDoc.BottomMargin - 2;

                                            content.SetColorStroke(BaseColor.DARK_GRAY);
                                            content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
                                            content.Stroke();
                                            HomePage.slipStickTestInformation.ReportStatus = "Reports Generated Successfully.";
                                        }
                                        fileName = string.Empty;
                                    }
                                }
                            }



                            //  }
                            pdfDoc.Close();
                            //ElpisOPCServerMainWindow.homePage.txtStatusMessage.Text = "Reports Generated Successfully. ";
                            ElpisOPCServerMainWindow.pump_Test.txtStatusMessage.Text = "Reports Generated Successfully. ";
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (pdfDoc != null && pdfDoc.IsOpen())
                    pdfDoc.Close();
            }
            ElpisOPCServerMainWindow.homePage.ReportTab.IsEnabled = true;
        }

        private void AddTestParameterInformation(Hold_MidPositionLineBTestInformation testInformation, Document pdfDoc)
        {
            Paragraph para2 = new Paragraph();
            PdfPTable table = new PdfPTable(4);
            PdfPCell cell = new PdfPCell(new Paragraph("Testing Parameters for Pressure LineA", new Font(Font.FontFamily.HELVETICA, 18f, Font.BOLD, BaseColor.BLACK))) { Border = Rectangle.NO_BORDER, PaddingTop = 5, PaddingBottom = 10 };
            cell.Colspan = 4;
            table.AddCell(cell);
            Chunk c = new Chunk("Holding Pressure LineA (Bar)", new Font(Font.FontFamily.TIMES_ROMAN, 15f, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c)) { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInformation.HoldingPressureLineA.ToString(), new Font(Font.FontFamily.HELVETICA, 16f, Font.ITALIC, BaseColor.BLACK))) { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);

            c = new Chunk("Holding Pressure LineB (Bar)", new Font(Font.FontFamily.TIMES_ROMAN, 15f, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c)) { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInformation.HoldingPressureLineB.ToString(), new Font(Font.FontFamily.HELVETICA, 16f, Font.ITALIC, BaseColor.BLACK))) { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);

            c = new Chunk("Holding Time LineB (Min)", new Font(Font.FontFamily.TIMES_ROMAN, 15f, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c)) { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInformation.HoldingTimeLineB.ToString(), new Font(Font.FontFamily.HELVETICA, 16f, Font.ITALIC, BaseColor.BLACK))) { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);

            c = new Chunk("Cylinder Movement (mm)", new Font(Font.FontFamily.TIMES_ROMAN, 15f, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c)) { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInformation.CylinderMovement.ToString(), new Font(Font.FontFamily.HELVETICA, 16f, Font.ITALIC, BaseColor.BLACK))) { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);


            PdfPCell cell1 = new PdfPCell(table);
            cell1.AddElement(table);
            PdfPTable table1 = new PdfPTable(1);
            table1.AddCell(cell1);
            pdfDoc.Add(table1);
        }

        /// <summary>
        /// This Method add the Test Status Row for the Hold_Mid Position Test report PDF file.
        /// </summary>
        /// <param name="pdfDoc"></param>
        /// <param name="status"></param>
        private void AddTestStatus(Document pdfDoc, string status)
        {
            Paragraph para = new Paragraph();
            PdfPTable table = new PdfPTable(2);
            PdfPCell cell = new PdfPCell(new Paragraph("Test Result :", new Font(Font.FontFamily.TIMES_ROMAN, 25f, Font.BOLD, BaseColor.BLACK))) { Border = Rectangle.ALIGN_CENTER, PaddingTop = 15, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment =Element.ALIGN_CENTER };
          
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(status, new Font(Font.FontFamily.TIMES_ROMAN, 25f, Font.BOLD, BaseColor.BLACK))) { Border = Rectangle.ALIGN_CENTER, PaddingTop = 15, HorizontalAlignment = Element.ALIGN_LEFT , VerticalAlignment=Element.ALIGN_CENTER};
           
            table.AddCell(cell);
            pdfDoc.Add(table);
        }

        /// <summary>
        /// This Method add the Maximum Allowable Test Pressure Row for the Hold_Mid Position Test report pdf file.
        /// </summary>
        /// <param name="testInformation"></param>
        /// <param name="pdfDoc"></param>
        private void AddMaximumAllowablePewssure(Hold_MidPositionLineATestInformation testInformation, Document pdfDoc)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This Method add the Testing Parameters Row for the Hold_Mid Position Test report pdf file.
        /// </summary>
        /// <param name="testInformation"></param>
        /// <param name="pdfDoc"></param>
        private void AddTestParameterInformation(dynamic testInformation, Document pdfDoc, string line)
        {
            Paragraph para2 = new Paragraph();
            PdfPCell cell = null;
            PdfPTable table = new PdfPTable(4);
            if (line == "LineA")
            {
                cell = new PdfPCell(new Paragraph("Testing Parameters for Pressure LineA", new Font(Font.FontFamily.HELVETICA, 15f, Font.BOLD, BaseColor.BLACK))) { BackgroundColor = BaseColor.LIGHT_GRAY, PaddingTop = 5, PaddingBottom = 5, VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER };
                cell.Colspan = 4;
                table.AddCell(cell);
            }
            if (line == "LineB")
            {
                cell = new PdfPCell(new Paragraph("Testing Parameters for Pressure LineB", new Font(Font.FontFamily.HELVETICA, 15f, Font.BOLD, BaseColor.BLACK))) { BackgroundColor = BaseColor.LIGHT_GRAY, PaddingTop = 5, PaddingBottom = 5, VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER };
                cell.Colspan = 4;
                table.AddCell(cell);
            }

            Chunk c = new Chunk("Cylinder Movement At Start Of Test (mm)", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInformation.InitialCylinderMovement.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);

            c = new Chunk("Cylinder Movement At End Of Test  (mm)", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInformation.CylinderMovement.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);



            if (line == "LineA")
            {
                c = new Chunk("Holding Pressure Line A At start of Test  (bar)", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInformation.HoldingLineAInitialPressure.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);

                c = new Chunk("Holding Pressure LineA At End Of Test (bar)", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInformation.HoldingPressureLineA.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);




                c = new Chunk("Holding Time LineA  (min)", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInformation.HoldingTimeLineA.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);

                c = new Chunk("Holding Pressure LineB  (bar)", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInformation.HoldingPressureLineB.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);




            }
            if (line == "LineB")
            {
                c = new Chunk("Holding Pressure LineB At Start Of Test (bar)", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInformation.InitialPressureLineB.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                c = new Chunk("Holding Pressure LineB At End of Test (bar)", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInformation.HoldingPressureLineB.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);

                c = new Chunk("Holding Time LineB (min)", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInformation.HoldingTimeLineB.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);

                c = new Chunk("Holding Pressure LineA  (bar)", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInformation.HoldingPressureLineA.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
            }







            PdfPCell cell1 = new PdfPCell(table);
            cell1.AddElement(table);
            PdfPTable table1 = new PdfPTable(1);
            table1.AddCell(cell1);
            pdfDoc.Add(table1);
        }

        /// <summary>
        /// This Method add the Testing Parameters Row for the Slip Stick Test report pdf file.
        /// </summary>
        /// <param name="testInformation"></param>
        /// <param name="pdfDoc"></param>
        private void AddTestParameterInformation(Slip_StickTestInformation testInformation, Document pdfDoc)
        {
            Paragraph para2 = new Paragraph();
            PdfPTable table = new PdfPTable(4);
            PdfPCell cell = new PdfPCell(new Paragraph("Testing Parameters", new Font(Font.FontFamily.HELVETICA, 15f, Font.BOLD, BaseColor.BLACK))) { BackgroundColor = BaseColor.LIGHT_GRAY, PaddingTop = 5, PaddingBottom = 5 ,HorizontalAlignment=Element.ALIGN_CENTER};
            cell.Colspan = 4;
            table.AddCell(cell);

            //Chunk c = new Chunk("Flow (lpm)", new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.NORMAL, BaseColor.BLACK));
            //cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
            //table.AddCell(cell);
            //cell = new PdfPCell(new Paragraph(testInformation.Flow.ToString(), new Font(Font.FontFamily.HELVETICA, 12f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
            //table.AddCell(cell);

            Chunk c = new Chunk("Initial Pressure  (bar)", new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInformation.InitialPressure.ToString(), new Font(Font.FontFamily.HELVETICA, 12f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);

            c = new Chunk("Pressure  (bar)", new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInformation.PressureAfterFirstCylinderMovement.ToString(), new Font(Font.FontFamily.HELVETICA, 12f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);

            c = new Chunk("Initial Cylinder Position  (mm)", new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInformation.InitialCylinderMovement.ToString(), new Font(Font.FontFamily.HELVETICA, 12f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);

          

            c = new Chunk("Cylinder Movement  (mm)", new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInformation.CylinderFirstMovement.ToString(), new Font(Font.FontFamily.HELVETICA, 12f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
            table.AddCell(cell);

            
            PdfPCell cell1 = new PdfPCell(table);
            cell1.AddElement(table);
            PdfPTable table1 = new PdfPTable(1);
            table1.AddCell(cell1);
            pdfDoc.Add(table1);
        }

        /// <summary>
        /// This Method add the Number of cycles completed Row for the StrokeTest report pdf file.
        /// </summary>
        /// <param name="pdfDoc"></param>
        /// <param name="cycles"></param>
        //private void AddNumberofCycles(Document pdfDoc, string cycles)
        //{
        //    try
        //    {

        //        Font arial_Heading = FontFactory.GetFont("Arial", 13, Font.BOLD, BaseColor.BLACK);
        //        Font arial_Content = FontFactory.GetFont("Arial", 12.5f, BaseColor.BLACK);

        //        Paragraph para = new Paragraph();
        //        PdfPTable table = new PdfPTable(4);
        //        PdfPCell cell = new PdfPCell(new Paragraph("Number of Cycles Completed", arial_Heading)) { PaddingTop = 2,  Border = Rectangle.NO_BORDER };
        //        cell.Colspan = 2;
        //        table.AddCell(cell);
        //        cell = new PdfPCell(new Paragraph(cycles, arial_Content)) { PaddingTop = 2,  Border = Rectangle.NO_BORDER };
        //        cell.Colspan = 2;
        //        table.AddCell(cell);
             
        //        pdfDoc.Add(table);
        //        //    MessageBox.Show("No of Cycles Added");
        //    }
        //    catch (Exception)
        //    {

        //    }
        //}

        private void AddCommnetBox(Document pdfDoc,string comment)
        {
            try
            {

                Font arial_Heading = FontFactory.GetFont("Arial", 10f, Font.NORMAL, BaseColor.BLACK);
                Paragraph para = new Paragraph();
                PdfPTable table = new PdfPTable(1);
                PdfPCell cell = new PdfPCell(new Paragraph(string.Format("NOTE :{0}", comment), arial_Heading)) { Border=Rectangle.NO_BORDER,VerticalAlignment=Element.ALIGN_BOTTOM,PaddingTop=5};
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);
            
            }
            catch (Exception)
            {

            }

        }

        //private void TestStatus(Document pdfDoc, string TestStatus)
        //{
        //    try
        //    {

        //        Font arial_Heading = FontFactory.GetFont("Arial", 13, Font.BOLD, BaseColor.BLACK);
        //        Font arial_Content = FontFactory.GetFont("Arial", 12.5f, BaseColor.BLACK);

        //        Paragraph para = new Paragraph();
        //        PdfPTable table = new PdfPTable(4);
        //        PdfPCell cell = new PdfPCell(new Paragraph("Test Result", arial_Heading)) { PaddingTop = 10, PaddingBottom = 10, Border = Rectangle.NO_BORDER };
        //        cell.Colspan = 2;
        //        table.AddCell(cell);
        //        cell = new PdfPCell(new Paragraph(TestStatus, arial_Content)) { PaddingTop = 10, PaddingBottom = 15, Border = Rectangle.NO_BORDER  };
        //        cell.Colspan = 2;
        //        table.AddCell(cell);
        //        pdfDoc.Add(table);
        //        //    MessageBox.Show("No of Cycles Added");
        //    }
        //    catch (Exception)
        //    {

        //    }
        //}

        /// <summary>
        /// This Method add the Maximum Allowable Test Line Pressure(A,B) Row for the Stroke Test report pdf file.
        /// </summary>
        /// <param name="testInfo"></param>
        /// <param name="pdfDoc"></param>       
        private void AddMaximumAllowablePewssure(StrokeTestInformation testInfo, Document pdfDoc)
        {

            try
            {
                Font arial_Heading = FontFactory.GetFont("Arial", 15, BaseColor.BLACK);
                Font arial_Content = FontFactory.GetFont("Arial", 11.5f, BaseColor.BLACK);

                Paragraph para = new Paragraph();
                PdfPTable table = new PdfPTable(4);
                PdfPCell cell = new PdfPCell(new Paragraph("Maximum Allowable Test Pressure", arial_Heading)) { BackgroundColor = BaseColor.LIGHT_GRAY, PaddingTop = 5, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER };
                cell.Colspan = 4;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Line A Pressure (bar)", arial_Content)) { PaddingBottom = 5f, PaddingTop = 5 };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(testInfo.LineAPressureInput.ToString(), arial_Content)) { PaddingBottom = 5f, PaddingTop = 5 };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Line B Pressure (bar) ", arial_Content)) { PaddingBottom = 5f, PaddingTop = 5, Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.LineBPressureInput.ToString(), arial_Content)) { PaddingBottom = 5f, PaddingTop = 5 };
                table.AddCell(cell);

                PdfPCell cell1 = new PdfPCell(table);
                cell1.AddElement(table);
                PdfPTable table1 = new PdfPTable(1);
                table1.AddCell(cell1);
                pdfDoc.Add(table1);
                //  MessageBox.Show("Max Pressure Added");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// This Method Adds Graphs to the PDF File.
        /// </summary>
        /// <param name="seriesCollection"></param>
        /// <param name="labelCollection"></param>
        /// <param name="pdfDoc"></param>
        private void AddGraphstoCertificate(ObservableCollection<SeriesCollection> seriesCollection, ObservableCollection<List<string>> labelCollection, Document pdfDoc, TestType testType)
        {
            try
            {
                Font arial_Heading = FontFactory.GetFont("Arial", 15, BaseColor.BLACK);
                Font arial_Content = FontFactory.GetFont("Arial", 12, BaseColor.BLACK);


                if (seriesCollection != null && labelCollection != null)
                {
                    PdfPTable table = new PdfPTable(seriesCollection.Count);
                    //table.WidthPercentage = 90;
                    PdfPCell cell = new PdfPCell(new Paragraph("Graph Information", arial_Heading)) { BackgroundColor = BaseColor.LIGHT_GRAY, PaddingTop = 5, PaddingBottom = 5, VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER };//, PaddingLeft = -10, PaddingRight = -10 };
                    cell.Colspan = seriesCollection.Count;
                    table.AddCell(cell);
                    cell = new PdfPCell() { MinimumHeight = 10, Border = Rectangle.NO_BORDER };
                    cell.Colspan = seriesCollection.Count;
                    table.AddCell(cell);

                    pdfDoc.Add(table);

                    table = new PdfPTable(seriesCollection.Count);
                    table.WidthPercentage = 90;

                    for (int i = 0; i < seriesCollection.Count; i++)
                    {
                        string imageFile = string.Format("{0}\\Chart.png", Directory.GetCurrentDirectory());


                        bool isCreated = GenerateImageFromGraph(seriesCollection[i], labelCollection[i], imageFile, testType, seriesCollection.Count);
                        if (isCreated)
                        {
                            iTextSharp.text.Image graph = iTextSharp.text.Image.GetInstance(imageFile);
                            //graph.ScalePercent(150f);
                            cell = new PdfPCell(graph, true);
                            table.AddCell(cell);
                            File.Delete(imageFile);
                        }

                    }
                    pdfDoc.Add(table);
                    // MessageBox.Show("Graphs Added");
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// This Method Add Certificate information to the PDF File.
        /// </summary>
        /// <param name="testInfo"></param>
        /// <param name="pdfDoc"></param>
        private void AddCeritificateInformation(dynamic testInfo, Document pdfDoc)
        {
            PdfPTable table = new PdfPTable(3);
            table.WidthPercentage = 85;
            PdfPCell cell = new PdfPCell(new Paragraph("Certificate Information", new Font(Font.FontFamily.TIMES_ROMAN, 15f, Font.BOLD, BaseColor.BLACK))) { PaddingTop = 5, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY };
            cell.Colspan = 3;
            table.AddCell(cell);

            Chunk c = new Chunk("Customer Name", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c)) { PaddingTop = 5, PaddingBottom = 5 };  // { Border = Rectangle.NO_BORDER, PaddingLeft = 5f };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInfo.CustomerName.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
            cell.Colspan = 2;
            table.AddCell(cell);

            c = new Chunk("Job Number", new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Paragraph(c)) { PaddingTop = 5, PaddingBottom = 5 }; // { Border = Rectangle.NO_BORDER, PaddingLeft = 15f };
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(testInfo.JobNumber.ToString(), new Font(Font.FontFamily.HELVETICA, 13f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
            cell.Colspan = 2;
            table.AddCell(cell);



            //c = new Chunk("Test Name", new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.BOLD, BaseColor.BLACK));
            //cell = new PdfPCell(new Paragraph(c)) { Border = Rectangle.NO_BORDER, PaddingBottom = 5f };
            //table.AddCell(cell);
            //cell = new PdfPCell(new Paragraph(testInfo.TestName.ToString())) { Border = Rectangle.NO_BORDER, PaddingBottom = 5f };
            //cell.Colspan = 2;
            //table.AddCell(cell);

            //c = new Chunk("Report Number", new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.BOLD, BaseColor.BLACK));
            //cell = new PdfPCell(new Paragraph(c)) { Border = Rectangle.NO_BORDER, PaddingBottom = 5f };
            //table.AddCell(cell);
            //cell = new PdfPCell(new Paragraph(testInfo.ReportNumber, new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.BOLD, BaseColor.BLACK))) { Border = Rectangle.NO_BORDER, PaddingBottom = 5f };
            //cell.Colspan = 2;
            //table.AddCell(cell);

            PdfPCell cell1 = new PdfPCell(table);// { PaddingTop = 10f, PaddingBottom = 10f };
            cell1.AddElement(table);
            PdfPTable table1 = new PdfPTable(1);
            table1.AddCell(cell1);
            pdfDoc.Add(table1);
            //  MessageBox.Show("Certificate Information Added");
        }

        /// <summary>
        /// This Method add Cylinder Information to the PDF File.
        /// </summary>
        /// <param name="testInfo"></param>
        /// <param name="pdfDoc"></param>
        private void AddCylinderInformation(dynamic testInfo, Document pdfDoc)
        {
            try
            {
                Font arial_Heading = FontFactory.GetFont("Arial", 15, BaseColor.BLACK);
                Font arial_Content = FontFactory.GetFont("Arial", 12, BaseColor.BLACK);

                // Paragraph para2 = new Paragraph();
                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 85;
                table.HeaderRows = 2;
                PdfPCell cell = new PdfPCell(new Paragraph("Cylinder Information", arial_Heading)) { Border = Rectangle.BOTTOM_BORDER, PaddingTop = 5, PaddingBottom = 5, BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER }; // new Font(Font.FontFamily.TIMES_ROMAN, 14f, Font.BOLD, BaseColor.BLACK)
                cell.Colspan = 4;
                table.AddCell(cell);

                Chunk c = new Chunk("Cylinder Number", arial_Content); //new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.BOLD, BaseColor.BLACK)
                cell = new PdfPCell(new Paragraph(c)) { PaddingTop = 4, PaddingBottom = 4 };// { Border = Rectangle.NO_BORDER, PaddingLeft = 15f };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.CylinderNumber.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 };// { Border = Rectangle.NO_BORDER }; //new Font(Font.FontFamily.HELVETICA, 12f, Font.ITALIC, BaseColor.BLACK)

                table.AddCell(cell);

                c = new Chunk("Bore Size", arial_Content);
                cell = new PdfPCell(new Paragraph(c)) { PaddingTop = 5, PaddingBottom = 5 }; // { Border = Rectangle.NO_BORDER, PaddingLeft = 15f };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.BoreSize.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 };// { Border = Rectangle.NO_BORDER };

                table.AddCell(cell);

                c = new Chunk("Rod Size", arial_Content);
                cell = new PdfPCell(new Paragraph(c)) { PaddingTop = 5, PaddingBottom = 5 };// { Border = Rectangle.NO_BORDER, PaddingLeft = 15f }; //, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.RodSize.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 };// { Border = Rectangle.NO_BORDER };

                table.AddCell(cell);

                c = new Chunk("Stroke Length", arial_Content);
                cell = new PdfPCell(new Paragraph(c)) { PaddingTop = 5, PaddingBottom = 5 }; // { Border = Rectangle.NO_BORDER, PaddingLeft = 15f, PaddingBottom = 5f };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.StrokeLength.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 };// { Border = Rectangle.NO_BORDER };

                table.AddCell(cell);


                PdfPCell cell1 = new PdfPCell(table);// { PaddingTop = 10f, PaddingBottom = 10f };
                cell1.AddElement(table);
                PdfPTable table1 = new PdfPTable(1);
                table1.AddCell(cell1);
                pdfDoc.Add(table1);
                // MessageBox.Show("Cylinder Information Added");
            }
            catch (Exception)
            {

            }
        }


        /// <summary>
        /// This Method Add Header(Logo,Test DataTime) to the PDF File.
        /// </summary>
        /// <param name="testInfo"></param>
        /// <param name="pdfDoc"></param>
        private void AddHeadertoCertificate(dynamic testInfo, Document pdfDoc, PdfWriter pdfWriter)
        {
            Paragraph para1 = new Paragraph() { };
            // LogoImages(pdfDoc);
            PdfPTable table = new PdfPTable(3);
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.WidthPercentage = 94;
            table.SetWidths(new float[] { 1.12f, 1.5f, 2f });


            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image = HomePage.image;
            image.Height = 110f;
            using (MemoryStream ms = new MemoryStream())
            {
                System.Windows.Media.Imaging.BmpBitmapEncoder bbe = new BmpBitmapEncoder();
                bbe.Frames.Add(BitmapFrame.Create(new Uri(image.Source.ToString(), UriKind.RelativeOrAbsolute)));

                bbe.Save(ms);
                System.Drawing.Image img2 = System.Drawing.Image.FromStream(ms);

                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(img2, BaseColor.BLACK);
                Font f = new Font(Font.FontFamily.COURIER, 15.0f, Font.BOLD, BaseColor.BLACK);
                Chunk c = new Chunk("SunPowerGen", f);
                logo.ScalePercent(50f);
                PdfPCell cell = new PdfPCell(logo);
                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cell.PaddingTop = 3;
                cell.PaddingBottom = 3;
                cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                cell.Border = PdfPCell.BOX;
                cell.BorderWidth = 1.1f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);


                Font f1 = new Font(Font.FontFamily.COURIER, 24.0f, Font.BOLD, BaseColor.BLACK);
                Chunk c1 = new Chunk("TEST REPORT", f1);

                cell = new PdfPCell(new Paragraph(c1)) { PaddingBottom = 5f, PaddingTop = 5f };
                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                cell.Border = PdfPCell.BOX;
                cell.BorderWidth = 1.1f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                PdfPTable ta1 = new PdfPTable(1);
                ta1.HorizontalAlignment = Element.ALIGN_CENTER;

                //cell = new PdfPCell(new Paragraph("Test DateTime") { Alignment = Element.ALIGN_CENTER });
                //cell.BorderColor = BaseColor.WHITE;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //ta1.AddCell(cell);
                //cell = new PdfPCell(new Paragraph(testInfo.TestDateTime, new Font(Font.FontFamily.TIMES_ROMAN, 10f)) { Alignment = Element.ALIGN_CENTER });
                //cell.BorderColor = BaseColor.WHITE;
                //ta1.AddCell(cell);
                //ta1.AddCell(cell);
                //ta1.DefaultCell.Border = Rectangle.NO_BORDER;
                //table.AddCell(ta1);
                //cell = new PdfPCell();
                //cell.Border = PdfPCell.BOX;

                //table.AddCell(cell);

                PdfPTable table2 = new PdfPTable(2);
                table2.SetWidths(new float[] { 1, 1.6f });


                c = new Chunk("Test Date", new Font(Font.FontFamily.TIMES_ROMAN, 11f, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER, PaddingLeft = 15f };
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                table2.AddCell(cell);

                cell = new PdfPCell(new Paragraph(DateTime.Today.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 12f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                table2.AddCell(cell);

                c = new Chunk("Customer Name", new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER, PaddingLeft = 15f };
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                table2.AddCell(cell);

                cell = new PdfPCell(new Paragraph(testInfo.CustomerName.ToString(), new Font(Font.FontFamily.HELVETICA, 12f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };               
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                table2.AddCell(cell);

                c = new Chunk("Job Number", new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Paragraph(c));// { Border = Rectangle.NO_BORDER, PaddingLeft = 15f };
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                table2.AddCell(cell);

                cell = new PdfPCell(new Paragraph(testInfo.JobNumber.ToString(), new Font(Font.FontFamily.HELVETICA, 12f, Font.ITALIC, BaseColor.BLACK)));// { Border = Rectangle.NO_BORDER };                
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                table2.AddCell(cell);

                table2.DefaultCell.Border = Rectangle.BOX;
                table2.DefaultCell.BorderColor = BaseColor.LIGHT_GRAY;
                //ta1.AddCell(table);
                //ta1.WriteSelectedRows(0, -1, pdfDoc.LeftMargin, pdfDoc.PageSize.Height - 36, pdfWriter.DirectContent);
                cell = new PdfPCell(table2);
                cell.AddElement(table2);
                cell.Border = PdfPCell.BOX;
                cell.BorderWidth = 1.1f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                //para1.Add(table);
                pdfDoc.Add(table);
            }
            // MessageBox.Show("Header Added");
        }


        /// <summary>
        /// This Method add the Testing Parameters to the Stroke Test report file.
        /// </summary>
        /// <param name="testInfo"></param>
        /// <param name="pdfDoc"></param>
        private void AddTestParameterInformation(StrokeTestInformation testInfo, Document pdfDoc)
        {
            try
            {
                Font arial_Heading = FontFactory.GetFont("Arial", 15, BaseColor.BLACK);
                Font arial_Content = FontFactory.GetFont("Arial", 11.5f, BaseColor.BLACK);

                PdfPTable table = new PdfPTable(4);
                PdfPCell cell = new PdfPCell(new Paragraph("Testing Parameters", arial_Heading)) { BackgroundColor = BaseColor.LIGHT_GRAY, PaddingTop = 6, PaddingBottom = 5, VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER };
                cell.Colspan =4;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                Chunk c = new Chunk("Flow (lpm)", arial_Content);
                cell = new PdfPCell(new Paragraph(c)) { Border = Rectangle.NO_BORDER, PaddingTop = 5, PaddingBottom = 5 };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.Flow.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 };
                table.AddCell(cell);

               

                c = new Chunk("Pressure LineA  (bar)", arial_Content);
                cell = new PdfPCell(new Paragraph(c)) { PaddingTop = 5, PaddingBottom = 5, Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.PressureLineA.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 };
                table.AddCell(cell);

                c = new Chunk("Pressure LineB  (bar)", arial_Content);
                cell = new PdfPCell(new Paragraph(c)) { PaddingTop = 5, PaddingBottom = 5 };// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.PressureLineB.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 };
                table.AddCell(cell);

                c = new Chunk("No of Cycles", arial_Content);
                cell = new PdfPCell(new Paragraph(c)) { PaddingTop = 5, PaddingBottom = 5 };// { Border = Rectangle.NO_BORDER };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.NoofCycles.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 };
                table.AddCell(cell);

                c = new Chunk("Stroke Length (mm)", arial_Content);
                cell = new PdfPCell(new Paragraph(c)) { Border = Rectangle.NO_BORDER, PaddingTop = 5, PaddingBottom = 5 };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.StrokeLengthValue.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 };
                table.AddCell(cell);

                c = new Chunk("Number of Cycles Completed", arial_Content);
                cell = new PdfPCell(new Paragraph(c)) { Border = Rectangle.NO_BORDER, PaddingTop = 5, PaddingBottom = 5 };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(testInfo.NoofCyclesCompleted.ToString(), arial_Content)) { PaddingTop = 5, PaddingBottom = 5 };
                table.AddCell(cell);



                PdfPCell cell1 = new PdfPCell(table);
                cell1.AddElement(table);
                PdfPTable table1 = new PdfPTable(1);
                table1.AddCell(cell1);
                pdfDoc.Add(table1);
                // MessageBox.Show("Test Parameters Added");
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// This Method add the Testing Parameters to the Stroke Test report file.
        /// </summary>
        /// <param name="testInfo"></param>
        /// <param name="pdfDoc"></param>
        private void AddFootertoCertificate(PdfWriter pdfWriter, Document pdfDoc)
        {
            //if(pdfDoc.PageCount)
            string website = string.Empty;
            string address = string.Empty;
            string refer = string.Empty;
            try
            {
                website = ConfigurationManager.AppSettings["WebSiteAddress"].ToString();
                address = ConfigurationManager.AppSettings["Address"].ToString();
                refer = ConfigurationManager.AppSettings["Ref"].ToString();
            }
            catch (Exception) { }
            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 90;
            PdfPCell cell = new PdfPCell(new Paragraph(website, new Font(Font.FontFamily.COURIER, 14f, Font.BOLD, BaseColor.BLACK))) { Border = Rectangle.NO_BORDER, PaddingTop = 5, PaddingBottom = 5 };
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph(address, new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL, BaseColor.BLACK))) { Border = Rectangle.NO_BORDER, PaddingTop = 2, PaddingBottom = 2 };
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph(refer, new Font(Font.FontFamily.TIMES_ROMAN, 8.5f, Font.NORMAL, BaseColor.DARK_GRAY))) { Border = Rectangle.NO_BORDER, PaddingTop = 2, PaddingBottom = 2 };
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);
            table.DefaultCell.Border = Rectangle.TOP_BORDER;


            PdfPCell cell1 = new PdfPCell(table) { Border = Rectangle.TOP_BORDER };
            cell1.AddElement(table);
            PdfPTable table1 = new PdfPTable(1);
            table1.WidthPercentage = 90;
            table1.AddCell(cell1);


            //cell = new PdfPCell() { Border = Rectangle.NO_BORDER, MinimumHeight = 10 };
            //pdfDoc.Add(cell);
            //table1.WriteSelectedRows(0, -1, pdfDoc.LeftMargin, pdfDoc.PageSize.Height - 36, pdfWriter.DirectContent);
            pdfDoc.Add(table1);

        }


        //internal void GenerateCSVFile(TestType testType, ICeritificateInformation testInfo, string noofCyclesCompleted, IChartValues chartValues1 = null, IChartValues chartValue2 = null, string header1 = null, string header2 = null)
        //{
        //    dynamic testInformation = Helper.GetTestObject(testType, testInfo);
        //    List<string> stringBuilder1 = new List<string>();
        //    List<string> stringBuilder2 = new List<string>();
        //    StringBuilder finalstring = new StringBuilder();

        //    if (chartValue2 != null)
        //    {
        //        stringBuilder1 = Helper.BuildStrig(chartValues1, header1);
        //        stringBuilder2 = Helper.BuildStrig(chartValue2, header2);
        //        int maxSize = Math.Max(stringBuilder1.Count, stringBuilder2.Count);
        //        finalstring.AppendLine(string.Format("Number of times data read:{0}", maxSize));

        //        finalstring = new StringBuilder();
        //        for (int i = 0; i < maxSize; i++)
        //        {
        //            if (stringBuilder1.Count <= maxSize && stringBuilder2.Count <= maxSize)
        //                finalstring.AppendLine(string.Format("{0},{1}", stringBuilder1[i].ToString(), stringBuilder2[i].ToString()));
        //        }

        //        finalstring.AppendLine(string.Format("CustomerName,{0}", testInformation.CustomerName));
        //        finalstring.AppendLine(string.Format("JobNumber,{0}", testInformation.JobNumber));
        //        finalstring.AppendLine(string.Format("ReportNumber,{0}", testInformation.ReportNumber));
        //        finalstring.AppendLine(string.Format("Test Date,{0}", testInformation.TestDateTime));
        //        if (testType == TestType.StrokeTest)
        //            AddStrokeTestTestParameters(testInformation, finalstring);
        //        else if (testType == TestType.HoldMidPositionTest)
        //            AddHoldorMidPositionTestParameters(testInformation, finalstring);
        //        else if (testType == TestType.SlipStickTest)
        //            AddSliporStickTestParameters(testInformation, finalstring);

        //        finalstring.AppendLine(string.Format("Rod Size,{0}", testInformation.RodSize));
        //        finalstring.AppendLine(string.Format("Bore Size,{0}", testInformation.BoreSize));
        //        finalstring.AppendLine(string.Format("Stroke Length,{0}", testInformation.StrokeLength));
        //        finalstring.AppendLine(string.Format("Number of Cycles,{0}", testInformation.NoofCycles));
        //        finalstring.AppendLine(string.Format("Number of Cycles Completed,{0}", noofCyclesCompleted));

        //    }
        //    else
        //    {

        //    }
        //    string csvFileName = string.Empty; ;
        //    if (testType == TestType.StrokeTest)
        //        csvFileName = string.Format(@"{0}\{2}\{1}_{2}_StrokeTest_001_Data_{3}.csv", ReportLocation, FilePrefix, testInformation.JobNumber, testInformation.TestDateTime.Replace(':', '_').Replace('/', '-'));
        //    if (!File.Exists(csvFileName))
        //    {
        //        WriteDatatoFile(testInformation.JobNumber, finalstring, csvFileName);
        //    }
        //    else
        //    {
        //        MessageBoxResult messageOption = MessageBox.Show("File with same name already exists.\nDo you want replace it?", "SPG Report Tool", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //        if (messageOption == MessageBoxResult.Yes)
        //        {
        //            File.Delete(csvFileName);
        //            WriteDatatoFile(testInformation.JobNumber, finalstring, csvFileName);
        //        }
        //    }

        //}

        /// <summary>
        /// This Methos generates the image from the SeriesCollection and add this image to the PDF file.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="labels"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool GenerateImageFromGraph(SeriesCollection series, List<string> labels, string fileName, TestType testType, int columnCount)
        {
            string xAxixName = string.Empty;
            string yAxixName = string.Empty;
            AxesCollection axesColection = null;
            if (testType == TestType.StrokeTest)
            {
                xAxixName = "Time";
                yAxixName = series[0].Title.Replace("Flow","Flow\\StrokeLength");
                if (yAxixName == "Flow\\StrokeLength")
                {
                    axesColection = new AxesCollection() { new Axis { Title = yAxixName.Split('\\')[0], Foreground = Brushes.Black, LabelFormatter = value => value.ToString("N2"), Position = AxisPosition.RightTop },
                                                           new Axis { Title = yAxixName.Split('\\')[1], Foreground = Brushes.Black, LabelFormatter = value => value.ToString("N2") } };
                }
                if(yAxixName== "Pressure LineA"||yAxixName== "Pressure LineB")
                {
                    yAxixName = @"PressureLineA\PressureLineB";
                    axesColection = new AxesCollection() { new Axis { Title = yAxixName.Split('\\')[0], Foreground = Brushes.Black, LabelFormatter = value => value.ToString("N2") },
                                                           new Axis { Title = yAxixName.Split('\\')[1], Foreground = Brushes.Black, LabelFormatter = value => value.ToString("N2") ,Position=AxisPosition.RightTop}};
                }
            }

            else if (testType == TestType.HoldMidPositionTest)
            {
                xAxixName = "Time";
                yAxixName = @"HoldingPressure LineA\Cylinder Movement";
                axesColection = new AxesCollection() { new Axis { Title = yAxixName.Split('\\')[0], Foreground = Brushes.Black, LabelFormatter = value => value.ToString("N2") },
                                               new Axis { Title = yAxixName.Split('\\')[1], Foreground = Brushes.Black, LabelFormatter = value => value.ToString("N2"),Position=AxisPosition.RightTop }};
            }
            else if (testType == TestType.SlipStickTest)
            {
                xAxixName = "Cylinder Movement";
                yAxixName = "Pressure";
                axesColection = new AxesCollection() { new Axis { Title = yAxixName, Foreground = Brushes.Black, LabelFormatter = value => value.ToString("N2") } };
            }
            else if (testType == TestType.HoldMidPositionLineBTest)
            {
                xAxixName = "Time";
                yAxixName = @"HoldingPressure LineB\Cylinder Movement";
                axesColection = new AxesCollection() { new Axis { Title = yAxixName.Split('\\')[0], Foreground = Brushes.Black, LabelFormatter = value => value.ToString("N2") },
                                               new Axis { Title = yAxixName.Split('\\')[1], Foreground = Brushes.Black, LabelFormatter = value => value.ToString("N2"),Position=AxisPosition.RightTop }};
            }
            int heigth = 0;
            int width = 0;
            if (columnCount == 2)
            {
                width = 550;
                heigth = 500;
            }
            else if (columnCount == 1)
            {
                //12/08/2018 chnages
                heigth = 400;
                width = 750;
            }
            else
            {
                width = 550;
                heigth = 500;
            }

            var currentChart = new LiveCharts.Wpf.CartesianChart  // Width = 550, Height = 500,
            {
                DisableAnimations = true,
                Width = width,
                Height = heigth,
                AxisX = new AxesCollection() { new Axis { Title = xAxixName, Foreground = Brushes.Black, Labels = labels } },
               
                AxisY = axesColection,
                LegendLocation = LegendLocation.Bottom,
                //Series = new SeriesCollection
                //{
                //    series
                //}
                Series = series,
                
                // FontSize=15
            };
            
            var viewbox = new Viewbox();
            viewbox.Child = currentChart;
            viewbox.Measure(currentChart.RenderSize);
            viewbox.Arrange(new Rect(new Point(0, 0), currentChart.RenderSize));
            currentChart.Update(true, true); //force chart redraw
            viewbox.UpdateLayout();
            //png file was created at the root directory.
            return (Helper.SaveToPng(currentChart, fileName));

        }

        internal void AddHeader(dynamic testInfo, Document document, PdfWriter pdfWriter)
        {
            AddHeadertoCertificate(testInfo, document, pdfWriter);
        }

        internal void AddFooter(Document document, PdfWriter pdfWriter)
        {
            AddFootertoCertificate(pdfWriter, document);
        }



        #endregion

        #region CSV Data File related operations

        /// <summary>
        /// Generates a .CSV file with observed values in a predefined location.
        /// </summary>
        /// <param name="testType"></param>
        /// <param name="testInfo"></param>
        /// <param name="noofCyclesCompleted"></param>
        /// <param name="lineSeriesCollection"></param>
        /// <param name="labelCollection"></param>
        internal void GenerateCSVFile(TestType testType, dynamic testInfo, ObservableCollection<LineSeries> lineSeriesCollection, ObservableCollection<List<string>> labelCollection, bool createNew = false, string noofCyclesCompleted = null)
        {
            try
            {
                dynamic testInformation = Helper.GetTestObject(testType, testInfo);
                List<string> stringBuilder1 = new List<string>();
                List<string> stringBuilder2 = new List<string>();
                StringBuilder finalstring = new StringBuilder();
                StringBuilder dataString = new StringBuilder();
                List<string> datacollectionList = new List<string>();
                finalstring.AppendLine(string.Format("CustomerName,{0}", testInformation.CustomerName));
                finalstring.AppendLine(string.Format("JobNumber,J{0}", testInformation.JobNumber));
                finalstring.AppendLine(string.Format("ReportNumber,{0}", testInformation.ReportNumber));
                finalstring.AppendLine(string.Format("Test Date,{0}", testInformation.TestDateTime));
                if (testType == TestType.StrokeTest)
                    AddStrokeTestTestParameters(testInformation, finalstring);
                else if (testType == TestType.HoldMidPositionTest)
                    AddHoldorMidPositionTestParameters(testInformation, finalstring);
                else if (testType == TestType.HoldMidPositionLineBTest)
                    AddHoldMidPositionLineBTestParameters(testInfo, finalstring);
                else if (testType == TestType.SlipStickTest)
                    AddSliporStickTestParameters(testInformation, finalstring);

                finalstring.AppendLine(string.Format("Cylinder Number,{0}", testInformation.CylinderNumber));
                finalstring.AppendLine(string.Format("Rod Size,{0}", testInformation.RodSize));
                finalstring.AppendLine(string.Format("Bore Size,{0}", testInformation.BoreSize));
                finalstring.AppendLine(string.Format("Stroke Length,{0}", testInformation.StrokeLength));
                if (testType == TestType.StrokeTest)
                {
                    finalstring.AppendLine(string.Format("Number of Cycles,{0}", testInformation.NoofCycles));
                    finalstring.AppendLine(string.Format("Number of Cycles Completed,{0}", noofCyclesCompleted));
                }

                for (int count = 0; count < lineSeriesCollection.Count; count++)
                {
                    List<string> labels = labelCollection[count];
                    List<string> xData = Helper.BuildStrig(labels, "");
                    List<string> yData = Helper.BuildStrig(lineSeriesCollection[count].Values, lineSeriesCollection[count].Title);
                    if (datacollectionList != null && datacollectionList.Count == 0)
                    {
                        if (xData != null)
                        {
                            for (int i = 0; i < Math.Min(xData.Count, yData.Count); i++)
                            {
                                datacollectionList.Add(string.Format("{0},{1}", xData[i], yData[i]));
                            }
                        }
                    }
                    else
                    {
                        for (int position = 0; position < datacollectionList.Count; position++)
                        {
                            datacollectionList[position] = string.Format("{0},{1},{2}", datacollectionList[position], xData[position], yData[position]);
                        }
                    }
                }
                for (int size = 0; size < datacollectionList.Count; size++)
                {
                    finalstring.AppendLine(datacollectionList[size]);
                }

                string csvFileName = string.Empty;
                if (testType == TestType.StrokeTest)
                    csvFileName = string.Format(@"{0}\{2}_{5}\{3}\{1}_{2}_StrokeTest_001_Data_{4}.csv", ReportLocation, FilePrefix, testInformation.JobNumber, testInformation.CylinderNumber,"" /*testInformation.TestDateTime.Replace(':', '_').Replace('/', '_')*/, testInformation.CustomerName);
                else if (testType == TestType.SlipStickTest)
                    csvFileName = string.Format(@"{0}\{2}_{5}\{3}\{1}_{2}_SlipStickTest_003_Data_{4}.csv", ReportLocation, FilePrefix, testInformation.JobNumber, testInformation.CylinderNumber,"" /*testInformation.TestDateTime.Replace(':', '_').Replace('/', '-')*/, testInformation.CustomerName);
                else if (testType == TestType.HoldMidPositionTest)
                    csvFileName = string.Format(@"{0}\{2}_{5}\{3}\{1}_{2}_HMPATest_002_Data_{4}.csv", ReportLocation, FilePrefix, testInformation.JobNumber, testInformation.CylinderNumber,"" /*testInformation.TestDateTime.Replace(':', '_').Replace('/', '-')*/, testInformation.CustomerName);
                else if (testType == TestType.HoldMidPositionLineBTest)
                    csvFileName = string.Format(@"{0}\{2}_{5}\{3}\{1}_{2}_HMPBTest_002_Data_{4}.csv", ReportLocation, FilePrefix, testInformation.JobNumber, testInformation.CylinderNumber,"" /*testInformation.TestDateTime.Replace(':', '_').Replace('/', '-')*/, testInformation.CustomerName);
                if (!File.Exists(csvFileName))
                {
                    WriteDatatoFile(testInformation.JobNumber, testInformation.CylinderNumber, testInformation.CustomerName, finalstring, csvFileName);
                }
                else
                {
                  // MessageBoxResult messageOption = MessageBox.Show("Data File with same name already exists.\nDo you want replace it?", "SPG Report Tool", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    //if (!createNew) /*messageOption == MessageBoxResult.Yes)*/ //Override file
                    //{
                    //    try
                    //    {
                    //        File.Delete(csvFileName);
                    //        WriteDatatoFile(testInformation.JobNumber, testInformation.CylinderNumber, testInformation.CustomerName, finalstring, csvFileName);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        // MessageBox.Show(ex.Message, "SPG Reporting Tool", MessageBoxButton.OK, MessageBoxImage.Error);
                    //        MessageBox.Show("File is Open");
                    //        ElpisOPCServerMainWindow.homePage.txtStatusMessage.Text = "CSV File is Open";
                    //    }

                    //}
                    //else //Create new data file
                    //{
                        int fileCount = 0;
                        while (File.Exists(csvFileName))
                        {
                            fileCount++;
                            if (fileCount == 1)
                                csvFileName = string.Concat(csvFileName.Substring(0, csvFileName.LastIndexOf('.')), '(', fileCount, ')', ".csv");
                            else
                                csvFileName = string.Concat(csvFileName.Substring(0, csvFileName.LastIndexOf('(')), '(', fileCount, ')', ".csv");
                        }
                        WriteDatatoFile(testInformation.JobNumber, testInformation.CylinderNumber, testInformation.CustomerName, finalstring, csvFileName);
                    //}
                }
            }
            catch (Exception ex)
            {
                ElpisServer.Addlogs("Report Tool", "Generate CSV", ex.Message, LogStatus.Error);

            }
        }

        private void AddHoldMidPositionLineBTestParameters(Hold_MidPositionLineBTestInformation testInformation, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine(string.Format("Allowable Pressure Drop,{0}", testInformation.AllowablePressureDrop));
            stringBuilder.AppendLine(string.Format("Holding PressureLineA,{0}", testInformation.HoldingPressureLineA));
            stringBuilder.AppendLine(string.Format("Holding PressureLineB,{0}", testInformation.HoldingPressureLineB));
            stringBuilder.AppendLine(string.Format("Cylinder Movement,{0}", testInformation.CylinderMovement));
            stringBuilder.AppendLine(string.Format("Holding TimeLineB,{0}", testInformation.HoldingTimeLineB));
            stringBuilder.AppendLine(string.Format("Initial Pressure Line B,{0}", testInformation.InitialPressureLineB));
            stringBuilder.AppendLine(string.Format("Initial Cylinder Movement,{0}", testInformation.InitialCylinderMovement));
            stringBuilder.AppendLine(string.Format("Test Result,{0}", testInformation.TestStatusB));
        }

        /// <summary>
        /// Get Slip_Stick test information.
        /// </summary>
        /// <param name="testInformation"></param>
        /// <param name="finalstring"></param>
        private void AddSliporStickTestParameters(Slip_StickTestInformation testInformation, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine(string.Format("Initial Pressure,{0}", testInformation.InitialPressure));
            stringBuilder.AppendLine(string.Format("Pressure,{0}", testInformation.PressureAfterFirstCylinderMovement));
            //stringBuilder.AppendLine(string.Format("Flow,{0}", testInformation.Flow));
            //stringBuilder.AppendLine(string.Format("Cylinder Movement,{0}", testInformation.CylinderMovement));
            stringBuilder.AppendLine(string.Format("Initial Cylinder Movement,{0}", testInformation.InitialCylinderMovement));
            stringBuilder.AppendLine(string.Format("Cylinder Movement ,{0}", testInformation.CylinderFirstMovement));


        }

        /// <summary>
        ///Get Hold_Mid Position test information.
        /// </summary>
        /// <param name="testInformation"></param>
        /// <param name="finalstring"></param>
        private void AddHoldorMidPositionTestParameters(Hold_MidPositionLineATestInformation testInformation, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine(string.Format("Allowable Pressure Drop,{0}", testInformation.AllowablePressureDrop));
            stringBuilder.AppendLine(string.Format("Holding PressureLineA,{0}", testInformation.HoldingPressureLineA));
            stringBuilder.AppendLine(string.Format("Holding PressureLineB,{0}", testInformation.HoldingPressureLineB));
            stringBuilder.AppendLine(string.Format("Cylinder Movement,{0}", testInformation.CylinderMovement));
            stringBuilder.AppendLine(string.Format("Holding TimeLineA,{0}", testInformation.HoldingTimeLineA));
            stringBuilder.AppendLine(string.Format("Holding Line A Initial Pressure,{0}", testInformation.HoldingLineAInitialPressure));
            stringBuilder.AppendLine(string.Format("Cylinder Movement Initial value,{0}", testInformation.InitialCylinderMovement));
            stringBuilder.AppendLine(string.Format("Test Result,{0}", testInformation.TestStatusA));


        }

        /// <summary>
        /// Get Hold_Mid Position test information.
        /// </summary>
        /// <param name="testInformation"></param>
        /// <param name="stringBuilder"></param>
        private void AddStrokeTestTestParameters(StrokeTestInformation testInformation, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine(string.Format("PressureLineA,{0}", testInformation.PressureLineA));
            stringBuilder.AppendLine(string.Format("PressureLineB,{0}", testInformation.PressureLineB));
            stringBuilder.AppendLine(string.Format("Flow,{0}", testInformation.Flow));
            stringBuilder.AppendLine(string.Format("LineAPressureInput,{0}", testInformation.LineAPressureInput));
            stringBuilder.AppendLine(string.Format("LineBPressureInput,{0}", testInformation.LineBPressureInput));
            stringBuilder.AppendLine(string.Format("StrokeLengthValue,{0}", testInformation.StrokeLengthValue));
            stringBuilder.AppendLine(string.Format("Comment,{0}", testInformation.Comment));
        }

        /// <summary>
        /// Write the test information in CSV File.
        /// </summary>
        /// <param name="jobNumber"></param>
        /// <param name="finalstring"></param>
        /// <param name="csvFileName"></param>
        private void WriteDatatoFile(string jobNumber, string cylindeNumber, string customerName, StringBuilder finalstring, string csvFileName)
        {
            try
            {
                Directory.CreateDirectory(string.Format(@"{0}\{1}_{3}\{2}", ReportLocation, jobNumber, cylindeNumber, customerName));
                File.AppendAllText(csvFileName, finalstring.ToString());// + Environment.NewLine);
                                                                        //MessageBox.Show("Data file created in following path:\n" + csvFileName, "SPG Report Tool", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("File Path:" + csvFileName + "\n" + ex.Message);
            }
        }

        #endregion

    }

}
