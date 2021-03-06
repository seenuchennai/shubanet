﻿using System;
using ShubhaRt;
using System.Configuration;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;
using System.IO;
using Microsoft.VisualBasic;
using System.Globalization;
using FileHelpers.RunTime;
using System.Data;
using Microsoft.Office.Interop.Excel;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Threading;
using log4net;
using log4net.Config;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized ;
using System.Collections;
using System.IO.Compression;
using System.Diagnostics;

using System.IO.Packaging;
using Ionic.Zlib;
using Ionic.Zip;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using ManagedWinapi.Windows;
using ManagedWinapi.Accessibility;
using Microsoft.Win32;



namespace StockD
{
     

      
       

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        string AppId = "1";
        string PathForYahooRT = "";
        string baseurlForYahooRt = "";
        List<string> yahoortname = new List<String>();
        List<string> yahoortdata = new List<String>();
        System.Windows.Threading.DispatcherTimer DispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
        Type ExcelType;
        object ExcelInst;
        Type type;
        List<string> symbolname = new List<String>();
        List<string> exchagename = new List<string>();

        IRtdServer m_server;

        object[] args = new object[3];
        List<string> marketsymbol = new List<string>();
        List<string> Exchangename = new List<string>();

        List<int> marketsymboltoremove = new List<int>();


        List<string> companynameforprocessing = new List<string>();
        List<string> yahoosysmbolforprocessing = new List<string>();

        List<string> YahooSymbolsave = new List<string>();
        List<string> YahooNamesave = new List<string>();

        List<string> YahooExchangesave = new List<string>();
        List<Int32> yahoosymbolindextoremove = new List<int>();

        string url1 = "http://www.goog";
        WebClient Client = new WebClient();
        int flag = 0;
       
        double value = 0;
        List<string> nameofbseindex = new List<string>();//imp
        List<string> namemcxindex = new List<string> { "COMDEX", "METAL", "ENRGY", "AGRI" };

        List<string> namespotindex = new List<string> { "scomdex", "smetal", "senergy", "sagri" };

        public MainWindow()
        {

        }
        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))


                yield return day;
        }
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);



        private void prograss()
        {

            //Stores the value of the ProgressBar



            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(ProgressBar1.SetValue);


            value += 20;
            Dispatcher.Invoke(updatePbDelegate,
                System.Windows.Threading.DispatcherPriority.Background,
                new object[] { System.Windows.Controls.ProgressBar.ValueProperty, value });
        }



        //Download start 
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnExit.IsEnabled = true;

            if (dtStartDate.Text == "" || dtEndDate.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Please Select Date.. ");

                return;

            }


            DateTime dtstart, dtend;
            if (dtStartDate.Text != "")
            {
                dtstart = Convert.ToDateTime(dtStartDate.Text);
                dtend = Convert.ToDateTime(dtEndDate.Text);

                if (dtstart > dtend)
                {

                    System.Windows.MessageBox.Show("Please Enter  Date more than start Date ");

                }
            }
            if (txtTargetFolder.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Please Set Path.. ");
                return;

            }

            string strYearDir = txtTargetFolder.Text + "\\Downloads\\BSEBlock";

            if (Directory.Exists(strYearDir))
                Directory.Delete(strYearDir, true);

            strYearDir = txtTargetFolder.Text + "\\Downloads\\BSEBulk";

            if (Directory.Exists(strYearDir))
                Directory.Delete(strYearDir, true);


            string dest_filename;

            dest_filename = txtTargetFolder.Text + "\\Reports";
            if (!Directory.Exists(dest_filename))
                Directory.CreateDirectory(dest_filename);


            lbl_Download.Visibility = Visibility.Visible;
            lbl_Download.Content = "Please Wait File Is Downloading.....";
            //Configure the ProgressBar
            ProgressBar1.Minimum = 0;
            ProgressBar1.Maximum = short.MaxValue;
            ProgressBar1.Value = 0;

            //Stores the value of the ProgressBar


            //Create a new instance of our ProgressBar Delegate that points
            //  to the ProgressBar's SetValue method.
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(ProgressBar1.SetValue);

            //Tight Loop:  Loop until the ProgressBar.Value reaches the max
            do
            {
                ProgressBar1.Visibility = Visibility.Visible;

                btnStart.IsEnabled = false;
                /*Update the Value of the ProgressBar:
                  1)  Pass the "updatePbDelegate" delegate that points to the ProgressBar1.SetValue method
                  2)  Set the DispatcherPriority to "Background"
                  3)  Pass an Object() Array containing the property to update (ProgressBar.ValueProperty) and the new value */
                value += 10;
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { System.Windows.Controls.ProgressBar.ValueProperty, value });

                string baseurl, filename = "";
                DateTime StartDate, EndDate;
                strYearDir = txtTargetFolder.Text + "\\Downloads";


                StartDate = Convert.ToDateTime(dtStartDate.Text);
                EndDate = Convert.ToDateTime(dtEndDate.Text);

                if (!Directory.Exists(strYearDir))
                    Directory.CreateDirectory(strYearDir);




                if (chkCombinedReport.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netnsecombine.html");
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\combined_report" + formatdate(day) + ".zip";
                        baseurl = "http://www.nseindia.com/archives/combine_report/combined_report" + formatdate(day) + ".zip";

                        //http://www.nseindia.com/archives/combine_report/combined_report16052013.zip

                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);


                            //dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_combined_report_" + formatdate(day) + ".zip";

                            //movefile(strYearDir, dest_filename);
                        }


                    }

                }


                if (Cb_NSE_PR.IsChecked == true)
                {
                    prograss();


                    strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                    baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";
                    if (!System.IO.File.Exists(strYearDir))
                    {
                        prograss();
                        downliaddata(strYearDir, baseurl);
                    }


                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, year;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }

                        if (day.Month < 10)
                        {

                            date1 = date1 + "0" + day.Month.ToString();
                        }
                        else
                        {
                            date1 = date1 + day.Month.ToString();
                        }
                        year = day.Year.ToString();

                        string lastTwoChars = year.Substring(year.Length - 2);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + ".zip";
                        if (!Directory.Exists(strYearDir))
                        {
                            baseurl = "http://www.nseindia.com/archives/equities/bhavcopy/pr/PR" + date1 + lastTwoChars + ".zip";
                        }
                        //http://www.nseindia.com/archives/equities/bhavcopy/pr/PR160513.zip
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);
                        }
                        if (System.IO.File.Exists(strYearDir))
                        {


                            using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                            {
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars))
                                {
                                    zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars);


                                }

                            }

                        }



                        //Creating Report Files 
                        dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_NEWHIGH_NEWLOW_.csv";
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\HL" + date1 + lastTwoChars + ".csv";
                        movefile(strYearDir, dest_filename);


                        //BC

                        dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_CORPORATE_ACTION.csv";
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\BC" + date1 + lastTwoChars + ".csv";
                        movefile(strYearDir, dest_filename);
                        //BH
                        dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_BAND_HIT.csv";
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\BH" + date1 + lastTwoChars + ".csv";
                        movefile(strYearDir, dest_filename);
                        //GL
                        dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_TOP10_GAINER_LOSER.csv";
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\GL" + date1 + lastTwoChars + ".csv";
                        movefile(strYearDir, dest_filename);

                        //BM AND AN
                        dest_filename = txtTargetFolder.Text + "\\Reports\\nseannouncements.csv";
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\BM" + date1 + lastTwoChars + ".txt";
                        movefile(strYearDir, dest_filename);


                        //    //fo
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\fo" + date1 + day.Year + ".zip";
                        try
                        {

                            using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                            {
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\FO" + date1 + lastTwoChars))
                                {
                                    zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\FO" + date1 + lastTwoChars);
                                }
                            }


                            strYearDir = txtTargetFolder.Text + " \\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\FO" + date1 + lastTwoChars + "\\FO" + date1 + day.Year + ".csv";
                            string[] PRFO = new string[1] { "" };
                            PRFO[0] = strYearDir;

                            strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                            baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";

                            string sec = strYearDir;
                            if (!System.IO.File.Exists(strYearDir))
                            {
                                prograss();
                                downliaddata(strYearDir, baseurl);


                            }
                            string daytostore = "";
                            string monthtostore = "";

                            if (day.Day <= 9)
                            {
                                daytostore = "0" + day.Day.ToString();
                            }
                            else
                            {
                                daytostore = day.Day.ToString();

                            }
                            if (day.Month <= 9)
                            {
                                monthtostore = "0" + day.Month.ToString();
                            }
                            else
                            {
                                monthtostore = day.Month.ToString();

                            }

                            string datetostore1 = day.Year + monthtostore + daytostore;
                            try
                            {
                                ExecuteFUTUREProcessing(PRFO, "FO", datetostore1, sec);
                                filetransfer(PRFO[0], txtTargetFolder.Text + "\\STD_CSV\\NSE_Equity_Futures_fo" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                Deletspace(txtTargetFolder.Text + "\\STD_CSV\\NSE_Equity_Futures_fo" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                                }
                                if (!Directory.Exists(txtTargetFolder.Text + "\\FCharts"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\FCharts");
                                }
                                if (!Directory.Exists(txtTargetFolder.Text + "\\AdvanceGet"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\AdvanceGet");
                                }

                                if (comboBox1.SelectedItem.ToString() == "Amibroker")
                                {

                                    System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_Equity_Futures_fo" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\Amibroker\\NSE_Equity_Futures_fo" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                    Amibroker(txtTargetFolder.Text + "\\Amibroker\\NSE_Equity_Futures_fo" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                }
                                if (comboBox1.SelectedItem.ToString() == "FCharts")
                                {

                                    System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_Equity_Futures_fo" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\FCharts\\NSE_Equity_Futures_fo" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                    Fchart(txtTargetFolder.Text + "\\FCharts\\NSE_Equity_Futures_fo" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                }
                                if (comboBox1.SelectedItem.ToString() == "AdvanceGet")
                                {

                                    System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_Equity_Futures_fo" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\AdvanceGet\\NSE_Equity_Futures_fo" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                    Fchart(txtTargetFolder.Text + "\\AdvanceGet\\NSE_Equity_Futures_fo" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                }


                            }
                            catch
                            {
                            }
                        }
                        catch
                        {
                        }





                    }

                }



                if (Cb_NSE_EOD_BhavCopy.IsChecked == true)
                {


                    visit("http://list.shubhalabha.in/netnsecm.html");



                    strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                    baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";

                    string sec = strYearDir;
                    if (!System.IO.File.Exists(strYearDir))
                    {
                        prograss();
                        downliaddata(strYearDir, baseurl);


                    }







                    prograss();


                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, date2;


                        strYearDir = txtTargetFolder.Text + "\\Downloads\\MTO_" + formatdate(day) + ".csv";

                        baseurl = " http://nseindia.com/archives/equities/mto/MTO_" + formatdate(day) + ".DAT";

                        string[] mto = new string[1] { "" };

                        mto[0] = strYearDir;

                        if (!System.IO.File.Exists(strYearDir))
                        {

                            downliaddata(strYearDir, baseurl);

                            //Webpage visit
                            HttpWebRequest request = WebRequest.Create("http://list.shubhalabha.in/netnsecm.html") as HttpWebRequest;
                            //  WebClient webClient = new WebClient();
                            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                            {
                                // Get the response stream  
                                StreamReader reader = new StreamReader(response.GetResponseStream());

                                // Console application output  

                                Console.WriteLine(reader.ReadToEnd());
                            }

                        }




                        if (day.Day < 10)
                        {
                            date1 = "0" + (day.Day).ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }

                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv.zip";
                        baseurl = "http://www.nseindia.com/content/historical/EQUITIES/" + day.Year.ToString() + "/" + strMonthName.Substring(0, 3).ToUpper() + "/cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv.zip";

                        //  http://www.nseindia.com/content/historical/EQUITIES/2013/MAY/cm17MAY2013bhav.csv.zip


                        downliaddata(strYearDir, baseurl);

                        try
                        {

                            if (!Directory.Exists(strYearDir))
                            {
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav"))
                                {
                                    using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                                    {
                                        if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav"))
                                        {
                                            zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav");

                                        }
                                    }


                                    string[] strnse = new string[1] { "" };
                                    strnse[0] = txtTargetFolder.Text + "\\Downloads\\cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav\\cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv";


                                    if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\Metastock"))
                                    {
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV\\Metastock");
                                    }
                                    ExecuteNSEEQUITYProcessing(mto, strnse, sec, "STDCSV", txtTargetFolder.Text + "\\");
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");

                                    filetransfer(strnse[0], txtTargetFolder.Text + "\\STD_CSV\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                    if (comboBox1.SelectedItem == "Metastock")
                                    {

                                        commandpromptcall(txtTargetFolder.Text + "\\STD_CSV\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\STD_CSV\\Metastock\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhavv");
                                    }
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                                    {
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                                    }
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\FCharts"))
                                    {
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\FCharts");
                                    }
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\AdvanceGet"))
                                    {
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\AdvanceGet");
                                    }
                                    if (comboBox1.SelectedItem == "Amibroker")
                                    {
                                        System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\Amibroker\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");


                                        Amibroker(txtTargetFolder.Text + "\\Amibroker\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                    }
                                    if (comboBox1.SelectedItem == "FCharts")
                                    {
                                        System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\FCharts\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");


                                        Fchart(txtTargetFolder.Text + "\\FCharts\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                    }
                                    if (comboBox1.SelectedItem == "AdvanceGet")
                                    {
                                        System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\AdvanceGet\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");


                                        AdvanceGet(txtTargetFolder.Text + "\\AdvanceGet\\Nse_Cash_Market_cm" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                    }


                                }
                            }
                        }
                        catch
                        {
                        }

















                    }

                }


                if (Cb_NSE_Index.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netindex.html");


                    prograss();

                    try
                    {
                        foreach (DateTime day in EachDay(StartDate, EndDate))
                        {
                            System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                            string strMonthName = mfi.GetMonthName(day.Month).ToString();


                            strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                            baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";

                            string sec = strYearDir;
                            if (!System.IO.File.Exists(strYearDir))
                            {
                                prograss();
                                downliaddata(strYearDir, baseurl);


                            }
                            string secname = strYearDir;

                            strYearDir = txtTargetFolder.Text + "\\Downloads\\NseIndex" + formatdate(day) + ".csv";

                            baseurl = "http://nseindia.com/content/indices/ind_close_all_" + formatdate(day) + ".csv";



                            downliaddata(strYearDir, baseurl);
                           

                            //process 
                            if (System.IO.File.Exists(strYearDir))
                            {

                                try
                                {
                                    string[] nseindex = new string[1] { "" };
                                    nseindex[0] = strYearDir;
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                                    ExecuteINDEXProcessing(nseindex, "NSEINDEX", txtTargetFolder.Text + "\\Download", secname);


                                    filetransfer(nseindex[0], txtTargetFolder.Text + "\\STD_CSV\\NSE_Indices_NSE_Index" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                                    {
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                                    }
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\FCharts"))
                                    {
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\FCharts");
                                    }
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\AdvanceGet"))
                                    {
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\AdvanceGet");
                                    }
                                    if (comboBox1.SelectedItem == "Amibroker")
                                    {
                                        System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_Indices_NSE_Index" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv", txtTargetFolder.Text + "\\Amibroker\\NSE_Indices_NSE_Index" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");


                                        Amibroker(txtTargetFolder.Text + "\\Amibroker\\NSE_Indices_NSE_Index" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");
                                    }
                                    if (comboBox1.SelectedItem.ToString() == "FCharts")
                                    {
                                        System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_Indices_NSE_Index" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv", txtTargetFolder.Text + "\\FCharts\\NSE_Indices_NSE_Index" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");


                                        Fchart(txtTargetFolder.Text + "\\FCharts\\NSE_Indices_NSE_Index" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");
                                    }

                                    if (comboBox1.SelectedItem.ToString() == "AdvanceGet")
                                    {
                                        System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_Indices_NSE_Index" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv", txtTargetFolder.Text + "\\AdvanceGet\\NSE_Indices_NSE_Index" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");


                                        AdvanceGet(txtTargetFolder.Text + "\\AdvanceGet\\NSE_Indices_NSE_Index" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");
                                    }


                                }
                                catch
                                {
                                }

                            }

                        }


                    }
                    catch
                    {
                    }

                }

                if (ChkYahooIEOD1.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netyahooieod.html");

                    prograss();
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoo1min";

                    if (!Directory.Exists(strYearDir))
                        Directory.CreateDirectory(strYearDir);
                    //string [] yahooieod1 = new string[20] ;
                    List<string> yahooieod1 = new List<String>();

                    //{"ACROPETAL.ns","ADANIENT.ns","ADANIPOWE.ns","ADFFOODS.ns","ADHUNIK.ns","ADORWELD.ns","ADSL.ns","ADVANIHOT.ns","ADVANTA.ns","AEGISCHEM.ns","AFL.ns","AFTEK.ns","AREVAT&D.ns","M&M.ns" };
                    companynameforprocessing.Clear();
                    yahoosysmbolforprocessing.Clear();

                    string datetostore = "";

                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\YahooSymbol.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                yahoosysmbolforprocessing.Add(line);
                                i++;

                            }
                        }

                    }
                    catch
                    {
                    }

                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\YahooCompany.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                companynameforprocessing.Add(line);
                                i++;

                            }
                        }

                    }
                    catch
                    {
                    }
                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();


                        for (int i = 0; i < yahoosysmbolforprocessing.Count; i++)
                        {

                            string dayforname = "";
                            if (day.Day <= 9)
                            {
                                dayforname = "0" + day.Day.ToString();
                            }
                            else
                            {
                                dayforname = day.Day.ToString();


                            }
                            strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoo1min\\" + dayforname + yahoosysmbolforprocessing[i] + ".csv";

                            baseurl = "http://chartapi.finance.yahoo.com/instrument/1.0/" + yahoosysmbolforprocessing[i] + "/chartdata;type=quote;range=1d/csv/";


                            // "http://chartapi.finance.yahoo.com/instrument/1.0/ACROPETAL.ns/chartdata;type=quote;range=1d/csv/"

                            downliaddata(strYearDir, baseurl);
                            //Webpage visit
                            HttpWebRequest request = WebRequest.Create("http://list.shubhalabha.in/netyahooieod.html") as HttpWebRequest;
                            //  WebClient webClient = new WebClient();
                            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                            {
                                // Get the response stream  
                                StreamReader reader = new StreamReader(response.GetResponseStream());

                                // Console application output  

                                Console.WriteLine(reader.ReadToEnd());
                            }


                            try
                            {
                                string[] csvFileNames = new string[1] { "" };


                                csvFileNames[0] = txtTargetFolder.Text + "\\Downloads\\Yahoo1min\\" + dayforname + yahoosysmbolforprocessing[i] + ".csv";



                                string monthtostore = "";
                                if (day.Month <= 9)
                                {
                                    monthtostore = "0" + day.Month.ToString();
                                }
                                else
                                {
                                    monthtostore = day.Month.ToString();

                                }
                                datetostore = day.Year.ToString() + monthtostore + dayforname;
                                ExecuteYAHOOProcessing(csvFileNames, datetostore, "YAHOO1MIN",i);
                                if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                                }
                                if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\Yahoo1min"))
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\Yahoo1min");
                                JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\Yahoo1min\\Yahoo1min" + yahoosysmbolforprocessing[i] + datetostore + ".csv");
                            }
                            catch
                            {

                            }
                        }
                    }

                }



                if (Cb_Yahoo_Realtime.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netyahoort.html");

                    prograss();
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\YahooRT";

                    if (!Directory.Exists(strYearDir))
                        Directory.CreateDirectory(strYearDir);
                    // string[] yahooieod5 = new string[20];
                    List<string> YahooRT = new List<String>();
                    string yahoortsymbol = "";
                    //{"ACROPETAL.ns","ADANIENT.ns","ADANIPOWE.ns","ADFFOODS.ns","ADHUNIK.ns","ADORWELD.ns","ADSL.ns","ADVANIHOT.ns","ADVANTA.ns","AEGISCHEM.ns","AFL.ns","AFTEK.ns","AREVAT&D.ns","M&M.ns","^AEX","^AORD","^ATX","^BFX ","^HSI","^JKSE","^KLSE","^KS11","^N225","^NZ50","^OMXSPI","^OSEAX","^SMSI","^SSEC","^SSMI","^STI","^TWII","000001.SS","^GSPC","^IXIC","^DJI","^DJT","^DJU","^DJA","^TV.N","^NYA","^NUS","^NIN","^NWL","^NTM","^TV.O","^NDX","^IXBK","^IXFN","^IXF","^IXID","^IXIS","^IXK","^IXTR","^IXUT","^NBI","^OEX","^MID","^SML","^SPSUPX","^XAX","^IIX","^NWX","^XMI","^PSE","^SOXX","^RUI","^RUA","^DOT","^DWC","^BATSK","^DJC","^XAU","^TYX","^TNX","^FVX","^IRX","^FCHI","^FTSE","^GDAXI","NIFTY","^NSEI"};

                    companynameforprocessing.Clear();
                    yahoosysmbolforprocessing.Clear();


                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\YahooSymbol.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                yahoosysmbolforprocessing.Add(line);
                                i++;

                            }
                        }

                    }
                    catch
                    {
                    }

                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\YahooCompany.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                companynameforprocessing.Add(line);
                                i++;

                            }
                        }

                    }
                    catch
                    {
                    }

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {


                        try
                        {

                            System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                            string strMonthName = mfi.GetMonthName(day.Month).ToString();

                            strYearDir = txtTargetFolder.Text + "\\Downloads\\YahooRT\\yahoort.csv";

                            baseurl = "http://download.finance.yahoo.com/d/quotes.csv?s=^" + yahoortsymbol.Substring(0, yahoortsymbol.Length - 1) + "&f=snl1d1t1c1ohgv&e=.csv%20";

                            string monthtostore = "";
                            if (day.Month <= 9)
                            {
                                monthtostore = "0" + day.Month.ToString();
                            }
                            else
                            {
                                monthtostore = day.Month.ToString();

                            }
                            string dayforname = "";
                            if (day.Day <= 9)
                            {
                                dayforname = "0" + day.Day.ToString();
                            }
                            else
                            {
                                dayforname = day.Day.ToString();


                            }
                            // "http://chartapi.finance.yahoo.com/instrument/5.0/ACROPETAL.ns/chartdata;type=quote;range=1d/csv/"
                            //http://download.finance.yahoo.com/d/quotes.csv?s=^DJI+TCS+AA+AXP+BA+C+CAT+DD+DIS+EK+GE+HD+HON+HPQ+IBM+INTC+IP+JNJ+JPM+KO+MCD+MMM+MO+MRK+MSFT+PG+T+UTX+WMT+XOM&f=snl1d1t1c1ohgv&e=.csv%20[^]
                            downliaddata(strYearDir, baseurl);
                            string datetostrore = day.Year.ToString() + monthtostore + dayforname;
                            string[] namert = new string[1] { "" };
                            namert[0] = strYearDir;
                            ExecuteYAHOOProcessing(namert, datetostrore, "YAHOORT",0);
                            if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\RT"))
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\RT");
                            string[] csvFileNames = new string[1] { "" };
                            csvFileNames[0] = strYearDir;
                            System.IO.File.Copy(csvFileNames[0], txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\RT\\YAHOORT.csv");
                        }
                        catch
                        {
                        }

                    }

                }

                if (ChkYahooIEOD5.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netyahooieod.html");

                    prograss();
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoo5min";

                    if (!Directory.Exists(strYearDir))
                        Directory.CreateDirectory(strYearDir);
                    // string[] yahooieod5 = new string[20];
                    List<string> yahooieod5 = new List<String>();

                    //{"ACROPETAL.ns","ADANIENT.ns","ADANIPOWE.ns","ADFFOODS.ns","ADHUNIK.ns","ADORWELD.ns","ADSL.ns","ADVANIHOT.ns","ADVANTA.ns","AEGISCHEM.ns","AFL.ns","AFTEK.ns","AREVAT&D.ns","M&M.ns","^AEX","^AORD","^ATX","^BFX ","^HSI","^JKSE","^KLSE","^KS11","^N225","^NZ50","^OMXSPI","^OSEAX","^SMSI","^SSEC","^SSMI","^STI","^TWII","000001.SS","^GSPC","^IXIC","^DJI","^DJT","^DJU","^DJA","^TV.N","^NYA","^NUS","^NIN","^NWL","^NTM","^TV.O","^NDX","^IXBK","^IXFN","^IXF","^IXID","^IXIS","^IXK","^IXTR","^IXUT","^NBI","^OEX","^MID","^SML","^SPSUPX","^XAX","^IIX","^NWX","^XMI","^PSE","^SOXX","^RUI","^RUA","^DOT","^DWC","^BATSK","^DJC","^XAU","^TYX","^TNX","^FVX","^IRX","^FCHI","^FTSE","^GDAXI","NIFTY","^NSEI"};

                    companynameforprocessing.Clear();
                    yahoosysmbolforprocessing.Clear();

                    string datetostore = "";

                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\YahooSymbol.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                yahoosysmbolforprocessing.Add(line);
                                i++;

                            }
                        }

                    }
                    catch
                    {
                    }

                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\YahooCompany.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                companynameforprocessing.Add(line);
                                i++;

                            }
                        }

                    }
                    catch
                    {
                    }
                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();


                        for (int i = 0; i < yahoosysmbolforprocessing.Count(); i++)
                        {


                            string dayforname = "";
                            if (day.Day <= 9)
                            {
                                dayforname = "0" + day.Day.ToString();
                            }
                            else
                            {
                                dayforname = day.Day.ToString();


                            }

                            strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoo5min\\" + dayforname + yahoosysmbolforprocessing[i] + ".csv";

                            baseurl = "http://chartapi.finance.yahoo.com/instrument/5.0/" + yahoosysmbolforprocessing[i] + "/chartdata;type=quote;range=5d/csv/";


                            // "http://chartapi.finance.yahoo.com/instrument/5.0/ACROPETAL.ns/chartdata;type=quote;range=1d/csv/"

                            downliaddata(strYearDir, baseurl);


                            try
                            {
                                string[] csvFileNames = new string[1] { "" };
                                csvFileNames[0] = txtTargetFolder.Text + "\\Downloads\\Yahoo5min\\" + dayforname + yahoosysmbolforprocessing[i] + ".csv";

                                string monthtostore = "";
                                if (day.Month <= 9)
                                {
                                    monthtostore = "0" + day.Month.ToString();
                                }
                                else
                                {
                                    monthtostore = day.Month.ToString();

                                }

                                datetostore = day.Year.ToString() + monthtostore + dayforname;
                                log4net.Config.XmlConfigurator.Configure();
                                ILog log = LogManager.GetLogger(typeof(MainWindow));
                                log.Debug("yahoo File Processing strated....... ");
                                ExecuteYAHOOProcessing(csvFileNames, datetostore, "YAHOO5MIN",i );
                                log.Debug("yahoo File Processing End....... ");

                                if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                                }

                                if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\Yahoo5min"))
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\Yahoo5min");

                                JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\Yahoo5min\\Yahoo5min" + yahoosysmbolforprocessing[i] + datetostore + ".csv");
                            }
                            catch (Exception ex)
                            {
                                log4net.Config.XmlConfigurator.Configure();
                                ILog log = LogManager.GetLogger(typeof(MainWindow));
                                log.Debug(ex.Message);
                            }




                        }

                    }

                }
                if (chkYahooFundamental.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netyahoofunda.html");

                    prograss();
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoofun";

                    if (!Directory.Exists(strYearDir))
                        Directory.CreateDirectory(strYearDir);
                    // string[] yahoofun = new string[];
                    List<string> yahoofun = new List<String>();

                    // { "tatasteel.ns","ADANIENT.ns","ADANIPOWE.ns","ADFFOODS.ns","ADHUNIK.ns","ADORWELD.ns","ADSL.ns","ADVANIHOT.ns","ADVANTA.ns","AEGISCHEM.ns","AFL.ns","AFTEK.ns","AREVAT&D.ns","M&M.ns"};

                    companynameforprocessing.Clear();
                    yahoosysmbolforprocessing.Clear();


                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\YahooSymbol.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                yahoosysmbolforprocessing.Add(line);
                                i++;

                            }
                        }

                    }
                    catch
                    {
                    }

                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\YahooCompany.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                companynameforprocessing.Add(line);
                                i++;

                            }
                        }

                    }
                    catch
                    {
                    }
                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, date2;
                        if (day.Day < 10)
                        {
                            date1 = "0" + (day.Day - 1).ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }

                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }
                        try
                        {
                            for (int i = 0; i < yahoosysmbolforprocessing.Count(); i++)
                            {



                                strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoofun\\" + day.Day + yahoosysmbolforprocessing[i] + ".csv";
                                baseurl = "http://download.finance.yahoo.com/d/quotes.csv?s=" + yahoosysmbolforprocessing[i] + "&f=snl1ee7e8e9r5b4j4p5s6s7r1qdt8j1f6&e=.csv";
                                // "http://download.finance.yahoo.com/d/quotes.csv?s=ADANIENT.ns&f=snl1ee7e8e9r5b4j4p5s6s7r1qdt8j1f6&e=.csv"


                                downliaddata(strYearDir, baseurl);



                                string tempfilepath = "";


                                if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\Yahoofun1"))
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\Downloads\\Yahoofun1");

                                tempfilepath = txtTargetFolder.Text + "\\Downloads\\Yahoofun1\\" + day.Day + yahoosysmbolforprocessing[i] + ".csv";
                                var delimiter = ",";
                                var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");
                                using (var writer = new StreamWriter(tempfilepath))
                                using (var reader = new StreamReader(strYearDir))
                                {
                                    string line = null;
                                    // line = reader.ReadLine();
                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        var columns = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                                        for (int j = 0; j < columns.Count() - 1; j++)
                                        {
                                            if (columns[j].Trim() == "N/A" || columns[j].Trim() == "\\N/A\\")
                                            {
                                                columns[j] = "0";
                                            }
                                        }


                                        writer.WriteLine(string.Join(delimiter, columns));


                                    }

                                }



                            }

                            if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\FUNDAMENTAL"))
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\FUNDAMENTAL");

                            string[] csvFileNames = Directory.GetFiles(txtTargetFolder.Text + "\\Downloads\\Yahoofun1", "*.csv");
                            Joinbseindex(csvFileNames, txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\FUNDAMENTAL\\Yahoo_Fundamental.csv");


                            if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                            }

                        }
                        catch
                        {

                        }

                    }


                }

                if (chkYahooEOD.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netyahooeod.html");

                    prograss();
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\YahooEod";

                    if (!Directory.Exists(strYearDir))
                        Directory.CreateDirectory(strYearDir);
                    List<string> yahooeod = new List<String>();
                    companynameforprocessing.Clear();
                    yahoosysmbolforprocessing.Clear();

                    string datetostore = "";

                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\YahooSymbol.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                yahoosysmbolforprocessing.Add(line);
                                i++;

                            }
                        }

                    }
                    catch
                    {
                    }

                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\YahooCompany.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                companynameforprocessing.Add(line);
                                i++;

                            }
                        }

                    }
                    catch
                    {
                    }
                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, date2;
                        if (day.Day < 10)
                        {
                            date1 = "0" + (day.Day - 1).ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }

                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        for (int i = 0; i < yahoosysmbolforprocessing.Count(); i++)
                        {
                            strYearDir = txtTargetFolder.Text + "\\Downloads\\YahooEod\\" + date1 + yahoosysmbolforprocessing[i] + ".csv";
                            string e1 = Convert.ToInt32(date1) + 1.ToString();
                            baseurl = "http://ichart.finance.yahoo.com/table.csv?s=" + yahoosysmbolforprocessing[i] + "&a=" + date2 + day.Month + "&b=" + date1 + "&c=" + day.Year + "&d=" + date2 + "&e" + e1 + "&f=" + day.Year + "&g=d";
                            //http://ichart.finance.yahoo.com/table.csv?s=ADANIENT.ns&a=045&b=01&c=2013&d=04&e=02&f=2013&g=d"

                            downliaddata(strYearDir, baseurl);
                        }


                        datetostore = day.Year.ToString() + date2 + date1;



                    }



                    try
                    {
                        string[] csvFileNames = new string[1] { "" };

                        csvFileNames = Directory.GetFiles(txtTargetFolder.Text + "\\Downloads\\YahooEod", "*.csv");

                        ExecuteYAHOOProcessing(csvFileNames, datetostore, "YAHOOEOD",0);
                        if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                        {
                            Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                        }
                        if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\YAHOO"))
                        {
                            Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV\\YAHOO");
                        }
                        if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\EOD"))
                        {
                            Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\EOD");
                        }
                        string[] nameformove = new string[1] { "" };
                        nameformove[0] = csvFileNames[0];



                        JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\EOD\\YahooEod.csv");
                        //commandpromptcall(txtTargetFolder.Text + "\\STD_CSV\\YAHOO\\EOD\\YahooEod.csv", txtTargetFolder.Text + "\\STD_CSV\\Metastock\\YahooEod");

                        if (Directory.Exists(txtTargetFolder.Text + "\\Downloads\\YahooEod"))
                        {
                            Directory.Delete(txtTargetFolder.Text + "\\Downloads\\YahooEod", true);
                        }
                    }
                    catch
                    {

                    }


                }

                if (ChkGoogleEOD.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netgoogleieod.html");

                    prograss();
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\Googleeod";

                    if (!Directory.Exists(strYearDir))
                        Directory.CreateDirectory(strYearDir);
                    List<string> GoogleEod = new List<String>();

                    //{ "LICHSGFIN.nse","ADANIENT.nse","ADANIPOWE.nse","ADFFOODS.nse","ADHUNIK.nse","ADORWELD.nse","ADSL.nse","ADVANIHOT.nse","ADVANTA.nse","AEGISCHEM.nse","AFL.nse","AFTEK.nse","AREVAT&D.nse","M&M.nse",".AEX,indexeuro",".AORD,indexasx",".HSI,indexhangseng",",.N225,indexnikkei",".NSEI,nse",".NZ50,nze",".TWII,tpe","000001,sha","CNX100,nse","CNX500,nse","CNXENERGY,nse","CNXFMCG,nse","CNXINFRA,nse","CNXIT,nse"};
                    try
                    {

                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\GoogleEOD.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                GoogleEod.Add(line);
                                i++;

                            }
                        }

                    }
                    catch
                    {

                    }
                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();

                        for (int i = 0; i < GoogleEod.Count(); i++)
                        {
                            strYearDir = txtTargetFolder.Text + "\\Downloads\\Googleeod\\" + day.Day + GoogleEod[i] + ".csv";
                            baseurl = "http://www.google.com/finance/getprices?q=" + GoogleEod[i] + "&x=NSE&i=60&p=5d&f=d,o,h,l,c,v&df=cpct&auto=1&ts=1266701290218";
                            // "http://www.google.com/finance/getprices?q=LICHSGFIN&x=LICHSGFIN&i=d&p=15d&f=d,o,h,l,c,v"
                            //http://www.google.com/finance/getprices?q=RELIANCE&x=NSE&i=60&p=5d&f=d,c,o,h,l&df=cpct&auto=1&ts=1266701290218 [^]

                            downliaddata(strYearDir, baseurl);

                            ////////////////////metastock
                            

                            try
                            {
                                string[] csvFileNames = new string[1] { "" };
                                csvFileNames[0] = txtTargetFolder.Text + "\\Downloads\\Googleeod\\" + day.Day + GoogleEod[i] + ".csv";



                                string datetostore = "";
                                string monthtostore = "";
                                string dayforname = "";
                                if (day.Month <= 9)
                                {
                                    monthtostore = "0" + day.Month.ToString();
                                }
                                else
                                {
                                    monthtostore = day.Month.ToString();

                                } if (day.Day <= 9)
                                {
                                    dayforname = "0" + day.Day.ToString();
                                }
                                else
                                {
                                    dayforname = day.Day.ToString();

                                }
                                datetostore = day.Year.ToString() + monthtostore + dayforname;

                                datetostore = day.Year.ToString() + day.Month.ToString() + day.Day.ToString();

                                log4net.Config.XmlConfigurator.Configure();
                                ILog log = LogManager.GetLogger(typeof(MainWindow));
                                log.Debug("yahoo File Processing strated....... ");
                                ExecuteYAHOOProcessing(csvFileNames, datetostore, "GOOGLEEOD",i);
                                log.Debug("yahoo File Processing End....... ");
                                if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\\\GoogleEod"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV\\\\GoogleEod");
                                }
                                if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\GoogleEod"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\GoogleEod");
                                }
                                if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\Metastock"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV\\Metastock");
                                }
                                JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\STD_CSV\\GoogleEod\\Googleeod" + GoogleEod[i] + datetostore + ".csv");
                               
                               commandpromptcall(txtTargetFolder.Text + "\\STD_CSV\\GoogleEod\\Googleeod" + GoogleEod[i] + datetostore + ".csv", txtTargetFolder.Text + "\\STD_CSV\\Metastock\\" + GoogleEod[i] + datetostore + ".csv");

                            }
                            catch (Exception ex)
                            {
                                log4net.Config.XmlConfigurator.Configure();
                                ILog log = LogManager.GetLogger(typeof(MainWindow));
                                log.Debug(ex.Message);
                            }




                        }

                    }


                }

                if (Cb_MCX_Google_IEOD_5min.IsChecked == true)
                {

                    visit("http://list.shubhalabha.in/netgoogleieod.html");


                    prograss();
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\GoogleIeod5MIN";

                    if (!Directory.Exists(strYearDir))
                        Directory.CreateDirectory(strYearDir);
                    string[] GoogleIEod = new string[20];
                    //{ "LICHSGFIN.nse", "ADANIENT.nse", "ADANIPOWE.nse", "ADFFOODS.nse", "ADHUNIK.nse", "ADORWELD.nse", "ADSL.nse", "ADVANIHOT.nse", "ADVANTA.nse", "AEGISCHEM.nse", "AFL.nse", "AFTEK.nse", "AREVAT&D.nse", "M&M.nse", ".AEX,indexeuro", ".AORD,indexasx", ".HSI,indexhangseng", ",.N225,indexnikkei", ".NSEI,nse", ".NZ50,nze", ".TWII,tpe", "000001,sha", "CNX100,nse", "CNX500,nse", "CNXENERGY,nse", "CNXFMCG,nse", "CNXINFRA,nse", "CNXIT,nse" };

                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\GoogleIEOD.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                GoogleIEod[i] = line;
                                i++;

                            }
                        }

                    }
                    catch
                    {

                    }


                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();

                        for (int i = 0; i < 14; i++)
                        {
                            string dayforname = "";

                            if (day.Day <= 9)
                            {
                                dayforname = "0" + day.Day.ToString();
                            }
                            else
                            {
                                dayforname = day.Day.ToString();

                            }

                            strYearDir = txtTargetFolder.Text + "\\Downloads\\GoogleIeod5MIN\\" + dayforname + GoogleIEod[i] + ".csv";
                            baseurl = "http://www.google.com/finance/getprices?q=" + GoogleIEod[i] + "&x=NSE&i=300&p=" + Daysforgoogle.SelectedItem.ToString() + "d&f=d,o,h,l,c,v&df=cpct&auto=1&ts=1266701290218";

                            // "http://www.google.com/finance/getprices?q=LICHSGFIN&x=LICHSGFIN&i=60&p=15d&f=d,o,h,l,c,v"


                            downliaddata(strYearDir, baseurl);



                            try
                            {
                                string[] csvFileNames = new string[1] { "" };
                                csvFileNames[0] = txtTargetFolder.Text + "\\Downloads\\GoogleIeod5MIN\\" + dayforname + GoogleIEod[i] + ".csv";



                                string datetostore = "";
                                string monthtostore = "";
                                if (day.Month <= 9)
                                {
                                    monthtostore = "0" + day.Month.ToString();
                                }
                                else
                                {
                                    monthtostore = day.Month.ToString();

                                }

                                datetostore = day.Year.ToString() + monthtostore + dayforname;
                                log4net.Config.XmlConfigurator.Configure();
                                ILog log = LogManager.GetLogger(typeof(MainWindow));
                                log.Debug("Google File Processing strated....... ");
                                ExecuteYAHOOProcessing(csvFileNames, datetostore, "GOOGLEEOD5MIN",0);
                                log.Debug("Google File Processing End....... ");

                                if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                                }
                                if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\Google"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                                }
                                System.IO.File.Copy(csvFileNames[0], txtTargetFolder.Text + "\\STD_CSV\\GoogleIeod5MIN" + GoogleIEod[i] + ".csv");
                                //// JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\STD_CSV\\GoogleIeod" + GoogleIEod[i] + datetostore + ".csv");
                            }
                            catch (Exception ex)
                            {
                                log4net.Config.XmlConfigurator.Configure();
                                ILog log = LogManager.GetLogger(typeof(MainWindow));
                                log.Debug(ex.Message);
                            }





                        }

                    }


                }


                if (ChkGoogleIEOD.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netgoogleieod.html");

                    prograss();
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\GoogleIeod";

                    if (!Directory.Exists(strYearDir))
                        Directory.CreateDirectory(strYearDir);
                    string[] GoogleIEod = new string[20];
                    //{ "LICHSGFIN.nse", "ADANIENT.nse", "ADANIPOWE.nse", "ADFFOODS.nse", "ADHUNIK.nse", "ADORWELD.nse", "ADSL.nse", "ADVANIHOT.nse", "ADVANTA.nse", "AEGISCHEM.nse", "AFL.nse", "AFTEK.nse", "AREVAT&D.nse", "M&M.nse", ".AEX,indexeuro", ".AORD,indexasx", ".HSI,indexhangseng", ",.N225,indexnikkei", ".NSEI,nse", ".NZ50,nze", ".TWII,tpe", "000001,sha", "CNX100,nse", "CNX500,nse", "CNXENERGY,nse", "CNXFMCG,nse", "CNXINFRA,nse", "CNXIT,nse" };

                    try
                    {
                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\GoogleIEOD.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                GoogleIEod[i] = line;
                                i++;

                            }
                        }

                    }
                    catch
                    {

                    }


                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        string dayforname = "";

                        if (day.Day <= 9)
                        {
                            dayforname = "0" + day.Day.ToString();
                        }
                        else
                        {
                            dayforname = day.Day.ToString();

                        }

                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();

                        for (int i = 0; i < 14; i++)
                        {
                            strYearDir = txtTargetFolder.Text + "\\Downloads\\GoogleIeod\\" + dayforname + GoogleIEod[i] + ".csv";
                            baseurl = "http://www.google.com/finance/getprices?q=" + GoogleIEod[i] + "&x=NSE&i=60&p=" + Daysforgoogle.SelectedItem.ToString() + "d&f=d,o,h,l,c,v&df=cpct&auto=1&ts=1266701290218";

                            // "http://www.google.com/finance/getprices?q=LICHSGFIN&x=LICHSGFIN&i=60&p=15d&f=d,o,h,l,c,v"


                            downliaddata(strYearDir, baseurl);



                            try
                            {
                                string[] csvFileNames = new string[1] { "" };
                                csvFileNames[0] = txtTargetFolder.Text + "\\Downloads\\GoogleIeod\\" + dayforname + GoogleIEod[i] + ".csv";



                                string datetostore = "";
                                datetostore = DateTime.Today.ToString("yyyyMMdd");
                                log4net.Config.XmlConfigurator.Configure();
                                ILog log = LogManager.GetLogger(typeof(MainWindow));
                                log.Debug("Google File Processing strated....... ");
                                ExecuteYAHOOProcessing(csvFileNames, datetostore, "GOOGLEEOD",0);
                                log.Debug("Google File Processing End....... ");

                                if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                                }
                                if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\Google"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                                }
                                System.IO.File.Copy(csvFileNames[0], txtTargetFolder.Text + "\\STD_CSV\\googleeod1min_" + GoogleIEod[i] + ".csv");
                                //// JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\STD_CSV\\GoogleIeod" + GoogleIEod[i] + datetostore + ".csv");
                            }
                            catch (Exception ex)
                            {
                                log4net.Config.XmlConfigurator.Configure();
                                ILog log = LogManager.GetLogger(typeof(MainWindow));
                                log.Debug(ex.Message);
                            }





                        }

                    }


                }



                if (chkEquity.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netnseo.html");

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, year;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }

                        if (day.Month < 10)
                        {

                            date1 = date1 + "0" + day.Month.ToString();
                        }
                        else
                        {
                            date1 = date1 + day.Month.ToString();
                        }
                        year = day.Year.ToString();


                        string lastTwoChars = year.Substring(year.Length - 2);

                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + ".zip";

                        baseurl = "http://www.nseindia.com/archives/equities/bhavcopy/pr/PR" + date1 + lastTwoChars + ".zip";

                        //http://www.nseindia.com/archives/equities/bhavcopy/pr/PR160513.zip

                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);

                        }
                        if (System.IO.File.Exists(strYearDir))
                        {


                            using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                            {
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars))
                                {
                                    zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars);


                                }

                            }

                        }



                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\FO" + date1 + day.Year + ".zip";
                        try
                        {

                            using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                            {
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\FO" + date1 + lastTwoChars))
                                {
                                    zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\FO" + date1 + lastTwoChars);
                                }
                            }


                            strYearDir = txtTargetFolder.Text + " \\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\Fo" + date1 + lastTwoChars + "\\OP" + date1 + day.Year + ".csv";
                            string[] PRFO = new string[1] { "" };
                            PRFO[0] = strYearDir;

                            strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                            baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";

                            string sec = strYearDir;
                            if (!System.IO.File.Exists(strYearDir))
                            {
                                prograss();
                                downliaddata(strYearDir, baseurl);


                            }


                            ExecuteOPTIONProcessing(PRFO, "OP", txtTargetFolder.Text + "\\STD_CSV", sec);
                            filetransfer(PRFO[0], txtTargetFolder.Text + "\\STD_CSV\\NSE_Equity_Options_OP" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                            }
                            if (!Directory.Exists(txtTargetFolder.Text + "\\FCharts"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\FCharts");
                            }
                            if (!Directory.Exists(txtTargetFolder.Text + "\\AdvanceGet"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\AdvanceGet");
                            }
                            if (comboBox1.SelectedItem.ToString() == "Amibroker")
                            {

                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_Equity_Options_OP" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\Amibroker\\NSE_Equity_Options_OP" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                Amibroker(txtTargetFolder.Text + "\\Amibroker\\NSE_Equity_Options_OP" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }
                            if (comboBox1.SelectedItem.ToString() == "FCharts")
                            {

                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_Equity_Options_OP" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\FCharts\\NSE_Equity_Options_OP" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                Fchart(txtTargetFolder.Text + "\\FCharts\\NSE_Equity_Options_OP" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }
                            if (comboBox1.SelectedItem.ToString() == "AdvanceGet")
                            {

                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_Equity_Options_OP" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\AdvanceGet\\NSE_Equity_Options_OP" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                AdvanceGet(txtTargetFolder.Text + "\\AdvanceGet\\NSE_Equity_Options_OP" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }

                        }
                        catch
                        {
                        }





                    }
                }

                if (Cb_NSE_Forex_Options.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netnseo.html");

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, year;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }

                        if (day.Month < 10)
                        {

                            date1 = date1 + "0" + day.Month.ToString();
                        }
                        else
                        {
                            date1 = date1 + day.Month.ToString();
                        }
                        year = day.Year.ToString();

                        string lastTwoChars = year.Substring(year.Length - 2);

                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + ".zip";

                        baseurl = "http://www.nseindia.com/archives/equities/bhavcopy/pr/PR" + date1 + lastTwoChars + ".zip";

                        //http://www.nseindia.com/archives/equities/bhavcopy/pr/PR160513.zip
                        try
                        {
                            if (!Directory.Exists(strYearDir))
                            {
                                downliaddata(strYearDir, baseurl);

                            }



                            if (System.IO.File.Exists(strYearDir))
                            {


                                using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                                {
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars))
                                    {
                                        zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars);


                                    }

                                }

                            }


                            strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\cd" + date1 + day.Year + ".zip";




                            using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                            {
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\CD" + date1 + lastTwoChars))
                                {
                                    zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\CD" + date1 + lastTwoChars);
                                }
                            }


                            strYearDir = txtTargetFolder.Text + " \\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\CD" + date1 + lastTwoChars + "\\CO" + date1 + day.Year + ".csv";

                            string[] PRFO = new string[1] { "" };
                            PRFO[0] = strYearDir;

                            strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                            baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";

                            string sec = strYearDir;
                            if (!System.IO.File.Exists(strYearDir))
                            {
                                prograss();
                                downliaddata(strYearDir, baseurl);


                            }



                            ExecuteOPTIONProcessing(PRFO, "FO", txtTargetFolder.Text + "\\STD_CSV", sec);
                            filetransfer(PRFO[0], txtTargetFolder.Text + "\\STD_CSV\\NSE_FOREX_Option_CO" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                            if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                            }
                            if (!Directory.Exists(txtTargetFolder.Text + "\\FCharts"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\FCharts");
                            }
                            if (!Directory.Exists(txtTargetFolder.Text + "\\AdvanceGet"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\AdvanceGet");
                            }
                            if (comboBox1.SelectedItem.ToString() == "Amibroker")
                            {
                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_FOREX_Option_CO" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\Amibroker\\NSE_FOREX_Option_CO" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");


                                Amibroker(txtTargetFolder.Text + "\\Amibroker\\NSE_FOREX_Option_CO" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }
                            if (comboBox1.SelectedItem.ToString() == "FCharts")
                            {
                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_FOREX_Option_CO" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\FCharts\\NSE_FOREX_Option_CO" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");


                                Fchart(txtTargetFolder.Text + "\\FCharts\\NSE_FOREX_Option_CO" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }
                            if (comboBox1.SelectedItem.ToString() == "AdvanceGet")
                            {
                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_FOREX_Option_CO" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\AdvanceGet\\NSE_FOREX_Option_CO" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");


                                AdvanceGet(txtTargetFolder.Text + "\\AdvanceGet\\NSE_FOREX_Option_CO" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }

                        }
                        catch
                        {
                        }
                    }
                }

                if (chkNseForex.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netnsefo.html");

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, year;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }

                        if (day.Month < 10)
                        {

                            date1 = date1 + "0" + day.Month.ToString();
                        }
                        else
                        {
                            date1 = date1 + day.Month.ToString();
                        }
                        year = day.Year.ToString();

                        string lastTwoChars = year.Substring(year.Length - 2);


                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + ".zip";

                        baseurl = "http://www.nseindia.com/archives/equities/bhavcopy/pr/PR" + date1 + lastTwoChars + ".zip";

                        //http://www.nseindia.com/archives/equities/bhavcopy/pr/PR160513.zip

                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);

                        }




                        if (System.IO.File.Exists(strYearDir))
                        {


                            using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                            {
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars))
                                {
                                    zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars);


                                }

                            }

                        }



                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\cd" + date1 + day.Year + ".zip";
                        try
                        {

                            using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                            {
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\Cd" + date1 + lastTwoChars))
                                {
                                    zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\CD" + date1 + lastTwoChars);
                                }
                            }


                            strYearDir = txtTargetFolder.Text + " \\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\CD" + date1 + lastTwoChars + "\\Cf" + date1 + day.Year + ".csv";
                            string[] PRFO = new string[1] { "" };
                            PRFO[0] = strYearDir;

                            strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                            baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";

                            string sec = strYearDir;
                            if (!System.IO.File.Exists(strYearDir))
                            {
                                prograss();
                                downliaddata(strYearDir, baseurl);


                            }
                            string daytostore = "";
                            string monthtostore = "";

                            if (day.Day <= 9)
                            {
                                daytostore = "0" + day.Day.ToString();
                            }
                            else
                            {
                                daytostore = day.Day.ToString();

                            }
                            if (day.Month <= 9)
                            {
                                monthtostore = "0" + day.Month.ToString();
                            }
                            else
                            {
                                monthtostore = day.Month.ToString();

                            }

                            string datetostore1 = day.Year.ToString() + monthtostore + daytostore;
                            try
                            {

                                ExecuteFUTUREProcessing(PRFO, "CF", datetostore1, sec);
                                filetransfer(PRFO[0], txtTargetFolder.Text + "\\STD_CSV\\NSE_FOREX_Futures_CF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                                }
                                if (!Directory.Exists(txtTargetFolder.Text + "\\FCharts"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\FCharts");
                                }
                                if (!Directory.Exists(txtTargetFolder.Text + "\\AdvanceGet"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\AdvanceGet");
                                }

                                if (comboBox1.SelectedItem.ToString() == "Amibroker")
                                {

                                    System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_FOREX_Futures_CF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\Amibroker\\NSE_FOREX_Futures_CF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                    Amibroker(txtTargetFolder.Text + "\\Amibroker\\NSE_FOREX_Futures_CF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                }
                                if (comboBox1.SelectedItem.ToString() == "FCharts")
                                {

                                    System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_FOREX_Futures_CF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\FCharts\\NSE_FOREX_Futures_CF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                    Fchart(txtTargetFolder.Text + "\\FCharts\\NSE_FOREX_Futures_CF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                }
                                if (comboBox1.SelectedItem == "AdvanceGet")
                                {

                                    System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_FOREX_Futures_CF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\AdvanceGet\\NSE_FOREX_Futures_CF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                    AdvanceGet(txtTargetFolder.Text + "\\AdvanceGet\\NSE_FOREX_Futures_CF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                                }

                            }
                            catch
                            {
                            }
                        }
                        catch
                        {
                        }

                    }
                }


                if (Cb_NSE_SME.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netnsesme.html");


                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, date2, date3, year;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                            date3 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                            date3 = day.Day.ToString();

                        }

                        if (day.Month < 10)
                        {

                            date1 = date1 + "0" + day.Month.ToString();
                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date1 = date1 + day.Month.ToString();
                            date2 = day.Month.ToString();

                        }
                        year = day.Year.ToString();

                        string lastTwoChars = year.Substring(year.Length - 2);


                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + ".zip";

                        baseurl = "http://www.nseindia.com/archives/equities/bhavcopy/pr/PR" + date1 + lastTwoChars + ".zip";

                        //http://www.nseindia.com/archives/equities/bhavcopy/pr/PR160513.zip
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);

                        }

                        try
                        {

                            if (System.IO.File.Exists(strYearDir))
                            {


                                using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                                {
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars))
                                    {
                                        zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars);


                                    }

                                }

                            }

                            strYearDir = txtTargetFolder.Text + " \\Downloads\\PR" + date1 + lastTwoChars + "\\sme" + date1 + lastTwoChars + ".csv";
                            if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");

                            string[] smeetf = new string[1] { "" };
                            smeetf[0] = strYearDir;

                            strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                            baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";

                            string sec = strYearDir;
                            if (!System.IO.File.Exists(strYearDir))
                            {
                                prograss();
                                downliaddata(strYearDir, baseurl);


                            }


                            ExecuteSMEETFProcessing(smeetf, "SME_SME", txtTargetFolder.Text + "\\STD_CSV", sec);
                            filetransfer(smeetf[0], txtTargetFolder.Text + "\\STD_CSV\\NSE_SME" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                            }
                            if (!Directory.Exists(txtTargetFolder.Text + "\\FCharts"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\FCharts");
                            }
                            if (!Directory.Exists(txtTargetFolder.Text + "\\AdvanceGet"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\AdvanceGet");
                            }

                            if (comboBox1.SelectedItem == "Amibroker")
                            {

                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_SME" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\Amibroker\\NSE_SME" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                Amibroker(txtTargetFolder.Text + "\\Amibroker\\NSE_SME" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }
                            if (comboBox1.SelectedItem == "FCharts")
                            {

                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_SME" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\FCharts\\NSE_SME" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                Fchart(txtTargetFolder.Text + "\\FCharts\\NSE_SME" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }
                            if (comboBox1.SelectedItem == "AdvanceGet")
                            {

                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_SME" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\AdvanceGet\\NSE_SME" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");

                                AdvanceGet(txtTargetFolder.Text + "\\AdvanceGet\\NSE_SME" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }


                        }
                        catch
                        {

                        }

                    }
                }


                if (Cb_NSE_ETF.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netnseetf.html");

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, year;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }

                        if (day.Month < 10)
                        {

                            date1 = date1 + "0" + day.Month.ToString();
                        }
                        else
                        {
                            date1 = date1 + day.Month.ToString();
                        }
                        year = day.Year.ToString();

                        string lastTwoChars = year.Substring(year.Length - 2);

                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + ".zip";

                        baseurl = "http://www.nseindia.com/archives/equities/bhavcopy/pr/PR" + date1 + lastTwoChars + ".zip";

                        //http://www.nseindia.com/archives/equities/bhavcopy/pr/PR160513.zip
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);

                        }
                        try
                        {

                            if (System.IO.File.Exists(strYearDir))
                            {


                                using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                                {
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars))
                                    {
                                        zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars);


                                    }

                                }

                            }

                            strYearDir = txtTargetFolder.Text + " \\Downloads\\PR" + date1 + lastTwoChars + "\\etf" + date1 + lastTwoChars + ".csv";
                            if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");

                            string[] smeetf = new string[1] { "" };
                            smeetf[0] = strYearDir;
                            strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                            baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";

                            string sec = strYearDir;
                            if (!System.IO.File.Exists(strYearDir))
                            {
                                prograss();
                                downliaddata(strYearDir, baseurl);


                            }

                            ExecuteSMEETFProcessing(smeetf, "SME_ETF", txtTargetFolder.Text + "\\STD_CSV", sec);
                            filetransfer(smeetf[0], txtTargetFolder.Text + "\\STD_CSV\\NSE_ETF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                            }
                            if (!Directory.Exists(txtTargetFolder.Text + "\\FCharts"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\FCharts");
                            }
                            if (!Directory.Exists(txtTargetFolder.Text + "\\AdvanceGet"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\AdvanceGet");
                            }
                            if (comboBox1.SelectedItem == "Amibroker")
                            {
                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_ETF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\Amibroker\\NSE_ETF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");


                                Amibroker(txtTargetFolder.Text + "\\Amibroker\\NSE_ETF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }

                            if (comboBox1.SelectedItem == "FCharts")
                            {
                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_ETF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\FCharts\\NSE_ETF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");


                                Fchart(txtTargetFolder.Text + "\\FCharts\\NSE_ETF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }
                            if (comboBox1.SelectedItem == "AdvanceGet")
                            {
                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NSE_ETF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv", txtTargetFolder.Text + "\\AdvanceGet\\NSE_ETF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");


                                AdvanceGet(txtTargetFolder.Text + "\\AdvanceGet\\NSE_ETF" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + "bhav.csv");
                            }

                        }
                        catch
                        {

                        }

                    }
                }


                if (Cb_NSE_Market_Activity.IsChecked == true)
                {
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, year;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }

                        if (day.Month < 10)
                        {

                            date1 = date1 + "0" + day.Month.ToString();
                        }
                        else
                        {
                            date1 = date1 + day.Month.ToString();
                        }
                        year = day.Year.ToString();

                        string lastTwoChars = year.Substring(year.Length - 2);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\MA" + date1 + lastTwoChars + ".csv";

                        baseurl = "http://www.nseindia.com/archives/equities/mkt/MA" + date1 + lastTwoChars + ".csv";

                        //http://www.nseindia.com/archives/equities/mkt/MA160513.csv
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);
                            dest_filename = txtTargetFolder.Text + "\\STD_CSV\\NSE_MARKET_ACTIVITY.csv";

                            movefile(strYearDir, dest_filename);
                        }

                        string datetoprocess = year + date1;

                        if (System.IO.File.Exists(strYearDir))
                        {
                            NSEAD_Processing(strYearDir, dest_filename, datetoprocess);


                        }

                    }

                }

                if (Cb_NSE_events.IsChecked == true)
                {
                    downliaddata(txtTargetFolder.Text + "\\Reports\\CA_ALL_FORTHCOMING.csv", " http://www.nseindia.com/corporates/datafiles/CA_ALL_FORTHCOMING.csv");
                    downliaddata(txtTargetFolder.Text + "\\Reports\\BM_Latest_Announced.csv", " http://www.nseindia.com/corporates/datafiles/BM_Latest_Announced.csv");

                }

                if (Cb_NSE_Bulk_Deal.IsChecked == true)
                {

                    prograss();


                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string day1, month;


                        if (day.Day < 10)
                        {
                            day1 = "0" + day.Day.ToString() + "-";
                        }
                        else
                        {
                            day1 = day.Day.ToString() + "-";

                        }

                        if (day.Month < 10)
                        {

                            month = "0" + day.Month.ToString() + "-";
                        }
                        else
                        {
                            month = day.Month.ToString() + "-";

                        }


                        string date1 = day1 + month + day.Year;
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\" + date1 + "-TO-" + date1 + "_bulk.csv";

                        baseurl = "http://www.nseindia.com/content/equities/bulkdeals/datafiles/" + date1 + "-TO-" + date1 + "_bulk.csv";

                        // baseurl=" http://www.nseindia.com/content/equities/bulkdeals/datafiles/06-05-2013-TO-09-05-2013_bulk.csv";

                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);



                            dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_Bulk_Deal.csv";

                            movefile(strYearDir, dest_filename);

                        }

                    }

                }

                if (Cb_NSE_Block_Deal.IsChecked == true)
                {
                    prograss();


                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string day1, month;


                        if (day.Day < 10)
                        {
                            day1 = "0" + day.Day.ToString() + "-";
                        }
                        else
                        {
                            day1 = day.Day.ToString() + "-";

                        }

                        if (day.Month < 10)
                        {

                            month = "0" + day.Month.ToString() + "-";
                        }
                        else
                        {
                            month = day.Month.ToString() + "-";

                        }


                        string date1 = day1 + month + day.Year;
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\" + date1 + "-TO-" + date1 + "_block.csv";

                        baseurl = "http://www.nseindia.com/content/equities/bulkdeals/datafiles/" + date1 + "-TO-" + date1 + "_block.csv";

                        // baseurl=" http://www.nseindia.com/content/equities/bulkdeals/datafiles/09-05-2013-TO-09-05-2013_block.csv
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);


                            dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_Block_Deal.csv";

                            movefile(strYearDir, dest_filename);
                        }
                    }

                }


                if (Cb_NSE_India_Vix.IsChecked == true)
                {
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string day1;


                        if (day.Day < 10)
                        {
                            day1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            day1 = day.Day.ToString();

                        }

                        string date1 = day1 + "-" + strMonthName + "-" + day.Year;


                        strYearDir = txtTargetFolder.Text + "\\Downloads\\" + date1 + "_" + date1 + ".csv";
                        baseurl = "http://www.nseindia.com/content/vix/histdata/hist_india_vix_" + date1 + "_" + date1 + ".csv";

                        // baseurl=" http://www.nseindia.com/content/vix/histdata/hist_india_vix_06-May-2013_06-May-2013.csv
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);
                        }
                    }

                }
                if (MCXSX_Forex_Future.IsChecked == true)
                {

                    prograss();
                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string day1;


                        if (day.Day < 10)
                        {
                            day1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            day1 = day.Day.ToString();

                        }

                        string date1 = day1 + "-" + strMonthName + "-" + day.Year;


                        strYearDir = txtTargetFolder.Text + "\\Downloads\\currency" + date1 + ".xls";
                        baseurl = "http://www.mcx-sx.com/downloads/daily/CurrencyDownloads/Bhavcopy%20" + strMonthName + "%20" + day.Day + ",%20" + day.Year + ".xls";

                        // baseurl=" http://www.mcx-sx.com/downloads/daily/CurrencyDownloads/Bhavcopy%20June%207,%202013.xls
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);

                            string[] name1 = new string[1] { "" };
                            name1[0] = strYearDir;
                            try
                            {
                                ExecuteMCSSXFOREXProcessing(name1, day.Year + date1.ToString(), txtTargetFolder.Text);

                            }
                            catch
                            {
                            }
                        }
                    }

                }

                if (Cb_NSE_Vix.IsChecked == true)
                {
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string day1;


                        if (day.Day < 10)
                        {
                            day1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            day1 = day.Day.ToString();

                        }

                        string date1 = day1 + "-" + strMonthName + "-" + day.Year;


                        strYearDir = txtTargetFolder.Text + "\\Downloads\\" + date1 + "_" + date1 + ".csv";
                        baseurl = "http://www.nseindia.com/content/vix/histdata/hist_india_vix_" + date1 + "_" + date1 + ".csv";

                        // baseurl=" http://www.nseindia.com/content/vix/histdata/hist_india_vix_06-May-2013_06-May-2013.csv
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);
                        }
                    }

                }

                if (BSE_Delivary_Data.IsChecked == true)
                {
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string year, date1;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }


                        year = day.Year.ToString();

                        string lastTwoChars = year.Substring(year.Length - 2);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\SCBSEALL" + date1 + lastTwoChars + ".zip";
                        baseurl = "http://www.bseindia.com/BSEDATA/gross/" + day.Year + "/SCBSEALL" + date1 + lastTwoChars + ".zip";
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);
                        }
                    }

                }


                if (Cb_BSE_CASH_MARKET.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netbsecm.html");

                    BSE_Delivary_Data.IsChecked = true;

                    prograss();
                    List<string> nameofdirtodelete = new List<String>();


                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string day1, year, date1, date2;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                            day1 = "0" + day.Day.ToString(); //USe For SCBSEALL file

                        }
                        else
                        {
                            date1 = day.Day.ToString();
                            day1 = day.Day.ToString();

                        }

                        if (day.Month < 10)
                        {

                            date1 = date1 + "0" + day.Month.ToString();
                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date1 = date1 + day.Month.ToString();
                            date2 = day.Month.ToString();

                        }
                        year = day.Year.ToString();

                        string lastTwoChars = year.Substring(year.Length - 2);


                        strYearDir = txtTargetFolder.Text + "\\Downloads\\SCBSEALL" + date1 + lastTwoChars + ".zip";
                        baseurl = "http://www.bseindia.com/BSEDATA/gross/" + day.Year + "/SCBSEALL" + day1 + date2 + ".zip";
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);
                        }


                        string[] scball = new string[1] { "" };



                        if (System.IO.File.Exists(strYearDir))
                        {
                            try
                            {
                                using (var zip1 = Ionic.Zip.ZipFile.Read(strYearDir))
                                {
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\SCBSEALL" + date1 + lastTwoChars))
                                    {
                                        zip1.ExtractAll(txtTargetFolder.Text + "\\Downloads\\SCBSEALL" + date1 + lastTwoChars);
                                        strYearDir = txtTargetFolder.Text + "\\Downloads\\SCBSEALL" + date1 + lastTwoChars + "\\SCBSEALL" + date1 + ".txt";
                                        scball[0] = strYearDir;
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                System.Windows.MessageBox.Show(ex.Message);
                            }
                        }




                        //  http://www.bseindia.com/BSEDATA/gross/2012/SCBSEALL2311.zip

                        strYearDir = txtTargetFolder.Text + "\\Downloads\\eq" + date1 + lastTwoChars + "_csv.zip";
                        baseurl = " http://www.bseindia.com/download/BhavCopy/Equity/eq" + date1 + lastTwoChars + "_csv.zip";

                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);
                        }




                        try
                        {

                            if (!Directory.Exists(strYearDir))
                            {

                                using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                                {
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\eq" + date1 + lastTwoChars + "_csv"))
                                    {
                                        zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\eq" + date1 + lastTwoChars + "_csv");
                                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Eq" + date1 + lastTwoChars + "_csv\\Eq" + date1 + lastTwoChars + ".csv";
                                    }
                                }


                                string[] strbse = new string[1] { "" };
                                strbse[0] = strYearDir;

                                try
                                {
                                    ExecuteBSEEQUITYProcessing(strbse, scball, "SDTCSV", txtTargetFolder.Text + "\\");
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                                    {
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                                    }
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\FCharts"))
                                    {
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\FCharts");
                                    }
                                    if (!Directory.Exists(txtTargetFolder.Text + "\\AdvanceGet"))
                                    {
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\AdvanceGet");
                                    }
                                     if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV\\Metastock"))
                                    {
                                        Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV\\Metastock");
                                    }
                                    
                                    filetransfer(strbse[0], txtTargetFolder.Text + "\\STD_CSV\\Bse_Cash_Market" + date1 + day.Year + "bhav.csv");
                                    if (comboBox1.SelectedItem == "Metastock")
                                    {

                                    commandpromptcall(txtTargetFolder.Text + "\\STD_CSV\\Bse_Cash_Market" + date1 + day.Year + "bhav.csv",txtTargetFolder.Text + "\\STD_CSV\\Metastock\\Bse_Cash_Market" + date1 + day.Year + "bhav");
                                    }
                                    if (comboBox1.SelectedItem == "Amibroker")
                                    {

                                        System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\Bse_Cash_Market" + date1 + day.Year + "bhav.csv", txtTargetFolder.Text + "\\Amibroker\\Bse_Cash_Market" + date1 + day.Year + "bhav.csv");

                                        Amibroker(txtTargetFolder.Text + "\\Amibroker\\Bse_Cash_Market" + date1 + day.Year + "bhav.csv");
                                    }
                                    if (comboBox1.SelectedItem == "FCharts")
                                    {

                                        System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\Bse_Cash_Market" + date1 + day.Year + "bhav.csv", txtTargetFolder.Text + "\\FCharts\\Bse_Cash_Market" + date1 + day.Year + "bhav.csv");

                                        Fchart(txtTargetFolder.Text + "\\FCharts\\Bse_Cash_Market" + date1 + day.Year + "bhav.csv");
                                    }
                                    if (comboBox1.SelectedItem == "AdvanceGet")
                                    {

                                        System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\Bse_Cash_Market" + date1 + day.Year + "bhav.csv", txtTargetFolder.Text + "\\AdvanceGet\\Bse_Cash_Market" + date1 + day.Year + "bhav.csv");

                                        AdvanceGet(txtTargetFolder.Text + "\\AdvanceGet\\Bse_Cash_Market" + date1 + day.Year + "bhav.csv");
                                    }

                                }
                                catch
                                {
                                }
                            }

                        }
                        catch
                        {
                        }

                        if (Directory.Exists(txtTargetFolder.Text + "\\Downloads\\Eq" + date1 + lastTwoChars + "_csv"))
                        {
                            Directory.Delete(txtTargetFolder.Text + "\\Downloads\\Eq" + date1 + lastTwoChars + "_csv", true);

                        }
                    }


                }



                if (Cb_BSE_Equity_Futures.IsChecked == true)
                {

                    prograss();


                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string  year, date1, date2, datetoselect;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        year = day.Year.ToString();
                        string lastTwoChars = year.Substring(year.Length - 2);
                        datetoselect = date1 + "-" + date2 + "-" + lastTwoChars;

                        strYearDir = txtTargetFolder.Text + "\\Downloads\\bhavcopy" + datetoselect + ".zip";
                        baseurl = "http://www.bseindia.com/download/Bhavcopy/Derivative/bhavcopy" + datetoselect + ".zip";
                        //http://www.bseindia.com/download/Bhavcopy/Derivative/bhavcopy23-11-12.zip
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);
                        }
                    }

                }

                if (Cb_Reports.IsChecked == true)
                {
                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, date2, year;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }

                        if (day.Month < 10)
                        {
                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        year = day.Year.ToString();

                        string lastTwoChars = year.Substring(year.Length - 2);
                        strYearDir = txtTargetFolder.Text + "\\Reports\\fii_stats_" + date1 + "-" + strMonthName.Substring(0, 3) + "-" + day.Year + ".csv";

                        baseurl = "http://www.nseindia.com/content/fo/fii_stats_" + date1 + "-" + strMonthName.Substring(0, 3) + "-" + day.Year + ".xls";

                        //http://www.nseindia.com/content/fo/fii_stats_23-Nov-2012.xls [^]

                        downliaddata(strYearDir, baseurl);


                        strYearDir = txtTargetFolder.Text + "\\Downloads\\fao_participant_oi" + date1 + date2 + day.Year + ".csv";

                        baseurl = "http://www.nseindia.com/content/nsccl/fao_participant_oi_" + date1 + date2 + day.Year + ".csv";
                        // http://www.nseindia.com/content/nsccl/fao_participant_oi_22112012.csv
                        downliaddata(strYearDir, baseurl);


                        string destination = txtTargetFolder.Text + "\\Reports\\NSE_fao_participant_oi_reports.csv";

                        movefile(strYearDir, destination);


                        strYearDir = txtTargetFolder.Text + "\\Downloads\\fao_participant_vol" + date1 + date2 + day.Year + ".csv";

                        baseurl = "http://www.nseindia.com/content/nsccl/fao_participant_vol_" + date1 + date2 + day.Year + ".csv";
                        //http://www.nseindia.com/content/nsccl/fao_participant_vol_22112012.csv 
                        downliaddata(strYearDir, baseurl);

                        destination = txtTargetFolder.Text + "\\Reports\\NSE_fao_participant_vol_reports.csv";

                        movefile(strYearDir, destination);


                    }
                }

                if (BSE_Block.IsChecked == true)
                {
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string  year, date1, date2, datetoselect;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        year = day.Year.ToString();
                        string lastTwoChars = year.Substring(year.Length - 2);
                        datetoselect = date1 + "-" + date2 + "-" + lastTwoChars;
                        strMonthName = strMonthName.Substring(0, 3);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\BSEBlock";

                        if (!Directory.Exists(strYearDir))
                            Directory.CreateDirectory(strYearDir);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\BSEBlock\\Block_" + day.Day + ".csv";
                        baseurl = "http://www.bseindia.com/stockinfo/BulkBlockFiles/Block_" + date1 + strMonthName + day.Year + ".csv";
                        //http://www.bseindia.com/stockinfo/BulkBlockFiles/Block_26Dec2012.csv
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);

                            string[] csvFileNames = Directory.GetFiles(txtTargetFolder.Text + "\\Downloads\\BSEBlock", "*.csv");

                            JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\Reports\\bseblockdeals.csv");



                            dest_filename = txtTargetFolder.Text + "\\Reports\\bseblockdeals.csv";
                        }

                        // movefile(strYearDir, dest_filename);

                    }


                }

                if (BSE_Bulk.IsChecked == true)
                {
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string  year, date1, date2, datetoselect;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        year = day.Year.ToString();
                        string lastTwoChars = year.Substring(year.Length - 2);
                        datetoselect = date1 + "-" + date2 + "-" + lastTwoChars;
                        strMonthName = strMonthName.Substring(0, 3);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\BSEBulk";

                        if (!Directory.Exists(strYearDir))
                            Directory.CreateDirectory(strYearDir);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\BSEBulk\\Bulk_" + day.Day + ".csv";
                        baseurl = "http://www.bseindia.com/stockinfo/BulkBlockFiles/Bulk_" + date1 + strMonthName + day.Year + ".csv";
                        //http://www.bseindia.com/stockinfo/BulkBlockFiles/Block_26Dec2012.csv
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);

                            string[] csvFileNames = Directory.GetFiles(txtTargetFolder.Text + "\\Downloads\\BSEBulk", "*.csv");

                            JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\Reports\\bsebulkdeals.csv");

                            dest_filename = txtTargetFolder.Text + "\\Reports\\bsebulkdeals.csv";


                            // movefile(strYearDir, dest_filename);
                        }

                    }



                }

                if (BSE_Index.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netindex.html");

                    prograss();
                    prograss();



                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string  year, date1, date2, datetoselect;
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse";
                        if (!Directory.Exists(strYearDir))
                            Directory.CreateDirectory(strYearDir);

                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }
                        nameofbseindex.Clear();
                        year = day.Year.ToString();
                        string lastTwoChars = year.Substring(year.Length - 2);
                        datetoselect = date2 + "/" + date1 + "/" + day.Year;
                        filename = day.Day.ToString();
                        //BSE30
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSE30.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSE30%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BSE30");
                        //MIDCAP
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\MIDCAP.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=MIDCAP%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("MIDCAP");

                        //SMLCAP
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\SMLCAP.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=SMLCAP%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("SMLCAP");

                        //BSE100
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSE100.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSE100%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BSE100");

                        //BSE200
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSE200.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSE200%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BSE200");


                        //BSE500
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSE500.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSE500%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BSE500");




                        //AUTO
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\AUTO.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=AUTO%20%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("AUTO");

                        //BANKEX
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BANKEX.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BANKEX%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BANKEX");

                        //BSECD
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSECD.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSECD%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BSECD");

                        //BSECG
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSECG.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSECG%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BSECG");


                        //BSEFMCG
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSEFMCG.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSEFMCG&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BSEFMCG");

                        //BSEHC
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSEHC.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSEHC%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BSEHC");


                        //BSEIT
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSEIT.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSEIT%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BSEHC");

                        //METAL
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\METAL.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=METAL%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("METAL");

                        //OILGAS
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\OILGAS.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=OILGAS%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("OILGAS");


                        //POWER
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\POWER.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=POWER%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("POWER");

                        //BSEPSU
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSEPSU.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSEPSU%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BSEPSU");

                        //REALTY
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\REALTY.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=REALTY%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("REALTY");

                        //TECK
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\TECK.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=TECK%20%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("TECK");


                        //DOL
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\DOL.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=DOL30%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("DOL");

                        //DOL100
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\DOL100.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=DOL100%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("DOL100");

                        //DOL200
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\DOL200.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=DOL200%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("DOL200");

                        //SHA50
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\SHA50.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=SHA50%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("SHA50");

                        //GREENX
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\GREENX.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=GREENX%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("GREENX");

                        //BSEIPO
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSEIPO.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSEIPO%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("BSEIPO");

                        //CARBON
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\CARBON.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=CARBON%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("CARBON");

                        //SMEIPO
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\SMEIPO.csv";
                        baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=SMEIPO%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                        downliaddata(strYearDir, baseurl);
                        nameofbseindex.Add("SMEIPO");

                        strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                        baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";

                        string sec = strYearDir;
                        if (!System.IO.File.Exists(strYearDir))
                        {
                            prograss();
                            downliaddata(strYearDir, baseurl);


                        }
                        string secname = strYearDir;
                        string[] csvFileNames = Directory.GetFiles(txtTargetFolder.Text + "\\Downloads\\bse", "*.csv");
                        if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                            Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                        string daytostore = "";
                        string monthtostore = "";

                        if (day.Day <= 9)
                        {
                            daytostore = "0" + day.Day.ToString();
                        }
                        else
                        {
                            daytostore = day.Day.ToString();

                        }
                        if (day.Month <= 9)
                        {
                            monthtostore = "0" + day.Month.ToString();
                        }
                        else
                        {
                            monthtostore = day.Month.ToString();

                        }

                        string datetostore1 = day.Year.ToString() + monthtostore + daytostore;
                        try
                        {

                            JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\Downloads\\BSE_INDICES_BSEIndex" + day.Day + day.Month + day.Year + ".csv");
                            string[] bsefilename = new string[1] { "" };
                            bsefilename[0] = txtTargetFolder.Text + "\\Downloads\\BSE_INDICES_BSEIndex" + day.Day + day.Month + day.Year + ".csv";
                            ExecuteINDEXProcessing(bsefilename, "BSEINDEX", day.Year + monthtostore + daytostore, secname);
                            filetransfer(bsefilename[0], txtTargetFolder.Text + "\\STD_CSV\\BSE_INDICES_BSEIndex" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");
                            if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                            }
                            if (!Directory.Exists(txtTargetFolder.Text + "\\FCharts"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\FCharts");
                            }
                            if (!Directory.Exists(txtTargetFolder.Text + "\\AdvanceGet"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\AdvanceGet");
                            }

                            if (comboBox1.SelectedItem == "Amibroker")
                            {
                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\BSE_INDICES_BSEIndex" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv", txtTargetFolder.Text + "\\Amibroker\\BSE_INDICES_BSEIndex" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");


                                Amibroker(txtTargetFolder.Text + "\\Amibroker\\BSE_INDICES_BSEIndex" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");
                            }
                            if (comboBox1.SelectedItem == "FCharts")
                            {
                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\BSE_INDICES_BSEIndex" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv", txtTargetFolder.Text + "\\FCharts\\BSE_INDICES_BSEIndex" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");


                                Fchart(txtTargetFolder.Text + "\\FCharts\\BSE_INDICES_BSEIndex" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");
                            }
                            if (comboBox1.SelectedItem == "AdvanceGet")
                            {
                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\BSE_INDICES_BSEIndex" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv", txtTargetFolder.Text + "\\AdvanceGet\\BSE_INDICES_BSEIndex" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");


                                AdvanceGet(txtTargetFolder.Text + "\\AdvanceGet\\BSE_INDICES_BSEIndex" + day.Day + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");
                            }

                        }
                        catch
                        {
                        }

                    }



                }

                if (National_Spot_Exchange.IsChecked == true)
                {
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string  year, date1, date2, datetoselect;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        year = day.Year.ToString();
                        string lastTwoChars = year.Substring(year.Length - 2);
                        datetoselect = date2 + date1 + day.Year;
                        strMonthName = strMonthName.Substring(0, 3);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\NSEL_" + day.Day + ".csv";
                        baseurl = "http://www.nationalspotexchange.com//NSELBhavCopyFiles///25052013//hdy2zs5511tyhiunba5ybyjt//NSEL_" + datetoselect + ".csv";
                        //http://www.nationalspotexchange.com//NSELBhavCopyFiles///25052013//hdy2zs5511tyhiunba5ybyjt//NSEL_05242013.csv
                        if (!Directory.Exists(strYearDir))
                        {
                            //downliaddata(strYearDir, baseurl);





                            //try
                            //{
                            //    prograss();
                            //    //If Data is Not Present For Date Then  Exception Occure And It Get Added Into List Box  
                            //    // Client.DownloadFile("http://www.mcx-sx.com/downloads/daily/EquityDownloads/Market%20Statistics%20Report_" + date1 + ".csv.", File_path);

                            //    log4net.Config.XmlConfigurator.Configure();
                            //    ILog log = LogManager.GetLogger(typeof(MainWindow));
                            //    log.Debug(baseurl + "Download Started at " + DateTime.Now.ToString("HH:mm:ss tt"));

                            //    Client.Headers.Add("Accept", "application/zip");
                            //    Client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                            //    Client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1");
                            //    Client.DownloadFile(baseurl, strYearDir);


                            //    log.Debug(baseurl + "Download Completed at " + DateTime.Now.ToString("HH:mm:ss tt"));

                            //    //string clientHeader = "DATE" + "," + "TICKER" + " " + "," + "NAME" + "," + " " + "," + " " + "," + "OPEN" + "," + "HIGH" + "," + "LOW" + "," + "CLOSE" + "," + "VOLUME" + "," + "OPENINT" + Environment.NewLine;

                            //    //Format_Header(File_path, clientHeader);
                            //}
                            //catch (Exception ex)
                            //{
                            //    if ((ex.ToString().Contains("404")) || (ex.ToString().Contains("400")))
                            //    {
                            //        log4net.Config.XmlConfigurator.Configure();
                            //        ILog log = LogManager.GetLogger(typeof(MainWindow));
                            //        log.Warn("Data Not Found For " + url);

                            //    }
                            //}

                        }
                    }


                }

                if (chkNseNcdex.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netncdex.html");

                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string  year, date1, date2, datetoselect;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        year = day.Year.ToString();
                        string lastTwoChars = year.Substring(year.Length - 2);
                        datetoselect = date2 + date1 + day.Year;
                        strMonthName = strMonthName.Substring(0, 3);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\NCDEX_" + day.Day + ".csv";
                        baseurl = "http://www.ncdex.com/Downloads/Bhavcopy_Summary_File/Export_csv/" + date2 + "-" + date1 + "-" + day.Year + ".csv";
                        //http://www.ncdex.com/Downloads/Bhavcopy_Summary_File/Export_csv/11-23-2012.csv
                        string dest = txtTargetFolder.Text + "\\STD_CSV\\NCDEX_" + date2 + "-" + date1 + "-" + day.Year + ".csv";
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);

                            string[] strbse = new string[1] { "" };
                            strbse[0] = strYearDir;
                            string daytostore = "";
                            string monthtostore = "";

                            if (day.Day <= 9)
                            {
                                daytostore = "0" + day.Day.ToString();
                            }
                            else
                            {
                                daytostore = day.Day.ToString();

                            }
                            if (day.Month <= 9)
                            {
                                monthtostore = "0" + day.Month.ToString();
                            }
                            else
                            {
                                monthtostore = day.Month.ToString();

                            }

                            string datetostore1 = day.Year.ToString() + monthtostore + daytostore;

                            try
                            {
                                NCDEX_Processing(strbse, datetostore1, txtTargetFolder.Text + "\\");
                                filetransfer(strbse[0], txtTargetFolder.Text + "\\STD_CSV\\NCDEX_MARKET" + datetostore1 + ".csv");
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                                }
                                if (!Directory.Exists(txtTargetFolder.Text + "\\FCharts"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\FCharts");
                                }

                                //if (comboBox1.SelectedItem == "Amibroker")
                                //{
                                //    System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NCDEX_MARKET" + datetostore + ".csv", txtTargetFolder.Text + "\\Amibroker\\NCDEX_MARKET" + datetostore + ".csv");


                                //    Amibroker(txtTargetFolder.Text + "\\Amibroker\\NCDEX_MARKET" + datetostore + ".csv");
                                //}
                                //if (comboBox1.SelectedItem == "FCharts")
                                //{
                                //    System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\NCDEX_MARKET" + datetostore + ".csv", txtTargetFolder.Text + "\\FCharts\\NCDEX_MARKET" + datetostore + ".csv");


                                //    Fchart(txtTargetFolder.Text + "\\FCharts\\NCDEX_MARKET" +datetostore+ ".csv");
                                //}

                            }
                            catch
                            {
                            }
                        }





                    }


                }


                if (MCXSX_Equity_Futures.IsChecked == true)
                {
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string  year, date1, date2, datetoselect;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        year = day.Year.ToString();
                        string lastTwoChars = year.Substring(year.Length - 2);
                        datetoselect = date1 + date2 + day.Year;
                        strMonthName = strMonthName.Substring(0, 3);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\MarketStatisticsReport" + day.Day + ".csv";
                        baseurl = "http://www.mcx-sx.com/downloads/daily/EquityDownloads/Market%20Statistics%20Report_" + datetoselect + ".csv";
                        //http://www.mcx-sx.com/downloads/daily/EquityDownloads/Market%20Statistics%20Report_15042013.csv.

                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);


                            dest_filename = txtTargetFolder.Text + "\\Reports\\MarketStatisticsReport" + day.Day + ".csv";

                            movefile(strYearDir, dest_filename);

                        }

                        //process 
                        if (System.IO.File.Exists(strYearDir))
                        {
                            string[] mcxsx = new string[1] { "" };
                            mcxsx[0] = strYearDir;
                            try
                            {
                                ExecuteMCSSXProcessing(mcxsx, day.Year + date2 + date1.ToString(), txtTargetFolder.Text);
                                filetransfer(mcxsx[0], txtTargetFolder.Text + "\\STD_CSV\\MCX_Equity_FUTURE_" + date1 + strMonthName.Substring(0, 3).ToUpper() + day.Year + ".csv");
                            }
                            catch
                            {

                            }

                        }

                    }


                }

                if (MCXSX_Currency.IsChecked == true)
                {
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string  year, date1, date2, datetoselect;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        year = day.Year.ToString();
                        string lastTwoChars = year.Substring(year.Length - 2);
                        datetoselect = date1 + date2 + day.Year;
                        strMonthName = strMonthName.Substring(0, 3);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Currency_MarketStatisticsReport" + day.Day + ".xls";
                        baseurl = "http://www.mcx-sx.com/downloads/daily/CurrencyDownloads/Bhavcopy%20" + strMonthName + "%20" + date1 + ",%20" + day.Year + ".xls";
                        //http://www.mcx-sx.com/downloads/daily/CurrencyDownloads/Bhavcopy%20May%2016,%202013.xls.
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);
                        }

                    }


                }


                if (MCXSX_Block.IsChecked == true)
                {
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string  year, date1, date2, datetoselect;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        year = day.Year.ToString();
                        string lastTwoChars = year.Substring(year.Length - 2);
                        datetoselect = day.Year + date2 + date1;
                        strMonthName = strMonthName.Substring(0, 3);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\MCX-SX-EQ_BLOCK_DEAL.csv";
                        baseurl = "http://www.mcx-sx.com/downloads/daily/EquityDownloads/MCX-SX-EQ_BLOCK_DEAL_" + datetoselect + ".csv";
                        //http://www.mcx-sx.com/downloads/daily/EquityDownloads/MCX-SX-EQ_BLOCK_DEAL_20130213.csv.
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);

                            dest_filename = txtTargetFolder.Text + "\\Reports\\MCX-SX-EQ_BLOCK_DEAL.csv";

                            movefile(strYearDir, dest_filename);
                        }
                    }


                }

                if (MCXSX_Bulk.IsChecked == true)
                {
                    prograss();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string  year, date1, date2, datetoselect;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        year = day.Year.ToString();
                        string lastTwoChars = year.Substring(year.Length - 2);
                        datetoselect = day.Year + date2 + date1;
                        strMonthName = strMonthName.Substring(0, 3);
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\MCX-SX-EQ_BULK_DEAL.csv";
                        baseurl = "http://www.mcx-sx.com/downloads/daily/EquityDownloads/MCX-SX-EQ_BULK_DEAL_" + datetoselect + ".csv";
                        //http://www.mcx-sx.com/downloads/daily/EquityDownloads/MCX-SX-EQ_BULK_DEAL_20130502.csv.
                        if (!Directory.Exists(strYearDir))
                        {
                            downliaddata(strYearDir, baseurl);


                            dest_filename = txtTargetFolder.Text + "\\Reports\\MCX-SX-EQ_BULK_DEAL.csv";

                            movefile(strYearDir, dest_filename);
                        }
                    }


                }



                if (MCXCommodity_Futures.IsChecked == true)
                {
                    WebClient webClient = new WebClient();

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {


                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string   date1, date2, datetoselect;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }

                        //*********************************************************

                        byte[] b = webClient.DownloadData("http://www.mcxindia.com/sitepages/BhavCopyDatewise.aspx");

                        string s = System.Text.Encoding.UTF8.GetString(b);
                        var __EVENTVALIDATION = ExtractVariable(s, "__EVENTVALIDATION");
                        //__EVENTVALIDATION.Dump();
                        var forms = new NameValueCollection();
                        // forms["__EVENTTARGET"] = "btnLink_Excel";
                        forms["__EVENTARGUMENT"] = "";
                        forms["__VIEWSTATE"] = ExtractVariable(s, "__VIEWSTATE");
                        forms["mTbdate"] = date2 + "/" + date1 + "/" + day.Year;
                        forms["__EVENTVALIDATION"] = __EVENTVALIDATION;
                        forms["mImgBtnGo.x"] = "13";
                        forms["mImgBtnGo.y"] = "6";
                        // forms["ScriptManager1"] = "MupdPnl|mImgBtnGo";


                        webClient.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                        var responseData = webClient.UploadValues(@"http://www.mcxindia.com/sitepages/BhavCopyDatewise.aspx", "POST", forms);



                        s = System.Text.Encoding.UTF8.GetString(responseData);
                        __EVENTVALIDATION = ExtractVariable(s, "__EVENTVALIDATION");

                        forms = new NameValueCollection();
                        forms["__EVENTTARGET"] = "btnLink_Excel";
                        forms["__EVENTARGUMENT"] = "";
                        forms["__VIEWSTATE"] = ExtractVariable(s, "__VIEWSTATE");
                        forms["mTbdate"] = date2 + "/" + date1 + "/" + day.Year; ;
                        forms["__EVENTVALIDATION"] = __EVENTVALIDATION;


                        webClient.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                        responseData = webClient.UploadValues(@"http://www.mcxindia.com/sitepages/BhavCopyDatewise.aspx", "POST", forms);

                        System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\" + day.Day + day.Month + day.Year + "ComodityBhavCopy.csv", responseData);





                        //process 
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\" + day.Day + day.Month + day.Year + "ComodityBhavCopy.csv";
                        if (System.IO.File.Exists(strYearDir))
                        {

                            string destfilepath = txtTargetFolder.Text + "\\Downloads\\Temp_FUTURE_STD.csv";
                            string dateformtoprocessingsave = formatdate(day);
                            //try
                            //{

                                string[] mcxbhavname = new string[1] { "" };
                                mcxbhavname[0] = strYearDir;

                                strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                                baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";

                                string sec = strYearDir;
                                if (!System.IO.File.Exists(strYearDir))
                                {
                                    prograss();
                                    downliaddata(strYearDir, baseurl);


                                }
                                datetoselect = day.Year + date2 + date1;
                                ExecuteFUTUREProcessing(mcxbhavname, "MCXBHAV", datetoselect, sec);
                                string mcxname = mcxbhavname[0];
                                filetransfer(mcxname, txtTargetFolder.Text + "\\STD_CSV\\Mcx_Com_MCX_" + datetoselect + "bhav.csv");

                                if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                                }
                                System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\Mcx_Com_MCX_" + datetoselect + "bhav.csv", txtTargetFolder.Text + "\\Amibroker\\Mcx_Com_MCX_" + datetoselect + "bhav.csv");

                                if (comboBox1.SelectedItem == "Amibroker")
                                {


                                    Amibroker(txtTargetFolder.Text + "\\Amibroker\\Mcx_Com_MCX_" + datetoselect + "bhav.csv");
                                }

                            //}
                            //catch
                            //{

                            //}


                        }


                    }
                }


                if (MCX_Index.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netindex.html");

                    MCXSX_Spot_Indices.IsChecked = true;
                    WebClient webClient = new WebClient();
                    string[] arrIndexValues = new string[] { "323", "324", "325", "326" };
                    string[] arrindexvaluesname = new string[] { "MCXCOMDEX", "MCXMETAL", "MCXENRGY", "MCXAGRI" };
                    string[] arrSpotIndexValues = new string[] { "327", "328", "329", "330" };


                    string datetoselect = "";
                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {

                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, date2;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            byte[] b = webClient.DownloadData("http://www.mcxindia.com/sitepages/indexhistory.aspx");

                            string s = System.Text.Encoding.UTF8.GetString(b);
                            var __EVENTVALIDATION = ExtractVariable(s, "__EVENTVALIDATION");
                            //__EVENTVALIDATION.Dump();
                            var forms = new NameValueCollection();
                            //  forms["__EVENTTARGET"] = "btnLink_Excel";
                            forms["__EVENTARGUMENT"] = "";
                            forms["__VIEWSTATE"] = ExtractVariable(s, "__VIEWSTATE");

                            forms["__EVENTVALIDATION"] = __EVENTVALIDATION;
                            forms["mDdlOtherIndex"] = arrIndexValues[i];
                            forms["mRbtLstSpotFut_0"] = "1";
                            forms["mTbFromDate"] = date2 + "/" + date1 + "/" + day.Year;

                            forms["mTbToDate"] = date2 + "/" + date1 + "/" + day.Year; ;


                            forms["mBtnGo.x"] = "130";
                            forms["mBtnGo.y"] = "40";

                            // forms["ScriptManager1"] = "MupdPnl|mImgBtnGo";


                            webClient.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                            var responseData = webClient.UploadValues(@"http://www.mcxindia.com/sitepages/indexhistory.aspx", "POST", forms);

                            // System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\index.html", responseData);


                            s = System.Text.Encoding.UTF8.GetString(responseData);
                            __EVENTVALIDATION = ExtractVariable(s, "__EVENTVALIDATION");

                            forms = new NameValueCollection();
                            forms["__EVENTTARGET"] = "linkButton";
                            forms["__EVENTARGUMENT"] = "";
                            forms["__VIEWSTATE"] = ExtractVariable(s, "__VIEWSTATE");

                            forms["__EVENTVALIDATION"] = __EVENTVALIDATION;
                            forms["mDdlOtherIndex"] = arrIndexValues[i];
                            forms["mRbtLstSpotFut_0"] = "1";
                            forms["mTbFromDate"] = "05/07/2013";
                            forms["mTbToDate"] = "05/07/2013";



                            webClient.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                            responseData = webClient.UploadValues(@"http://www.mcxindia.com/sitepages/indexhistory.aspx", "POST", forms);
                            if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\MCX_INDEX"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\Downloads\\MCX_INDEX");
                            }
                            System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\MCX_INDEX\\" + arrindexvaluesname[i] + "_" + day.Day + ".csv", responseData);
                            datetoselect = day.Year + date2 + date1.ToString();
                        }


                        string[] csvFileNames = Directory.GetFiles(txtTargetFolder.Text + "\\Downloads\\MCX_INDEX", "*.csv");


                        try
                        {
                            ExecuteINDEXProcessing(csvFileNames, "MCXINDEX", datetoselect, "SEC");


                            JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\Downloads\\MCXINDEX" + datetoselect + ".csv");
                            filetransfer(txtTargetFolder.Text + "\\Downloads\\MCXINDEX" + datetoselect + ".csv", txtTargetFolder.Text + "\\STD_CSV\\MCX_INDEX_" + datetoselect + ".csv");

                            Deletspace(txtTargetFolder.Text + "\\STD_CSV\\MCX_INDEX_" + datetoselect + ".csv");

                            if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");
                            }
                            System.IO.File.Copy(txtTargetFolder.Text + "\\STD_CSV\\MCX_INDEX_" + datetoselect + ".csv", txtTargetFolder.Text + "\\Amibroker\\MCX_INDEX_" + datetoselect + ".csv");

                            if (comboBox1.SelectedItem == "Amibroker")
                            {


                                Amibroker(txtTargetFolder.Text + "\\Amibroker\\MCX_INDEX_" + datetoselect + ".csv");
                            }
                            if (Directory.Exists(txtTargetFolder.Text + "\\Downloads\\MCX_INDEX"))
                            {
                                Directory.Delete(txtTargetFolder.Text + "\\Downloads\\MCX_INDEX", true);
                            }
                        }
                        catch 
                        {
                        }

                    }

                }








                if (MCXSX_Spot_Indices.IsChecked == true)
                {
                    visit("http://list.shubhalabha.in/netindices.html");

                    WebClient webClient = new WebClient();
                    string[] arrIndexValues = new string[] { "323", "324", "325", "326" };
                    string[] arrindexvaluesname = new string[] { "Spot_MCXCOMDEX", "Spot_MCXMETAL", "Spot_MCXENRGY", "Spot_MCXAGRI" };
                    string[] arrSpotIndexValues = new string[] { "327", "328", "329", "330" };

                    string datetoselect = "";

                    foreach (DateTime day in EachDay(StartDate, EndDate))
                    {

                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(day.Month).ToString();
                        string date1, date2;


                        if (day.Day < 10)
                        {
                            date1 = "0" + day.Day.ToString();
                        }
                        else
                        {
                            date1 = day.Day.ToString();
                        }
                        if (day.Month < 10)
                        {

                            date2 = "0" + day.Month.ToString();
                        }
                        else
                        {
                            date2 = day.Month.ToString();
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            byte[] b = webClient.DownloadData("http://www.mcxindia.com/sitepages/indexhistory.aspx");

                            string s = System.Text.Encoding.UTF8.GetString(b);
                            var __EVENTVALIDATION = ExtractVariable(s, "__EVENTVALIDATION");
                            //__EVENTVALIDATION.Dump();
                            var forms = new NameValueCollection();
                            //  forms["__EVENTTARGET"] = "btnLink_Excel";
                            forms["__EVENTARGUMENT"] = "";
                            forms["__VIEWSTATE"] = ExtractVariable(s, "__VIEWSTATE");

                            forms["__EVENTVALIDATION"] = __EVENTVALIDATION;
                            // forms["mDdlOtherIndex"] = arrSpotIndexValues [i];
                            forms["mRbtLstSpotFut"] = "0";

                            webClient.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                            var responseData = webClient.UploadValues(@"http://www.mcxindia.com/sitepages/indexhistory.aspx", "POST", forms);

                            System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\index.html", responseData);


                            s = System.Text.Encoding.UTF8.GetString(responseData);
                            __EVENTVALIDATION = ExtractVariable(s, "__EVENTVALIDATION");

                            forms = new NameValueCollection();
                            //forms["__EVENTTARGET"] = "linkButton";
                            forms["__EVENTARGUMENT"] = "";
                            forms["__VIEWSTATE"] = ExtractVariable(s, "__VIEWSTATE");

                            forms["__EVENTVALIDATION"] = __EVENTVALIDATION;
                            forms["mDdlOtherIndex"] = arrSpotIndexValues[i];
                            //forms["mRbtLstSpotFut"] = "0";
                            forms["mTbFromDate"] = date2 + "/" + date1 + "/" + day.Year;
                            forms["mTbToDate"] = date2 + "/" + date1 + "/" + day.Year; ;
                            forms["mBtnGo.x"] = "130";
                            forms["mBtnGo.y"] = "40";



                            webClient.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                            responseData = webClient.UploadValues(@"http://www.mcxindia.com/sitepages/indexhistory.aspx", "POST", forms);

                            System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\index1.html", responseData);
                            System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\index1111.csv", responseData);


                            s = System.Text.Encoding.UTF8.GetString(responseData);
                            __EVENTVALIDATION = ExtractVariable(s, "__EVENTVALIDATION");

                            forms = new NameValueCollection();
                            forms["__EVENTTARGET"] = "linkButton";
                            forms["__EVENTARGUMENT"] = "";
                            forms["__VIEWSTATE"] = ExtractVariable(s, "__VIEWSTATE");

                            forms["__EVENTVALIDATION"] = __EVENTVALIDATION;
                            forms["mDdlOtherIndex"] = arrSpotIndexValues[i];

                            // forms["mRbtLstSpotFut"] = "0";
                            forms["mTbFromDate"] = date2 + "/" + date1 + "/" + day.Year;
                            forms["mTbToDate"] = date2 + "/" + date1 + "/" + day.Year; ;


                            webClient.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                            responseData = webClient.UploadValues(@"http://www.mcxindia.com/sitepages/indexhistory.aspx", "POST", forms);
                            System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\index2.html", responseData);





                            if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\MCX_Spot_Index"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\Downloads\\MCX_Spot_Index");
                            }

                            System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\MCX_Spot_Index\\" + arrindexvaluesname[i] + "_" + day.Day + ".csv", responseData);


                            datetoselect = day.Year + date2 + date1.ToString();


                        }



                        string[] csvFileNames = Directory.GetFiles(txtTargetFolder.Text + "\\Downloads\\MCX_Spot_Index", "*.csv");
                        ExecuteINDEXProcessing(csvFileNames, "MCXSPOTINDEX", datetoselect, "SEC");


                        JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\Downloads\\MCX_SPOT_INDEX" + datetoselect + ".csv");
                        filetransfer(txtTargetFolder.Text + "\\Downloads\\MCX_SPOT_INDEX" + datetoselect + ".csv", txtTargetFolder.Text + "\\STD_CSV\\MCX_SPOT_INDEX" + datetoselect + ".csv");

                        Deletspace(txtTargetFolder.Text + "\\STD_CSV\\MCX_SPOT_INDEX" + datetoselect + ".csv");

                        if (Directory.Exists(txtTargetFolder.Text + "\\Downloads\\MCX_Spot_Index"))
                        {
                            Directory.Delete(txtTargetFolder.Text + "\\Downloads\\MCX_Spot_Index", true);
                        }

                    }


                }


                prograss();

                ProgressBar1.Value = ProgressBar1.Maximum;
            }
            while (ProgressBar1.Value != ProgressBar1.Maximum);

            btnStart.IsEnabled = true;
            lbl_Download.Content = "Download Completed ";
            System.Windows.Forms.MessageBox.Show("Download Completed Please See Log File In c:\\Temp");
            ProgressBar1.Visibility = Visibility.Hidden;

        }

        private static void Joinbseindex(string[] csvFileNames, string outputDestinationPath)
        {
            StringBuilder sb = new StringBuilder();


            foreach (string csvFileName in csvFileNames)
            {
                TextReader tr = new StreamReader(csvFileName);



                sb.AppendLine(tr.ReadToEnd());


                tr.Close();


            }


            File.WriteAllText(outputDestinationPath, sb.ToString());


        }

        private static void JoinCsvFiles(string[] csvFileNames, string outputDestinationPath)
        {
            StringBuilder sb = new StringBuilder();

            bool columnHeadersRead = false;

            foreach (string csvFileName in csvFileNames)
            {
                TextReader tr = new StreamReader(csvFileName);

                string columnHeaders = tr.ReadLine();

                // Skip appending column headers if already appended
                if (!columnHeadersRead)
                {
                    sb.AppendLine(columnHeaders);
                    columnHeadersRead = true;
                }




                sb.AppendLine(tr.ReadToEnd());

                tr.Close();


            }


            File.WriteAllText(outputDestinationPath, sb.ToString());


        }

        public DelimitedClassBuilder BuildNSECMPFile()
        {
            DelimitedClassBuilder cb = new DelimitedClassBuilder("CMPFILE", ",");

            cb.IgnoreFirstLines = 0;


            cb.AddField("Symbol", typeof(string));
            cb.AddField("Series", typeof(string));
            cb.AddField("Open", typeof(double));
            cb.AddField("High", typeof(double));
            cb.AddField("Low", typeof(double));
            cb.AddField("Close", typeof(double));
            cb.AddField("Last", typeof(double));
            cb.AddField("PrevClose", typeof(double));
            cb.AddField("Tottrdqty", typeof(int));
            cb.AddField("Tottrdval", typeof(double));
            cb.AddField("Timestamp", typeof(string));
            cb.AddField("Totaltrades", typeof(int));
            cb.AddField("Isin", typeof(string));
            cb.AddField("OI", typeof(int));
            cb.LastField.FieldNullValue = 0;

            return cb;
        }

        //taking file name of file from file path
        string GetFileNameWithPath(string[] strMTOArr, string strMTOFileNAme)
        {
            for (int i = 0; i < strMTOArr.Length; i++)
                if (((strMTOArr[i]).ToUpper()).Contains(strMTOFileNAme.ToUpper()))
                    return strMTOArr[i];

            return null;
        }




        public void ExecuteBSEEQUITYProcessing(string[] strBSECSVArr, string[] strSCBTXTArr, string datetostore, string strOutputFolder)
        {
            FileHelperEngine engineBSECSV = new FileHelperEngine(typeof(BSECSV));

            DelimitedClassBuilder cb = BuildNSECMPFile();
            FileHelperEngine engineSCBTXT = new FileHelperEngine(typeof(SCBTXT));


            foreach (string obj in strBSECSVArr)
            {

                //Get BSE Equity Filename day, month, year
                string[] words = obj.Split('\\');

                string strbseequityfilename = words[words.Length - 1];
                string strday = strbseequityfilename.Substring(2, 2);
                string strmon = strbseequityfilename.Substring(4, 2);
                string stryear = strbseequityfilename.Substring(6, 2);

                int index = obj.IndexOf("EQ");
                string dt = strbseequityfilename.Substring(2, 6);

                string scbtxtfilename = "SCBSEALL" + strbseequityfilename.Substring(2, 4) + ".TXT";

                if (!File.Exists(obj))
                {
                    //  AddMessageToLog("File " + strbseequityfilename + " does not exist!");
                    continue;
                }



                string SCBSETXTfilenamewithpath = strSCBTXTArr[0];



                BSECSV[] resbsecsv = engineBSECSV.ReadFile(obj) as BSECSV[];




                SCBTXT[] resscbtxt = engineSCBTXT.ReadFile(SCBSETXTfilenamewithpath) as SCBTXT[];




                int iTotalRows = resbsecsv.Length;


                for (int i = 0; i < iTotalRows; i++)
                {

                    //Copy OI from MTO
                    for (int j = 0; j < resscbtxt.Length; j++)
                    {
                        if (resbsecsv[i].sc_code == resscbtxt[j].scripcode)
                        {

                            resbsecsv[i].openint = resscbtxt[j].deliveryqty;
                            break;
                        }
                    }

                }



                BSECSVFINAL[] finalarr = new BSECSVFINAL[resbsecsv.Length];
                int icntr = 0;
                while (icntr < resbsecsv.Length)
                {
                    finalarr[icntr] = new BSECSVFINAL();
                    finalarr[icntr].ticker = resbsecsv[icntr].sc_code;
                    finalarr[icntr].name = resbsecsv[icntr].sc_name.Trim();


                    finalarr[icntr].date = "20" + stryear + strmon + strday; // String.Format("{0:yyyyMMdd}", myDate);
                    finalarr[icntr].open = resbsecsv[icntr].open;
                    finalarr[icntr].high = resbsecsv[icntr].high;
                    finalarr[icntr].low = resbsecsv[icntr].low;
                    finalarr[icntr].close = resbsecsv[icntr].close;
                    finalarr[icntr].volume = resbsecsv[icntr].no_of_shrs;
                    if ((resbsecsv[icntr].openint) == null)
                        resbsecsv[icntr].openint = 0;
                    finalarr[icntr].openint = resbsecsv[icntr].openint;  //enint;
                    finalarr[icntr].AUX1 = resbsecsv[icntr].net_turnov;


                    icntr++;
                }

                FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(BSECSVFINAL));
                engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT,AUX1";
                engineBSECSVFINAL.WriteFile(obj, finalarr);



            }




        }



        public void Amibroker(string strBSECSVArr)
        {
            visit("http://list.shubhalabha.in/netamibroker.html");

            FileHelperEngine engineBSECSV = new FileHelperEngine(typeof(amibrikerFINAL));

            DelimitedClassBuilder cb = BuildNSECMPFile();




            string[] words = strBSECSVArr.Split('\\');

            string strbseequityfilename = words[words.Length - 1];


            amibrikerFINAL[] resbsecsv = engineBSECSV.ReadFile(strBSECSVArr) as amibrikerFINAL[];

            // engineBSECSV.WriteFile(txtTargetFolder.Text + "\\abc.csv", resbsecsv);
            amibrikerFINAL[] finalarr = new amibrikerFINAL[resbsecsv.Length];


            int icntr = 0;
            while (icntr < resbsecsv.Length)
            {
                finalarr[icntr] = new amibrikerFINAL();
                finalarr[icntr].ticker = resbsecsv[icntr].ticker;
                finalarr[icntr].name = resbsecsv[icntr].name;


                finalarr[icntr].date = resbsecsv[icntr].date; // String.Format("{0:yyyyMMdd}", myDate);
                finalarr[icntr].open = resbsecsv[icntr].open;
                finalarr[icntr].high = resbsecsv[icntr].high;
                finalarr[icntr].low = resbsecsv[icntr].low;
                finalarr[icntr].close = resbsecsv[icntr].close;
                finalarr[icntr].volume = resbsecsv[icntr].volume;


                if (resbsecsv[icntr].openint == null)
                {
                    finalarr[icntr].openint = 0;
                }
                else
                {
                    finalarr[icntr].openint = resbsecsv[icntr].openint;  //enint;

                }

                if (resbsecsv[icntr].AUX1 == null || resbsecsv[icntr].AUX1 == "")
                {
                    finalarr[icntr].AUX1 = "0";


                }
                else
                {
                    finalarr[icntr].AUX1 = resbsecsv[icntr].AUX1;

                }


                icntr++;
            }

            FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(amibrikerFINAL));
            //engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT,AUX1";

            if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
            {
                Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");

            }

            engineBSECSVFINAL.WriteFile(strBSECSVArr, finalarr);


        }
        public void AdvanceGet(string strBSECSVArr)
        {
            visit("http://list.shubhalabha.in/netadvanceget.html");

            FileHelperEngine engineBSECSV = new FileHelperEngine(typeof(amibrikerFINAL));

            DelimitedClassBuilder cb = BuildNSECMPFile();




            string[] words = strBSECSVArr.Split('\\');

            string strbseequityfilename = words[words.Length - 1];


            amibrikerFINAL[] resbsecsv = engineBSECSV.ReadFile(strBSECSVArr) as amibrikerFINAL[];

            // engineBSECSV.WriteFile(txtTargetFolder.Text + "\\abc.csv", resbsecsv);
            AdvanceGetFINAL[] finalarr = new AdvanceGetFINAL[resbsecsv.Length];


            int icntr = 0;
            while (icntr < resbsecsv.Length)
            {
                finalarr[icntr] = new AdvanceGetFINAL();
                finalarr[icntr].ticker = resbsecsv[icntr].ticker;
                finalarr[icntr].name = resbsecsv[icntr].name;


                finalarr[icntr].date = resbsecsv[icntr].date; // String.Format("{0:yyyyMMdd}", myDate);
                finalarr[icntr].open = resbsecsv[icntr].open;
                finalarr[icntr].high = resbsecsv[icntr].high;
                finalarr[icntr].low = resbsecsv[icntr].low;
                finalarr[icntr].close = resbsecsv[icntr].close;
                finalarr[icntr].volume = resbsecsv[icntr].volume;
                finalarr[icntr].PER = "D";


                if (resbsecsv[icntr].openint == null)
                {
                    finalarr[icntr].openint = 0;
                }
                else
                {
                    finalarr[icntr].openint = resbsecsv[icntr].openint;  //enint;

                }





                icntr++;
            }

            FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(AdvanceGetFINAL));
            //engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT,AUX1";

            if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
            {
                Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");

            }

            engineBSECSVFINAL.HeaderText = "Ticker,Name,Per,Date,Open,High,Low,Close,Volume,OPENINT";

            engineBSECSVFINAL.WriteFile(strBSECSVArr, finalarr);


        }
        public void Fchart(string strBSECSVArr)
        {
            visit("http://list.shubhalabha.in/netfcharts.html");

            FileHelperEngine engineBSECSV = new FileHelperEngine(typeof(amibrikerFINAL));

            DelimitedClassBuilder cb = BuildNSECMPFile();




            string[] words = strBSECSVArr.Split('\\');

            string strbseequityfilename = words[words.Length - 1];


            amibrikerFINAL[] resbsecsv = engineBSECSV.ReadFile(strBSECSVArr) as amibrikerFINAL[];

            // engineBSECSV.WriteFile(txtTargetFolder.Text + "\\abc.csv", resbsecsv);
            FchartFINAL[] finalarr = new FchartFINAL[resbsecsv.Length];


            int icntr = 0;
            while (icntr < resbsecsv.Length)
            {
                finalarr[icntr] = new FchartFINAL();
                finalarr[icntr].ticker = resbsecsv[icntr].ticker;


                finalarr[icntr].date = resbsecsv[icntr].date; // String.Format("{0:yyyyMMdd}", myDate);
                finalarr[icntr].open = resbsecsv[icntr].open;
                finalarr[icntr].high = resbsecsv[icntr].high;
                finalarr[icntr].low = resbsecsv[icntr].low;
                finalarr[icntr].close = resbsecsv[icntr].close;
                finalarr[icntr].volume = resbsecsv[icntr].volume;


                if (resbsecsv[icntr].openint == null)
                {
                    finalarr[icntr].openint = 0;
                }
                else
                {
                    finalarr[icntr].openint = resbsecsv[icntr].openint;  //enint;

                }




                icntr++;
            }

            FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(FchartFINAL));
            //engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT,AUX1";

            if (!Directory.Exists(txtTargetFolder.Text + "\\Amibroker"))
            {
                Directory.CreateDirectory(txtTargetFolder.Text + "\\Amibroker");

            }

            engineBSECSVFINAL.WriteFile(strBSECSVArr, finalarr);


        }
        public void ExecuteYAHOOProcessing(string[] strBSECSVArr, string datetostore, string name,int count)
        {
            if (name == "GOOGLEEOD5MIN")
            {
                FileHelperEngine engineBSECSV1 = new FileHelperEngine(typeof(GOOGLE));

                DelimitedClassBuilder cb1 = BuildNSECMPFile();


                foreach (string obj in strBSECSVArr)
                {

                    //Get BSE Equity Filename day, month, year
                    string[] words = obj.Split('\\');

                    string strbseequityfilename = words[words.Length - 1];


                    GOOGLE[] resbsecsv1 = engineBSECSV1.ReadFile(obj) as GOOGLE[];


                    GOOGLEFINAL[] finalarr = new GOOGLEFINAL[resbsecsv1.Length];
                    int icntr = 0;
                    int hrs = Convert.ToInt32(GHRS.SelectedItem);
                    int min = Convert.ToInt32(GMIN.SelectedItem);
                    int hrstostore = Convert.ToInt32(hrs - 5);
                    int mintostore = Convert.ToInt32(min - 30);
                    DateTime timefromyahoo = DateTime.Today;

                    //if (hrs > 5 && min > 30)
                    //{
                    //    timefromyahoo = new DateTime(1970, 1, 1, hrstostore, mintostore, 0).AddSeconds(Convert.ToInt64(resbsecsv1[icntr].Name));
                    //}
                    //else if (hrs > 5 && min <= 30)
                    //{
                    //    timefromyahoo = new DateTime(1970, 1, 1, hrstostore, 0, 0).AddSeconds(Convert.ToInt64(resbsecsv1[icntr].Name));
                    //}
                    //else if (hrs < 5 && min > 30)
                    //{
                    //    timefromyahoo = new DateTime(1970, 1, 1, 0, mintostore, 0).AddSeconds(Convert.ToInt64(resbsecsv1[icntr].Name));
                    //}
                    //else
                    //{

                    // }

                    long valueforgoogletime = 1;

                    while (icntr < resbsecsv1.Length)
                    {
                        finalarr[icntr] = new GOOGLEFINAL();
                        if (resbsecsv1[icntr].Name.Contains('a'))
                        {
                            valueforgoogletime = Convert.ToInt64(resbsecsv1[icntr].Name.Substring(1, resbsecsv1[icntr].Name.Length - 1));
                        }
                        finalarr[icntr].ticker = strbseequityfilename.Substring(2, strbseequityfilename.Length - 6);
                        finalarr[icntr].name = strbseequityfilename.Substring(2, strbseequityfilename.Length - 6); ;


                        timefromyahoo = new DateTime(1970, 1, 1, 5, 30, 0).AddSeconds(valueforgoogletime);
                        valueforgoogletime = valueforgoogletime + 300;

                        string timetostore = timefromyahoo.Hour.ToString() + ":" + timefromyahoo.Minute.ToString() + ":" + timefromyahoo.Millisecond.ToString();

                        string[] yahoodate = timefromyahoo.ToString().Split('-');

                        datetostore = yahoodate[2].Substring(0, 4) + yahoodate[1] + yahoodate[0];



                        finalarr[icntr].date = datetostore; // String.Format("{0:yyyyMMdd}", myDate);
                        finalarr[icntr].open = resbsecsv1[icntr].OPEN_PRICE;
                        finalarr[icntr].high = resbsecsv1[icntr].HIGH_PRICE;
                        finalarr[icntr].low = resbsecsv1[icntr].LOW_PRICE;
                        finalarr[icntr].close = resbsecsv1[icntr].CLOSE_PRICE;
                        finalarr[icntr].volume = resbsecsv1[icntr].volume;
                        finalarr[icntr].time = timetostore;

                        finalarr[icntr].openint = 0;  //enint;


                        icntr++;
                    }

                    FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(GOOGLEFINAL));
                    engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Time,Open,High,Low,Close,Volume,OPENINT";
                    engineBSECSVFINAL.WriteFile(obj, finalarr);
                    log4net.Config.XmlConfigurator.Configure();
                    ILog log = LogManager.GetLogger(typeof(MainWindow));
                    log.Debug("Google File Processing ....... ");
                }
                return;


            }

            if (name == "GOOGLEEOD")
            {
                FileHelperEngine engineBSECSV1 = new FileHelperEngine(typeof(GOOGLE));

                DelimitedClassBuilder cb1 = BuildNSECMPFile();


                foreach (string obj in strBSECSVArr)
                {

                    //Get BSE Equity Filename day, month, year
                    string[] words = obj.Split('\\');

                    string strbseequityfilename = words[words.Length - 1];


                    GOOGLE[] resbsecsv1 = engineBSECSV1.ReadFile(obj) as GOOGLE[];


                    GOOGLEFINAL[] finalarr = new GOOGLEFINAL[resbsecsv1.Length];
                    int icntr = 0;
                    int hrs = Convert.ToInt32(GHRS.SelectedItem);
                    int min = Convert.ToInt32(GMIN.SelectedItem);
                    int hrstostore = Convert.ToInt32(hrs - 5);
                    int mintostore = Convert.ToInt32(min - 30);
                    DateTime timefromyahoo = DateTime.Today;

                    //if (hrs > 5 && min > 30)
                    //{
                    //    timefromyahoo = new DateTime(1970, 1, 1, hrstostore, mintostore, 0).AddSeconds(Convert.ToInt64(resbsecsv1[icntr].Name));
                    //}
                    //else if (hrs > 5 && min <= 30)
                    //{
                    //    timefromyahoo = new DateTime(1970, 1, 1, hrstostore, 0, 0).AddSeconds(Convert.ToInt64(resbsecsv1[icntr].Name));
                    //}
                    //else if (hrs < 5 && min > 30)
                    //{
                    //    timefromyahoo = new DateTime(1970, 1, 1, 0, mintostore, 0).AddSeconds(Convert.ToInt64(resbsecsv1[icntr].Name));
                    //}
                    //else
                    //{
                    long valueforgoogletime = 1;

                    // }


                    while (icntr < resbsecsv1.Length)
                    {
                        finalarr[icntr] = new GOOGLEFINAL();

                        if (resbsecsv1[icntr].Name.Contains('a'))
                        {
                            valueforgoogletime = Convert.ToInt64(resbsecsv1[icntr].Name.Substring(1, resbsecsv1[icntr].Name.Length - 1));
                        }

                        timefromyahoo = new DateTime(1970, 1, 1, 5, 30, 0).AddSeconds(valueforgoogletime);
                        valueforgoogletime = valueforgoogletime + 60;

                        string timetostore = timefromyahoo.Hour.ToString() + ":" + timefromyahoo.Minute.ToString() + ":" + timefromyahoo.Millisecond.ToString();


                        string[] yahoodate = timefromyahoo.ToString().Split('-');

                        datetostore = yahoodate[2].Substring(0, 4) + yahoodate[1] + yahoodate[0];
                        finalarr[icntr].ticker = strbseequityfilename.Substring(2, strbseequityfilename.Length - 6);
                        finalarr[icntr].name = strbseequityfilename.Substring(2, strbseequityfilename.Length - 6); ;


                        finalarr[icntr].date = datetostore; // String.Format("{0:yyyyMMdd}", myDate);
                        finalarr[icntr].open = resbsecsv1[icntr].OPEN_PRICE;
                        finalarr[icntr].high = resbsecsv1[icntr].HIGH_PRICE;
                        finalarr[icntr].low = resbsecsv1[icntr].LOW_PRICE;
                        finalarr[icntr].close = resbsecsv1[icntr].CLOSE_PRICE;
                        finalarr[icntr].volume = resbsecsv1[icntr].volume;
                        finalarr[icntr].time = timetostore;

                        finalarr[icntr].openint = 0;  //enint;


                        icntr++;
                    }

                    FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(GOOGLEFINAL));
                    engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Time,Open,High,Low,Close,Volume,OPENINT";
                    engineBSECSVFINAL.WriteFile(obj, finalarr);
                    log4net.Config.XmlConfigurator.Configure();
                    ILog log = LogManager.GetLogger(typeof(MainWindow));
                    log.Debug("Google File Processing ....... ");
                }
                return;


            }

            if (name == "YAHOOEOD")
            {
                FileHelperEngine engineBSECSV1 = new FileHelperEngine(typeof(YAHOOEOD));

                DelimitedClassBuilder cb1 = BuildNSECMPFile();


                foreach (string obj in strBSECSVArr)
                {

                    //Get BSE Equity Filename day, month, year
                    string[] words = obj.Split('\\');

                    string strbseequityfilename = words[words.Length - 1];

                    YAHOOEOD[] resbsecsv1 = engineBSECSV1.ReadFile(obj) as YAHOOEOD[];


                    YAHOOEODFINAL[] finalarr = new YAHOOEODFINAL[resbsecsv1.Length];
                    int icntr = 0;
                    while (icntr < resbsecsv1.Length)
                    {
                        finalarr[icntr] = new YAHOOEODFINAL();
                        finalarr[icntr].ticker = strbseequityfilename.Substring(2, strbseequityfilename.Length - 6);

                        int  nameofcompany = 0;
                        for (int i = 0; i < yahoosysmbolforprocessing .Count; i++)
                        {
                            if (finalarr[icntr].ticker == yahoosysmbolforprocessing[i])
                            {
                                nameofcompany = i ;
                            }
                        }

                        finalarr[icntr].name = companynameforprocessing[nameofcompany];
                        finalarr[icntr].date = datetostore; // String.Format("{0:yyyyMMdd}", myDate);
                        finalarr[icntr].open = resbsecsv1[icntr].OPEN_PRICE;
                        finalarr[icntr].high = resbsecsv1[icntr].HIGH_PRICE;
                        finalarr[icntr].low = resbsecsv1[icntr].LOW_PRICE;
                        finalarr[icntr].close = resbsecsv1[icntr].CLOSE_PRICE;
                        finalarr[icntr].volume = resbsecsv1[icntr].volume;

                        finalarr[icntr].openint = 0;  //enint;


                        icntr++;
                    }

                    FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(YAHOOEODFINAL));
                    engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT";
                    engineBSECSVFINAL.WriteFile(obj, finalarr);

                }
                return;


            }



            if (name == "YAHOORT")
            {
                FileHelperEngine engineBSECSV1 = new FileHelperEngine(typeof(YAHOORT));

                DelimitedClassBuilder cb1 = BuildNSECMPFile();


                foreach (string obj in strBSECSVArr)
                {

                    //Get BSE Equity Filename day, month, year
                    string[] words = obj.Split('\\');

                    string strbseequityfilename = words[words.Length - 1];
                    YAHOORT[] resbsecsv1 = engineBSECSV1.ReadFile(obj) as YAHOORT[];

                    YAHOORTFINAL[] finalarr = new YAHOORTFINAL[resbsecsv1.Length];
                    int icntr = 0;
                    while (icntr < resbsecsv1.Length)
                    {
                        finalarr[icntr] = new YAHOORTFINAL();
                        finalarr[icntr].ticker = resbsecsv1[icntr].Tiker;
                        finalarr[icntr].name = resbsecsv1[icntr].Name;

                        finalarr[icntr].time = resbsecsv1[icntr].time; // String.Format("{0:yyyyMMdd}", myDate);
                        finalarr[icntr].date = resbsecsv1[icntr].date; // String.Format("{0:yyyyMMdd}", myDate);
                        finalarr[icntr].open = resbsecsv1[icntr].PRICE;
                        finalarr[icntr].high = resbsecsv1[icntr].PRICE;
                        finalarr[icntr].low = resbsecsv1[icntr].PRICE;
                        finalarr[icntr].close = resbsecsv1[icntr].PRICE;
                        finalarr[icntr].volume = resbsecsv1[icntr].volume;

                        finalarr[icntr].openint = 0;  //enint;


                        icntr++;
                    }

                    FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(YAHOORTFINAL));
                    engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Time,Open,High,Low,Close,Volume,OPENINT";
                    engineBSECSVFINAL.WriteFile(obj, finalarr);

                }
                return;


            }



            if (name == "YAHOO5MIN")
            {
                FileHelperEngine engineBSECSV1 = new FileHelperEngine(typeof(YAHOO5MIN));

                DelimitedClassBuilder cb1 = BuildNSECMPFile();


                foreach (string obj in strBSECSVArr)
                {

                    //Get BSE Equity Filename day, month, year
                    string[] words = obj.Split('\\');

                    string strbseequityfilename = words[words.Length - 1];

                    YAHOO5MIN[] resbsecsv1 = engineBSECSV1.ReadFile(obj) as YAHOO5MIN[];

                    YAHOOFINAL[] finalarr = new YAHOOFINAL[resbsecsv1.Length];
                    int icntr = 0;
                    while (icntr < resbsecsv1.Length)
                    {
                        finalarr[icntr] = new YAHOOFINAL();
                        finalarr[icntr].ticker = strbseequityfilename.Substring(2, strbseequityfilename.Length - 6);

                       

                        int hrs = Convert.ToInt32(HRS.SelectedItem);
                        int min = Convert.ToInt32(MIN.SelectedItem);
                        //5 hrs and 30 min is default
                        int hrstostore = Convert.ToInt32(hrs - 5);
                        int mintostore = Convert.ToInt32(min - 30);
                        DateTime timefromyahoo = DateTime.Today;
                        if (hrs > 5 && min > 30)
                        {
                            timefromyahoo = new DateTime(1970, 1, 1, hrstostore, mintostore, 0).AddSeconds(Convert.ToInt64(resbsecsv1[icntr].Name));
                        }
                        else if (hrs > 5 && min <= 30)
                        {
                            timefromyahoo = new DateTime(1970, 1, 1, hrstostore, 0, 0).AddSeconds(Convert.ToInt64(resbsecsv1[icntr].Name));
                        }
                        else if (hrs < 5 && min > 30)
                        {
                            timefromyahoo = new DateTime(1970, 1, 1, 0, mintostore, 0).AddSeconds(Convert.ToInt64(resbsecsv1[icntr].Name));
                        }
                        else
                        {
                            timefromyahoo = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToInt64(resbsecsv1[icntr].Name));

                        }
                        string timetostore = yahootime(timefromyahoo);
                        finalarr[icntr].time = timetostore;
                        string[] yahoodate = timefromyahoo.ToString().Split('-');

                        datetostore = yahoodate[2].Substring(0, 4) + yahoodate[1] + yahoodate[0];
                        finalarr[icntr].name = companynameforprocessing[count ];
                        finalarr[icntr].date = datetostore; // String.Format("{0:yyyyMMdd}", myDate);

                        finalarr[icntr].open = resbsecsv1[icntr].OPEN_PRICE;
                        finalarr[icntr].high = resbsecsv1[icntr].HIGH_PRICE;
                        finalarr[icntr].low = resbsecsv1[icntr].LOW_PRICE;
                        finalarr[icntr].close = resbsecsv1[icntr].CLOSE_PRICE;
                        finalarr[icntr].volume = resbsecsv1[icntr].volume;

                        finalarr[icntr].openint = 0;  //enint;


                        icntr++;
                    }

                    FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(YAHOOFINAL));
                    engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,TIME,Open,High,Low,Close,Volume,OPENINT";
                    engineBSECSVFINAL.WriteFile(obj, finalarr);
                    log4net.Config.XmlConfigurator.Configure();
                    ILog log = LogManager.GetLogger(typeof(MainWindow));
                    log.Debug("Yahoo File Processing ....... ");
                    return;

                }


            }


            FileHelperEngine engineBSECSV = new FileHelperEngine(typeof(YAHOO));

            DelimitedClassBuilder cb = BuildNSECMPFile();


            foreach (string obj in strBSECSVArr)
            {

                //Get BSE Equity Filename day, month, year
                string[] words = obj.Split('\\');

                string strbseequityfilename = words[words.Length - 1];
                YAHOO[] resbsecsv = engineBSECSV.ReadFile(obj) as YAHOO[];


                YAHOOFINAL[] finalarr = new YAHOOFINAL[resbsecsv.Length];
                int icntr = 0;
                while (icntr < resbsecsv.Length)
                {
                    finalarr[icntr] = new YAHOOFINAL();
                    finalarr[icntr].ticker = strbseequityfilename.Substring(2, strbseequityfilename.Length - 6);
                   
                    int hrs = Convert.ToInt32(HRS.SelectedItem);
                    int min = Convert.ToInt32(MIN.SelectedItem);
                    //5 hrs and 30 min is default
                    int hrstostore = Convert.ToInt32(hrs - 5);
                    int mintostore = Convert.ToInt32(min - 30);
                    DateTime timefromyahoo = DateTime.Today;
                    if (hrs > 5 && min > 30)
                    {
                        timefromyahoo = new DateTime(1970, 1, 1, hrstostore, mintostore, 0).AddSeconds(Convert.ToInt64(resbsecsv[icntr].Name));
                    }
                    else if (hrs > 5 && min <= 30)
                    {
                        timefromyahoo = new DateTime(1970, 1, 1, hrstostore, 0, 0).AddSeconds(Convert.ToInt64(resbsecsv[icntr].Name));
                    }
                    else if (hrs < 5 && min > 30)
                    {
                        timefromyahoo = new DateTime(1970, 1, 1, 0, mintostore, 0).AddSeconds(Convert.ToInt64(resbsecsv[icntr].Name));
                    }
                    else
                    {
                        timefromyahoo = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToInt64(resbsecsv[icntr].Name));

                    }

                    string timetostore = yahootime(timefromyahoo);
                    finalarr[icntr].time = timetostore;

                    finalarr[icntr].name = companynameforprocessing[count ];

                    finalarr[icntr].date = datetostore; // String.Format("{0:yyyyMMdd}", myDate);
                    finalarr[icntr].open = resbsecsv[icntr].OPEN_PRICE;
                    finalarr[icntr].high = resbsecsv[icntr].HIGH_PRICE;
                    finalarr[icntr].low = resbsecsv[icntr].LOW_PRICE;
                    finalarr[icntr].close = resbsecsv[icntr].CLOSE_PRICE;
                    finalarr[icntr].volume = resbsecsv[icntr].volume;

                    finalarr[icntr].openint = 0;  //enint;


                    icntr++;
                }

                FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(YAHOOFINAL));
                engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,TIME,Open,High,Low,Close,Volume,OPENINT";
                engineBSECSVFINAL.WriteFile(obj, finalarr);





            }


        }


        public string yahootime(DateTime timetostore)
        {


            if (timetostore.Hour == 03)
            {
                if (timetostore.Minute > 30)
                {
                    return "19:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "20:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            if (timetostore.Hour == 04)
            {
                if (timetostore.Minute > 30)
                {
                    return "20:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "21:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            if (timetostore.Hour == 05)
            {
                if (timetostore.Minute > 30)
                {
                    return "21:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "22:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            if (timetostore.Hour == 06)
            {
                if (timetostore.Minute > 30)
                {
                    return "22:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "23:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            if (timetostore.Hour == 07)
            {
                if (timetostore.Minute > 30)
                {
                    return "23:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "24:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            if (timetostore.Hour == 08)
            {
                if (timetostore.Minute > 30)
                {
                    return "24:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "24:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }

            if (timetostore.Hour == 13)
            {
                if (timetostore.Minute > 30)
                {
                    return "00:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "00:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }



            else if (timetostore.Hour == 14)
            {
                if (timetostore.Minute < 30)
                {
                    return "00:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "01:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            else if (timetostore.Hour == 15)
            {
                if (timetostore.Minute < 30)
                {
                    return "01:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "02:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            else if (timetostore.Hour == 16)
            {
                if (timetostore.Minute < 30)
                {
                    return "02:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "03:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }

            else if (timetostore.Hour == 17)
            {
                if (timetostore.Minute < 30)
                {
                    return "03:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "04:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            else if (timetostore.Hour == 18)
            {
                if (timetostore.Minute < 30)
                {
                    return "04:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "05:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }

            else if (timetostore.Hour == 19)
            {
                if (timetostore.Minute < 30)
                {
                    return "05:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "06:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            else if (timetostore.Hour == 20)
            {
                if (timetostore.Minute < 30)
                {
                    return "06:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "07:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            else if (timetostore.Hour == 21)
            {
                if (timetostore.Minute < 30)
                {
                    return "07:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "08:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            else if (timetostore.Hour == 22)
            {
                if (timetostore.Minute < 30)
                {
                    return "08:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "09:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            else if (timetostore.Hour == 23)
            {
                if (timetostore.Minute < 30)
                {
                    return "09:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "10:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }
            else if (timetostore.Hour == 24)
            {
                if (timetostore.Minute < 30)
                {
                    return "10:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
                else
                {
                    return "11:" + timetostore.Minute.ToString() + ":" + timetostore.Second.ToString();

                }
            }


            return null;
        }

        public void ExecuteMCSSXProcessing(string[] strBSECSVArr, string datetostore, string strOutputFolder)
        {
            FileHelperEngine engineBSECSV = new FileHelperEngine(typeof(MCXSX));

            DelimitedClassBuilder cb = BuildNSECMPFile();


            foreach (string obj in strBSECSVArr)
            {

                //Get BSE Equity Filename day, month, year
                string[] words = obj.Split('\\');

                string strbseequityfilename = words[words.Length - 1];
                string strday = strbseequityfilename.Substring(2, 2);
                string strmon = strbseequityfilename.Substring(4, 2);
                string stryear = strbseequityfilename.Substring(6, 2);

                int index = obj.IndexOf("EQ");
                string dt = strbseequityfilename.Substring(2, 6);




                MCXSX[] resbsecsv = engineBSECSV.ReadFile(obj) as MCXSX[];







                int iTotalRows = resbsecsv.Length;



                /////////////////
                List<Int32> lowvalue = new List<int> { };

               
                    for (int i = 0; i < iTotalRows - 1; i++)
                    {

                        lowvalue.Add(Convert.ToInt32(resbsecsv[i].Date.Substring(3, 2)));

                    }
                




                ////////////////////

                MCXSXFINAL[] finalarr = new MCXSXFINAL[resbsecsv.Length];
                int icntr = 0;
                //////////////////
                while (icntr < resbsecsv.Length)
                {

                                        int lowmonth = lowvalue.Min();
                        if (Convert.ToInt32(resbsecsv[icntr].Date .Substring(3, 2)) == lowmonth)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-I";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].Date.Substring(3, 2)) == lowmonth + 1)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-II";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].Date.Substring(3, 2)) == lowmonth + 2)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-III";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].Date.Substring(3, 2)) == lowmonth + 3)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-IV";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].Date.Substring(3, 2)) == lowmonth + 4)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-V";
                        }

                        if (Convert.ToInt32(resbsecsv[icntr].Date.Substring(3, 2)) == lowmonth + 5)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-VI";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].Date.Substring(3, 2)) == lowmonth + 6)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-VII";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].Date.Substring(3, 2)) == lowmonth + 7)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-VIII";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].Date.Substring(3, 2)) == lowmonth + 8)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-IX";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].Date.Substring(3, 2)) == lowmonth + 9)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-X";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].Date.Substring(3, 2)) == lowmonth + 10)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-XI";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].Date.Substring(3, 2)) == lowmonth + 11)
                        {
                            resbsecsv[icntr].Symbol = resbsecsv[icntr].Symbol.Trim() + "-XII";
                        }




                    ///////////////////////




                    finalarr[icntr] = new MCXSXFINAL();
                    finalarr[icntr].ticker = resbsecsv[icntr].Symbol;
                    finalarr[icntr].date = datetostore;// String.Format("{0:yyyyMMdd}", myDate);
                    finalarr[icntr].open = resbsecsv[icntr].HIGH_PRICE;
                    finalarr[icntr].high = resbsecsv[icntr].OPEN_PRICE;
                    finalarr[icntr].low = resbsecsv[icntr].LOW_PRICE;
                    finalarr[icntr].close = resbsecsv[icntr].CLOSE_PRICE;
                    finalarr[icntr].volume = resbsecsv[icntr].volume;

                    finalarr[icntr].openint = 0;  //enint;


                    icntr++;
                }

                FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(MCXSXFINAL));
                engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT";
                engineBSECSVFINAL.WriteFile(obj, finalarr);





            }


        }
        public void ExecuteMCSSXFOREXProcessing(string[] strBSECSVArr, string datetostore, string strOutputFolder)
        {
            FileHelperEngine engineBSECSV = new FileHelperEngine(typeof(MCXSXFOREX));

            DelimitedClassBuilder cb = BuildNSECMPFile();


            foreach (string obj in strBSECSVArr)
            {

                //Get BSE Equity Filename day, month, year
                string[] words = obj.Split('\\');

                string strbseequityfilename = words[words.Length - 1];
                string strday = strbseequityfilename.Substring(2, 2);
                string strmon = strbseequityfilename.Substring(4, 2);
                string stryear = strbseequityfilename.Substring(6, 2);

                int index = obj.IndexOf("EQ");
                string dt = strbseequityfilename.Substring(2, 6);




                MCXSXFOREX[] resbsecsv = engineBSECSV.ReadFile(obj) as MCXSXFOREX[];








                int iTotalRows = resbsecsv.Length;






                ///////////////////








                ////////







                MCXSXFOREXFINAL[] finalarr = new MCXSXFOREXFINAL[resbsecsv.Length];
                int icntr = 0;
                while (icntr < resbsecsv.Length)
                {
                    finalarr[icntr] = new MCXSXFOREXFINAL();
                    finalarr[icntr].ticker = resbsecsv[icntr].instrument;
                    finalarr[icntr].date = datetostore;// String.Format("{0:yyyyMMdd}", myDate);
                    finalarr[icntr].open = resbsecsv[icntr].OPEN_PRICE;
                    finalarr[icntr].high = resbsecsv[icntr].HIGH_PRICE;
                    finalarr[icntr].low = resbsecsv[icntr].LOW_PRICE;
                    finalarr[icntr].close = resbsecsv[icntr].CLOSE_PRICE;
                    finalarr[icntr].volume = resbsecsv[icntr].volume;

                    finalarr[icntr].openint = 0;  //enint;


                    icntr++;
                }

                FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(MCXSXFOREXFINAL));
                engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT";
                engineBSECSVFINAL.WriteFile(obj, finalarr);





            }


        }


        public void ExecuteSMEETFProcessing(string[] strBSECSVArr, string name, string strOutputFolder, string strNSESEC)
        {
            FileHelperEngine engineSMEETF = new FileHelperEngine(typeof(SMEETF));


            DelimitedClassBuilder cb = BuildNSECMPFile();

            string strbseequityfilename;
            string strday;
            string strmon;
            string stryear;
            foreach (string obj in strBSECSVArr)
            {

                //Get BSE Equity Filename day, month, year
                string[] words = obj.Split('\\');

                if (name == "FO")
                {

                    strbseequityfilename = words[words.Length - 1];
                    strday = strbseequityfilename.Substring(2, 2);
                    strmon = strbseequityfilename.Substring(4, 2);
                    stryear = strbseequityfilename.Substring(8, 2);

                }
                else
                {
                    strbseequityfilename = words[words.Length - 1];
                    strday = strbseequityfilename.Substring(3, 2);
                    strmon = strbseequityfilename.Substring(5, 2);
                    stryear = strbseequityfilename.Substring(7, 2);

                }



                FileHelperEngine engineSEC = new FileHelperEngine(typeof(NSESEC));




                SMEETF[] resbsecsv = engineSMEETF.ReadFile(obj) as SMEETF[];






                int iTotalRows = resbsecsv.Length;








                optionFINAL[] finalarr = new optionFINAL[resbsecsv.Length];
                int icntr = 0;
                while (icntr < resbsecsv.Length)
                {
                    finalarr[icntr] = new optionFINAL();


                    finalarr[icntr].ticker = resbsecsv[icntr].SYMBOL;


                    //myDate = Convert.ToDateTime(dt);
                    //myDate = DateTime.ParseExact(dt, "ddMMyyyy", CultureInfo.InvariantCulture);

                    //myDate=Convert.ToDateTime(strday + "-"+ strmon + "-20" + stryear);
                    //finalarr[itmp].date = myDate.ToString("yyyyMMdd"); //String.Format("{0:yyyyMMdd}", dt);
                    finalarr[icntr].name = resbsecsv[icntr].SECURITY;

                    finalarr[icntr].date = "20" + stryear + strmon + strday; // String.Format("{0:yyyyMMdd}", myDate);
                    finalarr[icntr].open = resbsecsv[icntr].OPEN_PRICE;
                    finalarr[icntr].high = resbsecsv[icntr].HIGH_PRICE;
                    finalarr[icntr].low = resbsecsv[icntr].LOW_PRICE;
                    finalarr[icntr].close = resbsecsv[icntr].CLOSE_PRICE;
                    finalarr[icntr].volume = resbsecsv[icntr].NET_TRDQTY;
                    finalarr[icntr].openint = 0; //enint;


                    icntr++;
                }

                FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(optionFINAL));
                engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT";
                engineBSECSVFINAL.WriteFile(obj, finalarr);







            }


        }


        public void ExecuteFUTUREProcessing(string[] strBSECSVArr, string name, string datetostore, string strNSESEC)
        {











            FileHelperEngine engineSMEETF = new FileHelperEngine(typeof(SMEETF));
            FileHelperEngine engineFO = new FileHelperEngine(typeof(FO));


            DelimitedClassBuilder cb = BuildNSECMPFile();

            string strbseequityfilename;
            string strday;
            string strmon;
            string stryear;
            foreach (string obj in strBSECSVArr)
            {

                //Get BSE Equity Filename day, month, year
                string[] words = obj.Split('\\');



                strbseequityfilename = words[words.Length - 1];
                strday = strbseequityfilename.Substring(2, 2);
                strmon = strbseequityfilename.Substring(4, 2);
                stryear = strbseequityfilename.Substring(8, 2);




                FileHelperEngine engineSEC = new FileHelperEngine(typeof(NSESEC));




                FO[] resbsecsv = engineFO.ReadFile(obj) as FO[];






                int iTotalRows = resbsecsv.Length;




                List<Int32> lowvalue = new List<int> { };

                if (name == "MCXBHAV")
                {
                    for (int i = 0; i < iTotalRows - 1; i++)
                    {
                        string date = resbsecsv[i].EXP_DATE.Substring(3, 3).ToUpper();
                        if (date == "JAN")
                        {
                            date = "january";
                        }
                        else if (date == "FEB")
                        {
                            date = "February";

                        }
                        else if (date == "MAR")
                        {
                            date = "March";

                        }
                        else if (date == "APR")
                        {
                            date = "April";



                        }
                        else if (date == "JUN")
                        {
                            date = "June";

                        }
                        else if (date == "JUL")
                        {
                            date = "July";

                        }
                        else if (date == "AUG")
                        {
                            date = "August";

                        }
                        else if (date == "SEP")
                        {
                            date = "September";


                        }
                        else if (date == "OCT")
                        {
                            date = "october";

                        }
                        else if (date == "NOV")
                        {
                            date = "November";
                        }
                        else if (date == "DEC")
                        {
                            date = "December";

                        }
                        int monthno = DateTime.ParseExact(date, "MMMM", CultureInfo.CurrentCulture).Month;

                        lowvalue.Add(Convert.ToInt32(monthno));

                        resbsecsv[i].EXP_DATE = monthno.ToString();


                    }

                }
                else
                {
                    for (int i = 0; i < iTotalRows - 1; i++)
                    {

                        lowvalue.Add(Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)));

                    }
                }


                NSESEC[] ressec = engineSEC.ReadFile(strNSESEC) as NSESEC[];
                int countformcxbhavblankrow = 0;





                for (int i = 0; i < iTotalRows - 1; i++)
                {
                    int lowmonth = lowvalue.Min();

                    int flag = 0;

                    //Copy Security Name from SEC
                    for (int j = 0; j < ressec.Length; j++)
                    {
                        if ((ressec[j].Symbol == (string)resbsecsv[i].SYMBOL.Trim()))//series is save as sysmbol in fo file 
                        {

                            resbsecsv[i].SECURITY = ressec[j].SecurityName;

                            flag = 1;
                            break;
                        }


                    }
                    if (flag == 0)
                    {
                        resbsecsv[i].SECURITY = "";

                    }

                    if (name != "MCXBHAV")
                    {

                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-I";
                        }
                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 1)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-II";
                        }
                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 2)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-III";
                        }
                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 3)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-IV";
                        }
                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 4)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-V";
                        }

                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 5)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-VI";
                        }
                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 6)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-VII";
                        }
                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 7)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-VIII";
                        }
                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 8)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-IX";
                        }
                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 9)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-X";
                        }
                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 10)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-XI";
                        }
                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 11)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-XII";
                        }





                    }
                    else
                    {
                        if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-I";
                            countformcxbhavblankrow++;
                        }
                        else if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth + 1)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-II";
                            countformcxbhavblankrow++;

                        }
                        else if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth + 2)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-III";
                            countformcxbhavblankrow++;

                        }
                        else if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth + 4)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-IV";
                            countformcxbhavblankrow++;

                        }
                        else if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth + 5)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-V";
                            countformcxbhavblankrow++;

                        }
                        else if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth + 6)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-VI";
                            countformcxbhavblankrow++;

                        }
                        else if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth + 7)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-VII";
                            countformcxbhavblankrow++;

                        }
                        else if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth + 8)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-VIII";
                            countformcxbhavblankrow++;

                        }
                        else if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth + 9)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-IX";
                            countformcxbhavblankrow++;
                        }

                        else if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth + 10)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-X";
                            countformcxbhavblankrow++;

                        }
                        else if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth + 11)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-XI";
                            countformcxbhavblankrow++;

                        }
                        else if (Convert.ToInt32(resbsecsv[i].EXP_DATE) == lowmonth + 12)
                        {
                            resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-XII";
                            countformcxbhavblankrow++;

                        }
                        else
                        {

                            resbsecsv[i].SYMBOL = "";

                        }
                    }
                }






                FOFINAL[] finalarr = new FOFINAL[resbsecsv.Length - 1];
                int totallenth = resbsecsv.Length;
                if (name == "MCXBHAV")
                {
                    finalarr = new FOFINAL[countformcxbhavblankrow + 1];
                    totallenth = countformcxbhavblankrow;
                }
                int itmp = 0;
                int icntr = 0;
                while (icntr < resbsecsv.Length)
                {

                    //if (name != "MCXBHAV")
                    //{


                    if (resbsecsv[icntr].SYMBOL != "")
                    {
                        finalarr[itmp] = new FOFINAL();



                        finalarr[itmp].ticker = resbsecsv[icntr].SYMBOL;



                        finalarr[itmp].name = resbsecsv[icntr].SECURITY;



                        finalarr[itmp].date = datetostore; // String.Format("{0:yyyyMMdd}", myDate);
                        finalarr[itmp].open = resbsecsv[icntr].OPEN_PRICE;
                        finalarr[itmp].high = resbsecsv[icntr].HIGH_PRICE;
                        finalarr[itmp].low = resbsecsv[icntr].LOW_PRICE;
                        finalarr[itmp].close = resbsecsv[icntr].CLOSE_PRICE;
                        finalarr[itmp].volume = resbsecsv[icntr].NET_TRDQTY.Trim();

                        if (name == "MCXBHAV")
                        {
                            finalarr[itmp].name = resbsecsv[icntr].SYMBOL;
                            finalarr[itmp].volume = resbsecsv[icntr].OPEN_INT.ToString();

                        }

                        finalarr[itmp].openint =  (resbsecsv[icntr].OPEN_INT.ToString().Trim()); //enint;

                        if (name == "CF")
                        {
                            finalarr[itmp].AUX1 = resbsecsv[icntr].TRD_VAL.ToString().Trim(); //enint;

                        }
                        else
                        {
                            finalarr[itmp].AUX1 = "";


                        }

                        itmp++;


                    }
                    icntr++;
                }

                FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(FOFINAL));
                if (name == "CF")
                {
                    engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT,AUX1";
                }
                else
                {
                    engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT";

                }
                engineBSECSVFINAL.WriteFile(obj, finalarr);







            }


        }

        public static void SaveAs()
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook wbWorkbook = app.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Sheets wsSheet = wbWorkbook.Worksheets;


            wbWorkbook.SaveAs(@"c:\one.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            wbWorkbook.SaveAs(@"c:\two.csv", Microsoft.Office.Interop.Excel.XlFileFormat.xlCSVWindows, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            wbWorkbook.Close(false, "", true);
        }

        public void Deletspace(string Filename)
        {
            FileHelperEngine engineMCXindex = new FileHelperEngine(typeof(SpaceRemove));
            SpaceRemove[] resbsecsv = engineMCXindex.ReadFile(Filename) as SpaceRemove[];

            SpaceRemove[] finalarr = new SpaceRemove[resbsecsv.Length];
            int icntr = 0;
            while (icntr < resbsecsv.Length)
            {
                finalarr[icntr] = new SpaceRemove();


                finalarr[icntr].ticker = resbsecsv[icntr].ticker;

                // finalarr[icntr].ticker = strbseequityfilename.Substring(0,strbseequityfilename.Length - 4);


                finalarr[icntr].name = resbsecsv[icntr].name;

                //first col is not present as nseindex so data is capture as open =highprice,high=low_price and so on

                finalarr[icntr].date = resbsecsv[icntr].date;


                finalarr[icntr].open = resbsecsv[icntr].open;
                finalarr[icntr].high = resbsecsv[icntr].high;
                finalarr[icntr].low = resbsecsv[icntr].low;
                finalarr[icntr].close = resbsecsv[icntr].close;
                finalarr[icntr].volume = resbsecsv[icntr].volume;
                finalarr[icntr].openint = resbsecsv[icntr].openint; //enint;


                icntr++;


            }
            FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(SpaceRemove));


            engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT";



            engineBSECSVFINAL.WriteFile(Filename, finalarr);

        }
        public void ExecuteINDEXProcessing(string[] strBSECSVArr, string name, string datetostore, string strNSESEC)
        {
            if (name == "MCXSPOTINDEX")
            {

                FileHelperEngine engineMCXindex = new FileHelperEngine(typeof(MCXSPOTINDEX));

                DelimitedClassBuilder cb1 = BuildNSECMPFile();
                namemcxindex.Sort();



                foreach (string obj in strBSECSVArr)
                {
                    //Get BSE Equity Filename day, month, year
                    string[] words = obj.Split('\\');



                    MCXSPOTINDEX[] resbsecsv = engineMCXindex.ReadFile(obj) as MCXSPOTINDEX[];
                    int iTotalRows = resbsecsv.Length;


                    MCXSPOTINDEXFINAL[] finalarr = new MCXSPOTINDEXFINAL[resbsecsv.Length];
                    int icntr = 0;
                    while (icntr < resbsecsv.Length)
                    {
                        finalarr[icntr] = new MCXSPOTINDEXFINAL();



                        string strbseequityfilename1;
                        strbseequityfilename1 = words[words.Length - 1];

                        finalarr[icntr].ticker = strbseequityfilename1.Substring(0, strbseequityfilename1.Length - 7);

                        // finalarr[icntr].ticker = strbseequityfilename.Substring(0,strbseequityfilename.Length - 4);


                        finalarr[icntr].name = strbseequityfilename1.Substring(0, strbseequityfilename1.Length - 7); ;

                        //first col is not present as nseindex so data is capture as open =highprice,high=low_price and so on

                        finalarr[icntr].date = datetostore;


                        finalarr[icntr].open = resbsecsv[icntr].OPEN_PRICE;
                        finalarr[icntr].high = "0";
                        finalarr[icntr].low = "0";
                        finalarr[icntr].close = resbsecsv[icntr].CLOSE_PRICE.Substring(0, resbsecsv[icntr].CLOSE_PRICE.Length - 2);
                        finalarr[icntr].volume = "0";
                        finalarr[icntr].openint = 0; //enint;




                        flag = 1;
                        icntr++;
                    }

                    FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(MCXSPOTINDEXFINAL));


                    engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT";



                    engineBSECSVFINAL.WriteFile(obj, finalarr);







                }




                //if mcx index no need to execute following code 
                return;
            }



            if (name == "MCXINDEX")
            {

                FileHelperEngine engineMCXindex = new FileHelperEngine(typeof(MCXINDEX));

                DelimitedClassBuilder cb1 = BuildNSECMPFile();
                namemcxindex.Sort();



                foreach (string obj in strBSECSVArr)
                {
                    //Get BSE Equity Filename day, month, year
                    string[] words = obj.Split('\\');



                    MCXINDEX[] resbsecsv = engineMCXindex.ReadFile(obj) as MCXINDEX[];
                    int iTotalRows = resbsecsv.Length;


                    MCXINDEXFINAL[] finalarr = new MCXINDEXFINAL[resbsecsv.Length];
                    int icntr = 0;
                    while (icntr < resbsecsv.Length)
                    {
                        finalarr[icntr] = new MCXINDEXFINAL();



                        string strbseequityfilename1;
                        strbseequityfilename1 = words[words.Length - 1];

                        finalarr[icntr].ticker = strbseequityfilename1.Substring(0, strbseequityfilename1.Length - 7);

                        // finalarr[icntr].ticker = strbseequityfilename.Substring(0,strbseequityfilename.Length - 4);


                        finalarr[icntr].name = strbseequityfilename1.Substring(0, strbseequityfilename1.Length - 7); ;

                        //first col is not present as nseindex so data is capture as open =highprice,high=low_price and so on

                        finalarr[icntr].date = datetostore;


                        finalarr[icntr].open = resbsecsv[icntr].OPEN_PRICE;
                        finalarr[icntr].high = resbsecsv[icntr].HIGH_PRICE;
                        finalarr[icntr].low = resbsecsv[icntr].LOW_PRICE;
                        finalarr[icntr].close = resbsecsv[icntr].CLOSE_PRICE.Substring(0, resbsecsv[icntr].CLOSE_PRICE.Length - 2);
                        finalarr[icntr].volume = "0";
                        finalarr[icntr].openint = 0; //enint;




                        flag = 1;
                        icntr++;
                    }

                    FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(MCXINDEXFINAL));


                    engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT";



                    engineBSECSVFINAL.WriteFile(obj, finalarr);







                }




                //if mcx index no need to execute following code 
                return;
            }










            FileHelperEngine engineindex = new FileHelperEngine(typeof(Index));
            nameofbseindex.Sort();

            DelimitedClassBuilder cb = BuildNSECMPFile();

            string strbseequityfilename;
            string strday;
            string strmon;
            string stryear;
            string[] filename = new string[27] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            int filecount = 0;

            foreach (string obj in strBSECSVArr)
            {
                //Get BSE Equity Filename day, month, year
                string[] words = obj.Split('\\');








                FileHelperEngine engineSEC = new FileHelperEngine(typeof(NSESEC));




                Index[] resbsecsv = engineindex.ReadFile(obj) as Index[];






                int iTotalRows = resbsecsv.Length;








                NSESEC[] ressec = engineSEC.ReadFile(strNSESEC) as NSESEC[];






                for (int i = 0; i < iTotalRows - 1; i++)
                {

                    int flag = 0;

                    //Copy Security Name from SEC
                    for (int j = 0; j < ressec.Length; j++)
                    {
                        if ((ressec[j].Symbol == (string)resbsecsv[i].Name.Trim()))//series is save as sysmbol in fo file 
                        {

                            resbsecsv[i].security = ressec[j].SecurityName;

                            flag = 1;
                            break;
                        }


                    }
                    if (flag == 0)
                    {
                        resbsecsv[i].security = "";

                    }










                }






                IndexFINAL[] finalarr = new IndexFINAL[resbsecsv.Length];
                PEBEFINAL[] PEBE = new PEBEFINAL[resbsecsv.Length];

                int icntr = 0;
                while (icntr < resbsecsv.Length)
                {
                    finalarr[icntr] = new IndexFINAL();
                    PEBE[icntr] = new PEBEFINAL();
                    if (name == "NSEINDEX")
                    {
                        strbseequityfilename = words[words.Length - 1];
                        strday = strbseequityfilename.Substring(8, 2);
                        strmon = strbseequityfilename.Substring(10, 2);
                        stryear = strbseequityfilename.Substring(14, 2);


                        finalarr[icntr].ticker = resbsecsv[icntr].Name;
                        PEBE[icntr].ticker = resbsecsv[icntr].Name;


                        finalarr[icntr].name = resbsecsv[icntr].Name;  //sanme as tiker otherwise security name
                        PEBE[icntr].name = resbsecsv[icntr].Name;  //sanme as tiker otherwise security name

                        finalarr[icntr].date = "20" + stryear + strmon + strday; // String.Format("{0:yyyyMMdd}", myDate);
                        PEBE[icntr].date = "20" + stryear + strmon + strday; // String.Format("{0:yyyyMMdd}", myDate);

                        finalarr[icntr].open = resbsecsv[icntr].OPEN_PRICE;
                        PEBE[icntr].open = resbsecsv[icntr].OPEN_PRICE;

                        finalarr[icntr].high = resbsecsv[icntr].HIGH_PRICE;
                        PEBE[icntr].high = resbsecsv[icntr].HIGH_PRICE;

                        finalarr[icntr].low = resbsecsv[icntr].LOW_PRICE;
                        PEBE[icntr].low = resbsecsv[icntr].LOW_PRICE;

                        finalarr[icntr].close = resbsecsv[icntr].CLOSE_PRICE;
                        PEBE[icntr].close = resbsecsv[icntr].CLOSE_PRICE;

                        finalarr[icntr].volume = resbsecsv[icntr].Volume;
                        PEBE[icntr].volume = resbsecsv[icntr].Volume;

                        finalarr[icntr].openint = 0; //enint;
                        PEBE[icntr].openint = 0; //enint;
                        PEBE[icntr].PE = resbsecsv[icntr].NO_OF_TRADE;
                        PEBE[icntr].BE = resbsecsv[icntr].NOTION_VAL;   //be value NOTION_VAL

                        if (resbsecsv[icntr].NO_OF_TRADE == "-")
                        {
                            PEBE[icntr].PE = "0";
                        }//pe valeu saved in NO_OF_TRADE
                        if (resbsecsv[icntr].NOTION_VAL == "-,-")
                        {
                            PEBE[icntr].BE = "0";   //be value NOTION_VAL
                        }

                        if (resbsecsv[icntr].OPEN_PRICE == "-")
                        {
                            finalarr[icntr].open = "0";
                            PEBE[icntr].open = "0";

                        }
                        if (resbsecsv[icntr].HIGH_PRICE == "-")
                        {
                            finalarr[icntr].high = "0";
                            PEBE[icntr].high = "0";

                        }
                        if (resbsecsv[icntr].LOW_PRICE == "-")
                        {
                            finalarr[icntr].low = "0";
                            PEBE[icntr].low = "0";

                        }
                        if (resbsecsv[icntr].Volume == "-")
                        {
                            finalarr[icntr].volume = "0";
                            PEBE[icntr].volume = "0";

                        }



                    }

                    if (name == "BSEINDEX")
                    {

                        strbseequityfilename = words[words.Length - 1];

                        finalarr[icntr].ticker = nameofbseindex[icntr];

                        // finalarr[icntr].ticker = strbseequityfilename.Substring(0,strbseequityfilename.Length - 4);


                        finalarr[icntr].date = datetostore;

                        //first col is not present as nseindex so data is capture as open =highprice,high=low_price and so on
                        finalarr[icntr].name = nameofbseindex[icntr];//strbseequityfilename.Substring(0, strbseequityfilename.Length - 4);

                        finalarr[icntr].open = resbsecsv[icntr].Date1;
                        finalarr[icntr].high = resbsecsv[icntr].OPEN_PRICE;
                        finalarr[icntr].low = resbsecsv[icntr].HIGH_PRICE;
                        finalarr[icntr].close = resbsecsv[icntr].LOW_PRICE;
                        finalarr[icntr].volume = "0";
                        finalarr[icntr].openint = 0; //enint;

                    }




                    icntr++;
                }

                FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(IndexFINAL));


                engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT";



                engineBSECSVFINAL.WriteFile(obj, finalarr);
                filename[filecount] = obj;


                filecount++;


                if (name == "NSEINDEX")
                {
                    FileHelperEngine enginePEBEFINAL = new FileHelperEngine(typeof(PEBEFINAL));
                    enginePEBEFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT,PE,BE,DivYield";

                    //  enginePEBEFINAL.WriteFile(obj, fi);

                    enginePEBEFINAL.WriteFile(obj + "PEBE", PEBE);
                    // System.IO.File.Copy(obj, txtTargetFolder.Text + "\\STD_CSV\\PEBE"+datetostore +".csv");

                }
                //combine file 






            }
            //if (name == "BSEINDEX")
            //{
            //    //if(!System.IO.File.Exists(strOutputFolder))
            //    //{


            //    //System.IO.File.Create(strOutputFolder);

            //    //}
            //    Joinbseindex(filename, strOutputFolder);
            //}


        }
        public void ExecuteOPTIONProcessing(string[] strBSECSVArr, string name, string strOutputFolder, string strNSESEC)
        {
            FileHelperEngine engineOption = new FileHelperEngine(typeof(Option));


            DelimitedClassBuilder cb = BuildNSECMPFile();

            string strbseequityfilename;
            string strday;
            string strmon;
            string stryear;
            foreach (string obj in strBSECSVArr)
            {

                //Get BSE Equity Filename day, month, year
                string[] words = obj.Split('\\');



                strbseequityfilename = words[words.Length - 1];
                strday = strbseequityfilename.Substring(2, 2);
                strmon = strbseequityfilename.Substring(4, 2);
                stryear = strbseequityfilename.Substring(8, 2);




                FileHelperEngine engineSEC = new FileHelperEngine(typeof(NSESEC));




                Option[] resbsecsv = engineOption.ReadFile(obj) as Option[];






                int iTotalRows = resbsecsv.Length;


                List<Int32> lowvalue = new List<int> { };


                for (int i = 0; i < iTotalRows - 1; i++)
                {

                    lowvalue.Add(Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)));



                }



                NSESEC[] ressec = engineSEC.ReadFile(strNSESEC) as NSESEC[];

                for (int i = 0; i < iTotalRows - 1; i++)
                {

                    int lowmonth = lowvalue.Min();

                    int flag = 0;

                    //Copy Security Name from SEC
                    for (int j = 0; j < ressec.Length; j++)
                    {
                        if ((ressec[j].Symbol == (string)resbsecsv[i].SYMBOL.Trim()))//series is save as sysmbol in fo file 
                        {
                            resbsecsv[i].SECURITY = ressec[j].SecurityName;
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 0)
                    {
                        resbsecsv[i].SECURITY = "";

                    }


                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-I";
                    }
                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 1)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-II";
                    }
                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 2)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-III";
                    }
                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 3)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-IV";
                    }
                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 4)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-V";
                    }

                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 5)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-VI";
                    }
                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 6)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-VII";
                    }
                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 7)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-VIII";
                    }
                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 8)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-IX";
                    }
                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 9)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-X";
                    }
                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 10)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-XI";
                    }
                    if (Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(3, 2)) == lowmonth + 11)
                    {
                        resbsecsv[i].SYMBOL = resbsecsv[i].SYMBOL.Trim() + "-XII";
                    }


                }






                optionFINAL[] finalarr = new optionFINAL[resbsecsv.Length];
                int icntr = 0;


                while (icntr < resbsecsv.Length)
                {
                    finalarr[icntr] = new optionFINAL();


                    finalarr[icntr].ticker = resbsecsv[icntr].SYMBOL.Trim() + resbsecsv[icntr].STR_PRICE.ToString().Trim() + resbsecsv[icntr].OPT_TYPE.Trim();


                    //myDate = Convert.ToDateTime(dt);
                    //myDate = DateTime.ParseExact(dt, "ddMMyyyy", CultureInfo.InvariantCulture);

                    //myDate=Convert.ToDateTime(strday + "-"+ strmon + "-20" + stryear);
                    //finalarr[itmp].date = myDate.ToString("yyyyMMdd"); //String.Format("{0:yyyyMMdd}", dt);
                    finalarr[icntr].name = resbsecsv[icntr].SECURITY;

                    finalarr[icntr].date = "20" + stryear + strmon + strday; // String.Format("{0:yyyyMMdd}", myDate);
                    finalarr[icntr].open = resbsecsv[icntr].OPEN_PRICE;
                    finalarr[icntr].high = resbsecsv[icntr].HIGH_PRICE;
                    finalarr[icntr].low = resbsecsv[icntr].LOW_PRICE;
                    finalarr[icntr].close = resbsecsv[icntr].CLOSE_PRICE;
                    finalarr[icntr].volume = Convert.ToInt32(resbsecsv[icntr].TRD_VAL);
                    finalarr[icntr].openint = Convert.ToInt32(resbsecsv[icntr].OPEN_INT); //enint;


                    icntr++;
                }

                FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(optionFINAL));
                engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT";
                engineBSECSVFINAL.WriteFile(obj, finalarr);







            }


        }

        public void ExecuteNSEEQUITYProcessing(string[] strMTOArr, string[] strNSEArr, string strNSESEC, string strOutputFormat, string strOutputFolder)
        {
            FileHelperEngine engineMTO = new FileHelperEngine(typeof(NSEMTO));

            DelimitedClassBuilder cb = BuildNSECMPFile();
            FileHelperEngine engineCMP = new FileHelperEngine(typeof(NSECMP));

            FileHelperEngine engineSEC = new FileHelperEngine(typeof(NSESEC));

            foreach (string obj in strNSEArr)
            {

                //Get NSE Equity Filename day, month, year
                int index = obj.IndexOf("cm");

                string day = obj.Substring(index + 2, 2);
                string monthname = obj.Substring(index + 4, 3);
                string year = obj.Substring(index + 7, 4);
                int month = Convert.ToDateTime("01-" + monthname + "-2011").Month;

                if (month < 10)
                    monthname = "0";
                else
                    monthname = "";
                monthname += month.ToString();

                string MTOfilename = "MTO_" + day + monthname + year + ".DAT";

                string MTOfilenamewithpath = strMTOArr[0];

                if (!File.Exists(MTOfilenamewithpath))
                {
                    //AddMessageToLog("File " + MTOfilenamewithpath + " does not exist!");
                    continue;
                }

                NSEMTO[] resmto = engineMTO.ReadFile(MTOfilenamewithpath) as NSEMTO[];


                if (!File.Exists(obj))
                {
                    //AddMessageToLog("File " + obj + " does not exist!");
                    continue;
                }

                NSECMP[] rescmp = engineCMP.ReadFile(obj) as NSECMP[];

                if (!File.Exists(strNSESEC))
                {
                    //AddMessageToLog("File " + strNSESEC + " does not exist!");
                    continue;
                }

                NSESEC[] ressec = engineSEC.ReadFile(strNSESEC) as NSESEC[];

                int iTotalRows = rescmp.Length;


                for (int i = 0; i < iTotalRows; i++)
                {
                    if (rescmp[i].Series == "EQ" || rescmp[i].Series == "BE")
                    {

                        //Copy OI from MTO
                        for (int j = 0; j < resmto.Length; j++)
                        {
                            if ((resmto[j].NameOfSecurity == (string)rescmp[i].Symbol) && (resmto[j].series == (string)rescmp[i].Series))
                            {

                                rescmp[i].OI = resmto[j].DeliverableQty;
                                break;
                            }
                        }

                        //Copy Security Name from SEC
                        for (int j = 0; j < ressec.Length; j++)
                        {
                            if ((ressec[j].Symbol == (string)rescmp[i].Symbol))
                            {
                                rescmp[i].SecurityName = ressec[j].SecurityName;
                                break;
                            }
                        }

                    }

                }


                //engineCMP.HeaderText = "Symbol,Series,Open,High,Low,Close,Last,PrevClose,Tottrdqty,Tottrdval,Timestamp,Totaltrades,Isin,OI,SecurityName";

                //Dump File data
                engineCMP.HeaderText = "Ticker,Series,Open,High,Low,Close,Last,PrevClose,Volume,Tottrdval,Date,Totaltrades,Isin,OPENINT,NAME";
                engineCMP.WriteFile(obj, rescmp);

                int totrows = 0;

                int itmp = 0;
                int cnt = 0;
                //Calculate number of rows which have series as EQ or BE and are not NULL
                while (cnt < rescmp.Length)
                {
                    if (rescmp[cnt].Series == "EQ" || rescmp[cnt].Series == "BE")
                        totrows++;

                    cnt++;
                }

                NSECMPFINAL[] finalarr = new NSECMPFINAL[totrows];
                DateTime myDate;
                itmp = 0;
                int icntr = 0;
                while (icntr < rescmp.Length)
                {
                    if (rescmp[icntr].Series == "EQ" || rescmp[icntr].Series == "BE")
                    {
                        finalarr[itmp] = new NSECMPFINAL();
                        finalarr[itmp].Ticker = rescmp[icntr].Symbol;
                        finalarr[itmp].Name = rescmp[icntr].SecurityName;

                        myDate = DateTime.Parse(rescmp[icntr].Timestamp);
                        finalarr[itmp].Date = String.Format("{0:yyyyMMdd}", myDate);
                        finalarr[itmp].Open = rescmp[icntr].Open;
                        finalarr[itmp].High = rescmp[icntr].High;
                        finalarr[itmp].Low = rescmp[icntr].Low;
                        finalarr[itmp].Close = rescmp[icntr].Close;
                        finalarr[itmp].Volume = rescmp[icntr].Tottrdqty;
                        finalarr[itmp].OpenInt = rescmp[icntr].OI;
                        finalarr[itmp].Aux1 = rescmp[icntr].Tottrdval;



                        itmp++;
                    }
                    icntr++;
                }

                FileHelperEngine engineCMPFINAL = new FileHelperEngine(typeof(NSECMPFINAL));
                engineCMPFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,Openint,AUX1";
                engineCMPFINAL.WriteFile(obj, finalarr);

                //FileHelpers.CsvOptions options = new FileHelpers.CsvOptions("ImportRecord", ',', obj);
                //options.HeaderLines = 1;
                //FileHelperEngine test = new FileHelpers.CsvEngine(options);
                ////DataTable header = test.ReadStringAsDT(FileHelpers.CommonEngine.RawReadFirstLines(obj, 1));
                ////test.Options.IgnoreFirstLines = 0;
                //DataTable dttest = test.ReadFileAsDT(obj);


            }


        }



        private void NCDEX_Processing(string[] strBSECSVArr, string datetostore, string strOutputFolder)
        {


            FileHelperEngine engineBSECSV = new FileHelperEngine(typeof(NCDX));

            DelimitedClassBuilder cb = BuildNSECMPFile();


            foreach (string obj in strBSECSVArr)
            {

                //Get BSE Equity Filename day, month, year
                string[] words = obj.Split('\\');

                string strbseequityfilename = words[words.Length - 1];
                string strday = strbseequityfilename.Substring(2, 2);
                string strmon = strbseequityfilename.Substring(4, 2);
                string stryear = strbseequityfilename.Substring(6, 2);





                NCDX[] resbsecsv = engineBSECSV.ReadFile(obj) as NCDX[];









                int iTotalRows = resbsecsv.Length;

                List<Int32> lowvalue = new List<int> { };


                for (int i = 0; i < resbsecsv.Length; i++)
                {
                    if (resbsecsv[i].EXP_DATE.Substring(2, 1) == "/")
                    {
                        lowvalue.Add(Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(1, 1)));
                    }
                    else
                    {
                        lowvalue.Add(Convert.ToInt32(resbsecsv[i].EXP_DATE.Substring(1, 2)));

                    }
                }

                int lowmonth = lowvalue.Min();

                NCDXFINAL[] finalarr = new NCDXFINAL[resbsecsv.Length];
                int icntr = 0;
                char[] delimiterChars = { '\"', ':' };

                while (icntr < resbsecsv.Length)
                {
                    finalarr[icntr] = new NCDXFINAL();
                    finalarr[icntr].ticker = resbsecsv[icntr].SYMBOL.Trim();
                    string name = resbsecsv[icntr].Exbasis.Substring(1, resbsecsv[icntr].Exbasis.Length - 2);
                    finalarr[icntr].name = resbsecsv[icntr].Commodity.Trim() + name.Trim();
                    finalarr[icntr].name.Trim();

                    if (resbsecsv[icntr].EXP_DATE.Substring(2, 1) == "/")
                    {
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-I";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth + 1)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-II";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth + 2)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-III";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth + 3)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-IV";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth + 4)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-V";
                        }

                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth + 5)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-VI";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth + 6)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-VII";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth + 7)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-VIII";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth + 8)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-IX";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth + 9)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-X";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth + 10)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-XI";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 1)) == lowmonth + 11)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-XII";
                        }



                    }
                    else
                    {
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-I";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth + 1)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-II";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth + 2)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-III";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth + 3)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-IV";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth + 4)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-V";
                        }

                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth + 5)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-VI";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth + 6)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-VII";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth + 7)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-VIII";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth + 8)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-IX";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth + 9)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-X";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth + 10)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-XI";
                        }
                        if (Convert.ToInt32(resbsecsv[icntr].EXP_DATE.Substring(1, 2)) == lowmonth + 11)
                        {
                            finalarr[icntr].ticker = finalarr[icntr].ticker.Trim() + "-XII";
                        }



                    }







                    finalarr[icntr].date = datetostore;// String.Format("{0:yyyyMMdd}", myDate);
                    finalarr[icntr].open = resbsecsv[icntr].OPEN_PRICE;
                    finalarr[icntr].high = resbsecsv[icntr].HIGH_PRICE;
                    finalarr[icntr].low = resbsecsv[icntr].LOW_PRICE;
                    finalarr[icntr].close = resbsecsv[icntr].CLOSE_PRICE;

                    finalarr[icntr].volume = resbsecsv[icntr].TRD_VAL;

                    string na = resbsecsv[icntr].openint;
                    //string[] s = new string[1] {"" };
                    //   s[0]=na.Split(delimiterChars).ToString();


                    finalarr[icntr].openint = resbsecsv[icntr].openint;  //enint;
                    try
                    {
                        if (finalarr[icntr].openint.Contains("NA"))
                        {
                            finalarr[icntr].openint = "0"; //Convert.ToInt32(resbsecsv[icntr].openint);  //enint;
                        }
                        if (finalarr[icntr].volume.Contains("NA"))
                        {
                            finalarr[icntr].volume = "0"; //Convert.ToInt32(resbsecsv[icntr].openint);  //enint;
                        }
                        
                    }
                    catch { }


                    icntr++;
                }

                FileHelperEngine engineBSECSVFINAL = new FileHelperEngine(typeof(NCDXFINAL));
                engineBSECSVFINAL.HeaderText = "Ticker,Name,Date,Open,High,Low,Close,Volume,OPENINT";
                engineBSECSVFINAL.WriteFile(obj, finalarr);





            }



        }

        private void NSEAD_Processing(string sourcePath, string tempPath, string dateformtoprocess)
        {



            if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");


            var delimiter = ",";

            var firstLineContainsHeaders = true;
            // var tempPath =txtTargetFolder.Text+"\\Downloads\\NSE_STD.csv";
            var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");




            //NSE_std



            using (var writer = new StreamWriter(tempPath))


            // Create file as   <TICKER>,<NAME>,<DATE>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOLUME>,<OPENINT>
            // <OPENINT> is blank now 
            using (var reader = new StreamReader(sourcePath))
            {
                string line = null;
                string[] headers = new string[9] { "", "", "", "", "", "", "", "", "" };

                for (int i = 0; i < 8; i++)
                {
                    line = reader.ReadLine();

                }


                //Read Header and write into new file 
                if (firstLineContainsHeaders)
                {
                    // line = reader.ReadLine();

                    ////if (string.IsNullOrEmpty(line)) return;

                    // headers = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                    headers[0] = "TICKER";
                    headers[1] = "NAME";
                    headers[2] = "DATE";
                    headers[3] = "OPEN";
                    headers[4] = "HIGH";
                    headers[5] = "LOW";
                    headers[6] = "CLOSE";
                    headers[7] = "VOLUME";
                    headers[8] = "OPENINT";



                    writer.WriteLine(string.Join(delimiter, headers));

                }

                while ((line = reader.ReadLine()) != null)
                {

                    var columns = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                    if (columns.Count() > 1)
                    {

                        if (columns[1] == "ADVANCES" || columns[1] == "DECLINES" || columns[1] == "UNCHANGED")
                        {



                            headers[3] = columns[2];

                            headers[4] = columns[2];

                            headers[5] = columns[2];

                            headers[6] = columns[2];
                            headers[7] = "0";
                            headers[8] = "0";

                            headers[2] = dateformtoprocess;

                            headers[0] = "NSE_" + columns[1];


                            headers[1] = "NSE_" + columns[1];
                            writer.WriteLine(string.Join(delimiter, headers));


                        }


                    }











                }

            }

            string dest_filename = txtTargetFolder.Text + "\\STD_CSV\\NSE_Advance_D_nsead.csv";

            movefile(tempPath, dest_filename);



        }


        private string formatdate(DateTime day)
        {
            string date1;
            if (day.Day < 10)
            {
                date1 = "0" + day.Day.ToString();
            }
            else
            {
                date1 = day.Day.ToString();
            }

            if (day.Month < 10)
            {

                date1 = date1 + "0" + day.Month.ToString();
            }
            else
            {
                date1 = date1 + day.Month.ToString();
            }
            date1 = date1 + day.Year;
            return date1;
        }
        private void downliaddata(string path, string url)
        {


            try
            {
                prograss();
                //If Data is Not Present For Date Then  Exception Occure And It Get Added Into List Box  
                // Client.DownloadFile("http://www.mcx-sx.com/downloads/daily/EquityDownloads/Market%20Statistics%20Report_" + date1 + ".csv.", File_path);

                log4net.Config.XmlConfigurator.Configure();
                ILog log = LogManager.GetLogger(typeof(MainWindow));
                log.Debug(url + "Download Started at " + DateTime.Now.ToString("HH:mm:ss tt"));

                Client.Headers.Add("Accept", "application/zip");
                Client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                Client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1");
                Client.DownloadFile(url, path);


                log.Debug(url + "Download Completed at " + DateTime.Now.ToString("HH:mm:ss tt"));

                //string clientHeader = "DATE" + "," + "TICKER" + " " + "," + "NAME" + "," + " " + "," + " " + "," + "OPEN" + "," + "HIGH" + "," + "LOW" + "," + "CLOSE" + "," + "VOLUME" + "," + "OPENINT" + Environment.NewLine;

                //Format_Header(File_path, clientHeader);
            }
            catch (Exception ex)
            {
                if ((ex.ToString().Contains("404")) || (ex.ToString().Contains("400")))
                {
                    log4net.Config.XmlConfigurator.Configure();
                    ILog log = LogManager.GetLogger(typeof(MainWindow));
                    log.Warn("Data Not Found For " + url);

                }
            }


        }




        private void button2_Click(object sender, RoutedEventArgs e)
        {

        }



        private void dispatcherTimerForRT_Tick(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();

            if (RTD_server_name.SelectedItem == "NEST")
            {
                rtddata();
            }
            else if (RTD_server_name.SelectedItem == "NOW")
            {
                nowdata();
            }
            RtdataRecall();

        }
        private void dispatcherTimerForYahooRT_Tick(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            downliaddata(PathForYahooRT, baseurlForYahooRt);

        }

        private void RtdataRecall()
        {
            DispatcherTimer1.Tick += new EventHandler(dispatcherTimerForRT_Tick);
            DispatcherTimer1.Interval = new TimeSpan(0, 0, Convert.ToInt32(timetoRT.SelectedItem));
            DispatcherTimer1.Start();
            CommandManager.InvalidateRequerySuggested();

        }
        private void YahooRtrecall()
        {
            DispatcherTimer1.Tick += new EventHandler(dispatcherTimerForYahooRT_Tick);
            DispatcherTimer1.Interval = new TimeSpan(0, 0, 5);
            DispatcherTimer1.Start();
            CommandManager.InvalidateRequerySuggested();

        }


        public void rtddata()
        {
            CommandManager.InvalidateRequerySuggested();
            try
            {
                if (File.Exists(txtTargetFolder.Text + "\\realtimemetastock.csv"))
                {
                    File.Delete(txtTargetFolder.Text + "\\realtimemetastock.csv");
                }
                if (File.Exists(txtTargetFolder.Text + "\\realtimefchart.csv"))
                {
                    File.Copy(txtTargetFolder.Text + "\\realtimefchart.csv", txtTargetFolder.Text + "\\Finalrealtimefchart.csv",true );
                    File.Delete(txtTargetFolder.Text + "\\realtimefchart.csv");
                }
                if (File.Exists(txtTargetFolder.Text + "\\YahooRealTimeData.csv"))
                {

                    File.Delete(txtTargetFolder.Text + "\\YahooRealTimeData.csv");
                }
               
                yahoortdata.Clear();
                int flagfortotaldatacount = 0;
                using (var reader = new StreamReader(txtTargetFolder.Text + "\\NESTRt.txt"))
                {
                    string line = null;
                    int RTtopiccount = 0;
                    yahoortdata.Clear();

                    while ((line = reader.ReadLine()) != null)
                    {
                        CommandManager.InvalidateRequerySuggested();

                        yahoortname.Add(line);
                        Array retval;


                        int j = m_server.Heartbeat();

                        bool bolGetNewValue = true;
                        object[] arrayForSymbol = new object[2];

                        // RTtopiccount++;    //imp it change topic id 
                        CommandManager.InvalidateRequerySuggested();

                        arrayForSymbol[0] = line;
                        arrayForSymbol[1] = "Trading Symbol";


                        Array sysArrParams = (Array)arrayForSymbol;
                        m_server.ConnectData(RTtopiccount, sysArrParams, bolGetNewValue);

                        RTtopiccount++;    //imp it change topic id 
                        object[] arrayForLTT = new object[2];

                        CommandManager.InvalidateRequerySuggested();

                        arrayForLTT[0] = line;
                        arrayForLTT[1] = "LTT";

                        Array sysArrParams1 = (Array)arrayForLTT;
                        m_server.ConnectData(RTtopiccount, sysArrParams1, bolGetNewValue);

                        RTtopiccount++;    //imp it change topic id 

                        object[] arrayForLTP = new object[2];


                        arrayForLTP[0] = line;
                        arrayForLTP[1] = "LTP";

                        Array sysArrParams2 = (Array)arrayForLTP;
                        m_server.ConnectData(RTtopiccount, sysArrParams2, bolGetNewValue);

                        CommandManager.InvalidateRequerySuggested();

                        RTtopiccount++;    //imp it change topic id 

                        CommandManager.InvalidateRequerySuggested();


                        object[] arrayForVolume = new object[2];

                        arrayForVolume[0] = line;
                        arrayForVolume[1] = "Volume Traded Today";

                        Array sysArrParams3 = (Array)arrayForVolume;
                        m_server.ConnectData(RTtopiccount, sysArrParams3, bolGetNewValue);

                        CommandManager.InvalidateRequerySuggested();

                        RTtopiccount++;    //imp it change topic id 
                        object[] arrayForopenint = new object[2];

                        arrayForopenint[0] = line;
                        arrayForopenint[1] = "Open Interest";

                        Array sysArrParams4 = (Array)arrayForopenint;
                        m_server.ConnectData(RTtopiccount, sysArrParams4, bolGetNewValue);


                        RTtopiccount++;    //imp it change topic id 

                        CommandManager.InvalidateRequerySuggested();

                        retval = m_server.RefreshData(10);


                        for (int count = 0; count <= 4; count++)
                        {
                            m_server.DisconnectData(count);
                        }
                        foreach (var item in retval)
                        {

                            yahoortdata.Add(item.ToString());
                            CommandManager.InvalidateRequerySuggested();

                        }

                        m_server.ServerTerminate();
                        flagfortotaldatacount++;
                        CommandManager.InvalidateRequerySuggested();


                    }
                    CommandManager.InvalidateRequerySuggested();

                    string tempfilepath = txtTargetFolder.Text + "\\YahooRealTimeData.txt";
                    //log4net.Config.XmlConfigurator.Configure();
                    //ILog log = LogManager.GetLogger(typeof(MainWindow));
                    //log.Debug("Data Capturing At" + DateTime.Now.TimeOfDay);
                    string storeinfile1 = "";
                    CommandManager.InvalidateRequerySuggested();

                    //c=c+2 we not want 1st 3rd 5th and so on values.
                    int value = 5;
                    int flagtocheckfirstvaluefordate = 0;

                    for (int j = 5; j < yahoortdata.Count - 1; j = j + 10)
                    {
                        CommandManager.InvalidateRequerySuggested();

                        int c;
                        value = j + 5;
                        if (flagtocheckfirstvaluefordate == 0)
                        {
                            storeinfile1 = DateTime.Today.Date.ToShortDateString() + storeinfile1;
                            flagtocheckfirstvaluefordate = 1;
                            CommandManager.InvalidateRequerySuggested();

                        }
                        else
                        {
                            storeinfile1 = storeinfile1 + "," + DateTime.Today.Date.ToShortDateString();

                            flagtocheckfirstvaluefordate = 1;

                        }
                        for (c = j; c <= value - 1; c = c + 1)
                        {
                            CommandManager.InvalidateRequerySuggested();

                            storeinfile1 = storeinfile1 + "," + yahoortdata[c].ToString();

                        }


                        CommandManager.InvalidateRequerySuggested();

                        storeinfile1 = storeinfile1 + "\r\n";


                    }



                    //if count is greater than data required then dont write it in file
                    if (yahoortdata.Count <= flagfortotaldatacount * 10)
                    {
                        //<TICKER>,<NAME>,<PER>,<DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>,<OPENINT>
                        using (var writer = new StreamWriter(tempfilepath))

                            writer.WriteLine(storeinfile1);

                        if (Format_cb.SelectedItem == "Amibroker")
                        {
                            string realtimemetastock = "";

                            string datastoreforami="";
                            int count = 0;
                            using (var reader1 = new StreamReader(tempfilepath))
                            {
                                string line1 = null;
                                while ((line1 = reader1.ReadLine()) != null)
                                {

                                    string[] words = line1.Split(',');
                                    if (line1 != "")
                                    {
                                        if (count == 0)
                                        {
                                            realtimemetastock = realtimemetastock + " " + words[0] + " " + words[1] + " " + words[2] + " " + words[3] + " " + words[4] + " " + words[5];
                                            count++;
                                        }
                                        else
                                        {
                                            realtimemetastock = realtimemetastock + " " + words[1] + " " + words[2] + " " + words[3] + " " + words[4] + " " + words[5] + " " + words[6];


                                        }
                                        realtimemetastock = realtimemetastock + "\r\n";
                                    }
                                }
                            }


                            string filename = txtTargetFolder.Text + "\\AmibrokerRTdata.txt";
                                    //   System.Windows.MessageBox.Show(realtimemetastock);
                                    using (var writer = new StreamWriter(filename))
                                        writer.WriteLine(realtimemetastock);

                        }



                        if (Format_cb.SelectedItem == "Fchart")
                        {
                            int count = 0;
                            using (var reader1 = new StreamReader(tempfilepath))
                            {
                                string line1 = null;
                                string realtimemetastock = "";
                                while ((line1 = reader1.ReadLine()) != null)
                                {

                                    string[] words = line1.Split(',');
                                    if (line1 != "")
                                    {
                                        if (count == 0)
                                        {
                                            realtimemetastock = words[1] + "," + words[1] + "," + "I" + "," + words[0] + "," + words[2] + "," + words[3] + "," + words[3] + "," + words[3] + "," + words[3] + "," + words[4] + "," + words[5];
                                            count++;
                                        }
                                        else
                                        {
                                            realtimemetastock = words[2] + "," + words[2] + "," + "I" + "," + words[1] + "," + words[3] + "," + words[4] + "," + words[4] + "," + words[4] + "," + words[4] + "," + words[5] + "," + words[6];

                                        }
                                        string filename = txtTargetFolder.Text + "\\realtimefchart.csv";
                                        //   System.Windows.MessageBox.Show(realtimemetastock);
                                        using (var writer = new StreamWriter(filename, true))
                                            writer.WriteLine(realtimemetastock);
                                    }
                                }
                            }

                        }
                       
                        if (Format_cb.SelectedItem == "Metastock")
                        {
                            int count = 0;
                            string filename="";
                            using (var reader1 = new StreamReader(tempfilepath))
                            {
                                string line1 = null;
                                string realtimemetastock = "";
                                while ((line1 = reader1.ReadLine()) != null)
                                {

                                    if(line1!="")
                                    {
                                        string[] words = line1.Split(',');

                                    if (count == 0)
                                    {
                                         realtimemetastock = words[1] + "," + words[1] + "," + "I" + "," + words[0] + "," + words[2] + "," + words[3] + "," + words[3] + "," + words[3] + "," + words[3] + "," + words[4] + "," + words[5];
                                        count++;
                                    }
                                    else
                                    {
                                         realtimemetastock = words[2] + "," + words[2] + "," + "I" + "," + words[1] + "," + words[3] + "," + words[4] + "," + words[4] + "," + words[4] + "," + words[4] + "," + words[5] + "," + words[6];

                                    }
                                     filename = txtTargetFolder.Text + "\\realtimemetastock.csv";
                                    //   System.Windows.MessageBox.Show(realtimemetastock);
                                    using (var writer = new StreamWriter(filename, true))
                                        writer.WriteLine(realtimemetastock);
                                    }
                                }
                            }
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Intraday\\Metastock"))
                                {
                                    Directory.CreateDirectory(txtTargetFolder.Text + "\\Intraday\\Metastock");
                                }
                                // commandpromptcall(filename, txtTargetFolder.Text + "\\Intraday\\Metastock\\realtimemetastock");
                                try
                                {

                                    string filepath = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString();
                                    string processtostart = filepath.Substring(0, filepath.Length - 12) + "asc2ms.exe";

                                    File.Copy(processtostart, txtTargetFolder.Text + "\\asc2ms.exe",true );
                                }
                                catch
                                {
                                }    
                            System.Diagnostics.Process process = new System.Diagnostics.Process();
                                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                startInfo.FileName = "cmd.exe";
                                //startInfo.Arguments = "/C  C:\\asc2ms.exe -f C:\\data\\Metastock\\M.csv -r r -o C:\\data\\Metastock\\google\\e";
                                startInfo.Arguments = "/C  "+txtTargetFolder.Text+"\\asc2ms.exe -f " + filename  + " -r r -o " + txtTargetFolder.Text + "\\Intraday\\Metastock\\realtimemetastock --forceWrite=yes";
                                // startInfo.Arguments = @"/C  C:\asc2ms.exe -f C:\Documents and Settings\maheshwar\My Documents\BSe\Downloads\Googleeod -r r -o C:\Documents and Settings\maheshwar\My Documents\BSe\Downloads\Googleeod\Metastock\a" ;



                                process.StartInfo = startInfo;
                                process.Start();
                            
                        }
                    }

                    CommandManager.InvalidateRequerySuggested();


                    
                    if (Format_cb.SelectedItem == "Amibroker")
                    {
                    ExcelType.InvokeMember("Import", BindingFlags.InvokeMethod | BindingFlags.Public, null,
                         ExcelInst, args);


                    CommandManager.InvalidateRequerySuggested();

                    ExcelType.InvokeMember("RefreshAll", BindingFlags.InvokeMethod | BindingFlags.Public, null,
                           ExcelInst, new object[1] { "" });
}
                }
            }
            catch
            {
                CommandManager.InvalidateRequerySuggested();

                log4net.Config.XmlConfigurator.Configure();
                ILog log = LogManager.GetLogger(typeof(MainWindow));
                log.Debug("Error While Data Capture ....");

                CommandManager.InvalidateRequerySuggested();

            }
        }
        public void nowdata()
        {
            CommandManager.InvalidateRequerySuggested();
            try
            {
                if (File.Exists(txtTargetFolder.Text + "\\realtimemetastock.csv"))
                {
                    File.Delete(txtTargetFolder.Text + "\\realtimemetastock.csv");
                }
                if (File.Exists(txtTargetFolder.Text + "\\realtimemetastock.csv"))
                {
                    File.Copy(txtTargetFolder.Text + "\\realtimefchart.csv", txtTargetFolder.Text + "\\Finalrealtimefchart.csv", true);
                    File.Delete(txtTargetFolder.Text + "\\realtimefchart.csv");
                }
                if (File.Exists(txtTargetFolder.Text + "\\YahooRealTimeData.csv"))
                {
                    File.Delete(txtTargetFolder.Text + "\\YahooRealTimeData.csv");
                }
                yahoortdata.Clear();
                int flagfortotaldatacount = 0;
                using (var reader = new StreamReader(txtTargetFolder.Text + "\\NESTRt.txt"))
                {
                    string line = null;
                    int RTtopiccount = 0;
                    yahoortdata.Clear();

                    while ((line = reader.ReadLine()) != null)
                    {
                        CommandManager.InvalidateRequerySuggested();

                        yahoortname.Add(line);
                        Array retval;


                        int j = m_server.Heartbeat();

                        bool bolGetNewValue = true;
                        object[] arrayForSymbol = new object[3];

                        // RTtopiccount++;    //imp it change topic id 
                        CommandManager.InvalidateRequerySuggested();

                        arrayForSymbol[0] = "MktWatch";

                        arrayForSymbol[1] = line;
                        arrayForSymbol[2] = "Trading Symbol";


                        Array sysArrParams = (Array)arrayForSymbol;
                        m_server.ConnectData(RTtopiccount, sysArrParams, bolGetNewValue);

                        RTtopiccount++;    //imp it change topic id 
                        object[] arrayForLTT = new object[3];

                        CommandManager.InvalidateRequerySuggested();

                        arrayForLTT[0] = "MktWatch";
                        
                        arrayForLTT[1] = line;
                        arrayForLTT[2] = "Last Trade Time";

                        Array sysArrParams1 = (Array)arrayForLTT;
                        m_server.ConnectData(RTtopiccount, sysArrParams1, bolGetNewValue);

                        RTtopiccount++;    //imp it change topic id 

                        object[] arrayForLTP = new object[3];

                        arrayForLTP[0] = "MktWatch";

                        arrayForLTP[1] = line;
                        arrayForLTP[2] = "Last Traded Price";

                        Array sysArrParams2 = (Array)arrayForLTP;
                        m_server.ConnectData(RTtopiccount, sysArrParams2, bolGetNewValue);

                        CommandManager.InvalidateRequerySuggested();

                        RTtopiccount++;    //imp it change topic id 

                        CommandManager.InvalidateRequerySuggested();


                        object[] arrayForVolume = new object[3];
                        arrayForVolume[0] = "MktWatch";

                        arrayForVolume[1] = line;
                        arrayForVolume[2] = "Volume Traded Today";

                        Array sysArrParams3 = (Array)arrayForVolume;
                        m_server.ConnectData(RTtopiccount, sysArrParams3, bolGetNewValue);

                        CommandManager.InvalidateRequerySuggested();

                        RTtopiccount++;    //imp it change topic id 
                        object[] arrayForopenint = new object[3];
                        arrayForopenint[0] = "MktWatch";

                        arrayForopenint[1] = line;
                        arrayForopenint[2] = "Open Interest";

                        Array sysArrParams4 = (Array)arrayForopenint;
                        m_server.ConnectData(RTtopiccount, sysArrParams4, bolGetNewValue);


                        RTtopiccount++;    //imp it change topic id 

                        CommandManager.InvalidateRequerySuggested();

                        retval = m_server.RefreshData(10);


                        for (int count = 0; count <= 4; count++)
                        {
                            m_server.DisconnectData(count);
                        }
                        foreach (var item in retval)
                        {

                            yahoortdata.Add(item.ToString());
                            CommandManager.InvalidateRequerySuggested();

                        }

                        m_server.ServerTerminate();
                        flagfortotaldatacount++;
                        CommandManager.InvalidateRequerySuggested();


                    }
                    CommandManager.InvalidateRequerySuggested();

                    string tempfilepath = txtTargetFolder.Text + "\\YahooRealTimeData.txt";
                    //log4net.Config.XmlConfigurator.Configure();
                    //ILog log = LogManager.GetLogger(typeof(MainWindow));
                    //log.Debug("Data Capturing At" + DateTime.Now.TimeOfDay);
                    string storeinfile1 = "";
                    CommandManager.InvalidateRequerySuggested();

                    //c=c+2 we not want 1st 3rd 5th and so on values.
                    int value = 5;
                    int flagtocheckfirstvaluefordate = 0;

                    for (int j = 5; j < yahoortdata.Count - 1; j = j + 10)
                    {
                        CommandManager.InvalidateRequerySuggested();

                        int c;
                        value = j + 5;
                        if (flagtocheckfirstvaluefordate == 0)
                        {
                            storeinfile1 = DateTime.Today.Date.ToShortDateString() + storeinfile1;
                            flagtocheckfirstvaluefordate = 1;
                            CommandManager.InvalidateRequerySuggested();

                        }
                        else
                        {
                            storeinfile1 = storeinfile1 + "," + DateTime.Today.Date.ToShortDateString();

                            flagtocheckfirstvaluefordate = 1;

                        }
                        for (c = j; c <= value - 1; c = c + 1)
                        {
                            CommandManager.InvalidateRequerySuggested();

                            storeinfile1 = storeinfile1 + "," + yahoortdata[c].ToString();

                        }


                        CommandManager.InvalidateRequerySuggested();


                        //////////////////////////////////////




                        



                        storeinfile1 = storeinfile1 + "\r\n";


                    }



                    //if count is greater than data required then dont write it in file
                    if (yahoortdata.Count <= flagfortotaldatacount * 10)
                    {
                        //<TICKER>,<NAME>,<PER>,<DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>,<OPENINT>
                        using (var writer = new StreamWriter(tempfilepath))

                            writer.WriteLine(storeinfile1);

                        if (Format_cb.SelectedItem == "Amibroker")
                        {
                            int count = 0;
                            using (var reader1 = new StreamReader(tempfilepath))
                            {
                                string line1 = null;
                                string realtimemetastock = "";
                                while ((line1 = reader1.ReadLine()) != null)
                                {

                                    string[] words = line1.Split(',');
                                    if (count == 0)
                                    {
                                        realtimemetastock = words[1] + "," + words[1] + "," + "I" + "," + words[0] + "," + words[2] + "," + words[3] + "," + words[3] + "," + words[3] + "," + words[3] + "," + words[4] + "," + words[5];
                                        count++;
                                    }
                                    else
                                    {
                                        realtimemetastock = words[2] + "," + words[2] + "," + "I" + "," + words[1] + "," + words[3] + "," + words[4] + "," + words[4] + "," + words[4] + "," + words[4] + "," + words[5] + "," + words[6];

                                    }
                                    string filename = txtTargetFolder.Text + "\\YahooRealTimeData.csv";
                                    //   System.Windows.MessageBox.Show(realtimemetastock);
                                    using (var writer = new StreamWriter(filename, true))
                                        writer.WriteLine(realtimemetastock);
                                }
                            }

                        }

                        if (Format_cb.SelectedItem == "Fchart")
                        {
                            int count = 0;
                            using (var reader1 = new StreamReader(tempfilepath))
                            {
                                string line1 = null;
                                string realtimemetastock = "";
                                while ((line1 = reader1.ReadLine()) != null)
                                {

                                    string[] words = line1.Split(',');
                                    if (count == 0)
                                    {
                                        realtimemetastock = words[1] + "," + words[1] + "," + "I" + "," + words[0] + "," + words[2] + "," + words[3] + "," + words[3] + "," + words[3] + "," + words[3] + "," + words[4] + "," + words[5];
                                        count++;
                                    }
                                    else
                                    {
                                        realtimemetastock = words[2] + "," + words[2] + "," + "I" + "," + words[1] + "," + words[3] + "," + words[4] + "," + words[4] + "," + words[4] + "," + words[4] + "," + words[5] + "," + words[6];

                                    }
                                    string filename = txtTargetFolder.Text + "\\realtimefchart.csv";
                                    //   System.Windows.MessageBox.Show(realtimemetastock);
                                    using (var writer = new StreamWriter(filename, true))
                                        writer.WriteLine(realtimemetastock);
                                }
                            }

                        }

                        if (Format_cb.SelectedItem == "Metastock")
                        {
                            int count = 0;
                            string filename = "";
                            using (var reader1 = new StreamReader(tempfilepath))
                            {
                                string line1 = null;
                                string realtimemetastock = "";
                                while ((line1 = reader1.ReadLine()) != null)
                                {

                                    if (line1 != "")
                                    {
                                        string[] words = line1.Split(',');

                                        if (count == 0)
                                        {
                                            realtimemetastock = words[1] + "," + words[1] + "," + "I" + "," + words[0] + "," + words[2] + "," + words[3] + "," + words[3] + "," + words[3] + "," + words[3] + "," + words[4] + "," + words[5];
                                            count++;
                                        }
                                        else
                                        {
                                            realtimemetastock = words[2] + "," + words[2] + "," + "I" + "," + words[1] + "," + words[3] + "," + words[4] + "," + words[4] + "," + words[4] + "," + words[4] + "," + words[5] + "," + words[6];

                                        }
                                        filename = txtTargetFolder.Text + "\\realtimemetastock.csv";
                                        //   System.Windows.MessageBox.Show(realtimemetastock);
                                        using (var writer = new StreamWriter(filename, true))
                                            writer.WriteLine(realtimemetastock);
                                    }
                                }
                            }
                            if (!Directory.Exists(txtTargetFolder.Text + "\\Intraday\\Metastock"))
                            {
                                Directory.CreateDirectory(txtTargetFolder.Text + "\\Intraday\\Metastock");
                            }
                            // commandpromptcall(filename, txtTargetFolder.Text + "\\Intraday\\Metastock\\realtimemetastock");
                            System.Diagnostics.Process process = new System.Diagnostics.Process();
                            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            startInfo.FileName = "cmd.exe";
                            //startInfo.Arguments = "/C  C:\\asc2ms.exe -f C:\\data\\Metastock\\M.csv -r r -o C:\\data\\Metastock\\google\\e";
                            startInfo.Arguments = "/C  C:\\asc2ms.exe -f " + filename + " -r r -o " + txtTargetFolder.Text + "\\Intraday\\Metastock\\realtimemetastock --forceWrite=yes";
                            // startInfo.Arguments = @"/C  C:\asc2ms.exe -f C:\Documents and Settings\maheshwar\My Documents\BSe\Downloads\Googleeod -r r -o C:\Documents and Settings\maheshwar\My Documents\BSe\Downloads\Googleeod\Metastock\a" ;



                            process.StartInfo = startInfo;
                            process.Start();

                        }

                       
                    }

                    CommandManager.InvalidateRequerySuggested();



                    if (Format_cb.SelectedItem == "Amibroker")
                    {
                        ExcelType.InvokeMember("Import", BindingFlags.InvokeMethod | BindingFlags.Public, null,
                             ExcelInst, args);


                        CommandManager.InvalidateRequerySuggested();

                        ExcelType.InvokeMember("RefreshAll", BindingFlags.InvokeMethod | BindingFlags.Public, null,
                               ExcelInst, new object[1] { "" });
                    }
                }
            }
            catch
            {
                CommandManager.InvalidateRequerySuggested();

                log4net.Config.XmlConfigurator.Configure();
                ILog log = LogManager.GetLogger(typeof(MainWindow));
                log.Debug("Error While Data Capture ....");

                CommandManager.InvalidateRequerySuggested();

            }
        }

        private void LoadTree(SystemAccessibleObject sao)
        {
            CommandManager.InvalidateRequerySuggested();

            

            if (sao == null) return;
            IntPtr hwnd = sao.Window.HWnd;
            List<SystemAccessibleObject> parents = new List<SystemAccessibleObject>();
            parents.Add(sao);
            while (true)
            {
                sao = sao.Parent;
                if (sao == null) break;
                if (sao.Window.HWnd != hwnd) break;
                parents.Add(sao);
            }
            sao = parents[parents.Count - 1];
            parents.RemoveAt(parents.Count - 1);
            TreeViewItem curr = new TreeViewItem();
            int i = 0;
            sao = parents[parents.Count - 1];

            // List<string> symbolname = new List<String>();

            CommandManager.InvalidateRequerySuggested();

            symbolname.Clear();
            exchagename.Clear();
            dataGridforsymbol.Items.Clear();
           
            SystemAccessibleObject[] exchange = parents[1].Children;
            for (i = 0; i < sao.Children.Count() - 1; i++)
            {
               
                symbolname.Add(sao.Children.GetValue(i).ToString());
                string s = exchange[i].Description;
              
                string[] words = s.Split(',');

             int   i1 = words.Count();
             string s1 = words[i1 - 1];

                string[] words1 = s1.Split(':');


                if (words1[1].Trim() == "MCX")
                {
                    exchagename.Add("mcx_fo");

                }
                else if (words1[1].Trim() == "NSE")
                {
                    exchagename.Add("nse_cm");

                }
                else if (words1[1].Trim() == "NFO")
                {
                    exchagename.Add("nse_fo");

                }
                else if (words1[1].Trim() == "CDS")
                {
                    exchagename.Add("cde_fo");

                }



             //   System.Windows.MessageBox.Show(words1[1]);

            }

            string symboltowriteinfile = "";
            for (i = 0; i < sao.Children.Count() - 1; i++)
            {

                string searchWithinThis = symbolname[i];
                string searchForThis = "[";
                int firstCharacter = searchWithinThis.IndexOf(searchForThis);

                symbolname[i] = symbolname[i].Substring(0, firstCharacter);


                symboltowriteinfile = symboltowriteinfile + symbolname[i].Trim() + "\r\n";
                dataGridforsymbol.Items.Add(new Dataitemforsymbol { Column0 = "", Column1 = symbolname[i], Column2 = exchagename[i] });


            }


            //    System.IO.File.WriteAllText(txtTargetFolder.Text+ "//YahooRt.txt", symboltowriteinfile.Trim());








        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            // Lbl_internet.Content = DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
            url1 = "http://www.google.com";
            Check_internet_connetion(url1);

        }




        private void Check_internet_connetion(string url)
        {
            //Check Internet Connection Is Present Or Not


            //if (Net_Connection.Fill == "#FFF21C1C")
            //{
            DispatcherTimer DispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();

            try
            {
                System.Net.WebRequest myRequest = System.Net.WebRequest.Create(url);
                System.Net.WebResponse myResponse = myRequest.GetResponse();
                Net_Connection.Fill = new SolidColorBrush(Colors.Green);
                //Connection is ok time stop
                DispatcherTimer1.Stop();
            }
            catch (System.Net.WebException)
            {
                Net_Connection.Fill = new SolidColorBrush(Colors.Red);
                DispatcherTimer1.Tick += new EventHandler(dispatcherTimer_Tick);
                DispatcherTimer1.Interval = new TimeSpan(0, 0, 10);
                DispatcherTimer1.Start();
            }


        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            //     var row = GetDataGridRows(dataGrid3 ); 
            //    DataGridRow=

            //if (((System.Windows.Controls. CheckBox)sender).IsChecked == true)  
            //{
            //   SetCheckbox(row, true);  

            //}  
            //else  
            //{  
            // SetCheckbox(row, false);  
            //}  

        }
        //individual checking of checkbox  

        private void chebox_Click(object sender, RoutedEventArgs e)
        {
            object a = e.Source;
            System.Windows.Controls.CheckBox chk = (System.Windows.Controls.CheckBox)sender;
            if (chk.IsChecked == true)
            {

                yahoosymbolindextoremove.Add(dataGrid5.SelectedIndex);

            }


        }
        private void marketsymbolremove_Click(object sender, RoutedEventArgs e)
        {
            object a = e.Source;
            System.Windows.Controls.CheckBox chk = (System.Windows.Controls.CheckBox)sender;
            if (chk.IsChecked == true)
            {

                //  marketsymboltoremove.Add(savesymbol .SelectedIndex);

            }


        }


        private void chkDiscontinue_Click(object sender, RoutedEventArgs e)
        {
            object a = e.Source;
            System.Windows.Controls.CheckBox chk = (System.Windows.Controls.CheckBox)sender;
            if (chk.IsChecked == true)
            {
                //DataGridColumn column in dataGr


                dataGrid3.SelectedItem.ToString();

                DataItem row = (DataItem)dataGrid3.SelectedItems[0];
                var a1 = dataGrid3.Columns[3];




                YahooSymbolsave.Add(row.Column1);
                YahooNamesave.Add(row.Column2);
                YahooExchangesave.Add(row.Column3);



            }

        }
        private void symbolsave_Click(object sender, RoutedEventArgs e)
        {
            object a = e.Source;
            System.Windows.Controls.CheckBox chk = (System.Windows.Controls.CheckBox)sender;
            if (chk.IsChecked == true)
            {
                //DataGridColumn column in dataGr

                dataGridforsymbol.SelectedItem.ToString();


               // exchagenameenterbyuser = Interaction.InputBox("Enter Exchange Name For Selected Symbol", "Exchang name", "", -1, -1);

                Dataitemforsymbol row = (Dataitemforsymbol)dataGridforsymbol.SelectedItems[0];
               // var a1 = dataGridforsymbol.Columns[1];



                marketsymbol.Add(row.Column1);

                Exchangename.Add(row.Column2 );

            }

        }
        //loop through each row and change the checkbox value  
        private void SetCheckbox(IEnumerable<DataGridRow> row, bool value)
        {
            foreach (DataGridRow r in row)
            {
                DataRowView rv = (DataRowView)r.Item;
                foreach (DataGridColumn column in dataGrid3.Columns)
                {
                    if (column.GetType().Equals(typeof(DataGridTemplateColumn)))
                    {
                        rv.Row["Discontinue"] = value;


                    }
                }
            }
        }





        private void SendMail(string p_sEmailTo, string subject, string messageBody, bool isHtml)
        {
            var fromAddress = new MailAddress("webmaster@shubhalabha.in", "From Name");
            var toAddress = new MailAddress(p_sEmailTo, "To Name");
            subject = "Your Password";
            string body = "This is your password:" + subject + "\n ";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                //EnableSsl = false ,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //UseDefaultCredentials = false ,
                //Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        public void SetRegKey()
        {

            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.CreateSubKey(@"Software\");
            regKey.SetValue("ApplicationID", AppId);

        }


        public void GetRegKey()
        {


            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.CreateSubKey(@"Software\");

            try
            {
                object unm = regKey.GetValue("ApplicationID");

                string a = "1";
                string b = unm.ToString();

                if (unm != null)
                    //username .Text = regKey.GetValue("UserName").ToString();


                    if (b == a)
                    {
                        System.Windows.MessageBox.Show("Already Present ");
                    }


            }
            catch 
            {

            }
        }
        void wb_LoadCompleted(object sender, NavigationEventArgs e)
        {
            string script = "document.body.style.overflow ='hidden'";
            System.Windows.Controls.WebBrowser wb = (System.Windows.Controls.WebBrowser)sender;
            wb.InvokeScript("execScript", new Object[] { script, "JavaScript" });
        }

        private void wMain_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                string filepath = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString();
                string processtostart = filepath.Substring(0, filepath.Length - 12) + "asc2ms.exe";

                File.Copy(processtostart, "C:\\Windows\\System32\\asc2ms.exe",true );
               // File.Copy(processtostart, "C:\\Windows\\SysWOW64\\asc2ms.exe");


                processtostart = filepath.Substring(0, filepath.Length - 12) + "pthread.dll";

                File.Copy(processtostart, "C:\\pthread.dll", true);
                File.Copy(processtostart, "C:\\Windows\\System32\\pthread.dll", true);
               // File.Copy(processtostart, "C:\\Windows\\SysWOW64\\pthread.dll", true);

                processtostart = filepath.Substring(0, filepath.Length - 12) + "pthreadGC2.dll";

                File.Copy(processtostart, "C:\\Windows\\System32\\pthreadGC2.dll", true);
               // File.Copy(processtostart, "C:\\Windows\\SysWOW64\\pthreadGC2.dll", true);



            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message );
            }




            var delimiter = ",";

            var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");
           


            CommandManager.InvalidateRequerySuggested();

            try
            {
                System.Net.WebRequest myRequest = System.Net.WebRequest.Create("http://www.Google.co.in");
                System.Net.WebResponse myResponse = myRequest.GetResponse();


                Uri a= new System.Uri("http://shubhalabha.in/eng/ads/www/delivery/afr.php?zoneid=18&amp;target=_blank&amp;cb=INSERT_RANDOM_NUMBER_HERE");
                Uri a1 = new System.Uri("http://shubhalabha.in/eng/ads/www/delivery/afr.php?zoneid=17&amp;target=_blank&amp;cb=INSERT_RANDOM_NUMBER_HERE");
                Uri a2 = new System.Uri("http://shubhalabha.in/eng/ads/www/delivery/afr.php?zoneid=17&amp;target=_blank&amp;cb=INSERT_RANDOM_NUMBER_HERE");
                Uri a3 = new System.Uri("http://shubhalabha.in/eng/ads/www/delivery/afr.php?zoneid=17&amp;target=_blank&amp;cb=INSERT_RANDOM_NUMBER_HERE");
                wad .Source = a;
                wad1.Source = a1;
              
               wad2.Source = a2;
                wad3.Source = a3;
                //  wad4.Source = a4;


            }
            catch
            {


                wad.Visibility = Visibility.Hidden;
                wad1.Visibility = Visibility.Hidden;
                wad2.Visibility = Visibility.Hidden;
                wad3.Visibility = Visibility.Hidden;

            }
            RTD_server_name.Items.Add("NEST");
            RTD_server_name.Items.Add("NOW");


            comboBox1.Items.Add("CSV");
            comboBox1.Items.Add("FCharts");
            comboBox1.Items.Add("Amibroker");
            comboBox1.Items.Add("AdvanceGet");
            comboBox1.Items.Add("Metastock");
            selectfilebluk.Items.Add("NSE_bulk");
            selectfilebluk.Items.Add("NSE_block");
            selectfilebluk.Items.Add("BSE_bulk");
            selectfilebluk.Items.Add("BSE_block");
            Format_cb.Items.Add("Amibroker");
            Format_cb.Items.Add("Metastock");
            Format_cb.Items.Add("Fchart");

            Format_cb.SelectedIndex = 1;



            


            for (int i = 1; i < 60; i++)
            {
                timetoRT.Items.Add(i);
            }
            for (int i = 0; i < 12; i++)
            {
                GHRS.Items.Add(i);
            }

            for (int i = 0; i < 60; i++)
            {
                GMIN.Items.Add(i);
            }
            for (int i = 1; i < 50; i++)
            {
                Daysforgoogle.Items.Add(i);
            }


            for (int i = 0; i < 7; i++)
            {
                string[] nameofcol = new string[8] { "Date", "Symbol", "Security", "Client", "Buy", "Quantity Traded", "Trade", "Remarks" };


                var column = new DataGridTextColumn();
                //if (i < 4)
                //{
                column.Header = nameofcol[i];
                //}
                column.Binding = new System.Windows.Data.Binding("Column" + i);
                market.Columns.Add(column);
                // savesymbol.Columns.Add(column);


            }


            for (int i = 0; i < 3; i++)
            {
                string[] nameofcol = new string[3] { "", "Symbol" ,"Exchange"};


                var column = new DataGridTextColumn();
                //if (i < 4)
                //{
                column.Header = nameofcol[i];
                //}
                column.Binding = new System.Windows.Data.Binding("Column" + i);
                dataGridforsymbol.Columns.Add(column);
                // savesymbol.Columns.Add(column);


            }

            try
            {


                using (var reader = new StreamReader("C:\\Shubha\\YahooAll.csv"))
                {
                    string line = null;
                    string[] headers = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        headers = splitExpression.Split(line).Where(s => s != delimiter).ToArray();
                        dataGrid5.Items.Add(new DataItem { Column0 = "", Column1 = headers[0], Column2 = headers[1], Column3 = headers[2] });


                    }
                }

            }
            catch
            {

            }
            for (int i = 0; i < 2; i++)
            {
                string[] nameofcol = new string[2] { "", "Symbol" };


                var column = new DataGridTextColumn();
                //if (i < 4)
                //{
                column.Header = nameofcol[i];
                //}
                column.Binding = new System.Windows.Data.Binding("Column" + i);
                // dataGridforsymbol.Columns.Add(column);
                //  savesymbol.Columns.Add(column);


            }
            for (int i = 0; i < 4; i++)
            {
                string[] nameofcol = new string[4] { "", "Symbol", "Company Name", "Exchange" };


                var column = new DataGridTextColumn();
                //if (i < 4)
                //{
                column.Header = nameofcol[i];
                //}
                column.Binding = new System.Windows.Data.Binding("Column" + i);
                dataGrid5.Columns.Add(column);

            }

            for (int i = 0; i < 4; i++)
            {
                string[] nameofcol = new string[4] { "", "Symbol", "Company Name", "Exchange" };


                var column = new DataGridTextColumn();
                //if (i < 4)
                //{
                column.Header = nameofcol[i];
                //}
                column.Binding = new System.Windows.Data.Binding("Column" + i);
                dataGrid3.Columns.Add(column);

            }


            List<string> YahooSymbol = new List<string>();
            List<string> YahooName = new List<string>();

            List<string> YahooExchange = new List<string>();

            YahooSymbol.Clear();

            dataGrid3.Items.Clear();

             delimiter = ",";

            splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");

            try
            {
                using (var reader = new StreamReader(txtTargetFolder.Text + "\\Downloads\\yahoo.txt"))
                {
                    string line = null;
                    string[] headers = null;
                    int i = 0;
                    string name = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        headers = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                        i = 1;

                        for (i = 1; i < headers.Count() - 1; i = i + 6)
                        {
                            char[] delimiterChars = { '\"', ':' };
                            name = headers[i] + headers[i + 1] + headers[i + 2];
                            string[] words = name.Split(delimiterChars); //+ headers[i + 1].Split(delimiterChars) + headers[i + 2].Split(delimiterChars);
                            YahooSymbol.Add(words[4]);
                            YahooName.Add(words[9]);
                            YahooExchange.Add(words[14]);


                            //"{\"symbol\":\"INFY.NS\"\"name\": \"INFOSYS LIMITED\"\"exch\": \"NSI\""

                        }



                        for (i = 1; i < YahooSymbol.Count; i++)
                        {

                            dataGrid5.Items.Add(new DataItem { Column0 = "", Column1 = YahooSymbol[i], Column2 = YahooName[i], Column3 = YahooExchange[i] });
                        }



                    }
                }
            }
            catch
            {
            }



            // Your programmatically created DataGrid is attached to MainGrid here


            // create four columns here with same names as the DataItem's properties
            //string[] nameofcol = new string[3] { "Symbol", "Company Name", "Exchange" };

            //for (int i = 0; i <= 2; ++i)
            //{
            //    var column = new DataGridTextColumn();
            //    column.Header = nameofcol[i];
            //    column.Binding = new System.Windows.Data.Binding("Column" + i);
            //    dataGrid3.Columns.Add(column);
            //}

            //// create and add two lines of fake data to be displayed, here
            //dataGrid3.Items.Add(new DataItem { Column0 = "a.1", Column1 = "a.2", Column2 = "a.3" });
            //dataGrid3.Items.Add(new DataItem { Column0 = "b.1", Column1 = "b.2", Column2 = "b.3" });




            for (int i = 1; i < 12; i++)
            {
                HRS.Items.Add(i);
            }
            for (int i = 1; i < 60; i++)
            {
                MIN.Items.Add(i);
            }
            btnExit.IsEnabled = false;
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(typeof(MainWindow));
            log.Debug("Application Strated Successfully");


            dtEndDate.Text = DateTime.Today.Date.ToShortDateString();
            dtStartDate.Text = DateTime.Today.Date.ToShortDateString();


            string chktmp = ConfigurationManager.AppSettings["txtTargetFolder"];
            bool btemp = false;

            if(chktmp==null )
            {
                this.txtTargetFolder.Text  = "C:\\Data";
            }
            {
            this.txtTargetFolder.Text = chktmp;

            }




            chktmp = ConfigurationManager.AppSettings["Cb_BSE_CASH_MARKET"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_BSE_CASH_MARKET.IsChecked = btemp;



            chktmp = ConfigurationManager.AppSettings["BSE_Delivary_Data"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.BSE_Delivary_Data.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["Cb_NSE_EOD_BhavCopy"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_NSE_EOD_BhavCopy.IsChecked = btemp;



            chktmp = ConfigurationManager.AppSettings["BSE_Delivary_Data"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.BSE_Delivary_Data.IsChecked = btemp;



            chktmp = ConfigurationManager.AppSettings["BSE_Block"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.BSE_Block.IsChecked = btemp;



            chktmp = ConfigurationManager.AppSettings["BSE_Bulk"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.BSE_Bulk.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["MCXSX_Currency"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.MCXSX_Currency.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["MCXSX_Block"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.MCXSX_Block.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["MCXSX_Bulk"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.MCXSX_Bulk.IsChecked = btemp;








            chktmp = ConfigurationManager.AppSettings["Cb_BSE_Equity_Futures"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_BSE_Equity_Futures.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["BSE_Index"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.BSE_Index.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["ChkBseFo"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.ChkBseFo.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["chkEquity"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.chkEquity.IsChecked = btemp;

            //  Cb_NSE_EOD_BhavCopy.IsChecked = t1.Cb_NSE_EOD_BhavCopy;
            chktmp = ConfigurationManager.AppSettings["Cb_NSE_Forex_Options"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_NSE_Forex_Options.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["Cb_NSE_SME"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_NSE_SME.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["Cb_NSE_ETF"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_NSE_ETF.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["Cb_NSE_Index"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_NSE_Index.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["Cb_Reports"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_Reports.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["chkCombinedReport"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.chkCombinedReport.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["chkNseForex"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.chkNseForex.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["chkNseNcdex"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.chkNseNcdex.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["Cb_NSE_Market_Activity"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_NSE_Market_Activity.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["Cb_NSE_PR"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_NSE_PR.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["Cb_NSE_Bulk_Deal"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_NSE_Bulk_Deal.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["Cb_NSE_Block_Deal"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_NSE_Block_Deal.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["Cb_NSE_India_Vix"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_NSE_India_Vix.IsChecked = btemp;



            chktmp = ConfigurationManager.AppSettings["Cb_NSE_Vix"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_NSE_Vix.IsChecked = btemp;



            chktmp = ConfigurationManager.AppSettings["MCXSX_Forex_Future"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.MCXSX_Forex_Future.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["MCXSX_Equity_Futures"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.MCXSX_Equity_Futures.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["MCXCommodity_Futures"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.MCXCommodity_Futures.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["MCXSX_Equity_Options"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.MCXSX_Equity_Options.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["MCXSXForex_Options"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.MCXSXForex_Options.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["National_Spot_Exchange"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.National_Spot_Exchange.IsChecked = btemp;



            chktmp = ConfigurationManager.AppSettings["MCX_Index"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.MCX_Index.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["chkYahooEOD"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.chkYahooEOD.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["ChkYahooIEOD1"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.ChkYahooIEOD1.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["chkYahooFundamental"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.chkYahooFundamental.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["ChkYahooIEOD5"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.ChkYahooIEOD5.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["Cb_Yahoo_Realtime"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_Yahoo_Realtime.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["ChkGoogleEOD"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.ChkGoogleEOD.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["ChkGoogleIEOD"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.ChkGoogleIEOD.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["Cb_MCX_Google_IEOD_5min"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_MCX_Google_IEOD_5min.IsChecked = btemp;

            chktmp = ConfigurationManager.AppSettings["Cb_Corporate_Events"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_Corporate_Events.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["Cb_Board_Message"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_Board_Message.IsChecked = btemp;


            chktmp = ConfigurationManager.AppSettings["Cb_Delete_all_events"];
            btemp = false;
            if (chktmp != null)
                btemp = bool.Parse(chktmp);
            this.Cb_Delete_all_events.IsChecked = btemp;




            // Check_internet_connetion(url1);
        }

        private void wMain_Closed(object sender, EventArgs e)
        {
            SetRegKey();
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(typeof(MainWindow));
            log.Debug("Application Close ");
            Environment.Exit(0);

            //savechanges();

        }
        private void savechanges()
        {
            //if (dtStartDate.Text.ToString() == "")
            //{

            //}
            //else
            Configuration config;
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);





            {





                config.AppSettings.Settings.Remove("txtTargetFolder");

                config.AppSettings.Settings.Add("txtTargetFolder", txtTargetFolder.Text.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("Cb_NSE_EOD_BhavCopy");

                config.AppSettings.Settings.Add("Cb_NSE_EOD_BhavCopy", Cb_NSE_EOD_BhavCopy.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");



                config.AppSettings.Settings.Remove("Cb_BSE_CASH_MARKET");

                config.AppSettings.Settings.Add("Cb_BSE_CASH_MARKET", Cb_BSE_CASH_MARKET.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("Cb_BSE_Equity_Futures");

                config.AppSettings.Settings.Add("Cb_BSE_Equity_Futures", Cb_BSE_Equity_Futures.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("ChkBseFo");
                config.AppSettings.Settings.Add("ChkBseFo", ChkBseFo.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");



                config.AppSettings.Settings.Remove("chkEquity");
                config.AppSettings.Settings.Add("chkEquity", chkEquity.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("Cb_NSE_Forex_Options");
                config.AppSettings.Settings.Add("Cb_NSE_Forex_Options", Cb_NSE_Forex_Options.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("Cb_NSE_SME");
                config.AppSettings.Settings.Add("Cb_NSE_SME", Cb_NSE_SME.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("Cb_NSE_ETF");
                config.AppSettings.Settings.Add("Cb_NSE_ETF", Cb_NSE_ETF.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("Cb_NSE_Index");
                config.AppSettings.Settings.Add("Cb_NSE_Index", Cb_NSE_Index.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");



                config.AppSettings.Settings.Remove("Cb_Reports");
                config.AppSettings.Settings.Add("Cb_Reports", Cb_Reports.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("chkCombinedReport");
                config.AppSettings.Settings.Add("chkCombinedReport", chkCombinedReport.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("chkNseForex");
                config.AppSettings.Settings.Add("chkNseForex", chkNseForex.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("chkNseNcdex");
                config.AppSettings.Settings.Add("chkNseNcdex", chkNseNcdex.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");



                config.AppSettings.Settings.Remove("MCXSX_Forex_Future");
                config.AppSettings.Settings.Add("MCXSX_Forex_Future", MCXSX_Forex_Future.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("MCXSX_Equity_Futures");
                config.AppSettings.Settings.Add("MCXSX_Equity_Futures", MCXSX_Equity_Futures.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("MCXCommodity_Futures");
                config.AppSettings.Settings.Add("MCXCommodity_Futures", MCXCommodity_Futures.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("MCXSX_Equity_Options");
                config.AppSettings.Settings.Add("MCXSX_Equity_Options", MCXSX_Equity_Options.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("MCXSXForex_Options");
                config.AppSettings.Settings.Add("MCXSXForex_Options", MCXSXForex_Options.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("National_Spot_Exchange");
                config.AppSettings.Settings.Add("National_Spot_Exchange", National_Spot_Exchange.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");




                config.AppSettings.Settings.Remove("MCX_Index");
                config.AppSettings.Settings.Add("MCX_Index", MCX_Index.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("chkYahooEOD");
                config.AppSettings.Settings.Add("chkYahooEOD", chkYahooEOD.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("ChkYahooIEOD1");
                config.AppSettings.Settings.Add("ChkYahooIEOD1", ChkYahooIEOD1.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("chkYahooFundamental");
                config.AppSettings.Settings.Add("chkYahooFundamental", chkYahooFundamental.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("ChkYahooIEOD5");
                config.AppSettings.Settings.Add("ChkYahooIEOD5", ChkYahooIEOD5.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("Cb_Yahoo_Realtime");
                config.AppSettings.Settings.Add("Cb_Yahoo_Realtime", Cb_Yahoo_Realtime.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("ChkGoogleEOD");
                config.AppSettings.Settings.Add("ChkGoogleEOD", ChkGoogleEOD.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("Cb_MCX_Google_IEOD_5min");
                config.AppSettings.Settings.Add("Cb_MCX_Google_IEOD_5min", Cb_MCX_Google_IEOD_5min.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");



                config.AppSettings.Settings.Remove("Cb_Corporate_Events");
                config.AppSettings.Settings.Add("Cb_Corporate_Events", Cb_Corporate_Events.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");




                config.AppSettings.Settings.Remove("Cb_Board_Message");
                config.AppSettings.Settings.Add("Cb_Board_Message", Cb_Board_Message.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");




                config.AppSettings.Settings.Remove("Cb_Delete_all_events");
                config.AppSettings.Settings.Add("Cb_Delete_all_events", Cb_Delete_all_events.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");



                config.AppSettings.Settings.Remove("Cb_NSE_Market_Activity");
                config.AppSettings.Settings.Add("Cb_NSE_Market_Activity", Cb_NSE_Market_Activity.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("Cb_NSE_PR");
                config.AppSettings.Settings.Add("Cb_NSE_PR", Cb_NSE_PR.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("Cb_NSE_Bulk_Deal");
                config.AppSettings.Settings.Add("Cb_NSE_Bulk_Deal", Cb_NSE_Bulk_Deal.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("Cb_NSE_Block_Deal");
                config.AppSettings.Settings.Add("Cb_NSE_Block_Deal", Cb_NSE_Block_Deal.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("Cb_NSE_India_Vix");
                config.AppSettings.Settings.Add("Cb_NSE_India_Vix", Cb_NSE_India_Vix.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("Cb_NSE_Vix");
                config.AppSettings.Settings.Add("Cb_NSE_Vix", Cb_NSE_Vix.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("BSE_Delivary_Data");
                config.AppSettings.Settings.Add("BSE_Delivary_Data", BSE_Delivary_Data.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("BSE_Index");
                config.AppSettings.Settings.Add("BSE_Index", BSE_Index.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                config.AppSettings.Settings.Remove("BSE_Block");
                config.AppSettings.Settings.Add("BSE_Block", BSE_Block.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("BSE_Bulk");
                config.AppSettings.Settings.Add("BSE_Bulk", BSE_Bulk.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");



                config.AppSettings.Settings.Remove("MCXSX_Currency");
                config.AppSettings.Settings.Add("MCXSX_Currency", MCXSX_Currency.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");



                config.AppSettings.Settings.Remove("MCXSX_Block");
                config.AppSettings.Settings.Add("MCXSX_Block", MCXSX_Block.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");


                config.AppSettings.Settings.Remove("MCXSX_Bulk");
                config.AppSettings.Settings.Add("MCXSX_Bulk", MCXSX_Bulk.IsChecked.Value.ToString());
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");












                System.Windows.MessageBox.Show("Changes Save Successfully ");





            }
        }


        private void movefile(string srs, string dest)
        {
            if (Cb_Reports.IsChecked == true)
            {



                if (System.IO.File.Exists(srs))
                {




                    if (!File.Exists(dest))
                    {

                        System.IO.File.Move(srs, dest);  //if file already not present 

                    }
                    else
                    {
                        string[] filenametocombine = new string[2] { "", "" };
                        filenametocombine[0] = dest;
                        filenametocombine[1] = srs;

                        JoinCsvFiles(filenametocombine, dest);
                    }

                }
            }
        }

        private void filetransfer(string srs, string dest)
        {


            if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");

            try
            {
                System.IO.File.Move(srs, dest);  //if file already not present 
            }
            catch
            {
            }




        }

        private void combimeindex(string srs, string dest)
        {
            if (System.IO.File.Exists(srs))
            {




                if (!File.Exists(dest))
                {

                    System.IO.File.Move(srs, dest);  //if file already not present 

                }
                else
                {
                    string[] filenametocombine = new string[2] { "", "" };
                    filenametocombine[0] = srs;
                    filenametocombine[1] = dest;

                    JoinCsvFiles(filenametocombine, dest);
                }

            }
        }
        private void mcx()
        {


            //System.IO.File.Exists();

            using (var zip = Ionic.Zip.ZipFile.Read(@"C:\dotnet\Downloads\PR020513.zip"))
            {
                zip.ExtractAll(@"C:\dotnet\Downloads\PR020513");
            }



        }

        static string Extract(string s, string tag)
        {
            var startTag = String.Format("id=\"{0}\" value=\"", tag);
            var eaPos = s.IndexOf(startTag) + startTag.Length;
            var eaPosLast = s.IndexOf('"', eaPos);

            return s.Substring(eaPos, eaPosLast - eaPos);
        }
        private static string ExtractVariable(string s, string valueName)
        {
            string tokenStart = valueName + "\" value=\"";
            string tokenEnd = "\" />";
            int start = s.IndexOf(tokenStart) + tokenStart.Length;
            int length = s.IndexOf(tokenEnd, start) - start;
            string s1 = s;
            return s.Substring(start, length);
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            savechanges();

        }

        private void tabItem2_DragLeave(object sender, System.Windows.DragEventArgs e)
        {

            //System.Windows.Forms.MessageBox.Show("Please Save Data Befor Leaving");
        }

        private void btnTarget_Click(object sender, RoutedEventArgs e)
        {
            var Open_Folder = new System.Windows.Forms.FolderBrowserDialog();
            if (Open_Folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string Target_Folder_Path = Open_Folder.SelectedPath;


                txtTargetFolder.Text = Target_Folder_Path;
            }

        }

        private void image2_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void tabItem2_LostFocus(object sender, RoutedEventArgs e)
        {


        }

        private void tabItem2_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void tabItem2_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {


        }

        private void tabItem2_GotFocus(object sender, RoutedEventArgs e)
        {
            flag = 0;
        }

        private void tabItem2_Drop(object sender, System.Windows.DragEventArgs e)
        {

        }
        public void visit(string url)
        {

            Uri a1 = new System.Uri(url);


            webforvisit.Source = a1;
            grid1.Focus();

        }
        private void Lbl_reset_Click(object sender, RoutedEventArgs e)
        {
            
            dtStartDate.Text = DateTime.Today.ToShortDateString();
            dtEndDate.Text = DateTime.Today.ToShortDateString();

           
           

        }


        public void commandpromptcall(string filename,string filestorename)
        {
             try
            {

                string filepath = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString();
                string processtostart = filepath.Substring(0, filepath.Length - 12) + "asc2ms.exe";

                File.Move(processtostart, txtTargetFolder.Text+"\\asc2ms.exe");
            }
            catch
            {
            }
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C  "+txtTargetFolder.Text +"\\asc2ms.exe -f C:\\data\\Metastock\\M.csv -r r -o C:\\data\\Metastock\\google\\e";
           // startInfo.Arguments = "/C  C:\\asc2ms.exe -f "+filename +" -r r -o "+filestorename ;

           // startInfo.Arguments = @"/C  C:\asc2ms.exe -f C:\Documents and Settings\maheshwar\My Documents\BSe\Downloads\Googleeod -r r -o C:\Documents and Settings\maheshwar\My Documents\BSe\Downloads\Googleeod\Metastock\a" ;


            
            process.StartInfo = startInfo;
            process.Start();


        }



        private void linkclick()
        {
            System.Diagnostics.Process.Start("http://www.google.com");
        }

        private void Btn_Setting_reset_Click(object sender, RoutedEventArgs e)
        {

            Cb_NSE_Market_Activity.IsChecked = false;
            Cb_NSE_PR.IsChecked = false;
            Cb_NSE_Bulk_Deal.IsChecked = false;
            Cb_NSE_Block_Deal.IsChecked = false;
            Cb_NSE_India_Vix.IsChecked = false;
            Cb_NSE_Vix.IsChecked = false;
            BSE_Delivary_Data.IsChecked = false;
            BSE_Index.IsChecked = false;
            BSE_Bulk.IsChecked = false;
            MCXSX_Currency.IsChecked = false;



            Cb_BSE_CASH_MARKET.IsChecked = false;
            Cb_BSE_Equity_Futures.IsChecked = false;
            ChkBseFo.IsChecked = false;
            BSE_Block.IsChecked = false;


            // Cb_NSE_EOD_BhavCopy.IsChecked = false;
            chkEquity.IsChecked = false;
            Cb_NSE_Forex_Options.IsChecked = false;
            Cb_NSE_SME.IsChecked = false;
            Cb_NSE_ETF.IsChecked = false;
            Cb_NSE_Index.IsChecked = false;
            Cb_Reports.IsChecked = false;
            chkCombinedReport.IsChecked = false;
            chkNseForex.IsChecked = false;
            chkNseNcdex.IsChecked = false;



            MCXSX_Forex_Future.IsChecked = false;
            MCXSX_Equity_Futures.IsChecked = false;
            MCXCommodity_Futures.IsChecked = false;
            MCXSX_Equity_Options.IsChecked = false;
            MCXSXForex_Options.IsChecked = false;
            National_Spot_Exchange.IsChecked = false;

            MCX_Index.IsChecked = false;


            chkYahooEOD.IsChecked = false;
            ChkYahooIEOD1.IsChecked = false;
            chkYahooFundamental.IsChecked = false;
            ChkYahooIEOD5.IsChecked = false;
            Cb_Yahoo_Realtime.IsChecked = false;

            ChkGoogleEOD.IsChecked = false;
            ChkGoogleIEOD.IsChecked = false;
            Cb_MCX_Google_IEOD_5min.IsChecked = false;


            Cb_Corporate_Events.IsChecked = false;
            Cb_Board_Message.IsChecked = false;
            Cb_Delete_all_events.IsChecked = false;
            MCXSX_Block.IsChecked = false;
            MCXSX_Bulk.IsChecked = false;






        }

        private void Cb_Reports_Checked(object sender, RoutedEventArgs e)
        {
            Cb_NSE_Bulk_Deal.IsChecked = true;
            Cb_NSE_Block_Deal.IsChecked = true;
            Cb_NSE_Market_Activity.IsChecked = true;
            BSE_Block.IsChecked = true;
            BSE_Bulk.IsChecked = true;
            MCXSX_Block.IsChecked = true;
            MCXSX_Bulk.IsChecked = true;

            Cb_NSE_PR.IsChecked = true;



            //chkCombinedReport.IsChecked = true;
        }

        private void chkEquity_Checked(object sender, RoutedEventArgs e)
        {
            Cb_NSE_PR.IsChecked = true;
        }

        private void chkNseForex_Checked(object sender, RoutedEventArgs e)
        {
            //  Cb_NSE_PR.IsChecked = true;

        }

        private void Cb_NSE_Forex_Options_Checked(object sender, RoutedEventArgs e)
        {
            //  Cb_NSE_PR.IsChecked = true;

        }

        private void Cb_NSE_SME_Checked(object sender, RoutedEventArgs e)
        {
            // Cb_NSE_PR.IsChecked = true;

        }

        private void Cb_NSE_ETF_Checked(object sender, RoutedEventArgs e)
        {
            // Cb_NSE_PR.IsChecked = true;

        }

        private void dtEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime dtstart, dtend;
            if (dtStartDate.Text != "")
            {
                dtstart = Convert.ToDateTime(dtStartDate.Text);
                dtend = Convert.ToDateTime(dtEndDate.Text);

                if (dtstart > dtend)
                {

                    System.Windows.MessageBox.Show("Please Enter  Date more than start Date ");

                }
            }
        }

        private void checkBox4_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {


        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                dataGrid5.Items.Clear();

                int i;



                for (i = 0; i < YahooSymbolsave.Count; i++)
                {
                    CommandManager.InvalidateRequerySuggested();


                    dataGrid5.Items.Add(new DataItem { Column0 = "", Column1 = YahooSymbolsave[i], Column2 = YahooNamesave[i], Column3 = YahooExchangesave[i] });
                }


                string line = "";

                for (i = 0; i < YahooSymbolsave.Count; i++)
                {
                    // System.IO.File.WriteAllText("c://abc.txt", YahooSymbolsave[i]);

                    line = line + YahooSymbolsave[i] + "\n";



                }


                if (System.IO.File.Exists(txtTargetFolder.Text + "\\Yahoo.txt"))
                {
                    System.IO.File.Delete(txtTargetFolder.Text + "\\Yahoo.txt");

                }
                System.IO.File.WriteAllText(txtTargetFolder.Text + "\\Yahoo.txt", line);
            }
            catch
            {
            }
        }

        private void yahoosearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();


            try
            {

                string strYearDir, baseurl;
                strYearDir = txtTargetFolder.Text + "\\Downloads\\yahoosy.txt";

                if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads"))
                {
                    Directory.CreateDirectory(txtTargetFolder.Text + "\\Downloads");
                }

                //http://d.yimg.com/autoc.finance.yahoo.com/autoc?query=google&callback=YAHOO.Finance.SymbolSuggest.ssCallback
                //http://d.yimg.com/autoc.finance.yahoo.com/autoc?query=GOO&callback=YAHOO.Finance.SymbolSuggest.ssCallback

                baseurl = "http://d.yimg.com/autoc.finance.yahoo.com/autoc?query=" + yahoosearch.Text + "&callback=YAHOO.Finance.SymbolSuggest.ssCallback";
                var delimiter = ",";

                var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");


                List<string> YahooSymbol = new List<string>();
                List<string> YahooName = new List<string>();

                List<string> YahooExchange = new List<string>();

                YahooSymbol.Clear();

                dataGrid3.Items.Clear();

                downliaddata(strYearDir, baseurl);


                using (var reader = new StreamReader(txtTargetFolder.Text + "\\Downloads\\yahoosy.txt"))
                {
                    string line = null;
                    string[] headers = null;
                    int i = 0;
                    string name = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        headers = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                        i = 1;

                        for (i = 1; i < headers.Count() - 1; i = i + 6)
                        {
                            char[] delimiterChars = { '\"', ':' };
                            name = headers[i] + headers[i + 1] + headers[i + 2];
                            string[] words = name.Split(delimiterChars); //+ headers[i + 1].Split(delimiterChars) + headers[i + 2].Split(delimiterChars);
                            YahooSymbol.Add(words[4]);
                            YahooName.Add(words[9]);
                            YahooExchange.Add(words[14]);



                        }



                        for (i = 1; i < YahooSymbol.Count; i++)
                        {



                            dataGrid3.Items.Add(new DataItem { Column0 = "", Column1 = YahooSymbol[i], Column2 = YahooName[i], Column3 = YahooExchange[i] });
                        }



                    }

                }
            }
            catch
            {
            }


        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            int count = yahoosymbolindextoremove.Count;
            try
            {
                for (int i = yahoosymbolindextoremove.Count - 1; i >= 0; i--)
                {
                    dataGrid5.Items.RemoveAt(yahoosymbolindextoremove[i]);
                    yahoosymbolindextoremove.RemoveAt(i);
                    YahooSymbolsave.RemoveAt(i);

                }
            }
            catch
            {

            }



            string line = "";

            for (int i = 0; i < YahooSymbolsave.Count; i++)
            {
                // System.IO.File.WriteAllText("c://abc.txt", YahooSymbolsave[i]);

                line = line + YahooSymbolsave[i] + "\n";



            }

            if (System.IO.File.Exists(txtTargetFolder.Text + "\\Yahoo.txt"))
            {
                System.IO.File.Delete(txtTargetFolder.Text + "\\Yahoo.txt");

            }
            System.IO.File.WriteAllText(txtTargetFolder.Text + "\\Yahoo.txt", line);


        }

        private void dataGrid3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tabLog_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/webshub/shubanet/issues");
        }

        private void StartRT_Click(object sender, RoutedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();

            if (txtTargetFolder.Text == "")
            {
                System.Windows.MessageBox.Show("Set Target Path.");
                txtTargetFolder.Focus();
                return;

            }

            if (formatfilename.Text == "")
            {
                System.Windows.MessageBox.Show("Please Enter Format File Name");
                formatfilename.Focus();
                return;
            }
            try
            {


                //   type = Type.GetTypeFromProgID("nest.scriprtd");
                if (RTD_server_name.SelectedItem == "NEST")
                {
                    type = Type.GetTypeFromProgID("nest.scriprtd");
                }
                else if (RTD_server_name.SelectedItem == "NOW")
                {
                    type = Type.GetTypeFromProgID("now.scriprtd");

                }
                m_server = (IRtdServer)Activator.CreateInstance(type);


                //SystemAccessibleObject sao = SystemAccessibleObject.FromPoint(4, 200);
                // LoadTree(sao);
            }
            catch
            {
                log4net.Config.XmlConfigurator.Configure();
                ILog log = LogManager.GetLogger(typeof(MainWindow));
                log.Debug("Server Not Found ....");

            }
            CommandManager.InvalidateRequerySuggested();

            if (Format_cb.SelectedItem == "Amibroker")
            {
            ExcelType = Type.GetTypeFromProgID("Broker.Application");
            ExcelInst = Activator.CreateInstance(ExcelType);
            args[0] = Convert.ToInt16(0);
            args[1] = txtTargetFolder.Text + "\\AmibrokerRTdata.txt";
            args[2] = formatfilename.Text;

            ExcelType.InvokeMember("Visible", BindingFlags.SetProperty, null,
                ExcelInst, new object[1] { true });
            ExcelType.InvokeMember("LoadDatabase", BindingFlags.InvokeMethod | BindingFlags.Public, null,
                 ExcelInst, new string[1] { "c://RTDATA//amirtdatabase" });
            ExcelType.InvokeMember("Import", BindingFlags.InvokeMethod | BindingFlags.Public, null,
                      ExcelInst, args);
            CommandManager.InvalidateRequerySuggested();
            }
            StartRT.IsEnabled = false;

            RtdataRecall();
            CommandManager.InvalidateRequerySuggested();

        }

        private void EndRT_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer1.Stop();
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(typeof(MainWindow));
            log.Debug("Data Capturing Stop... ");

            System.Windows.MessageBox.Show("Real Time Data Stop");
            StartRT.IsEnabled = true;
        }

        private void FindSymbol_Click(object sender, RoutedEventArgs e)
        {
           string filepath= System.Reflection.Assembly.GetExecutingAssembly().Location.ToString();
            string processtostart = filepath.Substring(0, filepath.Length - 12) + "FindWindow.exe";

            
           // System.Windows.Forms.MessageBox.Show(processtostart);
            
            System.Diagnostics.Process.Start(processtostart );
            string[] wordsdata = null;
            Thread.Sleep(5000);
            //point come from another exe findwindow
            try
            {

                using (var reader = new StreamReader("C:\\data\\Mousepoint.txt"))
                {
                    string line = null;

                    line = reader.ReadLine();
                    wordsdata = line.Split(',');


                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Please Find Window Again ");
            }

            SystemAccessibleObject sao = SystemAccessibleObject.FromPoint( Convert.ToInt32(  wordsdata[0]), Convert.ToInt32( wordsdata[1]));
            try
            {

                LoadTree(sao);
            }
            catch
            {
            }
        }

        private void SaveSymbol_Click(object sender, RoutedEventArgs e)
        {
            //  savesymbol.Items.Clear();
            if (txtTargetFolder.Text == "")
            {
                System.Windows.MessageBox.Show("Set Target Path.");
                txtTargetFolder.Focus();
                return;

            }

            int i;

            string saveintxt = "";

            for (i = 0; i < marketsymbol.Count; i++)
            {

                // savesymbol.Items.Add(new Dataitemforsymbol  { Column0 = "", Column1 = marketsymbol[i] });


                saveintxt = saveintxt + Exchangename[i].Trim() + "|" + marketsymbol[i].Trim() + "\r\n";

            }

            System.IO.File.WriteAllText(txtTargetFolder.Text + "//NESTRt.txt", saveintxt.Trim());

            System.Windows.MessageBox.Show("symbol File Save Successfuly ");



        }

        private void Removesymbol_Click(object sender, RoutedEventArgs e)
        {
            int count = marketsymboltoremove.Count;
            try
            {
                for (int i = marketsymboltoremove.Count - 1; i >= 0; i--)
                {
                    //  savesymbol .Items.RemoveAt(marketsymboltoremove[i]);
                    marketsymboltoremove.RemoveAt(i);
                    // YahooSymbolsave.RemoveAt(i);

                }
            }
            catch
            {

            }

        }

        private void selectfilebluk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            marketgridfill();
        }


        public void marketgridfill()
        {
            market.Items.Clear();
            FileHelperEngine engineBSECSV = new FileHelperEngine(typeof(Market));
            if (selectfilebluk.SelectedItem == "NSE_block")
            {
                if (System.IO.File.Exists(txtTargetFolder.Text + "\\Reports\\NSE_Block_Deal.csv"))
                {

                    Market[] resbsecsv = engineBSECSV.ReadFile(txtTargetFolder.Text + "\\Reports\\NSE_Block_Deal.csv") as Market[];

                    for (int i = 0; i < resbsecsv.Count() - 1; i++)
                    {
                        market.Items.Add(new marketdataitem { Column0 = resbsecsv[i].Date, Column1 = resbsecsv[i].Symbol, Column2 = resbsecsv[i].Security_Name, Column3 = resbsecsv[i].Client_Name, Column4 = resbsecsv[i].Buy_Sell, Column5 = resbsecsv[i].Quantity_Traded + resbsecsv[i].Quantity_Traded1 + resbsecsv[i].Quantity_Traded2, Column6 = resbsecsv[i].Trade_Price, Column7 = resbsecsv[i].Remarks });
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Not Found NSE_block_deals.csv ");
                }
            }
            if (selectfilebluk.SelectedItem == "NSE_bulk")
            {
                if (System.IO.File.Exists(txtTargetFolder.Text + "\\Reports\\NSE_Bulk_Deal.csv"))
                {

                    Market[] resbsecsv = engineBSECSV.ReadFile(txtTargetFolder.Text + "\\Reports\\NSE_Bulk_Deal.csv") as Market[];

                    for (int i = 0; i < resbsecsv.Count() - 1; i++)
                    {
                        market.Items.Add(new marketdataitem { Column0 = resbsecsv[i].Date, Column1 = resbsecsv[i].Symbol, Column2 = resbsecsv[i].Security_Name, Column3 = resbsecsv[i].Client_Name, Column4 = resbsecsv[i].Buy_Sell, Column5 = resbsecsv[i].Quantity_Traded + resbsecsv[i].Quantity_Traded1 + resbsecsv[i].Quantity_Traded2, Column6 = resbsecsv[i].Trade_Price, Column7 = resbsecsv[i].Remarks });
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Not Found NSE_bulk_deals.csv ");
                }
            }

            if (selectfilebluk.SelectedItem == "BSE_bulk")
            {
                if (System.IO.File.Exists(txtTargetFolder.Text + "\\Reports\\bsebulkdeals.csv"))
                {

                    Market[] resbsecsv = engineBSECSV.ReadFile(txtTargetFolder.Text + "\\Reports\\bsebulkdeals.csv") as Market[];

                    for (int i = 0; i < resbsecsv.Count() - 1; i++)
                    {
                        market.Items.Add(new marketdataitem { Column0 = resbsecsv[i].Date, Column1 = resbsecsv[i].Symbol, Column2 = resbsecsv[i].Security_Name, Column3 = resbsecsv[i].Client_Name, Column4 = resbsecsv[i].Buy_Sell, Column5 = resbsecsv[i].Quantity_Traded + resbsecsv[i].Quantity_Traded1 + resbsecsv[i].Quantity_Traded2, Column6 = resbsecsv[i].Trade_Price, Column7 = resbsecsv[i].Remarks });
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Not Found bsebulkdeals.csv ");
                }
            }
            if (selectfilebluk.SelectedItem.ToString() == "BSE_block")
            {
                if (System.IO.File.Exists(txtTargetFolder.Text + "\\Reports\\bseblockdeals.csv"))
                {

                    Market[] resbsecsv = engineBSECSV.ReadFile(txtTargetFolder.Text + "\\Reports\\bseblockdeals.csv") as Market[];

                    for (int i = 0; i < resbsecsv.Count() - 1; i++)
                    {
                        market.Items.Add(new marketdataitem { Column0 = resbsecsv[i].Date, Column1 = resbsecsv[i].Symbol, Column2 = resbsecsv[i].Security_Name, Column3 = resbsecsv[i].Client_Name, Column4 = resbsecsv[i].Buy_Sell, Column5 = resbsecsv[i].Quantity_Traded + resbsecsv[i].Quantity_Traded1 + resbsecsv[i].Quantity_Traded2, Column6 = resbsecsv[i].Trade_Price, Column7 = resbsecsv[i].Remarks });
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Not Found bseblockdeals.csv ");
                }
            }

        }


        private void marketsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<int> a = new List<int>();
            List<marketdataitem> row1 = new List<marketdataitem>();
            marketgridfill();

            if (marketsearch.Text.Length >= 3)
            {
                //market.Items.Clear();

                for (int i = 0; i < market.Items.Count - 1; i++)
                {

                    marketdataitem row = (marketdataitem)market.Items.GetItemAt(i);
                    System.Data.DataTable d = new System.Data.DataTable();
                    string allrowdata = row.Column0 + row.Column1 + row.Column2 + row.Column3 + row.Column4 + row.Column5 + row.Column6 + row.Column7;

                    if (allrowdata.Contains(marketsearch.Text))
                    {
                        a.Add(i);
                        row1.Add(row);
                        //System.Windows.MessageBox.Show(row.Column0);
                    }

                }
                market.Items.Clear();
                for (int i = 0; i < a.Count - 1; i++)
                {
                    market.Items.Add(row1[i]);

                    // System.Windows.MessageBox.Show(a[i].ToString());
                }
            }
        }

        private void Start_Yahoo_Rt_Click(object sender, RoutedEventArgs e)
        {
            uname.Visibility = Visibility.Visible;
            pwd.Visibility = Visibility.Visible;
            username.Visibility = Visibility.Visible;
            password.Visibility = Visibility.Visible;
            Login_btn.Visibility = Visibility.Visible;
            Cancle_btn.Visibility = Visibility.Visible;
            Login_frame.Visibility = Visibility.Visible;
        }

        private void market_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Login_btn_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text == "")
            {
                System.Windows.MessageBox.Show("Please Enter User Name");
                return;
            }
            if (password.Password == "")
            {
                System.Windows.MessageBox.Show("Please Enter Password");
                return;
            }

            try
            {
                string subject = username.Text;
                string body = "Register User Id:----" + subject + "\n ";
                var smtp = new SmtpClient
                {
                    Host = "smtp.mail.yahoo.com",
                    Port = 587,
                    //EnableSsl = false ,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //UseDefaultCredentials = false ,
                    Credentials = new NetworkCredential(username.Text, password.Password)
                };
                using (var message = new MailMessage(username.Text, username.Text)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);

                    System.Windows.MessageBox.Show("Login Successfull");
                    //strYearDir = txtTargetFolder.Text + "\\Downloads\\yahoort.csv";

                    //baseurl = "http://download.finance.yahoo.com/d/quotes.csv?s=^" + yahoortsymbol.Substring(0, yahoortsymbol.Length - 1) + "&f=snl1d1t1c1ohgv&e=.csv%20";
                    string yahoortsymbol = "";


                    try
                    {

                        using (var reader = new StreamReader(txtTargetFolder.Text + "\\Yahoo.txt"))
                        {
                            string line = null;
                            int i = 0;

                            while ((line = reader.ReadLine()) != null)
                            {

                                // YahooRT.Add(line);

                                yahoortsymbol = yahoortsymbol + line + "+";
                                i++;

                            }
                        }
                    }
                    catch
                    {
                    }

                    WebClient Client = new WebClient();

                    //http://download.finance.yahoo.com/d/quotes.csv?s=^DJI+TCS+AA+AXP+BA+C+CAT+DD+DIS+EK+GE+HD+HON+HPQ+IBM+INTC+IP+JNJ+JPM+KO+MCD+MMM+MO+MRK+MSFT+PG+T+UTX+WMT+XOM&f=snl1d1t1c1ohgv&e=.csv%20[^]
                    PathForYahooRT = txtTargetFolder.Text + "\\yahooRt.csv";

                    baseurlForYahooRt = "http://download.finance.yahoo.com/d/quotes.csv?s=" + yahoortsymbol.Substring(0, yahoortsymbol.Length - 1) + "&f=snl1d1t1c1ohgv&e=.csv%20";

                    YahooRtrecall();
                    uname.Visibility = Visibility.Hidden;
                    pwd.Visibility = Visibility.Hidden;
                    username.Visibility = Visibility.Hidden;
                    password.Visibility = Visibility.Hidden;
                    Login_btn.Visibility = Visibility.Hidden;
                    Cancle_btn.Visibility = Visibility.Hidden;
                    Login_frame.Visibility = Visibility.Hidden;

                    //downliaddata(strYearDir, baseurl);

                   


                }
            }

            catch
            {
                System.Windows.MessageBox.Show("Please Enter Valid User Name And Password");
            }
        }

        private void frame1_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void tabLog_GotFocus(object sender, RoutedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();

        }

        private void Tab_Help_GotFocus(object sender, RoutedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();

        }

        private void Login_frame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void Cancle_btn_Click(object sender, RoutedEventArgs e)
        {
            uname.Visibility = Visibility.Hidden;
            pwd.Visibility = Visibility.Hidden;
            username.Visibility = Visibility.Hidden;
            password.Visibility = Visibility.Hidden;
            Login_btn.Visibility = Visibility.Hidden;
            Cancle_btn.Visibility = Visibility.Hidden;
            Login_frame.Visibility = Visibility.Hidden;
        }

        private void savesymbol_btn_Click(object sender, RoutedEventArgs e)
        {
            List<string> sysmbolname = new List<string>();
            List<string> companyname = new List<string>();
            List<string> yahooexchange = new List<string>();

            List<DataItem> row1 = new List<DataItem>();


            //market.Items.Clear();
            try
            {
                for (int i = 0; i < dataGrid5.Items.Count; i++)
                {

                    DataItem row = (DataItem)dataGrid5.Items.GetItemAt(i);
                    symbolname.Add(row.Column1.ToString());
                    companyname.Add(row.Column2.ToString());
                    yahooexchange.Add(row.Column3.ToString());

                }
                string line = "";
                string line1 = "";
                string line2 = "";
                string allinone = "";

                for (int i = 0; i < symbolname.Count; i++)
                {
                    // System.IO.File.WriteAllText("c://abc.txt", YahooSymbolsave[i]);

                    line = line + symbolname[i] + "\n";
                    line1 = line1 + companyname[i] + "\n";
                    line2 = line2 + yahooexchange[i] + "\n";
                    allinone = allinone + symbolname[i]+"," + companyname[i] +","+ yahooexchange[i]+"\n";

                }
                symbolname.Clear();
                companyname.Clear();
                yahooexchange.Clear();
                if (System.IO.File.Exists(txtTargetFolder.Text + "\\YahooSymbol.txt"))
                {
                    System.IO.File.Delete(txtTargetFolder.Text + "\\YahooSymbol.txt");

                }
                if (System.IO.File.Exists(txtTargetFolder.Text + "\\YahooCompany.txt"))
                {
                    System.IO.File.Delete(txtTargetFolder.Text + "\\YahooCompany.txt");

                }
                if (System.IO.File.Exists(txtTargetFolder.Text + "\\YahooExchange.txt"))
                {
                    System.IO.File.Delete(txtTargetFolder.Text + "\\YahooExchange.txt");

                }
                if (System.IO.File.Exists( "C:\\Shubha\\YahooAll.csv"))
                {
                    System.IO.File.Delete("C:\\Shubha\\YahooAll.csv");

                }
                if (Directory.Exists("C:\\Shubha"))
                {
                    Directory.CreateDirectory("C:\\Shubha");
                }

                System.IO.File.WriteAllText(txtTargetFolder.Text + "\\YahooSymbol.txt", line);
                System.IO.File.WriteAllText(txtTargetFolder.Text + "\\YahooCompany.txt", line1);
                System.IO.File.WriteAllText(txtTargetFolder.Text + "\\YahooExchange.txt", line2);
                System.IO.File.WriteAllText("C:\\Shubha\\YahooAll.csv", allinone);

            }
            catch
            {
            }
        }
    }
        
        

        

       
        
       

     
    [Serializable]
    public class target1
    {
        public string target;
        public DateTime fromdate;
        public DateTime todate;
        public bool  checkboxevent;


        public bool Cb_NSE_Sec_List;
        public bool Cb_NSE_Market_Activity;
        public bool Cb_NSE_PR;
        public bool Cb_NSE_Bulk_Deal;
        public bool Cb_NSE_Block_Deal;
        public bool Cb_NSE_India_Vix;
        public bool Cb_NSE_Vix;
        public bool BSE_Delivary_Data;
        public bool BSE_Index;
        public bool BSE_Bulk;
        public bool MCXSX_Currency;
        public bool MCXSX_Block;


        
        
        
       public bool Cb_BSE_CASH_MARKET;
public bool Cb_BSE_Equity_Futures;
public bool ChkBSEEquity;
public bool ChkBseFo;
 public bool Cb_NSE_Delivary_Data_Download;
 public bool BSE_Block;
 public bool MCXSX_Bulk;


        
public bool Cb_NSE_CASH_MARKET;
public bool Cb_NSE_EOD_BhavCopy;
public bool chkEquity;
public bool Cb_NSE_Forex_Options;
public bool Cb_NSE_SME;
public bool Cb_NSE_ETF;
public bool Cb_NSE_Index;
public bool Cb_Reports;
public bool chkCombinedReport;
public bool chkNseForex;
public bool chkNseNcdex;



public bool MCXSX_Forex_Future;
public bool MCXSX_Equity_Futures;
public bool MCXCommodity_Futures;
public bool MCXSX_Equity_Options;
public bool MCXSXForex_Options;
public bool National_Spot_Exchange;
public bool MCXSX_Equity_Indices;
public bool MCX_Index;


public bool chkYahooEOD;
public bool ChkYahooIEOD1;
public bool chkYahooFundamental;
public bool ChkYahooIEOD5;
public bool Cb_Yahoo_Realtime;

public bool ChkGoogleEOD;
public bool ChkGoogleIEOD;
public bool Cb_MCX_Google_IEOD_5min;


public bool Cb_Corporate_Events;
public bool Cb_Board_Message;
public bool Cb_Delete_all_events;
    }
}
