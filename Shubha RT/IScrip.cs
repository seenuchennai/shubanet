using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;

namespace StockD
{
    interface IScrip
    {
         string strScripName { get; set; }
         string strScripDirectory { get; set; }
         string strScripBaseURL { get; set; }
         bool bFireScrip { get; set; }

         bool DownloadQuote(string strUrl, string strBhavFileName);
         bool DownloadYahooIEOD(string strUrl, string strTopFolder, string strScripName, List<string> IEODList, Nullable<DateTime> dtStartDate, Nullable<DateTime> dtEndDate);
         bool DownloadGoogleData(string strUrl, string strTopFolder, string strScripName, List<string> IEODList);
         bool DownloadIndiaIndicesData(string strUrl, string strTopFolder, string strScripName, List<string> IEODList);
         Action<string> LogMessage { get; set; }
         string BuildBhavFileName(int iDay, string strMonthName, int iMonth, int iYear, string strScripName);
    }
}
