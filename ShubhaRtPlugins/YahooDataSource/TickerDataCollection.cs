using System;
using System.Collections.Generic;

namespace AmiBroker.Samples.YahooDataSource
{
    /// <summary>
    /// This class helps mapping and accessing TickerData objects used by the data plugin
    /// </summary>
    /// <remarks>
    /// When AmiBroker calls the plugin using its symbol, the plugin uses mapTickerTickerData list to map the AB symbol to TickerData.
    /// When AmiBroker calls the plugin using a new symbol, it is registered in mapTickerTickerData.
    /// </remarks>
    internal class TickerDataCollection
    {
        private SortedDictionary<string, TickerData> mapTickerTickerData;

        internal TickerDataCollection()
        {
            mapTickerTickerData = new SortedDictionary<string, TickerData>();
        }

        #region Mapping AB symbol to TickerData

        internal TickerData RegisterTicker(string ticker)
        {
            lock (mapTickerTickerData)
            {
                TickerData tickerData;

                if (mapTickerTickerData.TryGetValue(ticker, out tickerData))
                    return tickerData;

                tickerData = new TickerData(ticker);

                mapTickerTickerData.Add(ticker, tickerData);

                return tickerData;
            }
        }

        internal TickerData GetTickerData(string ticker)
        {
            TickerData result;

            lock (mapTickerTickerData)
            {
                mapTickerTickerData.TryGetValue(ticker, out result);
            }

            return result;
        }

        #endregion

        internal string[] GetAllTickers()
        {
            lock (mapTickerTickerData)
            {
                string[] result = new string[mapTickerTickerData.Count];

                mapTickerTickerData.Keys.CopyTo(result, 0);

                return result;
            }
        }
    }
}