using System;
using System.Collections.Generic;

using System.Text;
using System.Net;
using System.IO;
using System.Collections;
using log4net;
using log4net.Config;
namespace StockD
{
    class NSE : IScrip
    {
        #region properties

        public bool bFireScrip
        {
            get;
            set;
        }

        public string strScripBaseURL
        {
            get;
            set;
        }

        public Action<string> LogMessage
        {
            get;
            set;
        }

        public string strScripName
        {
            get;
            set;
        }

        public string strScripDirectory
        {
            get;
            set;
        }

        #endregion

        #region methods

        public string BuildBhavFileName(int iDay, string strMonthName, int iMonth, int iYear, string strScripName)
        {
            string strBhavFileName = string.Empty;
            string strFromString = string.Empty;


            if (strScripName == "NSE Equity")
                strBhavFileName = "cm";
            else
            if (strScripName == "NSE FO")
                strBhavFileName = "fo";
            else
            if (strScripName == "MTO")
                strBhavFileName = "MTO_";
            else
            if (strScripName == "NSE Forex")
                strBhavFileName = "PR";
            else
                if (strScripName == "VIX")
                    strBhavFileName = "hist_india_vix_";
            else
            if (strScripName == "BULKDEAL")
                strBhavFileName = "";
            else
            if (strScripName == "BLOCKDEAL")
                strBhavFileName = "";
            else
            if (strScripName == "FIIFUTURES")
                strBhavFileName = "fii_stats_";
            else
            if (strScripName == "COMBINEDREPORT")
                strBhavFileName = "combined_report";
            else
            if (strScripName == "BSE_EQUITY")
                strBhavFileName = "eq";
            else
            if (strScripName == "BSE_MTO")
            {
                strBhavFileName = "SCBSEALL";
            }
            else
            if (strScripName == "BSE_FO")
                strBhavFileName = "bhavcopy";
            else
            if (strScripName == "MFNSE")
                strBhavFileName = "MA";
            else
            if (strScripName == "FOP")
                strBhavFileName = "fao_participant_oi_";


            if ((strScripName == "NSE Equity") || (strScripName == "NSE FO"))
            {
                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName = strBhavFileName + iDay.ToString() + strMonthName + iYear.ToString() + "bhav.csv.zip";
            }


            if (strScripName == "MTO")
            {
                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName += iDay.ToString();

                if (iMonth < 10)
                    strBhavFileName += "0";

                strBhavFileName += iMonth.ToString() + iYear.ToString() + ".DAT";
            }


            if (strScripName == "NSE Forex")
            {
                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName += iDay.ToString();

                if (iMonth < 10)
                    strBhavFileName += "0";

                strBhavFileName += iMonth.ToString() + (iYear.ToString()).Substring(2) + ".zip";
            }



            if (strScripName == "VIX")
            {
                if (iDay < 10)
                    strFromString = "0";

                strFromString += iDay.ToString() + "-";
                if (iMonth < 10)
                    strFromString += "0";

                strFromString += iMonth.ToString() + "-";

                strFromString += iYear.ToString();


                strBhavFileName += strFromString + "_" + strFromString + ".csv";

            }

            if (strScripName == "NCDEX")
            {
                if (iMonth < 10)
                    strBhavFileName += "0";

                strBhavFileName += iMonth.ToString() + "-";

                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName += iDay.ToString() + "-" + iYear.ToString() + ".csv";
            }


            if ( (strScripName == "BULKDEAL") || (strScripName == "BLOCKDEAL") )
            {
                if (iDay < 10)
                    strFromString = "0";

                strFromString += iDay.ToString() + "-";
                if (iMonth < 10)
                    strFromString += "0";

                strFromString += iMonth.ToString() + "-";

                strFromString += iYear.ToString();

                if(strScripName == "BULKDEAL")
                    strBhavFileName += strFromString + "-TO-" + strFromString + "_bulk"+ ".csv";
                else
                    if (strScripName == "BLOCKDEAL")
                        strBhavFileName += strFromString + "-TO-" + strFromString + "_block" + ".csv";

            }

            if (strScripName == "FIIFUTURES")
            {
                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName += iDay.ToString() + "-";

                strMonthName = strMonthName.ToLower();
                    
                strMonthName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(strMonthName);

                strBhavFileName += strMonthName + "-";

                strBhavFileName += iYear.ToString() + ".xls";
            }

            if (strScripName == "COMBINEDREPORT")
            {
                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName += iDay.ToString();

                if (iMonth < 10)
                    strBhavFileName += "0";

                strBhavFileName += iMonth.ToString() + iYear.ToString() + ".zip";
            }


            if (strScripName == "BSE_EQUITY")
            {
                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName += iDay.ToString();

                if (iMonth < 10)
                    strBhavFileName += "0";

                strBhavFileName += iMonth.ToString() + (iYear.ToString()).Substring(2) + "_csv.zip";
            }

            if (strScripName == "BSE_FO")
            {
                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName += iDay.ToString() + "-";

                if (iMonth < 10)
                    strBhavFileName += "0";

                strBhavFileName += iMonth.ToString() + "-" + (iYear.ToString()).Substring(2) + ".zip";
            }

            if (strScripName == "BSE_MTO")
            {
                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName += iDay.ToString();


                if (iMonth < 10)
                    strBhavFileName += "0";

                strBhavFileName += iMonth.ToString() + ".zip";

            }

            if (strScripName == "MFAMFI")
            {
                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName += iDay.ToString() + "-";


                strBhavFileName += strMonthName + "-" + iYear.ToString() + ".csv";
            }

            if (strScripName == "MFNSE")
            {
                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName += iDay.ToString();

                if (iMonth < 10)
                    strBhavFileName += "0";

                strBhavFileName += iMonth.ToString() + (iYear.ToString()).Substring(2) + ".csv";
            }

            if (strScripName == "FOP")
            {
                if (iDay < 10)
                    strBhavFileName += "0";

                strBhavFileName += iDay.ToString();

                if (iMonth < 10)
                    strBhavFileName += "0";

                strBhavFileName += iMonth.ToString() + iYear.ToString() + ".csv";
            }
            return strBhavFileName;
        }


        public class CookieAwareWebClient : WebClient
        {
            private readonly CookieContainer m_container = new CookieContainer();

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                HttpWebRequest webRequest = request as HttpWebRequest;
                if (webRequest != null)
                {
                    webRequest.CookieContainer = m_container;
                }
                return request;
            }

            protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
            {
                WebResponse response = base.GetWebResponse(request, result);
                ReadCookies(response);
                return response;
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                WebResponse response = base.GetWebResponse(request);
                ReadCookies(response);
                return response;
            }

            private void ReadCookies(WebResponse r)
            {
                var response = r as HttpWebResponse;
                if (response != null)
                {
                    CookieCollection cookies = response.Cookies;
                    m_container.Add(cookies);
                }
            }
        }

        public bool DownloadIndiaIndicesData(string strUrl, string strTopFolder, string strScripName, List<string> IEODList)
        {
            string strTempurl = string.Empty;
            string strBhavFileName = Path.Combine(strTopFolder, strScripName);
            string tempstrBhavFileName = strBhavFileName;


            strTempurl = strUrl;

            foreach (string obj in IEODList)
            {
                string[] words = obj.Split('.');
                string strSymbol = words[0];
                string strExchange = words[1];

                if ((strSymbol != null) && (strExchange != null))
                {

                    try
                    {
                        strTempurl = strUrl;
                        tempstrBhavFileName = Path.Combine(strTopFolder, strScripName);
                        using (var client = new WebClient())
                        {
                            client.Headers.Add("Accept", "application/zip");
                            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1");

                            if (strScripName == "GOOGLEIEOD")
                                strTempurl += strSymbol + "&x=" + strExchange.ToUpper() + "&i=60&p=15d&f=d,o,h,l,c,v";
                            else if (strScripName == "GOOGLEEOD")
                                strTempurl += strSymbol + "&x=" + strExchange.ToUpper() + "&i=d&p=15d&f=d,o,h,l,c,v";

                            tempstrBhavFileName += "\\" + obj + ".csv";

                            client.DownloadFile(strTempurl, tempstrBhavFileName);
                        }

                    }
                    catch (WebException)
                    {
                        LogMessage(strTempurl + " Not found!" + Environment.NewLine);
                    }
                }
            }

            return true;
        }

        public bool DownloadGoogleData(string strUrl, string strTopFolder, string strScripName, List<string> IEODList)
        {
            string strTempurl = string.Empty;
            string strBhavFileName = Path.Combine(strTopFolder, strScripName);
            string tempstrBhavFileName = strBhavFileName;


            strTempurl = strUrl;

            foreach (string obj in IEODList)
            {
                string[] words = obj.Split('.');
                string strSymbol = words[0];
                string strExchange = words[0];

                if ((strSymbol != null) && (strExchange != null))
                {

                    try
                    {
                        strTempurl = strUrl;
                        tempstrBhavFileName = Path.Combine(strTopFolder, strScripName);
                        using (var client = new WebClient())
                        {
                            client.Headers.Add("Accept", "application/zip");
                            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1");

                            if (strScripName == "GOOGLEIEOD")
                                strTempurl += strSymbol + "&x=" + strExchange.ToUpper() + "&i=60&p=15d&f=d,o,h,l,c,v";
                            else if (strScripName == "GOOGLEEOD")
                                strTempurl += strSymbol + "&x=" + strExchange.ToUpper() + "&i=d&p=15d&f=d,o,h,l,c,v";

                            tempstrBhavFileName += "\\" + obj + ".csv";

                            client.DownloadFile(strTempurl, tempstrBhavFileName);
                        }

                    }
                    catch (WebException)
                    {
                        LogMessage(strTempurl + " Not found!" + Environment.NewLine);
                    }
                }
            }

            return true;
        }

        public bool DownloadYahooIEOD(string strUrl, string strTopFolder, string strScripName, List<string> IEODList, Nullable<DateTime> dtStartDate, Nullable<DateTime> dtEndDate)
        {
            string strTempurl = string.Empty;
            string strBhavFileName = Path.Combine(strTopFolder, strScripName);
            string tempstrBhavFileName = strBhavFileName;

 
               strTempurl = strUrl;

               foreach (string obj in IEODList)
               {
                   try
                   {
                    strTempurl = strUrl;
                    tempstrBhavFileName = Path.Combine(strTopFolder, strScripName);
                    using (var client = new WebClient())
                    {
                        client.Headers.Add("Accept", "application/zip");
                        client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                        client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1");

                        if (strScripName == "YAHOOIEOD1MIN")
                            strTempurl += obj + "/chartdata;type=quote;range=1d/csv/";
                        else if (strScripName == "YAHOOIEOD5MIN")
                            strTempurl += obj + "/chartdata;type=quote;range=5d/csv/";
                        else if (strScripName == "YAHOOFUNDAMENTAL")
                            strTempurl += obj + "&f=snl1ee7e8e9r5b4j4p5s6s7r1qdt8j1f6&e=.csv";
                        else if (strScripName == "YAHOOEOD")
                        {
                            strTempurl += obj;
                            strTempurl += "&a=";
                            if (dtStartDate.Value.Month < 10)
                            {
                                strBhavFileName += "0";
                                strTempurl += "0";
                            }

                            if (dtStartDate.Value.Month == 1)   // January
                            {
                                strBhavFileName += "0";
                                strTempurl += "0";
                            }
                            else
                            {
                                strBhavFileName += (dtStartDate.Value.Month - 1).ToString();
                                strTempurl += (dtStartDate.Value.Month - 1).ToString();
                            }

                            strTempurl += dtStartDate.Value.Month.ToString();

                            strTempurl += "&b=";
                            if (dtStartDate.Value.Day < 10)
                            {
                                strBhavFileName += "0";
                                strTempurl += "0";
                            }

                            strBhavFileName += dtStartDate.Value.Day.ToString();
                            strTempurl += dtStartDate.Value.Day.ToString();

                            strTempurl += "&c=";
                            strBhavFileName += dtStartDate.Value.Year.ToString();
                            strTempurl += dtStartDate.Value.Year.ToString();


                            strTempurl += "&d=";
                            if (dtEndDate.Value.Month < 10)
                            {
                                strBhavFileName += "0";
                                strTempurl += "0";
                            }

                            if (dtEndDate.Value.Month == 1)  //January
                            {
                                strBhavFileName += "0";
                                strTempurl += "0";
                            }
                            else
                            {
                                strBhavFileName += (dtEndDate.Value.Month - 1).ToString();
                                strTempurl += (dtEndDate.Value.Month - 1).ToString();
                            }

                            strTempurl += "&e=";
                            if (dtEndDate.Value.Day < 10)
                            {
                                strBhavFileName += "0";
                                strTempurl += "0";
                            }
                            strBhavFileName += dtEndDate.Value.Day.ToString();
                            strTempurl += dtEndDate.Value.Day.ToString();

                            strTempurl += "&f=";
                            strBhavFileName += dtEndDate.Value.Year.ToString();
                            strTempurl += dtEndDate.Value.Year.ToString();
                            strBhavFileName  += "&g=d";
                            strTempurl += "&g=d"; 
                        }

                        tempstrBhavFileName += "\\" + obj + ".csv";

                        client.DownloadFile(strTempurl, tempstrBhavFileName);
                    }

                }
                catch (WebException)
                {
                    LogMessage(strTempurl + " Not found!" + Environment.NewLine);
                }
            }
 
            return true;
        }

        public bool DownloadQuote(string strUrl, string strBhavFileName)
        {
            try
            {
                //using (var client = new WebClient())
                //{

                //    CookieContainer m_container = new CookieContainer();

                //    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.nseindia.com/products/dynaContent/equities/equities/bulkdeals.jsp?symbol=&segmentLink=13&symbolCount=&dateRange=day&fromDate=01-01-2012&toDate=21-09-2012");
                //    httpWebRequest.CookieContainer = m_container;
                //    httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1";
                //    httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //    HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                //    int cookieCount = m_container.Count;

                //    client.Headers.Add("Host", "www.nseindia.com");
                //    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1");
                //    client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                //    client.Headers.Add("Referer", "http://www.nseindia.com/products/content/equities/equities/eq_bulkdealsarchive.htm");
                //    client.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                //    client.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                //    client.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.3");
                //    client.Headers.Add("Cookie", response.Headers["Set-Cookie"]);

                //    client.DownloadFile("http://www.nseindia.com/content/equities/bulkdeals/datafiles/01-01-2012-TO-21-09-2012_bulk.csv", "01-01-2012-TO-21-09-2012_bulk.csv");
                //}

                using (var client = new WebClient())
                {
                    client.Headers.Add("Accept", "application/zip");


                    client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1");
                    client.DownloadFile(strUrl, strBhavFileName);

                    //client.Headers.Add("Referer", "http://www.nseindia.com/products/content/equities/equities/eq_bulkdealsarchive.htm");
                    //client.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                    //client.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                    //client.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.3");
                    //client.DownloadFile(@"http://www.nseindia.com/content/equities/bulkdeals/datafiles/01-01-2011-TO-21-09-2012_bulk.csv", @"01-01-2011-TO-21-09-2012_bulk.csv");

                }

            }
            catch (WebException e)
            {

                //HttpWebResponse response = (System.Net.HttpWebResponse)e.Response;
                //if (response.StatusCode == HttpStatusCode.NotFound)
                //{
                //    System.Diagnostics.Debug.WriteLine("Not found!");
                //    LogMessage(e.Response.ResponseUri + " Not found!" + Environment.NewLine);
                //}
                LogMessage(strUrl + " Not found!" + Environment.NewLine);
                log4net.Config.XmlConfigurator.Configure();
                ILog log = LogManager.GetLogger(typeof(MainWindow));
               

                log.Warn(strUrl + " Not found!");
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                ////request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.CacheIfAvailable);
                //WebResponse response = request.GetResponse();
                //StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII);

                //string retVal = reader.ReadToEnd();

                //response.Close();
                //reader.Close();
                //Utility.Save("C:\\Temp\\Stock\\cm04SEP2012bhav.csv.zip", retVal);

                
            }

            return true;
        }
        #endregion
    }
}