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
namespace StockD
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string url1 = "http://www.goog";
       
        public MainWindow()
        {
            
        }
        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))

                yield return day;
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

           string strYearDir = txtTargetFolder.Text + "\\Downloads";
            string baseurl;
             DateTime  StartDate, EndDate;

             StartDate = Convert.ToDateTime(dtStartDate.Text);
             EndDate = Convert.ToDateTime(dtEndDate.Text);

            if (!Directory.Exists(strYearDir))
                Directory.CreateDirectory(strYearDir);

            if (Cb_NSE_Sec_List.IsChecked == true)
            {
                strYearDir = txtTargetFolder.Text + "\\Downloads\\sec_list.csv";
                baseurl="http://www.nseindia.com/content/equities/sec_list.csv";
                downliaddata(strYearDir,baseurl);
            }
            if (Cb_NSE_EOD_BhavCopy.IsChecked == true)
            {

                foreach (DateTime day in EachDay(StartDate, EndDate))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    string strMonthName = mfi.GetMonthName(day.Month).ToString();
                    strYearDir = txtTargetFolder.Text + "\\Downloads\\cm"+day.Day  + strMonthName +day.Year +"bhav.csv.zip";
                    baseurl = "http://www.nseindia.com/content/historical/EQUITIES/"+day.Year .ToString()+"/" + strMonthName.ToUpper() + "/cm" + day.Day + strMonthName.ToUpper() + day.Year + "bhav.csv.zip";




                    downliaddata(strYearDir, baseurl);
                }
               
            }




        }
    
        private void downliaddata(string path,string url)
        {
             WebClient Client = new WebClient();

                    try
                    {

                        //If Data is Not Present For Date Then  Exception Occure And It Get Added Into List Box  
                       // Client.DownloadFile("http://www.mcx-sx.com/downloads/daily/EquityDownloads/Market%20Statistics%20Report_" + date1 + ".csv.", File_path);

                        Client.Headers.Add("Accept", "application/zip");
                        Client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                        Client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1");
                        Client.DownloadFile(url, path );
                        
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


            if (File.Exists(@"C:\Fileio.txt"))
            {
                FileStream fs = new FileStream(@"C:\Fileio.txt", FileMode.Open, FileAccess.Read);
                target1 t1 = (target1)bf.Deserialize(fs);
                txtTargetFolder.Text = t1.target;
                dtStartDate.Text = t1.fromdate.ToShortDateString();
                dtEndDate.Text = t1.todate.ToShortDateString();
                Cb_BSE_CASH_MARKET.IsChecked =  t1.Cb_BSE_CASH_MARKET;


             
Cb_BSE_Equity_Futures.IsChecked=t1.Cb_BSE_Equity_Futures;
ChkBSEEquity.IsChecked=t1.ChkBSEEquity;
ChkBseFo.IsChecked=t1.ChkBseFo;


Cb_NSE_CASH_MARKET.IsChecked=t1.Cb_NSE_CASH_MARKET;
Cb_NSE_EOD_BhavCopy.IsChecked=t1.Cb_NSE_EOD_BhavCopy;
chkEquity.IsChecked=t1.chkEquity;
Cb_NSE_Forex_Options.IsChecked=t1.Cb_NSE_Forex_Options;
Cb_NSE_SME.IsChecked=t1.Cb_NSE_SME;
 Cb_NSE_ETF.IsChecked=t1.Cb_NSE_ETF;
Cb_NSE_Index.IsChecked=t1.Cb_NSE_Index;
 Cb_Reports.IsChecked=t1.Cb_Reports;
 chkCombinedReport.IsChecked=t1.chkCombinedReport;
 chkNseForex.IsChecked=t1.chkNseForex;
 chkNseNcdex.IsChecked=t1.chkNseNcdex;


 Cb_NSE_Sec_List.IsChecked = t1.Cb_NSE_Sec_List;
 MCXSX_Forex_Future.IsChecked=t1.MCXSX_Forex_Future;
 MCXSX_Equity_Futures.IsChecked=t1.MCXSX_Equity_Futures;
 MCXCommodity_Futures.IsChecked=t1.MCXCommodity_Futures;
 MCXSX_Equity_Options.IsChecked=t1.MCXSX_Equity_Options;
 MCXSXForex_Options.IsChecked=t1.MCXSXForex_Options;
 National_Spot_Exchange.IsChecked=t1.National_Spot_Exchange;
 MCXSX_Equity_Indices.IsChecked=t1.MCXSX_Equity_Indices;
  MCX_Index.IsChecked=t1.MCX_Index;


 chkYahooEOD.IsChecked=t1.chkYahooEOD;
 ChkYahooIEOD1.IsChecked=t1.ChkYahooIEOD1;
 chkYahooFundamental.IsChecked=t1.chkYahooFundamental;
 ChkYahooIEOD5.IsChecked=t1.ChkYahooIEOD5;
 Cb_Yahoo_Realtime.IsChecked=t1.Cb_Yahoo_Realtime;

 ChkGoogleEOD.IsChecked=t1.ChkGoogleEOD;
 ChkGoogleIEOD.IsChecked=t1.ChkGoogleIEOD;
 Cb_MCX_Google_IEOD_5min.IsChecked=t1.Cb_MCX_Google_IEOD_5min;


 Cb_Corporate_Events.IsChecked=t1.Cb_Corporate_Events;
 Cb_Board_Message.IsChecked=t1.Cb_Board_Message;
 Cb_Delete_all_events.IsChecked=t1.Cb_Delete_all_events;


               
                fs.Close();



            }
            else
            {
                dtStartDate.Text = DateTime.Today.Date.ToString();
                dtEndDate.Text = DateTime.Today.Date.ToString();
                textBox1.Text = "";
            }
           
            Check_internet_connetion(url1);
        }

        private void wMain_Closed(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(typeof(MainWindow));
            log.Debug("Application Close ");
            savechanges();
           
        }
        private void savechanges()
        {
            if (dtStartDate.Text.ToString() == "")
            {

            }
            else
            {
                target1 t = new target1();
                t.fromdate = Convert.ToDateTime(dtStartDate.Text);
                t.todate = Convert.ToDateTime(dtEndDate.Text);
                t.target = txtTargetFolder.Text;
                
                t.Cb_BSE_CASH_MARKET=Cb_BSE_CASH_MARKET.IsChecked.Value ;
t.Cb_BSE_Equity_Futures=Cb_BSE_Equity_Futures.IsChecked.Value;
t.ChkBSEEquity=ChkBSEEquity.IsChecked.Value;
t.ChkBseFo=ChkBseFo.IsChecked.Value;


t.Cb_NSE_CASH_MARKET=Cb_NSE_CASH_MARKET.IsChecked.Value;
t.Cb_NSE_EOD_BhavCopy=Cb_NSE_EOD_BhavCopy.IsChecked.Value;
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

t.Cb_NSE_Sec_List = Cb_NSE_Sec_List.IsChecked.Value;

                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(@"C:\Fileio.txt", FileMode.Create, FileAccess.Write);
                bf.Serialize(fs, t);

                fs.Close();
            }
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
           
           
        }

        private void tabItem2_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            savechanges();
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

       

     }
    [Serializable]
    public class target1
    {
        public string target;
        public DateTime fromdate;
        public DateTime todate;
        public bool  checkboxevent;


        public bool Cb_NSE_Sec_List;

       public bool Cb_BSE_CASH_MARKET;
public bool Cb_BSE_Equity_Futures;
public bool ChkBSEEquity;
public bool ChkBseFo;


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
