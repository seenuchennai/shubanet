using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Globalization;

namespace StockD
{
    public static class EODData
    {
        private static bool bSelectAll;
        private static bool bIgnoreWeekend;


        #region Properties

        #endregion

        #region Methods

            public static int GetEndOfYear(Nullable<DateTime> dtTempDate)
            {

                Nullable<DateTime> lastMonthOfThisYear = new DateTime(dtTempDate.Value.Year, dtTempDate.Value.Month, 1).AddYears(1).AddMonths(-1);

                return lastMonthOfThisYear.Value.Day;
            }

            public static int GetEndOfMonth(Nullable<DateTime> dtTempDate)
            {
                
                Nullable<DateTime> lastDayOfThisMonth = new DateTime(dtTempDate.Value.Year, dtTempDate.Value.Month, 1).AddMonths(1).AddDays(-1);

                return lastDayOfThisMonth.Value.Day;
            }

            public static string BuildBhavFileName(int iDay, string strMonthName, int iYear)
            {
                string strBhavFileName = "cm";

                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName = strBhavFileName + iDay.ToString() + strMonthName + iYear.ToString() + "bhav.csv.zip";

                return strBhavFileName;
            }

            public static string AddSubFolders(string strdownloadfolder, string TopFolder)
            {
                strdownloadfolder = Path.Combine(strdownloadfolder, "EODData");
                //strdownloadfolder = Path.Combine(strdownloadfolder, "NSE");
                strdownloadfolder = Path.Combine(strdownloadfolder, TopFolder);

                return strdownloadfolder;
            }

            public static void StartButtonClicked(Settings objSettings, Action<string> AddMessageToLog)
            {
                //
                //http://nseindia.com/content/historical/EQUITIES/2012/SEP/cm04SEP2012bhav.csv.zip
                //

                IScrip[] Scrips = {
                                       
                                      //*****************************************

                                       new NSE { strScripName = "Cb_BSE_CASH_MARKET", strScripDirectory = "Cb_BSE_CASH_MARKET", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "Cb_NSE_CASH_MARKET", strScripDirectory = "Cb_NSE_CASH_MARKET", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "Cb_NSE_SME", strScripDirectory = "Cb_NSE_SME", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "Cb_NSE_ETF", strScripDirectory = "Cb_NSE_ETF", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "Cb_NSE_Index", strScripDirectory = "Cb_NSE_Index", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                      
                                       //MCX
                                       new NSE { strScripName = "MCXSX_Forex_Future", strScripDirectory = "MCXSX_Forex_Future", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "MCXSX_Equity_Futures", strScripDirectory = "MCXSX_Equity_Futures", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "MCXCommodity_Futures", strScripDirectory = "MCXCommodity_Futures", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "MCXSX_Equity_Options", strScripDirectory = "MCXSX_Equity_Options", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "MCXSXForex_Options", strScripDirectory = "MCXSXForex_Options", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                      new NSE { strScripName = "National_Spot_Exchange", strScripDirectory = "National_Spot_Exchange", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "MCXSX_Equity_Indices", strScripDirectory = "MCXSX_Equity_Indices", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "MCX_Index", strScripDirectory = "MCX_Index", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "Mutual_Funds_NAV", strScripDirectory = "Mutual_Funds_NAV", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                      
                                       
                                       


                                       











                                      //****************************************
                                      
                                      new NSE { strScripName = "NSE Equity", strScripDirectory = "NSE Equity", strScripBaseURL="http://nseindia.com/content/historical/EQUITIES", bFireScrip = false } ,
                                       new NSE { strScripName = "MTO", strScripDirectory = "NSE Equity", strScripBaseURL="http://nseindia.com/archives/equities/mto/", bFireScrip = false },
                                       new NSE { strScripName = "VIX", strScripDirectory = "NSE Equity", strScripBaseURL="http://www.nseindia.com/content/vix/histdata/", bFireScrip = false },
                                       new NSE { strScripName = "NSE FO", strScripDirectory = "NSE FO", strScripBaseURL = "http://nseindia.com/content/historical/DERIVATIVES" , bFireScrip = false } ,
                                       new NSE { strScripName = "NSE Forex", strScripDirectory = "NSE Forex", strScripBaseURL = "http://nseindia.com/archives/equities/bhavcopy/pr/", bFireScrip = false},
                                       new NSE { strScripName = "NCDEX", strScripDirectory = "NCDEX", strScripBaseURL = "http://www.ncdex.com/Downloads/Bhavcopy_Summary_File/Export_csv/", bFireScrip = false },
                                       new NSE { strScripName = "BULKDEAL", strScripDirectory = "BULKDEAL", strScripBaseURL = "http://www.nseindia.com/content/equities/bulkdeals/datafiles/", bFireScrip = false },
                                       new NSE { strScripName = "BLOCKDEAL", strScripDirectory = "BLOCKDEAL", strScripBaseURL = "http://www.nseindia.com/content/equities/bulkdeals/datafiles/", bFireScrip = false },
                                       new NSE { strScripName = "FIIFUTURES", strScripDirectory = "FIIFUTURES", strScripBaseURL = "http://www.nseindia.com/content/fo/", bFireScrip = false },
                                       new NSE { strScripName = "COMBINEDREPORT", strScripDirectory = "COMBINEDREPORT", strScripBaseURL = "http://www.nseindia.com/archives/combine_report/", bFireScrip = false },

                                       new NSE { strScripName = "BSE_EQUITY", strScripDirectory = "BSE_EQUITY", strScripBaseURL = "http://www.bseindia.com/bhavcopy/", bFireScrip = false },
                                       new NSE { strScripName = "BSE_MTO", strScripDirectory = "BSE_MTO", strScripBaseURL = "http://www.bseindia.com/BSEDATA/gross/", bFireScrip = false },
                                       new NSE { strScripName = "BSE_FO", strScripDirectory = "BSE_FO", strScripBaseURL = "http://www.bseindia.com/deri/downloads/BhavCopy/", bFireScrip = false },
                                       
                                       new NSE { strScripName = "YAHOOIEOD1MIN", strScripDirectory = "YAHOOIEOD1MIN", strScripBaseURL = "http://chartapi.finance.yahoo.com/instrument/1.0/", bFireScrip = false },
                                       new NSE { strScripName = "YAHOOIEOD5MIN", strScripDirectory = "YAHOOIEOD5MIN", strScripBaseURL = "http://chartapi.finance.yahoo.com/instrument/5.0/", bFireScrip = false },
                                       new NSE { strScripName = "YAHOOEOD", strScripDirectory = "YAHOOEOD", strScripBaseURL = "http://ichart.finance.yahoo.com/table.csv?s=", bFireScrip = false },
                                       new NSE { strScripName = "YAHOOFUNDAMENTAL", strScripDirectory = "YAHOOFUNDAMENTAL", strScripBaseURL = "http://download.finance.yahoo.com/d/quotes.csv?s=", bFireScrip = false },
                                       
                                       new NSE { strScripName = "GOOGLEEOD", strScripDirectory = "GOOGLEEOD", strScripBaseURL = "http://www.google.com/finance/getprices?q=", bFireScrip = false },
                                       new NSE { strScripName = "GOOGLEIEOD", strScripDirectory = "GOOGLEIEOD", strScripBaseURL = "http://www.google.com/finance/getprices?q=", bFireScrip = false },
                                       
                                       new NSE { strScripName = "MFAMFI", strScripDirectory = "MFAMFI", strScripBaseURL = "http://www.amfiindia.com/DownloadNAVHistoryReport_Po.aspx?frmdt=", bFireScrip = false },
                                       new NSE { strScripName = "MFNSE", strScripDirectory = "MFNSE", strScripBaseURL = "http://www.nseindia.com/archives/equities/mkt/", bFireScrip = false },

                                       new NSE { strScripName = "FOP", strScripDirectory = "FOP", strScripBaseURL = "http://www.nseindia.com/content/nsccl/", bFireScrip = false },

                                       new NSE { strScripName = "FOP", strScripDirectory = "FOP", strScripBaseURL = "http://www.nseindia.com/content/indices/histdata/", bFireScrip = false }
                                   };

                string strTOPFolder, strUrl, strTempUrl;
                int iEndDayOfMonth, iEndMonthofYear;
                bool bDoNOTDowload = false;
                string strBuildBhavFileName=string.Empty;


                foreach (IScrip objscrip in Scrips)
                {
                    if ((objSettings.ChkNseEquity) && (
                       (objscrip.strScripName == "NSE Equity") ||
                       (objscrip.strScripName == "MTO") ||
                       (objscrip.strScripName == "VIX")) )
                           objscrip.bFireScrip=true;


                    //************************************


                    if ((objSettings.Cb_BSE_CASH_MARKET) && objscrip.strScripName == "Cb_BSE_CASH_MARKET")
                        objscrip.bFireScrip = true;
                    








                    //MCX
                    if ((objSettings.MCXSX_Forex_Future) && objscrip.strScripName == "MCXSX_Forex_Future")
                        objscrip.bFireScrip = true;
                    if ((objSettings.MCXSX_Equity_Futures) && objscrip.strScripName == "MCXSX_Equity_Futures")
                        objscrip.bFireScrip = true;
                    if ((objSettings.MCXCommodity_Futures) && objscrip.strScripName == "MCXCommodity_Futures")
                        objscrip.bFireScrip = true;
                    if ((objSettings.MCXSX_Equity_Options) && objscrip.strScripName == "MCXSX_Equity_Options")
                        objscrip.bFireScrip = true;
                    if ((objSettings.National_Spot_Exchange) && objscrip.strScripName == "National_Spot_Exchange")
                        objscrip.bFireScrip = true;
                    if ((objSettings.MCXSXForex_Options) && objscrip.strScripName == "MCXSXForex_Options")
                        objscrip.bFireScrip = true;
                   
                    if ((objSettings.MCXSX_Equity_Indices) && objscrip.strScripName == "MCXSX_Equity_Indices")
                        objscrip.bFireScrip = true;
                    if ((objSettings.MCX_Index) && objscrip.strScripName == "MCX_Index")
                        objscrip.bFireScrip = true;
                    if ((objSettings.Mutual_Funds_NAV) && objscrip.strScripName == "Mutual_Funds_NAV")
                        objscrip.bFireScrip = true;


                    if ((objSettings.Cb_NSE_SME) && objscrip.strScripName == "Cb_NSE_SME")
                        objscrip.bFireScrip = true;


                    if ((objSettings.Cb_NSE_ETF) && objscrip.strScripName == "Cb_NSE_ETF")
                        objscrip.bFireScrip = true;


                    if ((objSettings.Cb_NSE_Index) && objscrip.strScripName == "Cb_NSE_Index")
                        objscrip.bFireScrip = true;






                    //***********************************


                    if ((objSettings.ChkNseFO) && objscrip.strScripName == "NSE FO")
                        objscrip.bFireScrip = true;

                    if ((objSettings.chkNseForex) && objscrip.strScripName == "NSE Forex")
                        objscrip.bFireScrip = true;


                    if ((objSettings.ChkNseNcdex) && objscrip.strScripName == "NCDEX")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkNseBulkdeal) && objscrip.strScripName == "BULKDEAL")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkNseBlockdeal) && objscrip.strScripName == "BLOCKDEAL")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkNseFIIFutures) && objscrip.strScripName == "FIIFUTURES")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkNseCombinedReport) && objscrip.strScripName == "COMBINEDREPORT")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkBseEquity) && ((objscrip.strScripName == "BSE_EQUITY") || (objscrip.strScripName == "BSE_MTO")))
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkBseFo) && objscrip.strScripName == "BSE_FO")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkYahooIEOD1) && objscrip.strScripName == "YAHOOIEOD1MIN")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkYahooIEOD5) && objscrip.strScripName == "YAHOOIEOD5MIN")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkYahooEOD) && objscrip.strScripName == "YAHOOEOD")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkYahooFundamental) && objscrip.strScripName == "YAHOOFUNDAMENTAL")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkGoogleEOD) && objscrip.strScripName == "GOOGLEEOD")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkGoogleIEOD) && objscrip.strScripName == "GOOGLEIEOD")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkMutualFund) &&  objscrip.strScripName == "MFAMFI")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkMutualFund) && objscrip.strScripName == "MFNSE")
                        objscrip.bFireScrip = true;

                    if ((objSettings.ChkFOP) && objscrip.strScripName == "FOP")
                        objscrip.bFireScrip = true;
                }
                
                foreach (IScrip obj in Scrips)
                {

                    if (obj.bFireScrip)
                    {

                        strUrl = obj.strScripBaseURL;
                        strTempUrl = strUrl;

                        int i = objSettings.StartDate.Value.Day;
                        int j = objSettings.StartDate.Value.Month;
                        int k = objSettings.StartDate.Value.Year;

                        if ((obj.strScripName == "BSE_EQUITY") || (obj.strScripName == "BSE_MTO") || obj.strScripName == "BSE_FO")
                             strTOPFolder = AddSubFolders(objSettings.TargetFolder, "BSE");
                        else

                        if ((obj.strScripName == "YAHOOIEOD1MIN") || (obj.strScripName == "YAHOOIEOD5MIN") || (obj.strScripName == "YAHOOEOD") || (obj.strScripName == "YAHOOFUNDAMENTAL"))
                            strTOPFolder = AddSubFolders(objSettings.TargetFolder, "YAHOO");
                        else

                        if ((obj.strScripName == "GOOGLEEOD") || (obj.strScripName == "GOOGLEIEOD"))
                            strTOPFolder = AddSubFolders(objSettings.TargetFolder, "GOOGLE");

                        else
                        if ((obj.strScripName == "MFAMFI") || (obj.strScripName == "MFNSE"))
                            strTOPFolder = AddSubFolders(objSettings.TargetFolder, "MUTUALFUND");
                        //else
                        //    if ((obj.strScripName == "MFNSE") || (obj.strScripName == "MFNSE"))
                        //        strTOPFolder = AddSubFolders(objSettings.TargetFolder, "MFNSE");

                        else
                        if ((obj.strScripName == "FOP") || (obj.strScripName == "FOP"))
                            strTOPFolder = AddSubFolders(objSettings.TargetFolder, "FOP");

                        else
                            strTOPFolder = AddSubFolders(objSettings.TargetFolder, "NSE");

                        Nullable<DateTime> dtCurrentDate = objSettings.StartDate;

                        obj.LogMessage = AddMessageToLog;

                        if ((obj.strScripName == "YAHOOIEOD1MIN") || (obj.strScripName == "YAHOOIEOD5MIN") || (obj.strScripName == "YAHOOEOD") || (obj.strScripName == "YAHOOFUNDAMENTAL") || (obj.strScripName == "GOOGLEEOD") || (obj.strScripName == "GOOGLEIEOD"))
                        {
                            if (!Directory.Exists(Path.Combine(strTOPFolder, obj.strScripName)))
                                Directory.CreateDirectory(Path.Combine(strTOPFolder, obj.strScripName));
                            if (obj.strScripName == "YAHOOIEOD1MIN")
                                obj.DownloadYahooIEOD(strUrl, strTOPFolder, obj.strScripName, objSettings.YahooIEOD1MinList, objSettings.StartDate, objSettings.EndDate);
                            else if (obj.strScripName == "YAHOOIEOD5MIN")
                                obj.DownloadYahooIEOD(strUrl, strTOPFolder, obj.strScripName, objSettings.YahooIEOD5MinList, objSettings.StartDate, objSettings.EndDate);
                            else if (obj.strScripName == "YAHOOEOD")
                                obj.DownloadYahooIEOD(strUrl, strTOPFolder, obj.strScripName, objSettings.YahooEODList, objSettings.StartDate, objSettings.EndDate);
                            else if (obj.strScripName == "YAHOOFUNDAMENTAL")
                                obj.DownloadYahooIEOD(strUrl, strTOPFolder, obj.strScripName, objSettings.YahooFundamentalList, objSettings.StartDate, objSettings.EndDate);
                            else if (obj.strScripName == "GOOGLEEOD")
                                obj.DownloadGoogleData(strUrl, strTOPFolder, obj.strScripName, objSettings.GoogleEODList);
                            else if (obj.strScripName == "GOOGLEIEOD")
                                obj.DownloadGoogleData(strUrl, strTOPFolder, obj.strScripName, objSettings.GoogleIEODList);

                            continue;
                        }

                        while (k <= objSettings.EndDate.Value.Year)
                        {
                            string strYearDir = Path.Combine(strTOPFolder, obj.strScripName, dtCurrentDate.Value.Year.ToString());


                            if (!Directory.Exists(strYearDir))
                                Directory.CreateDirectory(strYearDir);

                            //if (obj.strScripName == "VIX")
                            //{
                            //    strBuildBhavFileName = "hist_india_vix_";
                            //    if (i < 10)
                            //        strBuildBhavFileName += "0";
                            //    strBuildBhavFileName += i.ToString() + "-";
                            //    if (j < 10)
                            //        strBuildBhavFileName += "0";
                            //    strBuildBhavFileName += j.ToString() + "-";
                            //    strBuildBhavFileName += k.ToString() + "_";
                            //    if (objSettings.EndDate.Value.Day < 10)
                            //        strBuildBhavFileName += "0";
                            //    strBuildBhavFileName += objSettings.EndDate.Value.Day.ToString() + "-";
                            //    if (objSettings.EndDate.Value.Month < 10)
                            //        strBuildBhavFileName += "0";
                            //    strBuildBhavFileName += objSettings.EndDate.Value.Month.ToString() + "-" + objSettings.EndDate.Value.Year.ToString() + ".csv";

                            //    obj.DownloadQuote(obj.strScripBaseURL + strBuildBhavFileName, Path.Combine(strYearDir, strBuildBhavFileName));
                            //    break;
                            //}

                            iEndMonthofYear = 12;

                            if (dtCurrentDate.Value.Year == objSettings.EndDate.Value.Year)
                                iEndMonthofYear = objSettings.EndDate.Value.Month;

                            j = dtCurrentDate.Value.Month;

                            while (j <= iEndMonthofYear)
                            {
                                string strMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dtCurrentDate.Value.Month);

                                strMonthName = strMonthName.ToUpper();

                                string strMonthDir = Path.Combine(strYearDir, strMonthName);



                                if (!Directory.Exists(strMonthDir))
                                    Directory.CreateDirectory(strMonthDir);


                                iEndDayOfMonth = GetEndOfMonth(dtCurrentDate);

                                if ((dtCurrentDate.Value.Year == objSettings.EndDate.Value.Year) &&
                                  (dtCurrentDate.Value.Month == objSettings.EndDate.Value.Month))

                                    iEndDayOfMonth = objSettings.EndDate.Value.Day;


                                i = dtCurrentDate.Value.Day;

                                Nullable<DateTime> dtTempDate = dtCurrentDate;

                                while (i <= iEndDayOfMonth)
                                {

                                    // if (!obj.strScripName.Equals("VIX"))
                                    strBuildBhavFileName = obj.BuildBhavFileName(i, strMonthName, j, k, obj.strScripName);

                                    if (obj.strScripName == "NSE Equity" || obj.strScripName == "NSE FO")
                                        strUrl += "/" + dtCurrentDate.Value.Year.ToString() + "/" + strMonthName + "/" + strBuildBhavFileName;
                                    else
                                        if (obj.strScripName == "BSE_MTO")
                                            strUrl += dtCurrentDate.Value.Year.ToString() + "/" + strBuildBhavFileName;
                                        else
                                            strUrl += strBuildBhavFileName;

                                    if ((dtTempDate.Value.DayOfWeek == DayOfWeek.Saturday) && objSettings.ChkIgnoreSaturday)
                                        bDoNOTDowload = true;
                                    else
                                        if ((dtTempDate.Value.DayOfWeek == DayOfWeek.Sunday) && objSettings.ChkIgnoreSunday)
                                            bDoNOTDowload = true;

                                    if (!bDoNOTDowload)
                                        obj.DownloadQuote(strUrl, Path.Combine(strMonthDir, strBuildBhavFileName));

                                    i++;
                                    dtTempDate = dtTempDate.Value.AddDays(1);
                                    bDoNOTDowload = false;

                                    strUrl = strTempUrl;
                                }
                                j++;
                                dtCurrentDate = new DateTime(dtCurrentDate.Value.Year, dtCurrentDate.Value.Month, 1).AddMonths(1);
                            }
                            k++;

                        }
                    }
                }  // foreach (IScrip obj in Scrips)
            }
        #endregion

    }
}
