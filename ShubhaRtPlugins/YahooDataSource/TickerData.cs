using System;
using System.ComponentModel;
using AmiBroker.Data;

namespace AmiBroker.Samples.YahooDataSource
{
    public enum QuoteDataStatus : int
    {
        [Description("Offline")]
        Offline,                    // no quote update
        [Description("Failed")]
        Failed,                     // ticker is not valid, Yahoo does not find it, web request failed, etc.
        [Description("New")]
        New,                        // start updating historical quote
        [Description("Online")]
        Online                      // receive RT data, ticker is usable in charts and scans
    }

    /// <summary>
    /// Data for a single ticker
    /// </summary>
    /// <remarks>
    /// Each ticker that is used by AmiBroker (chart, scan or RT window) has a corresponding object of this type.
    /// All public properties of TickerData can be read by any AFL script using the GetExtraData AFL method.
    /// </remarks>
    public class TickerData
    {
        public const string DataSource = "Yahoo";       // identifies the data source type
        public string Ticker;                           // symbol as it is stored in AmiBroker
        public bool IsKnown;                            // if it is known by Yahoo
        internal int LastProcessed;                     // timestamp of last processed data

        // Quotes
        public QuoteDataStatus QuoteDataStatus;         // Status of quotation data of the ticker
        public int LastDownloaded;                      // timestamp of last quote that was merged into AB database
        internal QuotationList Quotes;                  // Temporary quotes of proper periodicity. Received ticks are merged into this quote list. This list is merged into AB's quotation array.

        // RecentInfo
        public bool UpdateRecentInfo;                   // Indicates if AmiBroker uses this ticker in RT window
        public RecentInfo RecentInfo;                   // RecentInfo data for AmiBroker's Real Time Window

        // trading status
        public int LastTickTime;                        // last tick's time
        public int LastTickDate;                        // last tick's date

        public TickerData(string ticker)
        {
            Ticker = ticker;
            QuoteDataStatus = QuoteDataStatus.Offline;
        }

        // mark ticker for automatic quotation update
        internal void MarkTickerForGetQuotes(Periodicity periodicity)
        {
            lock (this)
            {
                // create list to store downloaded data
                Quotes = new QuotationList(periodicity);

                // mark ticker for quote update
                QuoteDataStatus = QuoteDataStatus.New;
            }
        }

        // mark ticker for automatic RT window update
        internal void MarkTickerForRecentInfo()
        {
            lock (this)
            {
                // set default RI data
                RecentInfo = new RecentInfo();
                RecentInfo.Bitmap = RecentInfoField.Last | RecentInfoField.DateChange | RecentInfoField.DateUpdate;

                // mark ticker for RI update
                UpdateRecentInfo = true;
            }
        }
    }
}