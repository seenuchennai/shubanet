using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO;
using System.Collections.Specialized ;
using System.Collections;
using System.IO.Compression;
using System.IO.Packaging;
using Ionic.Zlib;
using Ionic.Zip;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Data.OleDb;
namespace StockD
{
     

      
       

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


   

        string url1 = "http://www.goog";
        int flag = 0;
        WebClient Client = new WebClient();
        double value = 0;
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
           

            //Create a new instance of our ProgressBar Delegate that points
            //  to the ProgressBar's SetValue method.
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(ProgressBar1.SetValue);

            //Tight Loop:  Loop until the ProgressBar.Value reaches the max
           
       
                
             
                /*Update the Value of the ProgressBar:
                  1)  Pass the "updatePbDelegate" delegate that points to the ProgressBar1.SetValue method
                  2)  Set the DispatcherPriority to "Background"
                  3)  Pass an Object() Array containing the property to update (ProgressBar.ValueProperty) and the new value */
               value += 10;
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { System.Windows.Controls.ProgressBar.ValueProperty, value });
        }




        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
           
            if (dtStartDate.Text == "" || dtEndDate.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Please Select Date.. ");

                return;

            }
            if(txtTargetFolder.Text=="")
            {
                System.Windows.Forms.MessageBox.Show("Please Set Path.. ");
                return;

            }

            string strYearDir = txtTargetFolder.Text + "\\Downloads\\BSEBlock";

            if (Directory.Exists(strYearDir))
                Directory.Delete(strYearDir,true);

            strYearDir = txtTargetFolder.Text + "\\Downloads\\BSEBulk";

            if (Directory.Exists(strYearDir))
                Directory.Delete(strYearDir,true );


            string  dest_filename;

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
                ProgressBar1.Visibility = Visibility.Visible ;
                
                btnStart.IsEnabled = false;
                /*Update the Value of the ProgressBar:
                  1)  Pass the "updatePbDelegate" delegate that points to the ProgressBar1.SetValue method
                  2)  Set the DispatcherPriority to "Background"
                  3)  Pass an Object() Array containing the property to update (ProgressBar.ValueProperty) and the new value */
                value += 10;
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { System.Windows.Controls.ProgressBar.ValueProperty, value });

            string baseurl,filename="";
             DateTime  StartDate, EndDate;
            strYearDir = txtTargetFolder.Text + "\\Downloads";

            
                 StartDate = Convert.ToDateTime(dtStartDate.Text);
                 EndDate = Convert.ToDateTime(dtEndDate.Text);
             
            if (!Directory.Exists(strYearDir))
                Directory.CreateDirectory(strYearDir);

            
            

            if (chkCombinedReport.IsChecked == true)
            {
                prograss();

                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\combined_report"+formatdate(day)+".zip";
                    baseurl = "http://www.nseindia.com/archives/combine_report/combined_report"+formatdate(day)+".zip";

                    //http://www.nseindia.com/archives/combine_report/combined_report16052013.zip


                    downliaddata(strYearDir, baseurl);

                   
                    dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_combined_report_" +formatdate(day)+".zip";

                    movefile(strYearDir, dest_filename);


                }
               
            }

          


            if (Cb_NSE_EOD_BhavCopy.IsChecked == true)
            {



              

                strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                baseurl = "http://www.nseindia.com/content/equities/sec_list.csv";
               if(!System.IO.File.Exists(strYearDir))
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


                    if(!System.IO.File.Exists(strYearDir))
                    {

                    downliaddata(strYearDir, baseurl);
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
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\cm" + date1 + strMonthName + day.Year + "bhav.csv.zip";
                    baseurl = "http://www.nseindia.com/content/historical/EQUITIES/" + day.Year.ToString() + "/" + strMonthName.ToUpper() + "/cm" + date1 + strMonthName.ToUpper() + day.Year + "bhav.csv.zip";

                    //  http://www.nseindia.com/content/historical/EQUITIES/2013/MAY/cm17MAY2013bhav.csv.zip


                    downliaddata(strYearDir, baseurl);

                    if (System.IO.File.Exists(strYearDir))
                    {
                        using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                        {
                            if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\cm" + date1 + strMonthName + day.Year + "bhav"))
                            {
                            zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\cm" + date1 + strMonthName + day.Year + "bhav");

                            }
                        }
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\cm" + date1 + strMonthName + day.Year + "bhav\\cm" + date1 + strMonthName + day.Year + "bhav.csv";
                        string mtopath = txtTargetFolder.Text + "\\Downloads\\MTO_" + formatdate(day) + ".csv";
                        string destfilepath = txtTargetFolder.Text + "\\STD_CSV\\Nse_Cash_Market_cm" + date1 + strMonthName + day.Year + ".csv";
                        string dateformtoprocessingsave = formatdate(day);
                        if(!System.IO.File.Exists(destfilepath))
                        {

                        NSE_Processing(strYearDir, mtopath,destfilepath ,dateformtoprocessingsave );

                        
                            //if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                            //    Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                            //strYearDir = dest_filename;
                            //destfilepath = txtTargetFolder.Text + "\\STD_CSV\\NSE_Standard_" + date1 + strMonthName + day.Year + ".csv";


                            //if (!Directory.Exists(dest_filename))
                            //{
                            //    dest_filename = txtTargetFolder.Text + "\\STD_CSV";

                            //    movefile(strYearDir, dest_filename);

                            //}


                        



                        }
                        if (Directory.Exists (txtTargetFolder.Text + "\\Downloads\\cm" + date1 + strMonthName + day.Year + "bhav"))
                        {
                       Directory.Delete (txtTargetFolder.Text + "\\Downloads\\cm" + date1 + strMonthName + day.Year + "bhav",true );


                        }

                    }

                }

            }


            if (Cb_NSE_Index.IsChecked == true)
            {
                prograss();


                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();



                    strYearDir = txtTargetFolder.Text + "\\Downloads\\ind_close_all_" + formatdate(day) + ".csv";

                    baseurl = "http://nseindia.com/content/indices/ind_close_all_" + formatdate(day) + ".csv";



                    downliaddata(strYearDir, baseurl);

                    //process 
                    if (System.IO.File.Exists(strYearDir))
                    {

                        string destfilepath = txtTargetFolder.Text + "\\STD_CSV\\NSE_INDEX_STD" + formatdate(day) + ".csv";
                        string dateformtoprocessingsave = formatdate(day);
                        string nameoffile = "NSE_INDEX";

                        FUTURE_Processing(strYearDir, destfilepath, dateformtoprocessingsave, nameoffile);




                    }

                }

            }

            if (ChkYahooIEOD1.IsChecked == true)
            {
                prograss();
                strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoo1min";

                if (!Directory.Exists(strYearDir))
                    Directory.CreateDirectory (strYearDir);
                string [] yahooieod1 = new string[] {"ACROPETAL.ns","ADANIENT.ns","ADANIPOWE.ns","ADFFOODS.ns","ADHUNIK.ns","ADORWELD.ns","ADSL.ns","ADVANIHOT.ns","ADVANTA.ns","AEGISCHEM.ns","AFL.ns","AFTEK.ns","AREVAT&D.ns","M&M.ns" };

                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();
                     

                    for(int i=0;i<14;i++)
                    {
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoo1min\\" + day.Day + yahooieod1[i] + ".csv";

                    baseurl = "http://chartapi.finance.yahoo.com/instrument/1.0/"+yahooieod1[i]+ "/chartdata;type=quote;range=1d/csv/";


                   // "http://chartapi.finance.yahoo.com/instrument/1.0/ACROPETAL.ns/chartdata;type=quote;range=1d/csv/"

                    downliaddata(strYearDir, baseurl);
                    }
                  
                }

            }

            if (ChkYahooIEOD5.IsChecked == true)
            {
                prograss();
                strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoo5min";

                if (!Directory.Exists(strYearDir))
                    Directory.CreateDirectory(strYearDir);
                string[] yahooieod5 = new string[] {"ACROPETAL.ns","ADANIENT.ns","ADANIPOWE.ns","ADFFOODS.ns","ADHUNIK.ns","ADORWELD.ns","ADSL.ns","ADVANIHOT.ns","ADVANTA.ns","AEGISCHEM.ns","AFL.ns","AFTEK.ns","AREVAT&D.ns","M&M.ns","^AEX","^AORD","^ATX","^BFX ","^HSI","^JKSE","^KLSE","^KS11","^N225","^NZ50","^OMXSPI","^OSEAX","^SMSI","^SSEC","^SSMI","^STI","^TWII","000001.SS","^GSPC","^IXIC","^DJI","^DJT","^DJU","^DJA","^TV.N","^NYA","^NUS","^NIN","^NWL","^NTM","^TV.O","^NDX","^IXBK","^IXFN","^IXF","^IXID","^IXIS","^IXK","^IXTR","^IXUT","^NBI","^OEX","^MID","^SML","^SPSUPX","^XAX","^IIX","^NWX","^XMI","^PSE","^SOXX","^RUI","^RUA","^DOT","^DWC","^BATSK","^DJC","^XAU","^TYX","^TNX","^FVX","^IRX","^FCHI","^FTSE","^GDAXI","NIFTY","^NSEI"};

                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();


                    for (int i = 0; i <81 ; i++)
                    {
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoo5min\\" + day.Day + yahooieod5[i] + ".csv";

                        baseurl = "http://chartapi.finance.yahoo.com/instrument/5.0/" + yahooieod5[i] + "/chartdata;type=quote;range=5d/csv/";


                        // "http://chartapi.finance.yahoo.com/instrument/5.0/ACROPETAL.ns/chartdata;type=quote;range=1d/csv/"

                        downliaddata(strYearDir, baseurl);
                    }

                }

            }
            if (chkYahooFundamental.IsChecked == true)
            {
                prograss();
                strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoofun";

                if (!Directory.Exists(strYearDir))
                    Directory.CreateDirectory(strYearDir);
                string[] yahoofun = new string[] { "tatasteel.ns","ADANIENT.ns","ADANIPOWE.ns","ADFFOODS.ns","ADHUNIK.ns","ADORWELD.ns","ADSL.ns","ADVANIHOT.ns","ADVANTA.ns","AEGISCHEM.ns","AFL.ns","AFTEK.ns","AREVAT&D.ns","M&M.ns"};

                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();
                    string date1,date2;
                    if (day.Day < 10)
                    {
                        date1 = "0" + (day.Day-1).ToString();
                    }
                    else
                    {
                        date1 = day.Day.ToString();
                    }

                    if (day.Month < 10)
                    {

                        date2 =  "0" + day.Month.ToString();
                    }
                    else
                    {
                        date2 = day.Month.ToString();
                    }

                    for (int i = 0; i < 14; i++)
                    {
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Yahoofun\\" + day.Day + yahoofun[i] + ".csv";
                        baseurl = "http://download.finance.yahoo.com/d/quotes.csv?s="+yahoofun [i]+"&f=snl1ee7e8e9r5b4j4p5s6s7r1qdt8j1f6&e=.csv";
                        // "http://download.finance.yahoo.com/d/quotes.csv?s=ADANIENT.ns&f=snl1ee7e8e9r5b4j4p5s6s7r1qdt8j1f6&e=.csv"


                        downliaddata(strYearDir, baseurl);
                    }

                }
                

            }

            if (chkYahooEOD.IsChecked == true)
            {
                prograss();
                strYearDir = txtTargetFolder.Text + "\\Downloads\\YahooEod";

                if (!Directory.Exists(strYearDir))
                    Directory.CreateDirectory(strYearDir);
                string[] yahooeod = new string[] { "YHOO","ADANIENT.ns","ADANIPOWE.ns","ADFFOODS.ns","ADHUNIK.ns","ADORWELD.ns","ADSL.ns","ADVANIHOT.ns","ADVANTA.ns","AEGISCHEM.ns","AFL.ns","AFTEK.ns","AREVAT&D.ns","M&M.ns","^AEX","^AORD","^ATX","^BFX ","^HSI","^JKSE","^KLSE","^KS11","^N225","^NZ50","^OMXSPI","^OSEAX","^SMSI","^SSEC","^SSMI","^STI","^TWII","000001.SS","^GSPC","^IXIC","^DJI","^DJT","^DJU","^DJA","^TV.N","^NYA","^NUS","^NI","^NWL","^NTM","^TV.O","^NDX","^IXBK","^IXFN","^IXF","^IXID","^IXIS","^IXK","^IXTR","^IXUT","^NBI","^OEX","^MID","^SML","^SPSUPX","^XAX","^IIX","^NWX","^XMI","^PSE","^SOXX","^RUI","^RUA","^DOT","^DWC","^BATSK","^DJC","^XAU","^TYX","^TNX","^FVX","^IRX","^FCHI","^FTSE","^GDAXI","NIFTY","^NSEI"};

                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();
                    string date1,date2;
                    if (day.Day < 10)
                    {
                        date1 = "0" + (day.Day-1).ToString();
                    }
                    else
                    {
                        date1 = day.Day.ToString();
                    }

                    if (day.Month < 10)
                    {

                        date2 =  "0" + day.Month.ToString();
                    }
                    else
                    {
                        date2 = day.Month.ToString();
                    }

                    for (int i = 0; i < 81; i++)
                    {
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\YahooEod\\" + day.Day + yahooeod[i] + ".csv";
                        string e1=Convert.ToInt32(date1)+1.ToString();
                        baseurl = "http://ichart.finance.yahoo.com/table.csv?s=ADANIENT.ns&a="+date2+day.Month+"&b="+date1+"&c="+day.Year+"&d="+date2+"&e"+ e1 +"&f="+day.Year +"&g=d";
                                  //http://ichart.finance.yahoo.com/table.csv?s=ADANIENT.ns&a=045&b=01&c=2013&d=04&e=02&f=2013&g=d"

                        downliaddata(strYearDir, baseurl);
                    }

                }
                

            }

            if (ChkGoogleEOD.IsChecked == true)
            {
                prograss();
                strYearDir = txtTargetFolder.Text + "\\Downloads\\Googleeod";

                if (!Directory.Exists(strYearDir))
                    Directory.CreateDirectory(strYearDir);
                string[] GoogleEod = new string[] { "LICHSGFIN.nse","ADANIENT.nse","ADANIPOWE.nse","ADFFOODS.nse","ADHUNIK.nse","ADORWELD.nse","ADSL.nse","ADVANIHOT.nse","ADVANTA.nse","AEGISCHEM.nse","AFL.nse","AFTEK.nse","AREVAT&D.nse","M&M.nse",".AEX,indexeuro",".AORD,indexasx",".HSI,indexhangseng",",.N225,indexnikkei",".NSEI,nse",".NZ50,nze",".TWII,tpe","000001,sha","CNX100,nse","CNX500,nse","CNXENERGY,nse","CNXFMCG,nse","CNXINFRA,nse","CNXIT,nse"};

                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();
                   
                    for (int i = 0; i < 14; i++)
                    {
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\Googleeod\\" + day.Day + GoogleEod[i] + ".csv";
                        baseurl = "http://www.google.com/finance/getprices?q=" + GoogleEod [i] + "&i=d&p=15d&f=d,o,h,l,c,v";
                        // "http://www.google.com/finance/getprices?q=LICHSGFIN&x=LICHSGFIN&i=d&p=15d&f=d,o,h,l,c,v"


                        downliaddata(strYearDir, baseurl);
                    }

                }


            }


            if (ChkGoogleIEOD.IsChecked == true)
            {
                prograss();
                strYearDir = txtTargetFolder.Text + "\\Downloads\\GoogleIeod";

                if (!Directory.Exists(strYearDir))
                    Directory.CreateDirectory(strYearDir);
                string[] GoogleIEod = new string[] { "LICHSGFIN.nse", "ADANIENT.nse", "ADANIPOWE.nse", "ADFFOODS.nse", "ADHUNIK.nse", "ADORWELD.nse", "ADSL.nse", "ADVANIHOT.nse", "ADVANTA.nse", "AEGISCHEM.nse", "AFL.nse", "AFTEK.nse", "AREVAT&D.nse", "M&M.nse", ".AEX,indexeuro", ".AORD,indexasx", ".HSI,indexhangseng", ",.N225,indexnikkei", ".NSEI,nse", ".NZ50,nze", ".TWII,tpe", "000001,sha", "CNX100,nse", "CNX500,nse", "CNXENERGY,nse", "CNXFMCG,nse", "CNXINFRA,nse", "CNXIT,nse" };

                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();

                    for (int i = 0; i < 14; i++)
                    {
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\GoogleIeod\\" + day.Day + GoogleIEod[i] + ".csv";
                        baseurl = "http://www.google.com/finance/getprices?q=" + GoogleIEod[i] + "&i=60&p=15d&f=d,o,h,l,c,v";
                        // "http://www.google.com/finance/getprices?q=LICHSGFIN&x=LICHSGFIN&i=60&p=15d&f=d,o,h,l,c,v"


                        downliaddata(strYearDir, baseurl);
                    }

                }


            }


            if (Cb_NSE_PR.IsChecked == true)
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
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + ".zip";

                    baseurl = "http://www.nseindia.com/archives/equities/bhavcopy/pr/PR" + date1 + lastTwoChars + ".zip";

                    //http://www.nseindia.com/archives/equities/bhavcopy/pr/PR160513.zip

                    downliaddata(strYearDir, baseurl);

                    if ( System.IO.File.Exists( strYearDir))
                    {


                         using (var zip = Ionic.Zip.ZipFile.Read(strYearDir ))
                    {
                        if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars))
                        {
                            zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars);


                        }

                    }

                        
                        
                        
                        
                        
                    dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_NEWHIGH_NEWLOW_" + date1 + lastTwoChars + ".csv";
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars+"\\HL" + date1 + lastTwoChars+".csv";
                    movefile(strYearDir, dest_filename);
                   
                        //AN
                        dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_CORPORATE_ANNOUCEMENT" + date1 + lastTwoChars + ".csv";
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\AN" + date1 + lastTwoChars + ".csv";
                    movefile(strYearDir, dest_filename);
                       //BC

                    dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_CORPORATE_ACTION" + date1 + lastTwoChars + ".csv";
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\BC" + date1 + lastTwoChars + ".csv";
                    movefile(strYearDir, dest_filename);
                        //BH
                    dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_BAND_HIT" + date1 + lastTwoChars + ".csv";
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\BH" + date1 + lastTwoChars + ".csv";
                    movefile(strYearDir, dest_filename);
//GL
                    dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_TOP10_GAINER_LOSER" + date1 + lastTwoChars + ".csv";
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\GL" + date1 + lastTwoChars + ".csv";
                    movefile(strYearDir, dest_filename);
                   
                         //fo
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\fo" + date1 + day.Year  + ".zip";
                   
                    using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                    {
                        if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\FO" + date1 + lastTwoChars))
                        {
                            zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\FO" + date1 + lastTwoChars);
                        }
                    }
                   

                    strYearDir  = txtTargetFolder.Text +" \\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\FO" + date1 + lastTwoChars +"\\FO" + date1 +day.Year +".csv";

                    
                        dest_filename = txtTargetFolder.Text + "\\STD_CSV\\NSE_Equity_Futures_fo" + date1 + lastTwoChars + ".csv";
                        if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                            Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");

                        if (!Directory.Exists(dest_filename))
                        {
                            movefile(strYearDir, dest_filename);

                        }

                    //Directory.Delete(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars, true);



                    }
                   
                    


                }

            }


                if(chkEquity.IsChecked==true )
                {

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
                        strYearDir = txtTargetFolder.Text + " \\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\FO" + date1 + lastTwoChars + "\\op" + date1 + day.Year + ".csv";
                        dest_filename = txtTargetFolder.Text + "\\STD_CSV\\NSE_Equity_Futures_OP" + date1 + lastTwoChars + ".csv";
                        if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                            Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");

                        if (!Directory.Exists(dest_filename ))
                        {
                            movefile(strYearDir, dest_filename);

                        }
                        
                    }
                }

                if (Cb_NSE_Forex_Options.IsChecked == true)
                {

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
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\cd" + date1 + day.Year + ".zip";

                        using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                        {
                            if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\CD" + date1 + lastTwoChars))
                            {
                                zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\CD" + date1 + lastTwoChars);
                            }
                        }


                        strYearDir = txtTargetFolder.Text + " \\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\CD" + date1 + lastTwoChars + "\\co" + date1 + day.Year + ".csv";


                        dest_filename = txtTargetFolder.Text + "\\STD_CSV\\NSE_Forex_Futures_co" + date1 + lastTwoChars + ".csv";
                        if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                            Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");

                        if (!Directory.Exists(dest_filename))
                        {
                            movefile(strYearDir, dest_filename);

                        }
                        
                    }
                }

                if (chkNseForex.IsChecked == true)
                {

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
                        strYearDir = txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\cd" + date1 + day.Year + ".zip";
                        if (!Directory.Exists(strYearDir ))
                        {
                            using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                            {
                                if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\CD" + date1 + lastTwoChars))
                                {
                                    zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\CD" + date1 + lastTwoChars);
                                }
                            }

                        }
                        strYearDir = txtTargetFolder.Text + " \\Downloads\\PR" + date1 + lastTwoChars + "\\Unzip1\\CD" + date1 + lastTwoChars + "\\cf" + date1 + day.Year + ".csv";


                        dest_filename = txtTargetFolder.Text + "\\STD_CSV\\NSE_Forex_Futures_cf" + date1 + lastTwoChars + ".csv";
                        if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                            Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");

                        if (!Directory.Exists(dest_filename))
                        {
                            movefile(strYearDir, dest_filename);

                        }

                    }
                }

                
                if (Cb_NSE_SME.IsChecked == true)
                {

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
                        strYearDir = txtTargetFolder.Text + " \\Downloads\\PR" + date1 + lastTwoChars + "\\sme" + date1 + lastTwoChars + ".csv";
                        dest_filename = txtTargetFolder.Text + "\\STD_CSV\\NSE_SME_sme" + date1 + lastTwoChars + ".csv";
                        if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                            Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");

                        if (!Directory.Exists(dest_filename ))
                        {
                            movefile(strYearDir, dest_filename);

                        }
                        
                    }
                }


                if (Cb_NSE_ETF.IsChecked == true)
                {

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
                        strYearDir = txtTargetFolder.Text + " \\Downloads\\PR" + date1 + lastTwoChars + "\\etf" + date1 + lastTwoChars + ".csv";
                        dest_filename = txtTargetFolder.Text + "\\STD_CSV\\NSE_ETF_etf" + date1 + lastTwoChars + ".csv";
                        if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                            Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");

                        if (!Directory.Exists(dest_filename ))
                        {
                            movefile(strYearDir, dest_filename);

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
                    string date1,year;
                    

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
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\MA" + date1 + lastTwoChars  + ".csv";

                    baseurl = "http://www.nseindia.com/archives/equities/mkt/MA" + date1 + lastTwoChars +".csv";

                //http://www.nseindia.com/archives/equities/mkt/MA160513.csv

                    downliaddata(strYearDir, baseurl);
                    dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_MARKET_ACTIVITY_"+date1+lastTwoChars+".csv";
                    
                    movefile(strYearDir,dest_filename );
                    
                  
                   




                }

            }

            if (Cb_NSE_Bulk_Deal.IsChecked == true)
            {
                prograss();


                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();
                    string day1,month, year;


                    if (day.Day < 10)
                    {
                        day1  = "0" + day.Day.ToString()+"-";
                    }
                    else
                    {
                        day1 =  day.Day.ToString() + "-";

                    }

                    if (day.Month < 10)
                    {

                        month  = "0" + day.Month.ToString()+"-";
                    }
                    else
                    {
                        month =  day.Month.ToString() + "-";

                    }
                  

                    string date1=day1 + month + day.Year ;
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\" + date1 +"-TO-"+date1+"_bulk.csv";

                    baseurl = "http://www.nseindia.com/content/equities/bulkdeals/datafiles/" + date1 + "-TO-" + date1 + "_bulk.csv";

              // baseurl=" http://www.nseindia.com/content/equities/bulkdeals/datafiles/06-05-2013-TO-09-05-2013_bulk.csv";

                    downliaddata(strYearDir, baseurl);



                    dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_Bulk_Deal" + date1  + ".csv";

                    movefile(strYearDir, dest_filename);



                }

            }

            if (Cb_NSE_Block_Deal.IsChecked == true)
            {
                prograss();


                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();
                    string day1, month, year;


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

                    downliaddata(strYearDir, baseurl);


                    dest_filename = txtTargetFolder.Text + "\\Reports\\NSE_Block_Deal" + date1 + ".csv";

                    movefile(strYearDir, dest_filename);

                }

            }


            if (Cb_NSE_India_Vix.IsChecked == true)
            {
                prograss();

                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();
                    string day1, month, year;


                    if (day.Day < 10)
                    {
                        day1 = "0" + day.Day.ToString() ;
                    }
                    else
                    {
                        day1 = day.Day.ToString() ;

                    }

                    string date1 = day1 +"-"+strMonthName +"-"+ day.Year;
                   

                    strYearDir = txtTargetFolder.Text + "\\Downloads\\" + date1 + "_" + date1 + ".csv";
                    baseurl = "http://www.nseindia.com/content/vix/histdata/hist_india_vix_"+ date1 + "_" + date1 + ".csv";

                    // baseurl=" http://www.nseindia.com/content/vix/histdata/hist_india_vix_06-May-2013_06-May-2013.csv

                    downliaddata(strYearDir, baseurl);
                }

            }

            
                 if (Cb_NSE_Vix.IsChecked == true)
            {
                prograss();

                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();
                    string day1, month, year;


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

                    downliaddata(strYearDir, baseurl);
                }

            }

                 if (BSE_Delivary_Data.IsChecked == true)
                 {
                     prograss();

                     foreach (DateTime day in EachDay(StartDate, EndDate))
                     {
                         System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                         string strMonthName = mfi.GetMonthName(day.Month).ToString();
                         string day1, month, year, date1;


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

                         downliaddata(strYearDir, baseurl);
                     }

                 }


         if (Cb_BSE_CASH_MARKET.IsChecked == true)
            {
                BSE_Delivary_Data.IsChecked = true;

                prograss();
            List<string> nameofdirtodelete = new List<String>();


                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();
                    string day1, month, year,date1;


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
                    }
                    else
                    {
                        date1 = date1 + day.Month.ToString();
                    }
                    year = day.Year.ToString();

                    string lastTwoChars = year.Substring(year.Length - 2);
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\eq" + date1 + lastTwoChars + "_csv.zip";
                    baseurl = " http://www.bseindia.com/download/BhavCopy/Equity/eq" + date1 + lastTwoChars + "_csv.zip";

                  
                    downliaddata(strYearDir, baseurl);




                    if (System.IO.File.Exists(strYearDir))
                    {
                        try
                        {
                            using (var zip = Ionic.Zip.ZipFile.Read(strYearDir))
                            {
                                zip.ExtractAll(txtTargetFolder.Text + "\\Downloads\\eq" + date1 + lastTwoChars + "_csv");
                                strYearDir = txtTargetFolder.Text + "\\Downloads\\Eq" + date1 + lastTwoChars + "_csv\\Eq" + date1 + lastTwoChars + ".csv";


                                string SCBSEALL = txtTargetFolder.Text + "\\Downloads\\SCBSEALL" + day1 + lastTwoChars + ".zip";
                                if (System.IO.File.Exists(SCBSEALL))
                                {
                                    try
                                    {
                                        using (var zip1 = Ionic.Zip.ZipFile.Read(SCBSEALL))
                                        {
                                            zip1.ExtractAll(txtTargetFolder.Text + "\\Downloads\\SCBSEALL" + day1 + lastTwoChars);
                                            SCBSEALL = txtTargetFolder.Text + "\\Downloads\\SCBSEALL" + day1 + lastTwoChars + "\\SCBSEALL" + day1 + lastTwoChars + ".txt";
                                            string SCBSEALLASCSV=txtTargetFolder.Text + "\\Downloads\\SCBSEALL" + day1 + lastTwoChars +"\\SCBSEALL" + day1 + lastTwoChars + ".csv";
                                            System.IO.File.Copy(SCBSEALL, SCBSEALLASCSV);
                                            SCBSEALL = SCBSEALLASCSV;

                                        }
                                    }
                                    catch(Exception ex )
                                    {
                                    }
                                }

                                string destfilepath = txtTargetFolder.Text + "\\STD_CSV\\BSE_Cash_Market_EQ" + date1 + lastTwoChars + ".csv";
                                string dateformtoprocessingsave = formatdate(day);

                                //If File Is Already Present Means It Process Befor this 
                                if (!System.IO.File.Exists(destfilepath))
                                {
                               BSE_Processing(strYearDir, SCBSEALL, destfilepath, dateformtoprocessingsave);

                               //if (!System.IO.File.Exists(destfilepath))
                               //{
                               //    if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                               //        Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");
                               //    strYearDir = dest_filename;
                               //    destfilepath = txtTargetFolder.Text + "\\STD_CSV\\BSE_Standard_" + date1 + lastTwoChars + ".csv";

                               //    if (!Directory.Exists(dest_filename))
                               //    {
                               //        dest_filename = txtTargetFolder.Text + "\\STD_CSV";

                               //        movefile(strYearDir, dest_filename);

                               //    }


                               //}


                                }
                                

                              
                               

                            }
                        }
                        catch(Exception ex)
                        {
                            //System.Windows.MessageBox.Show(ex.Message);
                        }
                       
                       
                    }

                    if( Directory.Exists(txtTargetFolder.Text + "\\Downloads\\Eq" + date1 + lastTwoChars + "_csv"))
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
                 string day1, month, year, date1,date2,datetoselect;


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

                     date2 =  "0" + day.Month.ToString();
                 }
                 else
                 {
                     date2 =  day.Month.ToString();
                 }

                 year = day.Year.ToString();
                 string lastTwoChars = year.Substring(year.Length - 2);
                 datetoselect = date1 + "-" + date2 + "-" + lastTwoChars;

                 strYearDir = txtTargetFolder.Text + "\\Downloads\\bhavcopy" + datetoselect + ".zip";
                 baseurl = "http://www.bseindia.com/download/Bhavcopy/Derivative/bhavcopy" +datetoselect + ".zip";
                 //http://www.bseindia.com/download/Bhavcopy/Derivative/bhavcopy23-11-12.zip
                 downliaddata(strYearDir, baseurl);
             }

         }
        
         if (BSE_Block.IsChecked == true)
         {
             prograss();

             foreach (DateTime day in EachDay(StartDate, EndDate))
             {
                 System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                 string strMonthName = mfi.GetMonthName(day.Month).ToString();
                 string day1, month, year, date1, date2, datetoselect;


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
                 strYearDir = txtTargetFolder.Text + "\\Downloads\\BSEBlock\\Block_" + day.Day  + ".csv";
                 baseurl = "http://www.bseindia.com/stockinfo/BulkBlockFiles/Block_" + date1 + strMonthName + day.Year + ".csv";
                 //http://www.bseindia.com/stockinfo/BulkBlockFiles/Block_26Dec2012.csv
                 downliaddata(strYearDir, baseurl);
                 string[] csvFileNames = Directory.GetFiles(txtTargetFolder.Text + "\\Downloads\\BSEBlock", "*.csv");

                 JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\Reports\\bseblockdeals.csv");



                 dest_filename = txtTargetFolder.Text + "\\Reports\\bseblockdeals.csv";

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
                 string day1, month, year, date1, date2, datetoselect;


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
                 downliaddata(strYearDir, baseurl);

                 string[] csvFileNames = Directory.GetFiles(txtTargetFolder.Text + "\\Downloads\\BSEBulk", "*.csv");

                 JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\Reports\\bsebulkdeals.csv");

                 dest_filename = txtTargetFolder.Text + "\\Reports\\bsebulkdeals.csv";

                // movefile(strYearDir, dest_filename);


             }

            

         }
             
         if (BSE_Index.IsChecked == true)
         {
             prograss();
             prograss();

             strYearDir =  txtTargetFolder.Text + "\\Downloads\\Bse";
             if (!Directory.Exists(strYearDir))
                 Directory.CreateDirectory(strYearDir);

             foreach (DateTime day in EachDay(StartDate, EndDate))
             {
                 System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                 string strMonthName = mfi.GetMonthName(day.Month).ToString();
                 string day1, month, year, date1, date2, datetoselect;


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
                 datetoselect = date2 + "/" + date1 + "/" +day.Year ;
                 filename=day.Day.ToString() ;
                    //BSE30
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSE30.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSE30%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //MIDCAP
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\MIDCAP.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=MIDCAP%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //SMLCAP
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\SMLCAP.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=SMLCAP%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);
                     //BSE100
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSE100.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSE100%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);
                   //BSE200
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSE200.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSE200%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //BSE500
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSE500.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSE500%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //BSE500
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSE500.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSE500%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //AUTO
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\AUTO.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=AUTO%20%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //BANKEX
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BANKEX.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BANKEX%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //BSECD
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSECD.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSECD%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                  //BSECG
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSECG.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSECG%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);


                     //BSEFMCG
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSEFMCG.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSEFMCG&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //BSEHC
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSEHC.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSEHC%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);


                     //BSEIT
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSEIT.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSEIT%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //METAL
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\METAL.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=METAL%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //OILGAS
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\OILGAS.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=OILGAS%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);


                     //POWER
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\POWER.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=POWER%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //BSEPSU
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSEPSU.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSEPSU%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //REALTY
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\REALTY.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=REALTY%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //TECK
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\TECK.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=TECK%20%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);


                     //DOL
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\DOL.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=DOL30%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //DOL100
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\DOL100.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=DOL100%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //DOL200
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\DOL200.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=DOL200%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //SHA50
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\SHA50.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=SHA50%20%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //GREENX
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\GREENX.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=GREENX%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);
                     //BSEIPO
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\BSEIPO.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=BSEIPO%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //CARBON
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\CARBON.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=CARBON%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     //SMEIPO
                     strYearDir = txtTargetFolder.Text + "\\Downloads\\Bse\\SMEIPO.csv";
                     baseurl = "http://www.bseindia.com/stockinfo/indices_main_excel.aspx?ind=SMEIPO%20&fromDate=" + datetoselect + "&toDate=" + datetoselect + "&DMY=D";
                     downliaddata(strYearDir, baseurl);

                     
                     string[] csvFileNames = Directory.GetFiles(txtTargetFolder.Text + "\\Downloads\\bse", "*.csv");

                     JoinCsvFiles(csvFileNames, txtTargetFolder.Text + "\\Downloads\\BSEIndex"+day.Day +".csv");

                
             }
         }

         if (National_Spot_Exchange.IsChecked == true)
         {
             prograss();

             foreach (DateTime day in EachDay(StartDate, EndDate))
             {
                 System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                 string strMonthName = mfi.GetMonthName(day.Month).ToString();
                 string day1, month, year, date1, date2, datetoselect;


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
                 datetoselect =  date2+date1  + day.Year;
                 strMonthName = strMonthName.Substring(0, 3);
                 strYearDir = txtTargetFolder.Text + "\\Downloads\\NSEL_" + day.Day + ".csv";
                 baseurl = "http://www.nationalspotexchange.com//NSELBhavCopyFiles///25052013//hdy2zs5511tyhiunba5ybyjt//NSEL_" + datetoselect + ".csv";
                 //http://www.nationalspotexchange.com//NSELBhavCopyFiles///25052013//hdy2zs5511tyhiunba5ybyjt//NSEL_05242013.csv
                 downliaddata(strYearDir, baseurl);
             }


         }

         
         if (MCXSX_Equity_Futures.IsChecked == true)
         {
             prograss();

             foreach (DateTime day in EachDay(StartDate, EndDate))
             {
                 System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                 string strMonthName = mfi.GetMonthName(day.Month).ToString();
                 string day1, month, year, date1, date2, datetoselect;


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
                 downliaddata(strYearDir, baseurl);


                 dest_filename = txtTargetFolder.Text + "\\Reports\\MarketStatisticsReport" + day.Day + ".csv";

                 movefile(strYearDir, dest_filename);



                 //process 
                 if (System.IO.File.Exists(strYearDir))
                 {

                     string destfilepath = txtTargetFolder.Text + "\\STD_CSV\\MCX_Equity_FUTURE_STD" + day.Day + ".csv";
                     string dateformtoprocessingsave = formatdate(day);
                     string nameoffile = "MCX_Equity";

                     FUTURE_Processing(strYearDir, destfilepath, dateformtoprocessingsave, nameoffile);




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
                 string day1, month, year, date1, date2, datetoselect;


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
                 baseurl = "http://www.mcx-sx.com/downloads/daily/CurrencyDownloads/Bhavcopy%20"+strMonthName +"%20"+date1 +",%20"+day.Year +".xls";
                 //http://www.mcx-sx.com/downloads/daily/CurrencyDownloads/Bhavcopy%20May%2016,%202013.xls.
                 downliaddata(strYearDir, baseurl);

                 ////process 
                 //strYearDir = txtTargetFolder.Text + "\\Downloads\\" + day.Day + day.Month + day.Year + "ComodityBhavCopy.csv";
                 //if (System.IO.File.Exists(strYearDir))
                 //{
                    


                 //    string destfilepath = txtTargetFolder.Text + "\\Downloads\\Temp_FUTURE_STD.csv";
                 //    string dateformtoprocessingsave = formatdate(day);
                 //    string nameoffile = "MCX_ComodityBhavCopy";

                 //    FUTURE_Processing(strYearDir, destfilepath, dateformtoprocessingsave, nameoffile);

                   


                 //}




             }


         }


         if (MCXSX_Block.IsChecked == true)
         {
             prograss();

             foreach (DateTime day in EachDay(StartDate, EndDate))
             {
                 System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                 string strMonthName = mfi.GetMonthName(day.Month).ToString();
                 string day1, month, year, date1, date2, datetoselect;


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
                 downliaddata(strYearDir, baseurl);

                 dest_filename = txtTargetFolder.Text + "\\Reports\\MCX-SX-EQ_BLOCK_DEAL_" + datetoselect + ".csv";

                 movefile(strYearDir, dest_filename);

             }


         }

         if (MCXSX_Bulk.IsChecked == true)
         {
             prograss();

             foreach (DateTime day in EachDay(StartDate, EndDate))
             {
                 System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                 string strMonthName = mfi.GetMonthName(day.Month).ToString();
                 string day1, month, year, date1, date2, datetoselect;


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
                 downliaddata(strYearDir, baseurl);


                 dest_filename = txtTargetFolder.Text + "\\Reports\\MCX-SX-EQ_BULK_DEAL_" + datetoselect + ".csv";

                 movefile(strYearDir, dest_filename);
             }


         }



         if (MCXCommodity_Futures.IsChecked == true)
         {
             WebClient webClient = new WebClient();

             foreach (DateTime day in EachDay(StartDate, EndDate))
             {


                 System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                 string strMonthName = mfi.GetMonthName(day.Month).ToString();
                 string day1, month, year, date1, date2, datetoselect;


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
                 forms["mTbdate"] = date2 +"/"+date1 +"/"+day.Year ;
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

                 System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\"+day.Day+day.Month+day.Year  +"ComodityBhavCopy.csv", responseData);


                 //process 
                 strYearDir=txtTargetFolder.Text + "\\Downloads\\"+day.Day+day.Month+day.Year  +"ComodityBhavCopy.csv";
                 if (System.IO.File.Exists(strYearDir))
                 {

                     string destfilepath = txtTargetFolder.Text + "\\STD_CSV\\Temp_FUTURE_STD.csv";
                     string dateformtoprocessingsave = formatdate(day);
                   string nameoffile="MCX_ComodityBhavCopy";

                         FUTURE_Processing(strYearDir, destfilepath, dateformtoprocessingsave,nameoffile );

                     
                   

                 }







             }
         }


         if (MCX_Index.IsChecked == true)
         {
             MCXSX_Spot_Indices.IsChecked = true;
             WebClient webClient = new WebClient();
           string[] arrIndexValues =  new string[]{"323","324","325","326"};
           string []arrindexvaluesname = new string[] { "MCXCOMDEX","MCXMETAL","MCXENRGY","MCXAGRI"};
     string[] arrSpotIndexValues = new string[]{"327","328","329","330"};
               


             foreach (DateTime day in EachDay(StartDate, EndDate))
             {

                 System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                 string strMonthName = mfi.GetMonthName(day.Month).ToString();
                 string day1, month, year, date1, date2, datetoselect;


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
                 for(int i=0;i<4;i++)
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

                 System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\"+arrindexvaluesname[i]+"_"+day.Day +".csv", responseData);
             }
             }


            
         }





        


         if (MCXSX_Spot_Indices.IsChecked == true)
         {
             WebClient webClient = new WebClient();
             string[] arrIndexValues = new string[] { "323", "324", "325", "326" };
             string[] arrindexvaluesname = new string[] { "Spot_MCXCOMDEX", "Spot_MCXMETAL", "Spot_MCXENRGY", "Spot_MCXAGRI" };
             string[] arrSpotIndexValues = new string[] { "327", "328", "329", "330" };



             foreach (DateTime day in EachDay(StartDate, EndDate))
             {

                 System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                 string strMonthName = mfi.GetMonthName(day.Month).ToString();
                 string day1, month, year, date1, date2, datetoselect;


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
                   //  forms["mDdlOtherIndex"] = arrSpotIndexValues [i];
                     forms["mRbtLstSpotFut"] = "0";
                   //  forms["mTbFromDate"] = date2 + "/" + date1 + "/" + day.Year;
                    // forms["mTbToDate"] = date2 + "/" + date1 + "/" + day.Year; ;


                    // forms["mBtnGo.x"] = "130";
                    // forms["mBtnGo.y"] = "40";

                     // forms["ScriptManager1"] = "MupdPnl|mImgBtnGo";


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
                     forms["mRbtLstSpotFut"] = "0";
                     forms["mTbFromDate"] = date2 + "/" + date1 + "/" + day.Year;
                     forms["mTbToDate"] = date2 + "/" + date1 + "/" + day.Year; ;


                     webClient.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                     responseData = webClient.UploadValues(@"http://www.mcxindia.com/sitepages/indexhistory.aspx", "POST", forms);

                     System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\index1.html", responseData);



                     s = System.Text.Encoding.UTF8.GetString(responseData);
                     __EVENTVALIDATION = ExtractVariable(s, "__EVENTVALIDATION");

                     forms = new NameValueCollection();
                     forms["__EVENTTARGET"] = "linkButton";
                     forms["__EVENTARGUMENT"] = "";
                     forms["__VIEWSTATE"] = ExtractVariable(s, "__VIEWSTATE");

                     forms["__EVENTVALIDATION"] = __EVENTVALIDATION;
                     forms["mDdlOtherIndex"] = arrSpotIndexValues[i];
                     forms["mRbtLstSpotFut"] = "0";
                     forms["mTbFromDate"] = date2 + "/" + date1 + "/" + day.Year;
                     forms["mTbToDate"] = date2 + "/" + date1 + "/" + day.Year; ;


                     webClient.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                     responseData = webClient.UploadValues(@"http://www.mcxindia.com/sitepages/indexhistory.aspx", "POST", forms);



                     System.IO.File.WriteAllBytes(txtTargetFolder.Text + "\\Downloads\\" + arrindexvaluesname[i] + "_" + day.Day + ".csv", responseData);
                 }
             }

           
         }
                         prograss();

                ProgressBar1.Value = ProgressBar1.Maximum;
         }
         while (ProgressBar1.Value != ProgressBar1.Maximum);

         btnStart.IsEnabled =true ;
         lbl_Download.Content = "Download Completed ";
         System.Windows.Forms.MessageBox.Show("Download Completed Please See Log File In c:\\Temp");
         ProgressBar1.Visibility = Visibility.Hidden;

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

                
            }
            
            File.WriteAllText(outputDestinationPath, sb.ToString());
            
          
        }


        private void MTO_Processing(string sourcePath,string dateforsave )
        {
            //var sourcePath = @"C:\MTO_03052013.csv";
            var delimiter = ",";
            var firstLineContainsHeaders = true;
           var tempPath = txtTargetFolder.Text +"\\Downloads\\MTO\\MTO_std_"+dateforsave +".csv";

           if (!Directory.Exists(txtTargetFolder.Text + "\\Downloads\\MTO"))
               Directory.CreateDirectory(txtTargetFolder.Text + "\\Downloads\\MTO");
            DateTime date1;
            var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");



            //mto file processing 

            using (var writer = new StreamWriter(tempPath))

            using (var reader = new StreamReader(sourcePath))
            {
                string line = null;
                string[] headers = new string[7];
                string[] headers1 = new string[7];




                //Read Header and write into new file 
                if (firstLineContainsHeaders)
                {







                    headers[0] = " Record Type";
                    headers[1] = "Sr No";
                    headers[2] = "Name of Security";
                    headers[3] = "SERIES";
                    headers[4] = "Quantity Traded";
                    headers[5] = " Deliverable Quantity(gross across client level)";
                    headers[6] = "Traded Quantity";








                    writer.WriteLine(string.Join(delimiter, headers));

                }

                //it read first three line which is not require 
                for (int i = 0; i < 4; i++)
                {
                    line = reader.ReadLine();

                }

                while ((line = reader.ReadLine()) != null)
                {



                    var columns = splitExpression.Split(line).Where(s => s != delimiter).ToArray();




                    //take only BE and EQ data 
                    if (columns[3] == "EQ" || columns[3] == "BE")
                    {
                        writer.WriteLine(string.Join(delimiter, columns));

                    }


                }

            }



        }




        



        private void NSE_Processing(string sourcePath, string mtopath, string tempPath,string dateformtoprocess)
        {
           // var sourcePath = @"C:\cm14MAY2013bhav.csv";

            if(!System.IO.File.Exists(mtopath ))
            {
                ILog log = LogManager.GetLogger(typeof(MainWindow));
                log.Debug("MTO File Is Not Present Can not Process"+sourcePath );
                return;

            }
            MTO_Processing(mtopath,dateformtoprocess );
          
            if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");


            var delimiter = ",";
            var firstLineContainsHeaders = true;
           // var tempPath =txtTargetFolder.Text+"\\Downloads\\NSE_STD.csv";
            DateTime date1;
            var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");




            //NSE_std
           


            using (var writer = new StreamWriter(tempPath))


            // Create file as   <TICKER>,<NAME>,<DATE>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOLUME>,<OPENINT>
            // <OPENINT> is blank now 
            using (var reader = new StreamReader(sourcePath))
            {
                string line = null;
                string[] headers = null;




                //Read Header and write into new file 
                if (firstLineContainsHeaders)
                {
                    line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line)) return;

                    headers = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                    headers[0] = "TICKER";
                    headers[1] = "NAME";
                    headers[2] = "DATE";
                    headers[3] = "OPEN";
                    headers[4] = "HIGH";
                    headers[5] = "LOW";
                    headers[6] = "CLOSE";
                    headers[7] = "VOLUME";
                    headers[8] = "OPENINT";
                    headers[9] = "AUX1";
                    headers[10] = "";
                    headers[11] = "";
                    headers[12] = "";
                    headers[13] = "";



                    writer.WriteLine(string.Join(delimiter, headers));

                }

                while ((line = reader.ReadLine()) != null)
                {

                    var columns = splitExpression.Split(line).Where(s => s != delimiter).ToArray();
                    if (columns[1] == "EQ" || columns[1] == "BE")
                    {
                        columns[0] = columns[0];
                        columns[1] = "";
                        columns[7] = columns[8];

                        columns[6] = columns[5];

                        columns[5] = columns[4];

                        columns[4] = columns[3];


                        columns[3] = columns[2];

                        //convert date to YYYYMMDD  format

                        string date = columns[10];  //take date
                        int finaldate;
                        date = columns[10].Substring(3, 3);



                        date = date.ToUpper();
                        if (date == "JAN")
                        {
                            date = "January";
                        }
                        else if (date == "FEB")
                        {
                            date = "February ";

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
                            date = "October";

                        }
                        else if (date == "NOV")
                        {
                            date = "November";
                        }
                        else if (date == "DEC")
                        {
                            date = "December";

                        }



                        finaldate = DateTime.ParseExact(date, "MMMM", CultureInfo.CurrentCulture).Month;
                        if (finaldate < 10)
                        {
                            date = "0" + finaldate.ToString();
                        }
                        else
                        {
                            date = finaldate.ToString();
                        }

                        //20 is for adding year as 2013
                        string datetostore = columns[10].Substring(7, 4) + date + columns[10].Substring(0, 2);
                        columns[2] = datetostore;
                        columns[8] = "";

                        columns[10] = "";
                        columns[11] = "";
                        columns[12] = "";
                        columns[13] = "";


                        var reader1 = new StreamReader(txtTargetFolder.Text + "\\Downloads\\sec_list.csv");
                        string line1 = null;
                        var reader2 = new StreamReader(txtTargetFolder.Text + "\\Downloads\\MTO\\MTO_std_" + dateformtoprocess + ".csv");
                        string line2 = null;
                        //Read sec_list file and copy data in to nse_std
                        while ((line1 = reader1.ReadLine()) != null)
                        {
                            var columns1 = splitExpression.Split(line1).Where(s => s != delimiter).ToArray();

                            if (columns[0] == columns1[0])
                            {
                                columns[1] = columns1[2];
                                break;
                            }


                        }

                        //Read MTO_std file and copy data in to nse_std
                        while ((line2 = reader2.ReadLine()) != null)
                        {
                            var columns2 = splitExpression.Split(line2).Where(s => s != delimiter).ToArray();

                            if (columns[0] == columns2[2])//match col of bhav copy symbol to mto sysmbol and copy Traded Quantity value to OPENINT
                            {
                                columns[8] = columns2[5];
                                break;
                            }


                        }

                        writer.WriteLine(string.Join(delimiter, columns));

                    }
                }

            }



            



        }
        private void BSE_Processing(string sourcePath, string SCBSEALL, string tempPath, string dateformtoprocess)
        {
           

            //if (!System.IO.File.Exists(mtopath))
            //{
            //    ILog log = LogManager.GetLogger(typeof(MainWindow));
            //    log.Debug("MTO File Is Not Present Can not Process" + sourcePath);
            //    return;

            //}
            //MTO_Processing(mtopath, dateformtoprocess);

            if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");


            var delimiter = ",";
            var delimiter1 = "|";

            var firstLineContainsHeaders = true;
            // var tempPath =txtTargetFolder.Text+"\\Downloads\\NSE_STD.csv";
            DateTime date1;
            var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");




            //NSE_std



            using (var writer = new StreamWriter(tempPath))


            // Create file as   <TICKER>,<NAME>,<DATE>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOLUME>,<OPENINT>
            // <OPENINT> is blank now 
            using (var reader = new StreamReader(sourcePath))
            {
                string line = null;
                string[] headers = null;




                //Read Header and write into new file 
                if (firstLineContainsHeaders)
                {
                    line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line)) return;

                    headers = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                    headers[0] = "TICKER";
                    headers[1] = "NAME";
                    headers[2] = "DATE";
                    headers[3] = "OPEN";
                    headers[4] = "HIGH";
                    headers[5] = "LOW";
                    headers[6] = "CLOSE";
                    headers[7] = "VOLUME";
                    headers[8] = "OPENINT";
                    headers[9] = "";
                    headers[10] = "";
                    headers[11] = "";
                    headers[12] = "";
                    headers[13] = "";



                    writer.WriteLine(string.Join(delimiter, headers));

                }

                while ((line = reader.ReadLine()) != null)
                {

                    var columns = splitExpression.Split(line).Where(s => s != delimiter).ToArray();
                    //date  col1 and 2 same no change 
                        columns[2] = "";
                    
                        columns[3] = columns[4];

                        columns[4] = columns[5];

                        columns[5] = columns[6];

                        columns[6] = columns[7];

                        columns[7] = columns[11];

                        //columns[3] = columns[2];

                        //convert date to YYYYMMDD  format


                        columns[8] = "";

                        columns[9] = "";
                        columns[10] = "";
                        columns[11] = "";
                        columns[12] = "";
                        columns[13] = "";


                    //if SCBSEALL File Present then copy date and DELIVERY QTY to OPENINT
                    if(System.IO.File.Exists(SCBSEALL))
                    {


                        var reader1 = new StreamReader(SCBSEALL);
                        string line1 = null;
                        //Read sec_list file and copy data in to nse_std
                        while ((line1 = reader1.ReadLine()) != null)
                        {
                            string[] values = line1.Split('|');//This is for read and spilt SCBSEALL file 


                           

                            //Check Sec_Code 
                            if (columns[0] == values [1])
                            {
                                //Copy Date and DELIVERY QTY into Date and Openint
                                columns[8] = values [2];
                                columns[2] = values[0];
                                break;
                            }


                        }    
                  

                    }

                       



                        writer.WriteLine(string.Join(delimiter, columns));

                    
                }

            }






        }
        private void FUTURE_Processing(string sourcePath, string tempPath, string dateformtoprocess,string nameoffile)
        {

            var delimiter = ",";
            var firstLineContainsHeaders = true;
            var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");


            if (!Directory.Exists(txtTargetFolder.Text + "\\STD_CSV"))
                Directory.CreateDirectory(txtTargetFolder.Text + "\\STD_CSV");



            List<Int32> lowvalue = new List<int> { };




    
            using (var writer = new StreamWriter(tempPath))


            //this for taking lowest date 
            using (var reader = new StreamReader(sourcePath))
            {
                string line = null;
                string[] headers = null;




                //Read Header and write into new file 
                if (firstLineContainsHeaders)
                {
                    line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line)) return;

                    headers = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                    headers[0] = "TICKER";
                    headers[1] = "NAME";
                    headers[2] = "DATE";
                    headers[3] = "OPEN";
                    headers[4] = "HIGH";
                    headers[5] = "LOW";
                    headers[6] = "CLOSE";
                    headers[7] = "VOLUME";
                    headers[8] = "OPENINT";
                    headers[9] = "";
                    headers[10] = "";
                    headers[11] = "";
                    if (nameoffile == "MCX_Equity")
                    {
                        headers[12] = "";
                        headers[13] = "";
                        headers[14] = "";
                       


                    }
                    if (nameoffile == "NSE_INDEX")
                    {
                        headers[12] = "";
                       



                    }


                    writer.WriteLine(string.Join(delimiter, headers));

                }
                int i = 0;
                while ((line = reader.ReadLine()) != null)
                {

                    var columns = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                    //convert date to YYYYMMDD  format

                    string date = columns[0];  //take date
                    int finaldate;
                    date = columns[0].Substring(3, 3);
                    date = date.ToUpper();
                    if (date == "JAN")
                    {
                        date = "January";
                    }
                    else if (date == "FEB")
                    {
                        date = "February ";

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
                        date = "October";

                    }
                    else if (date == "NOV")
                    {
                        date = "November";
                    }
                    else if (date == "DEC")
                    {
                        date = "December";

                    }




                    try
                    {
                        finaldate = DateTime.ParseExact(date, "MMMM", CultureInfo.CurrentCulture).Month;
                        if (finaldate < 10)
                        {
                            date = "0" + finaldate.ToString();
                        }
                        else
                        {
                            date = finaldate.ToString();
                        }
                        string datetostore = columns[2].Substring(7, 4) + date + columns[2].Substring(0, 2);

                        lowvalue.Add(Convert.ToInt32(datetostore));

                    }
                    catch
                    {
                    }
                    //    //20 is for adding year as 2013

                    if (nameoffile == "MCX_ComodityBhavCopy")
                    {

                        columns[0] = columns[1];
                        columns[7] = columns[8];
                        columns[1] = "";
                        columns[8] = "";/////OPen Int 
                        columns[9] = "";
                        columns[10] = "";
                        columns[11] = "";

                    writer.WriteLine(string.Join(delimiter, columns));

                    }

                    if (nameoffile == "MCX_Currency")
                    {

                        columns[0] = columns[1];
                        columns[2] = columns[3];
                        columns[1] = "";
                        columns[3] = columns[6];
                        columns[4] = columns[7];
                        columns[5] = columns[8];
                        columns[6] = columns[9];
                        columns[7] = columns[12];
                        columns[8] = columns[15];



                        columns[9] = "";
                        columns[10] = "";
                        columns[11] = "";
                        columns[12] = "";
                        columns[13] = "";
                        columns[14] = "";
                    writer.WriteLine(string.Join(delimiter, columns));


                    }
                    if (nameoffile == "MCX_Equity")
                    {

                        string temp, dateforMCXEquity;
                        temp = columns[0];

                        //10/5/2013
                        dateforMCXEquity = columns[0].Substring(6,4)+columns[0].Substring(3,2)+columns[0].Substring(0,2);
                        columns[0] = columns[2];

                        columns[2] = dateforMCXEquity;

                        columns[1] = "";
                        columns[3] = columns[5];
                        columns[4] = columns[6];
                        columns[5] = columns[7];
                        columns[6] = columns[8];
                        columns[7] = columns[12];
                        columns[8] = "";



                        columns[9] = "";
                        columns[10] = "";
                        columns[11] = "";
                        columns[12] = "";
                        columns[13] = "";
                        columns[14] = "";
                        writer.WriteLine(string.Join(delimiter, columns)); 

                    }
                    if (nameoffile == "NSE_INDEX")
                    {

                        string temp, dateforMCXEquity;
                        temp = columns[0];

                        //10/5/2013
                        dateforMCXEquity = columns[1].Substring(6,4)+columns[1].Substring(3,2)+columns[1].Substring(0,2);

                        columns[1] = "";
                        columns[6] = columns[5];
                        columns[5] = columns[4];
                        columns[4] = columns[3];
                        columns[3] = columns[2];
                        
                        columns[2]=dateforMCXEquity;
                        columns[7] = columns[8];
                        columns[8] = "";



                        columns[9] = "";
                        columns[10] = "";
                        columns[11] = "";
                        columns[12] = "";


                        var reader1 = new StreamReader(txtTargetFolder.Text + "\\Downloads\\sec_list.csv");
                        string line1 = null;

                        //Read sec_list file
                        while ((line1 = reader1.ReadLine()) != null)
                        {
                            var columns1 = splitExpression.Split(line1).Where(s => s != delimiter).ToArray();

                            if (columns[0] == columns1[0])
                            {
                                columns[1] = columns1[2];
                                break;
                            }


                        }
                        writer.WriteLine(string.Join(delimiter, columns));

                        
                        writer.WriteLine(string.Join(delimiter, columns)); 

                    }
                               
               
                    }
                if (nameoffile == "MCX_Equity" || nameoffile == "NSE_INDEX")
                {
                        //no need of date filtering so dont exicute following code which is for date date+1 and date+2;

                    return;
                }

            }

            lowvalue[0] = lowvalue[1];
            string l, lmonth;
            int lmon;
            l = lowvalue.Min().ToString();
            lmonth = l.Substring(4, 2);
            lmon = Convert.ToInt32(lmonth);

            sourcePath = tempPath ;//take file just save as lowest date 
            tempPath = txtTargetFolder.Text + "\\Downloads\\"+nameoffile +"_FUTURE_STD"+dateformtoprocess +".csv";
            

            using (var writer = new StreamWriter(tempPath))


            // This for taking store lowest date +2 data only 
            using (var reader = new StreamReader(sourcePath))
            {
                string line = null;
                string[] headers = null;




                //Read Header and write into new file 
                if (firstLineContainsHeaders)
                {
                    line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line)) return;

                    headers = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                    headers[0] = "TICKER";
                    headers[1] = "NAME";
                    headers[2] = "DATE";
                    headers[3] = "OPEN";
                    headers[4] = "HIGH";
                    headers[5] = "LOW";
                    headers[6] = "CLOSE";
                    headers[7] = "VOLUME";
                    headers[8] = "OPENINT";
                    headers[9] = "";
                    headers[10] = "";
                    headers[11] = "";



                    writer.WriteLine(string.Join(delimiter, headers));

                }
                int i = 0;
                while ((line = reader.ReadLine()) != null)
                {

                    var columns = splitExpression.Split(line).Where(s => s != delimiter).ToArray();



                    string date = columns[2];  //take date
                    int finaldate;
                    date = columns[2].Substring(3, 3);
                    date = date.ToUpper();
                    if (date == "JAN")
                    {
                        date = "January";
                    }
                    else if (date == "FEB")
                    {
                        date = "February ";

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
                        date = "October";

                    }
                    else if (date == "NOV")
                    {
                        date = "November";
                    }
                    else if (date == "DEC")
                    {
                        date = "December";

                    }





                    finaldate = DateTime.ParseExact(date, "MMMM", CultureInfo.CurrentCulture).Month;
                    if (finaldate < 10)
                    {
                        date = "0" + finaldate.ToString();
                    }
                    else
                    {
                        date = finaldate.ToString();
                    }

                    //    //20 is for adding year as 2013
                    string datetostore = columns[2].Substring(7, 4) + date + columns[2].Substring(0, 2);
                    columns[2] = datetostore;

                    int lmonth1, lmonth2;
                    lmonth1 = lmon + 1;
                    lmonth2 = lmon + 2;




                    int date1 = Convert.ToInt32(date);
                    if (date1 == lmon || date1 == lmonth1 || date1 == lmonth2)
                    {

                       


                        var reader1 = new StreamReader(txtTargetFolder.Text +"\\Downloads\\sec_list.csv");
                        string line1 = null;

                        //Read sec_list file
                        while ((line1 = reader1.ReadLine()) != null)
                        {
                            var columns1 = splitExpression.Split(line1).Where(s => s != delimiter).ToArray();

                            if (columns[0] == columns1[0])
                            {
                                if (date1 == lmon)
                                {
                                    columns[1] = columns1[2]+" -I";

                                }

                                if (date1 == lmonth1 )
                                {
                                    columns[1] = columns1[2] + " -II";

                                }
                                if (date1 == lmonth2 )
                                {
                                    columns[1] = columns1[2] + " -III";

                                }
                                break;
                            }


                        }
                        writer.WriteLine(string.Join(delimiter, columns));

                    }




                }

            }





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
        private void downliaddata(string path,string url)
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
                        Client.DownloadFile(url, path );


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
                            log.Warn("Data Not Found For " +url );
                           
                        }
                    }
                   

                }

      
       

        private void button2_Click(object sender, RoutedEventArgs e)
        {
           
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
       
        private void wMain_Loaded(object sender, RoutedEventArgs e)
        {
            
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(typeof(MainWindow));
            log.Debug("Application Strated Successfully");
            
            BinaryFormatter bf = new BinaryFormatter();
            //Use For Serialization Data Get Save In Fileio.txt file 

            Cb_NSE_EOD_BhavCopy.IsChecked = true;//dont delete it is imp
            if (File.Exists(@"C:\Fileio.txt"))
            {

                FileStream fs = new FileStream(@"C:\Fileio.txt", FileMode.Open, FileAccess.Read);
                target1 t1 = (target1)bf.Deserialize(fs);


                if (t1.target == "")
                {

                    if (!Directory.Exists(@"C:\Downloads"))
                    {
                        Directory.CreateDirectory(@"C:\Downloads");
                    }
                    txtTargetFolder.Text = @"C:\Downloads";

                }
                else
                {
                    txtTargetFolder.Text = t1.target;


                }
                




                 dtStartDate.Text = t1.fromdate.ToShortDateString();
                 dtEndDate.Text = t1.todate.ToShortDateString();
                dtEndDate.Text = DateTime.Today.ToShortDateString();
                dtStartDate.Text = DateTime.Today.ToShortDateString();
                Cb_BSE_CASH_MARKET.IsChecked = t1.Cb_BSE_CASH_MARKET;
                BSE_Delivary_Data.IsChecked = t1.BSE_Delivary_Data;
                BSE_Block.IsChecked = t1.BSE_Block;
                BSE_Bulk.IsChecked = t1.BSE_Bulk;
                MCXSX_Currency.IsChecked = t1.MCXSX_Currency;
                MCXSX_Block.IsChecked = t1.MCXSX_Block;
                MCXSX_Bulk.IsChecked = t1.MCXSX_Bulk;



                
                
                

                



                Cb_BSE_Equity_Futures.IsChecked = t1.Cb_BSE_Equity_Futures;
                BSE_Index.IsChecked = t1.BSE_Index;

                ChkBseFo.IsChecked = t1.ChkBseFo;


              //  Cb_NSE_EOD_BhavCopy.IsChecked = t1.Cb_NSE_EOD_BhavCopy;
                chkEquity.IsChecked = t1.chkEquity;
                Cb_NSE_Forex_Options.IsChecked = t1.Cb_NSE_Forex_Options;
                Cb_NSE_SME.IsChecked = t1.Cb_NSE_SME;
                Cb_NSE_ETF.IsChecked = t1.Cb_NSE_ETF;
                Cb_NSE_Index.IsChecked = t1.Cb_NSE_Index;
                Cb_Reports.IsChecked = t1.Cb_Reports;
                chkCombinedReport.IsChecked = t1.chkCombinedReport;
                chkNseForex.IsChecked = t1.chkNseForex;
                chkNseNcdex.IsChecked = t1.chkNseNcdex;


              

                Cb_NSE_Market_Activity.IsChecked = t1.Cb_NSE_Market_Activity;

                Cb_NSE_PR.IsChecked = t1.Cb_NSE_PR;
                Cb_NSE_Bulk_Deal.IsChecked = t1.Cb_NSE_Bulk_Deal;
                Cb_NSE_Block_Deal.IsChecked = t1.Cb_NSE_Block_Deal;
                Cb_NSE_India_Vix.IsChecked = t1.Cb_NSE_India_Vix;
                Cb_NSE_Vix.IsChecked = t1.Cb_NSE_Vix;





                MCXSX_Forex_Future.IsChecked = t1.MCXSX_Forex_Future;
                MCXSX_Equity_Futures.IsChecked = t1.MCXSX_Equity_Futures;
                MCXCommodity_Futures.IsChecked = t1.MCXCommodity_Futures;
                MCXSX_Equity_Options.IsChecked = t1.MCXSX_Equity_Options;
                MCXSXForex_Options.IsChecked = t1.MCXSXForex_Options;
                National_Spot_Exchange.IsChecked = t1.National_Spot_Exchange;
                MCXSX_Equity_Indices.IsChecked = t1.MCXSX_Equity_Indices;
                MCX_Index.IsChecked = t1.MCX_Index;


                chkYahooEOD.IsChecked = t1.chkYahooEOD;
                ChkYahooIEOD1.IsChecked = t1.ChkYahooIEOD1;
                chkYahooFundamental.IsChecked = t1.chkYahooFundamental;
                ChkYahooIEOD5.IsChecked = t1.ChkYahooIEOD5;
                Cb_Yahoo_Realtime.IsChecked = t1.Cb_Yahoo_Realtime;

                ChkGoogleEOD.IsChecked = t1.ChkGoogleEOD;
                ChkGoogleIEOD.IsChecked = t1.ChkGoogleIEOD;
                Cb_MCX_Google_IEOD_5min.IsChecked = t1.Cb_MCX_Google_IEOD_5min;


                Cb_Corporate_Events.IsChecked = t1.Cb_Corporate_Events;
                Cb_Board_Message.IsChecked = t1.Cb_Board_Message;
                Cb_Delete_all_events.IsChecked = t1.Cb_Delete_all_events;



                fs.Close();


            }
            else
            {
                dtStartDate.Text = DateTime.Today.Date.ToString();
                dtEndDate.Text = DateTime.Today.Date.ToString();
                textBox1.Text = "";
            }
           
           // Check_internet_connetion(url1);
        }

        private void wMain_Closed(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(typeof(MainWindow));
            log.Debug("Application Close ");
            //savechanges();
           
        }
        private void savechanges()
        {
            //if (dtStartDate.Text.ToString() == "")
            //{

            //}
            //else
            {
                flag = 1;
                target1 t = new target1();
               // t.fromdate = Convert.ToDateTime(dtStartDate.Text);
               // t.todate = Convert.ToDateTime(dtEndDate.Text);
                t.target = txtTargetFolder.Text;
                
                t.Cb_BSE_CASH_MARKET=Cb_BSE_CASH_MARKET.IsChecked.Value ;
t.Cb_BSE_Equity_Futures=Cb_BSE_Equity_Futures.IsChecked.Value;
t.ChkBseFo=ChkBseFo.IsChecked.Value;
                

//t.Cb_NSE_EOD_BhavCopy=Cb_NSE_EOD_BhavCopy.IsChecked.Value;
t.chkEquity=chkEquity.IsChecked.Value;
t.Cb_NSE_Forex_Options=Cb_NSE_Forex_Options.IsChecked.Value;
t.Cb_NSE_SME=Cb_NSE_SME.IsChecked.Value;
t.Cb_NSE_ETF = Cb_NSE_ETF.IsChecked.Value;
t.Cb_NSE_Index= Cb_NSE_Index.IsChecked.Value;
t.Cb_Reports= Cb_Reports.IsChecked.Value;
t.chkCombinedReport= chkCombinedReport.IsChecked.Value;
t.chkNseForex= chkNseForex.IsChecked.Value;
t.chkNseNcdex= chkNseNcdex.IsChecked.Value;
     


t.MCXSX_Forex_Future= MCXSX_Forex_Future.IsChecked.Value;
t.MCXSX_Equity_Futures = MCXSX_Equity_Futures.IsChecked.Value;
t.MCXCommodity_Futures= MCXCommodity_Futures.IsChecked.Value;
t.MCXSX_Equity_Options= MCXSX_Equity_Options.IsChecked.Value;
t.MCXSXForex_Options= MCXSXForex_Options.IsChecked.Value;
t.National_Spot_Exchange= National_Spot_Exchange.IsChecked.Value;
t.MCXSX_Equity_Indices= MCXSX_Equity_Indices.IsChecked.Value;
t.MCX_Index=  MCX_Index.IsChecked.Value;


t.chkYahooEOD= chkYahooEOD.IsChecked.Value;
t.ChkYahooIEOD1= ChkYahooIEOD1.IsChecked.Value;
t.chkYahooFundamental= chkYahooFundamental.IsChecked.Value;
t.ChkYahooIEOD5= ChkYahooIEOD5.IsChecked.Value;
t.Cb_Yahoo_Realtime= Cb_Yahoo_Realtime.IsChecked.Value;

t.ChkGoogleEOD= ChkGoogleEOD.IsChecked.Value;
t.ChkGoogleIEOD= ChkGoogleIEOD.IsChecked.Value;
t.Cb_MCX_Google_IEOD_5min= Cb_MCX_Google_IEOD_5min.IsChecked.Value;


t.Cb_Corporate_Events= Cb_Corporate_Events.IsChecked.Value;
t.Cb_Board_Message= Cb_Board_Message.IsChecked.Value;
t.Cb_Delete_all_events= Cb_Delete_all_events.IsChecked.Value;






t.Cb_NSE_Market_Activity = Cb_NSE_Market_Activity.IsChecked.Value;
t.Cb_NSE_PR = Cb_NSE_PR.IsChecked.Value;
t.Cb_NSE_Bulk_Deal = Cb_NSE_Bulk_Deal.IsChecked.Value;
t.Cb_NSE_Block_Deal = Cb_NSE_Block_Deal.IsChecked.Value; 
t.Cb_NSE_India_Vix = Cb_NSE_India_Vix.IsChecked.Value;
t.Cb_NSE_Vix = Cb_NSE_Vix.IsChecked.Value;
t.BSE_Delivary_Data = BSE_Delivary_Data.IsChecked.Value;
t.BSE_Index = BSE_Index.IsChecked.Value;
t.BSE_Block = BSE_Block.IsChecked.Value;
t.BSE_Bulk = BSE_Bulk.IsChecked.Value;
t.MCXSX_Currency = MCXSX_Currency.IsChecked.Value;
t.MCXSX_Block = MCXSX_Block.IsChecked.Value;
t.MCXSX_Bulk = MCXSX_Bulk.IsChecked.Value;


                
                


                


                


                
                
                
                

                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(@"C:\Fileio.txt", FileMode.Create, FileAccess.Write);
                bf.Serialize(fs, t);

                fs.Close();
                System.Windows.Forms.MessageBox.Show("Changes Save Succeessfully");
            }
        }


        private void movefile(string srs, string dest)
        {

              if(Cb_Reports.IsChecked==true )
                    {
                   

                    if (System.IO.File.Exists(srs))
                    {

                        if(!File.Exists(dest))
                        {
                        System.IO.File.Move(srs, dest);
                        }

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

        private void Lbl_reset_Click(object sender, RoutedEventArgs e)
        {
            dtEndDate.Text = "";
            dtStartDate.Text = "";
           // mcx();

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
            MCXSX_Equity_Indices.IsChecked = false;
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
            //chkCombinedReport.IsChecked = true;
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
