using System;
using System.Collections.Generic;

using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;


namespace StockD
{
    
    class StockDViewModel:INotifyPropertyChanged
    {
        #region Members
            private Settings objSettings;
            private ICommand mUpdater;
        #endregion

        #region Construction
            public StockDViewModel()
            {
                objSettings = new Settings();
                 
                
            }
        #endregion

        #region Properties


//****************************************************

            public bool Cb_BSE_CASH_MARKET
            {
                get { return objSettings.Cb_BSE_CASH_MARKET; }

                set
                {
                    objSettings.Cb_BSE_CASH_MARKET = value;

                    RaisePropertyChanged("Cb_BSE_CASH_MARKET");
                }
            }


        public bool Cb_NSE_CASH_MARKET
            {
                get { return objSettings.Cb_NSE_CASH_MARKET; }

                set
                {
                    objSettings.Cb_NSE_CASH_MARKET = value;

                    RaisePropertyChanged("Cb_NSE_CASH_MARKET");
                }
            }

        public bool Cb_NSE_SME
            {
                get { return objSettings.Cb_NSE_SME; }

                set
                {
                    objSettings.Cb_NSE_SME = value;

                    RaisePropertyChanged("Cb_NSE_SME");
                }
            }
        public bool Cb_NSE_ETF
            {
                get { return objSettings.Cb_NSE_ETF; }

                set
                {
                    objSettings.Cb_NSE_ETF = value;

                    RaisePropertyChanged("Cb_NSE_ETF");
                }
            }
        public bool Cb_NSE_Index
            {
                get { return objSettings.Cb_NSE_Index; }

                set
                {
                    objSettings.Cb_NSE_Index = value;

                    RaisePropertyChanged("Cb_NSE_Index");
                }
            }

        public bool MCXSX_Forex_Future
        {
            get { return objSettings.MCXSX_Forex_Future; }

            set
            {
                objSettings.MCXSX_Forex_Future = value;

                RaisePropertyChanged("MCXSX_Forex_Future");
            }
        }
        public bool MCXSX_Equity_Futures
        {
            get { return objSettings.MCXSX_Equity_Futures; }

            set
            {
                objSettings.MCXSX_Equity_Futures = value;

                RaisePropertyChanged("MCXSX_Equity_Futures");
            }
        }
        public bool MCXCommodity_Futures
        {
            get { return objSettings.MCXCommodity_Futures; }

            set
            {
                objSettings.MCXCommodity_Futures = value;

                RaisePropertyChanged("MCXCommodity_Futures");
            }
        }
        public bool MCXSX_Equity_Options
        {
            get { return objSettings.MCXSX_Equity_Options; }

            set
            {
                objSettings.MCXSX_Equity_Options = value;

                RaisePropertyChanged("MCXSX_Equity_Options");
            }
        }
        public bool MCXSXForex_Options
        {
            get { return objSettings.MCXSXForex_Options; }

            set
            {
                objSettings.MCXSXForex_Options = value;

                RaisePropertyChanged("MCXSXForex_Options");
            }
        }
        public bool National_Spot_Exchange
        {
            get { return objSettings.National_Spot_Exchange; }

            set
            {
                objSettings.National_Spot_Exchange = value;

                RaisePropertyChanged("National_Spot_Exchange");
            }
        }
        public bool MCXSX_Equity_Indices
        {
            get { return objSettings.MCXSX_Equity_Indices; }

            set
            {
                objSettings.MCXSX_Equity_Indices = value;

                RaisePropertyChanged("MCXSX_Equity_Indices");
            }
        }
        public bool MCX_Index
        {
            get { return objSettings.MCX_Index; }

            set
            {
                objSettings.MCX_Index = value;

                RaisePropertyChanged("MCX_Index");
            }
        }
        public bool Mutual_Funds_NAV
        {
            get { return objSettings.Mutual_Funds_NAV; }

            set
            {
                objSettings.Mutual_Funds_NAV = value;

                RaisePropertyChanged("Mutual_Funds_NAV");
            }
        }


        //********************************************************
            public string AppendText
            {
                get
                {
                    return Utility.strLog;
                }

                set
                {
                    Utility.strLog += value;
                    RaisePropertyChanged("AppendText");
                }
            }

            public string TargetFolder
            {
                get
                {
                    return objSettings.TargetFolder;
                }

                set
                {
                    objSettings.TargetFolder = value;

                    RaisePropertyChanged("TargetFolder");
                }
            }
            
            public bool ChkIgnoreSunday
            {
                get { return objSettings.ChkIgnoreSunday; }

                set
                {
                    objSettings.ChkIgnoreSunday = value;

                    RaisePropertyChanged("ChkIgnoreSunday");
                }
            }
      
            public bool ChkIgnoreSaturday
            {
                get { return objSettings.ChkIgnoreSaturday; }

                set
                {
                    objSettings.ChkIgnoreSaturday = value;

                    RaisePropertyChanged("ChkIgnoreSaturday");
                }
            }


            public bool ChkNseEquity
            {
                get { return objSettings.ChkNseEquity; }

                set
                {
                    objSettings.ChkNseEquity = value;

                    RaisePropertyChanged("ChkNseEquity");
                }
            }

            public bool ChkNseNcdex
            {
                get { return objSettings.ChkNseNcdex; }

                set
                {
                    objSettings.ChkNseNcdex = value;

                    RaisePropertyChanged("ChkNseNcdex");
                }
            }


            public bool ChkNseBulkdeal
            {
                get { return objSettings.ChkNseBulkdeal; }

                set
                {
                    objSettings.ChkNseBulkdeal = value;

                    RaisePropertyChanged("ChkNseBulkdeal");
                }
            }

            public bool ChkNseBlockdeal
            {
                get { return objSettings.ChkNseBlockdeal; }

                set
                {
                    objSettings.ChkNseBlockdeal = value;

                    RaisePropertyChanged("ChkNseBlockdeal");
                }
            }

            public bool ChkNseFIIFutures
            {
                get { return objSettings.ChkNseFIIFutures; }

                set
                {
                    objSettings.ChkNseFIIFutures = value;

                    RaisePropertyChanged("ChkNseFIIFutures");
                }
            }

            public bool ChkNseCombinedReport
            {
                get { return objSettings.ChkNseCombinedReport; }

                set
                {
                    objSettings.ChkNseCombinedReport = value;

                    RaisePropertyChanged("ChkNseCombinedReport");
                }
            }

            public bool ChkNseFO
            {
                get { return objSettings.ChkNseFO; }

                set
                {
                    objSettings.ChkNseFO = value;

                    RaisePropertyChanged("ChkNseFO");
                }
            }

            public bool chkNseForex
            {
                get { return objSettings.chkNseForex; }

                set
                {
                    objSettings.chkNseForex = value;

                    RaisePropertyChanged("ChkNseForex");
                }
            }

            public bool ChkBseEquity
            {
                get { return objSettings.ChkBseEquity; }

                set
                {
                    objSettings.ChkBseEquity = value;

                    RaisePropertyChanged("ChkBseEquity");
                }
            }

            public bool ChkBseFo
            {
                get { return objSettings.ChkBseFo; }

                set
                {
                    objSettings.ChkBseFo = value;

                    RaisePropertyChanged("ChkBseFo");
                }
            }

            public bool ChkYahooIEOD1
            {
                get { return objSettings.ChkYahooIEOD1; }

                set
                {
                    objSettings.ChkYahooIEOD1 = value;

                    RaisePropertyChanged("ChkYahooIEOD1");
                }
            }

            public bool ChkYahooIEOD5
            {
                get { return objSettings.ChkYahooIEOD5; }

                set
                {
                    objSettings.ChkYahooIEOD5 = value;

                    RaisePropertyChanged("ChkYahooIEOD5");
                }
            }

            public bool ChkYahooEOD
            {
                get { return objSettings.ChkYahooEOD; }

                set
                {
                    objSettings.ChkYahooEOD = value;

                    RaisePropertyChanged("ChkYahooEOD");
                }
            }


            public bool ChkYahooFundamental
            {
                get { return objSettings.ChkYahooFundamental; }

                set
                {
                    objSettings.ChkYahooFundamental = value;

                    RaisePropertyChanged("ChkYahooFundamental");
                }
            }

            public bool ChkGoogleEOD
            {
                get { return objSettings.ChkGoogleEOD; }

                set
                {
                    objSettings.ChkGoogleEOD = value;

                    RaisePropertyChanged("ChkGoogleEOD");
                }
            }

            public bool ChkGoogleIEOD
            {
                get { return objSettings.ChkGoogleIEOD; }

                set
                {
                    objSettings.ChkGoogleIEOD = value;

                    RaisePropertyChanged("ChkGoogleIEOD");
                }
            }

            public bool ChkMutualFund
            {
                get { return objSettings.ChkMutualFund; }

                set
                {
                    objSettings.ChkMutualFund = value;

                    RaisePropertyChanged("ChkMutualFund");
                }
            }

            public bool ChkIndiaIndices
            {
                get { return objSettings.ChkIndiaIndices; }

                set
                {
                    objSettings.ChkIndiaIndices = value;

                    RaisePropertyChanged("ChkIndiaIndices");
                }
            }

            public bool ChkFOP
            {
                get { return objSettings.ChkFOP; }

                set
                {
                    objSettings.ChkFOP = value;

                    RaisePropertyChanged("ChkFOP");
                }
            }

            public Nullable<DateTime> StartDate
            {
                get
                {
                    return objSettings.StartDate;

                }
                set
                {
                    if (objSettings.StartDate != value)
                    {
                        objSettings.StartDate = value;
                        RaisePropertyChanged("StartDate");
                    }
                }
            }

            public Nullable<DateTime> EndDate
            {
                get
                {
                    return objSettings.EndDate;
                }
                set
                {
                    if (objSettings.EndDate != value)
                    {
                        objSettings.EndDate = value;
                        RaisePropertyChanged("EndDate");
                    }
                }
            } 
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

            bool ValidateTargetFolder()
            {
                bool bRetVal;

                if (objSettings.TargetFolder == null)
                    bRetVal = false;
                else
                {
                    if (!Directory.Exists(objSettings.TargetFolder))
                        bRetVal = false;
                    else
                        bRetVal = true;
                }

                if(!bRetVal)
                    System.Windows.MessageBox.Show("Enter a Valid and Existing Directory Name!");

                return bRetVal;
            }

            bool ValidateDates()
            {
                bool bRetVal = false;

                if ((objSettings.StartDate.HasValue) || (objSettings.EndDate.HasValue))
                {

                    int i = objSettings.StartDate.Value.CompareTo(objSettings.EndDate.Value);

                    if (i > 0)
                    {
                        bRetVal = false;
                        System.Windows.MessageBox.Show("Start Date should be earlier than End Date!");
                    }
                    else
                        bRetVal = true;
                }

                return bRetVal;
            }

            bool ValidateInputs()
            {
                bool bRetVal=true;

                if (!ValidateDates())
                {
                    bRetVal = false;
                    return bRetVal;
                }

                if (!ValidateTargetFolder())
                {
                    bRetVal = false;
                    return bRetVal;
                }

                return bRetVal;

            }
            void RaisePropertyChanged(string propertyName)
            {
                // take a copy to prevent thread issues
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
       
     
            public void GetTargetDirectoryFolder()
            {
                    FolderBrowserDialog browse = new FolderBrowserDialog();

                    DialogResult result = browse.ShowDialog();

                    if (result.ToString() == "OK")
                    {
                        objSettings.TargetFolder = browse.SelectedPath;

                        RaisePropertyChanged("TargetFolder");

                    }

             }

            public void AddMessageToLog(string strMessage)
            {
                AppendText = strMessage;
            }
    
         #endregion
     
        #region Commands


            void ExecuteGetTargetDirectoryFolder(object parameter)
            {
                GetTargetDirectoryFolder();
            }

            bool CanGetTargetDirectoryFolderExecute(object parameter)
            {
                return true;
            }

            public ICommand UpdateTargetDirectory { get { return new RelayCommand<object>(ExecuteGetTargetDirectoryFolder, CanGetTargetDirectoryFolderExecute); } }
            
        
            void ExecuteIsCheckBoxClicked(object parameter)
            {

                if (parameter.Equals("Cb_BSE_CASH_MARKET"))
                {
                    if (Cb_BSE_CASH_MARKET )
                        Cb_BSE_CASH_MARKET = false;
                    else
                        Cb_BSE_CASH_MARKET = true;

                }
                  if (parameter.Equals("Cb_NSE_CASH_MARKET"))
                {
                    if (Cb_NSE_CASH_MARKET)
                        Cb_NSE_CASH_MARKET = false;
                    else
                        Cb_NSE_CASH_MARKET = true;

                }

                if (parameter.Equals("Cb_NSE_SME"))
                {
                    if (Cb_NSE_SME)
                        Cb_NSE_SME = false;
                    else
                        Cb_NSE_SME = true;

                }

                if (parameter.Equals("MCXSX_Forex_Future"))
                {
                    if (MCXSX_Forex_Future)
                        MCXSX_Forex_Future = false;
                    else
                        MCXSX_Forex_Future = true;

                }

                if (parameter.Equals("MCXSX_Equity_Futures"))
                {
                    if (MCXSX_Equity_Futures)
                        MCXSX_Equity_Futures = false;
                    else
                        MCXSX_Equity_Futures = true;

                }

                if (parameter.Equals("MCXCommodity_Futures"))
                {
                    if (MCXCommodity_Futures)
                        Cb_NSE_Index = false;
                    else
                        MCXCommodity_Futures = true;

                }

                if (parameter.Equals("MCXSX_Equity_Options"))
                 {
                     if (MCXSX_Equity_Options)
                         MCXSX_Equity_Options = false;
                     else
                         MCXSX_Equity_Options = true;

                 }
                if (parameter.Equals("MCXSXForex_Options"))
                 {
                     if (MCXSXForex_Options)
                         MCXSXForex_Options = false;
                     else
                         MCXSXForex_Options = true;

                 }
                if (parameter.Equals("National_Spot_Exchange"))
                 {
                     if (National_Spot_Exchange)
                         National_Spot_Exchange = false;
                     else
                         National_Spot_Exchange = true;

                 }
                if (parameter.Equals("MCXSX_Equity_Indices"))
                 {
                     if (MCXSX_Equity_Indices)
                         MCXSX_Equity_Indices = false;
                     else
                         MCXSX_Equity_Indices = true;

                 }
                if (parameter.Equals("MCX_Index"))
                 {
                     if (MCX_Index)
                         MCX_Index = false;
                     else
                         MCX_Index = true;

                 }
                if (parameter.Equals("Mutual_Funds_NAV"))
                 {
                     if (Mutual_Funds_NAV)
                         Mutual_Funds_NAV = false;
                     else
                         Mutual_Funds_NAV = true;

                 }
                

                //************************************************8
                if (parameter.Equals("chkEquity"))
                {
                    if (ChkNseEquity)
                        ChkNseEquity = false;
                    else
                        ChkNseEquity = true;

                }
                else if (parameter.Equals("ChkNseNcdex"))
                {
                    if (ChkNseNcdex)
                        ChkNseNcdex = false;
                    else
                        ChkNseNcdex = true;

                }
                else if (parameter.Equals("ChkNseBulkdeal"))
                {
                    if (ChkNseBulkdeal)
                        ChkNseBulkdeal = false;
                    else
                        ChkNseBulkdeal = true;

                }

                else if (parameter.Equals("ChkNseBlockdeal"))
                {
                    if (ChkNseBlockdeal)
                        ChkNseBlockdeal = false;
                    else
                        ChkNseBlockdeal = true;

                }

                else if (parameter.Equals("ChkNseFIIFutures"))
                {
                    if (ChkNseFIIFutures)
                        ChkNseFIIFutures = false;
                    else
                        ChkNseFIIFutures = true;

                }
                else if (parameter.Equals("ChkNseCombinedReport"))
                {
                    if (ChkNseCombinedReport)
                        ChkNseCombinedReport = false;
                    else
                        ChkNseCombinedReport = true;

                }

                else if (parameter.Equals("ChkNseFO"))
                {
                    if (ChkNseFO)
                        ChkNseFO = false;
                    else
                        ChkNseFO = true;

                }
                else if (parameter.Equals("ChkNseForex"))
                {
                    if (chkNseForex)
                        chkNseForex = false;
                    else
                        chkNseForex = true;

                }

                else if (parameter.Equals("chkBseEquity"))
                {
                    if (ChkBseEquity)
                        ChkBseEquity = false;
                    else
                        ChkBseEquity = true;

                }

                else if (parameter.Equals("chkBseFo"))
                {
                    if (ChkBseFo)
                        ChkBseFo = false;
                    else
                        ChkBseFo = true;
                }

                else if (parameter.Equals("ChkYahooIEOD1"))
                {
                    if (ChkYahooIEOD1)
                        ChkYahooIEOD1 = false;
                    else
                        ChkYahooIEOD1 = true;
                }

                else if (parameter.Equals("ChkYahooIEOD5"))
                {
                    if (ChkYahooIEOD5)
                        ChkYahooIEOD5 = false;
                    else
                        ChkYahooIEOD5 = true;
                }

                else if (parameter.Equals("ChkYahooEOD"))
                {
                    if (ChkYahooEOD)
                        ChkYahooEOD = false;
                    else
                        ChkYahooEOD = true;
                }

                else if (parameter.Equals("ChkYahooFundamental"))
                {
                    if (ChkYahooFundamental)
                        ChkYahooFundamental = false;
                    else
                        ChkYahooFundamental = true;
                }

                else if (parameter.Equals("ChkGoogleEOD"))
                {
                    if (ChkGoogleEOD)
                        ChkGoogleEOD = false;
                    else
                        ChkGoogleEOD = true;
                }

                else if (parameter.Equals("ChkGoogleIEOD"))
                {
                    if (ChkGoogleIEOD)
                        ChkGoogleIEOD = false;
                    else
                        ChkGoogleIEOD = true;
                }

                else if (parameter.Equals("ChkMutualFund"))
                {
                    if (ChkMutualFund)
                        ChkMutualFund = false;
                    else
                        ChkMutualFund = true;
                }

                else if (parameter.Equals("ChkIndiaIndices"))
                {
                    if (ChkIndiaIndices)
                        ChkIndiaIndices = false;
                    else
                        ChkIndiaIndices = true;
                }

                else if (parameter.Equals("ChkFOP"))
                {
                    if (ChkFOP)
                        ChkFOP = false;
                    else
                        ChkFOP = true;
                }

                else if (parameter.Equals("chkIgnoreSaturday"))
                {
                    if (ChkIgnoreSaturday)
                        ChkIgnoreSaturday = false;
                    else
                        ChkIgnoreSaturday = true;
                }
                else if (parameter.Equals("chkIgnoreSunday"))
                {
                    if (ChkIgnoreSunday)
                        ChkIgnoreSunday = false;
                    else
                        ChkIgnoreSunday = true;
                }

            }

            bool CanIsCheckBoxClicked(object parameter)
            {
                return true;
            }

            public ICommand IsCheckBoxClicked { get { return new RelayCommand<object>(ExecuteIsCheckBoxClicked, CanIsCheckBoxClicked); } }


            void ExecuteEODControlClicked(object parameter)
            {
                Action<string> mydelegate = AddMessageToLog;

                if (parameter.Equals("btnStart"))
                {
                    bool bRetval = false;

                   // if (objSettings.ChkNseEquity)
                   // {

                        if(ValidateInputs())
                            bRetval = true;

                        if (bRetval)
                        {
                            objSettings.Load();
                            EODData.StartButtonClicked(objSettings, AddMessageToLog);
                            System.Windows.MessageBox.Show("Finished!");
                        }
                   // }
                }
                else if (parameter.Equals("btnExit"))
                {
                    //App.Current.Shutdown();
                }
            }

            bool CanEODControlClicked(object parameter)
            {
                return true;
            }

            public ICommand EODControlClicked { get { return new RelayCommand<object>(ExecuteEODControlClicked, CanEODControlClicked); } }
        #endregion

   
    }

 
}
