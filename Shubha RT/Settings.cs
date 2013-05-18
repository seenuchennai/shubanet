using System;
using System.Collections.Generic;

using System.Text;
using System.ComponentModel;
using System.IO;
using System.Collections;
using System.Configuration;

namespace StockD
{
    public class Settings
    {
        #region Variables
        private string strTargetFolder;
        
        //Booleans to Check if Checkbox is Checked
        //***************************************
        private bool Cb_BSE_CASH_MARKET1;
       private bool Cb_NSE_CASH_MARKET1;
private bool Cb_NSE_SME1;
private bool Cb_NSE_ETF1;
private bool Cb_NSE_Index1;
        
private bool MCXSX_Forex_Future1;
private bool MCXSX_Equity_Futures1;
private bool MCXCommodity_Futures1;
private bool MCXSX_Equity_Options1;
private bool MCXSXForex_Options1;
private bool National_Spot_Exchange1;
private bool MCXSX_Equity_Indices1;
private bool MCX_Index1;
private bool Mutual_Funds_NAV1;
        //**************************
        private bool bChkNseEquity;
        private bool bChkNseFO;
        private bool bChkNseNcdex;
        private bool bChkNseBulkdeal;
        private bool bChkNseBlockdeal;
        private bool bChkNseFIIFutures;
        private bool bChkCombinedReport;
        private bool bchkNseForex;
        private bool bChkBseEquity;
        private bool bChkBseFo;
        private bool bChkYahooIEOD1;
        private bool bChkYahooIEOD5;
        private bool bChkYahooEOD;
        private bool bChkYahooFundamental;
        private bool bChkGoogleEOD;
        private bool bChkGoogleIEOD;
        private bool bChkMutualFund;
        private bool bChkFOP;
        private bool bChkIndiaIndices;
 
        //List for Symbols of Each File
        private List<string> lstYahooIEODSymbols1Min;
        private List<string> lstYahooIEODSymbols5Min;
        private List<string> lstYahooEODSymbols;
        private List<string> lstYahooFundamentalSymbols;
        private List<string> lstGoogleEODSymbols;
        private List<string> lstGoogleIEODSymbols;
        private List<string> lstIndiaIndicesSymbols;

        //Symbol File Names
        string strYahooIEOD1MinFile;
        string strYahooIEOD5MinFile;
        string strYahooEODFile;
        string strYahooFundamentalFile;
        string strGoogleEODFile;
        string strGoogleIEODFile;
        string strIndiaIndicesFile;

        private bool bChkIgnoreSaturday;
        private bool bChkIgnoreSunday;
        private Nullable<DateTime> dtStartDate;
        private Nullable<DateTime> dtEndDate;
        #endregion

        #region Properties

        //Properties for each CheckBox


        //***********************************************
        public bool Cb_BSE_CASH_MARKET
        {
            get { return Cb_BSE_CASH_MARKET1; }

            set
            {
                Cb_BSE_CASH_MARKET1 = value;
            }
        }

        public bool Cb_NSE_CASH_MARKET
        {
            get { return Cb_NSE_CASH_MARKET1; }

            set
            {
                Cb_NSE_CASH_MARKET1 = value;
            }
        }

        public bool Cb_NSE_SME
        {
            get { return Cb_NSE_SME1; }

            set
            {
                Cb_NSE_SME1 = value;
            }
        }

         public bool Cb_NSE_ETF
        {
            get { return Cb_NSE_ETF1; }

            set
            {
                Cb_NSE_ETF1 = value;
            }
        }

         public bool Cb_NSE_Index
        {
            get { return Cb_NSE_Index1; }

            set
            {
                Cb_NSE_Index1 = value;
            }
        }

         public bool MCXSX_Forex_Future
         {
             get { return MCXSX_Forex_Future1; }

             set
             {
                 MCXSX_Forex_Future1 = value;
             }
         }
         public bool MCXSX_Equity_Futures
         {
             get { return MCXSX_Equity_Futures1; }

             set
             {
                 MCXSX_Equity_Futures1 = value;
             }
         }
         public bool MCXCommodity_Futures
         {
             get { return MCXCommodity_Futures1; }

             set
             {
                 MCXCommodity_Futures1 = value;
             }
         }
         public bool MCXSX_Equity_Options
         {
             get { return MCXSX_Equity_Options1; }

             set
             {
                 MCXSX_Equity_Options1 = value;
             }
         }
         public bool MCXSXForex_Options
         {
             get { return MCXSXForex_Options1; }

             set
             {
                 MCXSXForex_Options1 = value;
             }
         }
         public bool National_Spot_Exchange
         {
             get { return National_Spot_Exchange1; }

             set
             {
                 National_Spot_Exchange1 = value;
             }
         }
         public bool MCXSX_Equity_Indices
         {
             get { return MCXSX_Equity_Indices1; }

             set
             {
                 MCXSX_Equity_Indices1 = value;
             }
         }
         public bool MCX_Index
         {
             get { return MCX_Index1; }

             set
             {
                 MCX_Index1 = value;
             }
         }
         public bool Mutual_Funds_NAV
         {
             get { return Mutual_Funds_NAV1; }

             set
             {
                 Mutual_Funds_NAV1 = value;
             }
         }
        //**********************************************

        public string TargetFolder
        {
            get
            {
                return strTargetFolder;
            }

            set
            {
                strTargetFolder = value;
            }
        }

        public bool ChkIgnoreSunday
        {
            get { return bChkIgnoreSunday; }

            set
            {
                bChkIgnoreSunday = value;
            }
        }

        public bool ChkIgnoreSaturday
        {
            get { return bChkIgnoreSaturday; }

            set
            {
                bChkIgnoreSaturday = value;
            }
        }

        public bool ChkNseNcdex
        {
            get { return bChkNseNcdex; }

            set
            {
                bChkNseNcdex = value;
            }
        }

        public bool ChkNseBulkdeal
        {
            get { return bChkNseBulkdeal; }

            set
            {
                bChkNseBulkdeal = value;
            }
        }

        public bool ChkNseBlockdeal
        {
            get { return bChkNseBlockdeal; }

            set
            {
                bChkNseBlockdeal = value;
            }
        }

        public bool ChkNseFIIFutures
        {
            get { return bChkNseFIIFutures; }

            set
            {
                bChkNseFIIFutures = value;
            }
        }

        public bool ChkNseCombinedReport
        {
            get { return bChkCombinedReport; }

            set
            {
                bChkCombinedReport = value;
            }
        }

        public bool ChkNseFO
        {
            get { return bChkNseFO; }

            set
            {
                bChkNseFO = value;
            }
        }

        public bool chkNseForex
        {
            get { return bchkNseForex; }

            set
            {
                bchkNseForex = value;
            }
        }

        public bool ChkNseEquity
        {
            get { return bChkNseEquity; }

            set
            {
                bChkNseEquity = value;
            }
        }

        public bool ChkBseEquity
        {
            get { return bChkBseEquity; }

            set
            {
                bChkBseEquity = value;
            }
        }

        public bool ChkBseFo
        {
            get { return bChkBseFo; }

            set
            {
                bChkBseFo = value;
            }
        }

        public bool ChkYahooIEOD1
        {
            get { return bChkYahooIEOD1; }

            set
            {
                bChkYahooIEOD1 = value;
            }
        }

        public bool ChkYahooIEOD5
        {
            get { return bChkYahooIEOD5; }

            set
            {
                bChkYahooIEOD5 = value;
            }
        }

        public bool ChkYahooEOD
        {
            get { return bChkYahooEOD; }

            set
            {
                bChkYahooEOD = value;
            }
        }

        public bool ChkYahooFundamental
        {
            get { return bChkYahooFundamental; }

            set
            {
                bChkYahooFundamental = value;
            }
        }

        public bool ChkGoogleEOD
        {
            get { return bChkGoogleEOD; }

            set
            {
                bChkGoogleEOD = value;
            }
        }

        public bool ChkGoogleIEOD
        {
            get { return bChkGoogleIEOD; }

            set
            {
                bChkGoogleIEOD = value;
            }
        }

        public bool ChkMutualFund
        {
            get { return bChkMutualFund; }

            set
            {
                bChkMutualFund = value;
            }
        }

        public bool ChkIndiaIndices
        {
            get { return bChkIndiaIndices; }

            set
            {
                bChkIndiaIndices = value;
            }
        }

        public bool ChkFOP
        {
            get { return bChkFOP; }

            set
            {
                bChkFOP = value;
            }
        }

        public List<string> YahooIEOD1MinList
        {
            get
            {
                return lstYahooIEODSymbols1Min;
            }
        }

        public List<string> YahooIEOD5MinList
        {
            get
            {
                return lstYahooIEODSymbols5Min;
            }
        }

        public List<string> YahooEODList
        {
            get
            {
                return lstYahooEODSymbols;
            }
        }

        public List<string> YahooFundamentalList
        {
            get
            {
                return lstYahooFundamentalSymbols;
            }
        }

        public List<string> GoogleEODList
        {
            get
            {
                return lstGoogleEODSymbols;
            }
        }

        public List<string> GoogleIEODList
        {
            get
            {
                return lstGoogleIEODSymbols;
            }
        }

        public List<string> IndiaIndicesList
        {
            get
            {
                return lstIndiaIndicesSymbols;
            }
        }

        public Nullable<DateTime> StartDate
        {
            get { return dtStartDate; }

            set
            {
                dtStartDate = value;
            }
        }

        public Nullable<DateTime> EndDate
        {
            get { return dtEndDate; }

            set
            {
                dtEndDate = value;
            }
        }
   
         #endregion

        #region Methods
        public void Save()
        {
         /*   using (Stream stream = File.Create(SettingsFile))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.WriteLine(MyNumber);
                writer.WriteLine(MyString);

                writer.Close();
                stream.Close();
            }*/
        }

        public void ReadDataFile(string strFile, List <string> Symbols)
        {
            using (Stream stream = File.OpenRead(strFile))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {

                //Read File
                string Line;
                while (!reader.EndOfStream)
                {
                    Line = reader.ReadLine();
                    Symbols.Add(Line);
                }

                reader.Close();
                stream.Close();
            }
        }

        public bool Load()
        {
            //Yahoo IEOD 1 Min
            if (ChkYahooIEOD1)
            { 
              if(File.Exists(YahooIEOD1MinFile))
                ReadDataFile(YahooIEOD1MinFile, lstYahooIEODSymbols1Min);
              else
              {
                //Create File YahooIEOD1MinFile
                System.Windows.MessageBox.Show("Yahoo IEOD 1 Min File Does Not Exist!");
                ChkYahooIEOD1 = false;
                return false;
               }
            }

            //Yahoo IEOD 5 Min
            if (ChkYahooIEOD5)
            {
                if (File.Exists(YahooIEOD5MinFile))
                    ReadDataFile(YahooIEOD5MinFile, lstYahooIEODSymbols5Min);
                else
                {
                    //Create File YahooIEOD1MinFile
                    System.Windows.MessageBox.Show("Yahoo IEOD 5 Min File Does Not Exist!");
                    ChkYahooIEOD5 = false;
                    return false;
                }
            }

            
            //Yahoo EOD
            if (ChkYahooEOD)
            {
                if (File.Exists(YahooEODFile))
                    ReadDataFile(YahooEODFile, lstYahooEODSymbols);
                else
                {
                    System.Windows.MessageBox.Show("Yahoo EOD File Does Not Exist!");
                    ChkYahooEOD = false;
                    return false;
                }
            }

            //Yahoo Fundamental
            if (ChkYahooFundamental)
            {
                if((File.Exists(YahooFundamentalFile)))
                    ReadDataFile(YahooFundamentalFile, lstYahooFundamentalSymbols);
                else
                {
                    System.Windows.MessageBox.Show("Yahoo Fundamental File Does Not Exist!");
                    ChkYahooFundamental = false;
                    return false;
                }
            }

            //Google EOD
            if (ChkGoogleEOD)
            {
                if (File.Exists(GoogleEODFile))
                    ReadDataFile(GoogleEODFile, lstGoogleEODSymbols);
                else
                {
                    System.Windows.MessageBox.Show("Google EOD File Does Not Exist!");
                    ChkGoogleEOD = false;
                    return false;
                }
            }

            //Google IEOD
            if (ChkGoogleIEOD)
            {
                if (File.Exists(GoogleIEODFile))
                    ReadDataFile(GoogleIEODFile, lstGoogleIEODSymbols);
                else
                {
                    System.Windows.MessageBox.Show("Google IEOD File Does Not Exist!");
                    ChkGoogleIEOD = false;
                    return false;
                }
            }

            //India Indices
            if (ChkIndiaIndices)
            {
                if (File.Exists(IndiaIndicesFile))
                    ReadDataFile(IndiaIndicesFile, lstIndiaIndicesSymbols);
                else
                {
                    System.Windows.MessageBox.Show("India Indices File Does Not Exist!");
                    ChkIndiaIndices = false;
                    return false;
                }
            }
            return true;
        }

        string YahooIEOD1MinFile
        {
            get
            {
                return Path.Combine(AppFolder, strYahooIEOD1MinFile);
            }
        }

        string YahooIEOD5MinFile
        {
            get
            {
                return Path.Combine(AppFolder, strYahooIEOD5MinFile);
            }
        }

        string YahooEODFile
        {
            get
            {
                return Path.Combine(AppFolder, strYahooEODFile);
            }
        }

        string YahooFundamentalFile
        {
            get
            {
                return Path.Combine(AppFolder, strYahooFundamentalFile);
            }
        }

        string GoogleEODFile
        {
            get
            {
                return Path.Combine(AppFolder, strGoogleEODFile);
            }
        }

        string GoogleIEODFile
        {
            get
            {
                return Path.Combine(AppFolder, strGoogleIEODFile);
            }
        }

        string IndiaIndicesFile
        {
            get
            {
                return Path.Combine(AppFolder, strIndiaIndicesFile);
            }
        }

        static string AppFolder
        {
            get
            {
                string folder;

                //folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                //folder = Path.Combine(folder, "Company");
                //folder = Path.Combine(folder, "StockD");

                //if (!Directory.Exists(folder))
                //    Directory.CreateDirectory(folder);

                folder = Environment.CurrentDirectory;
              
                return folder;

            }
        }


        public Settings()
        {
          
 
            strYahooIEOD1MinFile = ConfigurationManager.AppSettings["YahooIEOD1Min"]; 

            strYahooIEOD5MinFile = ConfigurationManager.AppSettings["YahooIEOD5Min"];

            strYahooEODFile = ConfigurationManager.AppSettings["YahooEOD"];

            strYahooFundamentalFile = ConfigurationManager.AppSettings["YahooFundamental"];

            strGoogleEODFile = ConfigurationManager.AppSettings["GoogleEOD"];

            strGoogleIEODFile = ConfigurationManager.AppSettings["GoogleIEOD"];

            strGoogleIEODFile = ConfigurationManager.AppSettings["IndiaIndices"];

            lstYahooIEODSymbols1Min = new List<string> ();
            lstYahooIEODSymbols5Min = new List<string> ();
            lstYahooEODSymbols  = new List<string> ();
            lstYahooFundamentalSymbols = new List<string>();
            lstGoogleEODSymbols = new List<string>();
            lstGoogleIEODSymbols = new List<string>();
            lstIndiaIndicesSymbols = new List<string>();
            
        }

        #endregion
    }
}
