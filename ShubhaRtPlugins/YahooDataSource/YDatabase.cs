using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using AmiBroker.Data;

namespace AmiBroker.Samples.YahooDataSource
{
    public class YDatabase
    {
        #region private variables

        private YConfiguration config;              // .NET Data Source's config data (from DatSource.XML file)
        private Workspace workspace;                // AmiBroker's Database settings window
        private Periodicity periodicity;            // db's base time interval (filled at the first call of GetQuotesEx. WorkSpace.TimeBase is not correct in same cases!)

        private bool connected;                     // status vars
        private bool inTimer;
        private bool firstGetQuotesExCall;

        private TickerDataCollection tickers;       // collection to hold tickers' data

        private Timer timer;                        // timer to initiate periodic data refresh and start date download on background threads
        private long lastRun;                       // the "last time" a refresh has been started

        private static long concurrentDownloads;         // number of concurrent data downloads running (WinINet max it in 2!)
        private static Queue<string> refreshQueue;  // tickers waiting to be refreshed

        #endregion

        /// <summary>
        /// constructor to save params and init objects
        /// </summary>
        /// <param name="config"></param>
        /// <param name="workSpace"></param>
        public YDatabase(YConfiguration config, Workspace workSpace)
        {
            this.config = config;
            this.workspace = workSpace;

            this.tickers = new TickerDataCollection();
            this.timer = new Timer(timer_tick, null, Timeout.Infinite, Timeout.Infinite);

            refreshQueue = new Queue<string>();
        }

        #region Connection and plugin status

        public void Connect()
        {
            LogAndMessage.LogAndAdd(MessageType.Info, "Start updating data at every " + config.RefreshPeriod.ToString() + " seconds.");

            // start timer
            timer.Change(1000, 300);

            // make it start an immediate refresh
            lastRun = -1;

            // indicate "successfull" connection
            connected = true;
        }

        public void Disconnect()
        {
            // stop timer
            timer.Change(Timeout.Infinite, Timeout.Infinite);

            // indicate disconnection
            connected = false;

            // clean up tickers from refreshQueue
            refreshQueue.Clear();

            LogAndMessage.LogAndAdd(MessageType.Info, "Stopped updating data.");
        }

        // get connection status to build plugin status
        public bool IsConnected
        {
            get { return connected; }
        }

        #endregion

        #region API calls

        public void GetQuotesEx(string ticker, ref QuotationArray quotes)
        {
            // save database periodicity at the very first call
            if (!firstGetQuotesExCall)
            {
                periodicity = quotes.Periodicity;
                firstGetQuotesExCall = true;
            }

            try
            {
                TickerData tickerData = tickers.GetTickerData(ticker);
                // if ticker is not yet known (first time to use in charts and no RT window use)
                if (tickerData == null)
                    tickerData = tickers.RegisterTicker(ticker);

                if (tickerData.QuoteDataStatus == QuoteDataStatus.Offline)
                {
                    // mark it for periodic Quotation data update
                    tickerData.MarkTickerForGetQuotes(periodicity);

                    // we do not want to run data refresh on AB's thread...
                    // we also want async processing of historical data download...
                    if (connected)
                        EnqueueTickerForRefresh(ticker);
                }

                lock (tickerData)
                {
                    // merge downloaded data to AB's quotation array
                    quotes.Merge(tickerData.Quotes);

                    // save last download time
                    tickerData.LastDownloaded = tickerData.LastProcessed;
                }
            }
            catch (Exception ex)
            {
                LogAndMessage.LogAndAdd(MessageType.Error, "Failed to subscribe to quote update: " + ex);
            }
        }

        public void UpdateRecentInfo(string ticker)
        {
            try
            {
                TickerData tickerData = tickers.GetTickerData(ticker);
                // if ticker is not yet known (first time to use in RT window and no use in charts)
                if (tickerData == null)
                    tickerData = tickers.RegisterTicker(ticker);

                tickerData.MarkTickerForRecentInfo();
            }
            catch (Exception ex)
            {
                LogAndMessage.LogAndAdd(MessageType.Error, "Failed to subscribe to real time window update: " + ex);
            }
        }

        internal AmiVar GetExtraData(string ticker, string name, Periodicity periodicity, int arraySize)
        {
            if (!connected)
                return new AmiVar(ATFloat.Null);            // to prevent AFL engine to report AFL method call failure

            try
            {
                TickerData tickerData = tickers.GetTickerData(ticker);

                // if it is a new ticker or  data is not available yet
                if (string.IsNullOrEmpty(name) | tickerData == null)
                {
                    return new AmiVar(ATFloat.Null);        // to prevent AFL engine to report AFL method call failure
                }

                lock (tickerData)
                {
                    string[] parts = name.Split('.');
                    object obj = tickerData;
                    Type type;

                    // walk the object hierarchy using reflection
                    for (int i = 0; i < parts.GetLength(0); i++)
                    {
                        type = obj.GetType();
                        obj = type.InvokeMember(parts[i], BindingFlags.Default | BindingFlags.GetField | BindingFlags.GetProperty, null, obj, null);
                        if (obj == null && i < parts.GetLength(0) - 1)
                        {
                            LogAndMessage.LogAndAdd(MessageType.Warning, "Extra data field does not exist: " + name, tickerData);
                            return new AmiVar(ATFloat.Null);            // to prevent AFL engine to report AFL method call failure
                        }
                    }

                    // convert object value to AmiVar and return it to AB

                    if (obj == null)       // it was a string or an object
                        return new AmiVar("");

                    Type valType = obj.GetType();

                    if (valType == typeof(bool))
                        return new AmiVar((bool)obj ? 1.0f : 0.0f);

                    if (valType.BaseType == typeof(System.Enum))
                        return new AmiVar((int)obj);

                    if (valType == typeof(short))
                        return new AmiVar((short)obj);

                    if (valType == typeof(int))
                        return new AmiVar((int)obj);

                    if (valType == typeof(long))
                        return new AmiVar((long)obj);

                    if (valType == typeof(float))
                        return new AmiVar((float)obj);

                    if (valType == typeof(double))
                        return new AmiVar((float)(double)obj);

                    if (valType == typeof(string))
                        return new AmiVar((string)obj);

                    if (valType == typeof(DateTime))
                        return new AmiVar(ATFloat.DateTimeToABDateNum((DateTime)obj));

                    if (valType == typeof(TimeSpan))
                        return new AmiVar(ATFloat.TimeSpanToABTimeNum((TimeSpan)obj));

                    return new AmiVar(ATFloat.Null);            // to prevent AFL engine to report AFL method call failure
                }
            }
            catch (MissingMethodException)
            {
                LogAndMessage.LogAndAdd(MessageType.Warning, "Extra data field does not exist: " + name);
                return new AmiVar(ATFloat.Null);                // to prevent AFL engine to report AFL method call failure
            }
            catch (Exception ex)
            {
                LogAndMessage.LogAndAdd(MessageType.Error, "Failed to get extra data: " + ex);
                return new AmiVar(ATFloat.Null);                // to prevent AFL engine to report AFL method call failure
            }
        }

        #endregion

        #region Data download and process

        /// <summary>
        /// Enqueue ticker for refresh on a background thread
        /// </summary>
        /// <param name="ticker"></param>
        private static void EnqueueTickerForRefresh(string ticker)
        {
            // if ticker is not yet enqueued
            if (!refreshQueue.Contains(ticker))
                refreshQueue.Enqueue(ticker);
        }

        /// <summary>
        /// Ticker event handler
        /// 1. It checks if the delay is over. Then it puts all tickers in the refreshQueue
        /// 2. It checks if there is anything in the refreshQueue and less then 3 requests are currently executing, then it starts up a threadpool thread to execute the refresh.
        /// </summary>
        /// <param name="sender"></param>
        private void timer_tick(object sender)
        {
            if (inTimer || !connected)
                return;

            inTimer = true;

            try
            {
                // if refresh is needed...
                long currentRun = (int)(DateTime.Now.TimeOfDay.TotalSeconds / config.RefreshPeriod);
                if (currentRun != lastRun)
                {
                    lastRun = currentRun;

                    // enqueue all tickers
                    string[] symbols = tickers.GetAllTickers();

                    foreach (string symbol in symbols)
                    {
                        EnqueueTickerForRefresh(symbol);
                    }
                }

                // it there are tickers enqueued and there are less then 3 downloads already running
                if (refreshQueue.Count > 0 && Interlocked.Read(ref  concurrentDownloads) < 3)
                {
                    // increment no of downloads
                    Interlocked.Increment(ref concurrentDownloads);

                    // dequeue the ticker and get the tickerdata for it
                    string ticker = refreshQueue.Dequeue();
                    TickerData tickerData = tickers.GetTickerData(ticker);

                    // make an idle threadpool thread execute the download in the background
                    ThreadPool.QueueUserWorkItem(RefreshTicker, tickerData);
                }
            }
            catch (Exception ex)
            {
                LogAndMessage.LogAndAdd(MessageType.Error, "Failed to process all tickers: " + ex);
            }
            finally
            {
                DataSourceBase.NotifyQuotesUpdate();
                inTimer = false;
            }
        }

        /// <summary>
        /// Refresh data (quotation and rt window) of a single ticker
        /// </summary>
        /// <param name="ticker"></param>
        private static void RefreshTicker(object ticker)
        {
            TickerData tickerData = (TickerData)ticker;

            // if ticker needs a quotaton refresh or RT window refresh
            if (tickerData.QuoteDataStatus > QuoteDataStatus.Offline || tickerData.UpdateRecentInfo)
            {
                HttpWebResponse response = YDatabase.GetWebData(tickerData);
                if (response != null)
                {
                    ProcessWebData(tickerData, response);

                    response.Close();
                }
            }

            // decrement no of downloads
            Interlocked.Decrement(ref concurrentDownloads);

            // notify AB to request quotes (execute GetQuotesEx again)
            DataSourceBase.NotifyQuotesUpdate();
        }

        /// <summary>
        /// Gets the web data for a ticker
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        private static HttpWebResponse GetWebData(TickerData ticker)
        {
            try
            {
                string yRange;
                // if quotes needs to be updated
                if (ticker.QuoteDataStatus >= QuoteDataStatus.Failed & ticker.Quotes != null)
                {
                    switch (ticker.Quotes.Periodicity)
                    {
                        case Periodicity.OneMinute:
                            yRange = "1d";
                            break;
                        case Periodicity.FiveMinutes:
                            yRange = "5d";
                            break;
                        case Periodicity.EndOfDay:
                            yRange = "3y";
                            break;
                        default:
                            throw new ArgumentException();
                    }
                }
                // if only RT window update is needed
                else
                {
                    yRange = "1d";
                }

                HttpWebResponse response = null;
                string url = "http://chartapi.finance.yahoo.com/instrument/1.0/" + ticker.Ticker + "/chartdata;type=quote;range=" + yRange + "/csv/";

                WebRequest request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;

                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                    return response;

                return null;
            }
            catch (Exception ex)
            {
                LogAndMessage.LogAndAdd(MessageType.Error, "Web request failed: " + ex);
                return null;
            }
        }

        /// <summary>
        /// Processes the web data to quotation list and rt window
        /// NOTE: AmiBroker' quotation array is not updated yet! It gets updated by this data in GetQuotesEx method call.
        /// </summary>
        /// <param name="tickerData"></param>
        /// <param name="data"></param>
        private static void ProcessWebData(TickerData tickerData, HttpWebResponse data)
        {
            if (tickerData == null | data == null)
                return;

            bool result = false;

            try
            {
                // opening response stream
                Stream stream = data.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                // reading the lines until it finds ticker
                string line = reader.ReadLine();
                while (!reader.EndOfStream && !line.StartsWith("ticker:"))
                    line = reader.ReadLine();

                // reading the next line
                line = reader.ReadLine();
                // if line indicates error
                if (line.StartsWith("errorid"))
                {
                    return;
                }

                string unit = string.Empty;
                if (!reader.EndOfStream && line.StartsWith("unit:"))
                {
                    unit = line;
                }

                // reading the lines until it finds volume (after it start the quote section)
                while (!reader.EndOfStream && !line.StartsWith("volume:"))
                    line = reader.ReadLine();

                if (reader.EndOfStream)
                    return;

                int ltzOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Seconds;

                CultureInfo ci = new CultureInfo("en-US");

                // processing quotation data from HTML stream
                Quotation quote = new Quotation();
                line = reader.ReadLine();

                lock (tickerData)
                {
                    if (tickerData.Quotes != null)
                        tickerData.Quotes.Clear();

                    while (!reader.EndOfStream)
                    {
                        int startIndex = 0;
                        int endIndex = line.IndexOf(',');
                        int timestamp = int.Parse(line.Substring(startIndex, endIndex - startIndex));

                        if (timestamp >= tickerData.LastDownloaded)
                        {
                            DateTime date;
                            if (unit == "unit:MIN") // timestamp: 1314192659
                            {
                                date = new DateTime(1970, 1, 1);
                                date = date.AddSeconds(timestamp + ltzOffset);
                            }
                            else // "unit:DAY", timestamp: 20100824
                            {
                                date = new DateTime(timestamp / 10000, timestamp / 100 % 100, timestamp % 100);
                            }

                            quote = new Quotation();
                            quote.DateTime = (AmiDate)date;

                            startIndex = endIndex + 1;
                            endIndex = line.IndexOf(',', startIndex);
                            quote.Price = float.Parse(line.Substring(startIndex, endIndex - startIndex), ci);

                            startIndex = endIndex + 1;
                            endIndex = line.IndexOf(',', startIndex);
                            quote.High = float.Parse(line.Substring(startIndex, endIndex - startIndex), ci);

                            startIndex = endIndex + 1;
                            endIndex = line.IndexOf(',', startIndex);
                            quote.Low = float.Parse(line.Substring(startIndex, endIndex - startIndex), ci);

                            startIndex = endIndex + 1;
                            endIndex = line.IndexOf(',', startIndex);
                            quote.Open = float.Parse(line.Substring(startIndex, endIndex - startIndex), ci);

                            startIndex = endIndex + 1;
                            endIndex = line.Length;
                            quote.Volume = float.Parse(line.Substring(startIndex, endIndex - startIndex), ci);

                            if (tickerData.Quotes != null)
                            {
                                tickerData.Quotes.Merge(quote);
                                tickerData.LastProcessed = timestamp;
                            }
                        }
                        line = reader.ReadLine();
                    }
                }


                // updating real time quote window using the last quote
                if (tickerData.UpdateRecentInfo)
                {
                    tickerData.RecentInfo.Last = quote.Price;

                    DateTime now = DateTime.Now;
                    int lastTickDate = now.Year * 10000 + now.Month * 100 + now.Day;
                    int lastTickTime = now.Hour * 10000 + now.Minute * 100 + now.Second;

                    tickerData.RecentInfo.DateChange = lastTickDate;
                    tickerData.RecentInfo.TimeChange = lastTickTime;
                    tickerData.RecentInfo.DateUpdate = lastTickDate;
                    tickerData.RecentInfo.TimeUpdate = lastTickTime;
                    DataSourceBase.NotifyRecentInfoUpdate(tickerData.Ticker, ref tickerData.RecentInfo);
                }

                tickerData.LastTickTime = quote.DateTime.Hour * 10000 + quote.DateTime.Minute * 100 + quote.DateTime.Second;
                tickerData.LastTickDate = quote.DateTime.Year * 10000 + quote.DateTime.Month * 100 + quote.DateTime.Day;

                result = true;
            }
            finally
            {
                // if ticker was used in a chart, scan, etc. we update it's quotation status
                if (tickerData.QuoteDataStatus != QuoteDataStatus.Offline)
                    tickerData.QuoteDataStatus = result ? QuoteDataStatus.Online : QuoteDataStatus.Failed;

                // if ticker data was downloaded and processed with no error at least once...
                if (tickerData.QuoteDataStatus == QuoteDataStatus.Online)
                    tickerData.IsKnown = true;
            }
        }

        #endregion
    }
}
