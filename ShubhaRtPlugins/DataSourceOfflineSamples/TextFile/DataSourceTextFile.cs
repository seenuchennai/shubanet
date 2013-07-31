using System;
using System.IO;
using AmiBroker.Data;
using AmiBroker.Utils.Data.DataSourceOffline;

namespace AmiBroker.Samples.DataSourceOfflineSamples.TextFile
{
    [ABDataSource("Sample - Csv/text file")]
    internal class DataSourceTextFile : DataSourceOffline
    {
        private PluginStatus pluginStatus = new PluginStatus(StatusCode.OK, System.Drawing.Color.Green, "OK", "Everything is fine...");

        public DataSourceTextFile(string settings)
            :base(settings)
        {
        }

        /// <summary>
        /// Get and build historical data for a ticker
        /// </summary>
        /// <param name="tickerData"></param>
        /// <remarks>
        /// Place short term resource allocation (e.g. local DB connection setup, etc.) here.
        /// Load and populate all quotation data to tickerData.Quotes using the Merge method. 
        /// See .NET for AmiBroker's Help on QuotationList class.
        /// </remarks>
        public override void Ticker_GetQuotes(TickerData tickerData)
        {
            // get the path to data file directory
            string dataDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dataDirectory = Directory.GetParent(dataDirectory).FullName;    // ...\AmiBroker\.NET for AmiBroker
            dataDirectory = Path.Combine(dataDirectory, @"Samples\Ascii");  // ...\AmiBroker\.NET for AmiBroker\Samples\Ascii

            // get the data file path to load for the ticker
            // --- replace it with your custom logic to build data file path using the ticker's name
            // --- E.g.:
            // --- string dataFilePath = Path.Combine(dataDirectory, tickerData.Ticker + ".csv");
            string dataFilePath = Path.Combine(dataDirectory, "demodata_hist.csv");

            try
            {
                // open data file of the ticker for reading
                using (FileStream fileStream = File.Open(dataFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        // skip the first line (header row)
                        // --- remove next line if your data files have no header row ---
                        streamReader.ReadLine();

                        // clear previously stored quotes in the plugin
                        tickerData.Quotes.Clear();

                        // read until the end of the file
                        while (!streamReader.EndOfStream)
                        {
                            // read a line (quotation record) from the file
                            string line = streamReader.ReadLine();

                            // convert the line to quotation object
                            Quotation quote = GetQuote(line);

                            // Merge method builds bars according to database's timebase
                            tickerData.Quotes.Merge(quote);
                        } 
                    }
                }
            }
            catch (Exception ex)
            {
                pluginStatus.Status = StatusCode.SevereError;
                pluginStatus.Color = System.Drawing.Color.Red;
                pluginStatus.ShortMessage = "Error";
                pluginStatus.LongMessage = ex.ToString();

                DataSourceBase.DotNetLog("Sample - Csv/text file", "Error", ex.ToString());
            }
        }

        /// <summary>
        /// Interpret text as a Quotation record and build a Quotation object form it.
        /// --- Customize this method to read quote from record
        /// </summary>
        /// <param name="record">Quotation record line of the CSV input file</param>
        /// <returns>Quotation object if successfull. It may throw exception on parse failures.</returns>
        private Quotation GetQuote(string record)
        {
            // CSV format:
            // ---------------------------------------------------------
            // Ticker,Date,Time,Open,High,Low,Close,Volume
            // EUR.USD,2007-07-09,00:00:00,1.3634,1.3635,1.3626,1.3627,-1
            // EUR.USD,2007-07-09,01:00:00,1.3627,1.3630,1.3623,1.3626,-1

            // line is split into fields separated by a ','
            string[] fields = record.Split(',');

            Quotation quote = new Quotation();

            //string ticker = fields[0];

            DateTime date = DateTime.Parse(fields[1]);
            TimeSpan time = TimeSpan.Parse(fields[2]);
            DateTime dateTime = date.Add(time);
            quote.DateTime = (AmiDate)dateTime;         // cast to AmiBroker's internal datetime type represented on 64 bit ulong

            quote.Open = float.Parse(fields[3]);
            quote.High = float.Parse(fields[4]);
            quote.Low = float.Parse(fields[5]);
            quote.Price = float.Parse(fields[6]);       // Close price
            quote.Volume = float.Parse(fields[7]);
            //quote.AuxData1 = 0f;
            //quote.AuxData2 = 0f;
            //quote.OpenInterest = 0f;

            return quote;
        }

        /// <summary>
        /// Ticker's quotation data is passed to AmiBroker
        /// </summary>
        /// <param name="tickerData"></param>
        /// <remarks>
        /// This method is called when quotes are passed to AmiBroker and there is no need for holding resources for this ticker.
        /// Release resources allocated in Ticker_GetQuotes(e.g. local DB connection setup, quote objects, etc.)
        /// </remarks>
        public override void Ticker_Ready(TickerData tickerData)
        {
            if (tickerData.Quotes != null)
                tickerData.Quotes.Clear();
        }

        /// <summary>
        /// Data API method to get plugins status
        /// </summary>
        /// <returns></returns>
        public override PluginStatus GetStatus()
        {
            return pluginStatus;
        }

        /// <summary>
        /// Data API method to get plugins status
        /// </summary>
        /// <returns></returns>
        public override bool SetTimeBase(Periodicity timeBase)
        {
            return timeBase == Periodicity.OneHour;
        }
    }
}