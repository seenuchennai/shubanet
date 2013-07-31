using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using AmiBroker;
using AmiBroker.Data;
using AmiBroker.PlugIn;

namespace AmiBroker.Samples.YahooDataSource
{
    [ABDataSource("Yahoo Real Time Data")]
    public class YDataSource : DataSourceBase
    {
        #region private variables

        private YDatabase database;
        private YConfiguration config;

        private string lastLongMessage;
        private int lastLongMessageTime;

        private bool allowMixedEodIntra;            // db have EOD and intraday data
        private int numBars;                        // max number of bars in the db

        private string currentTicker = null;

        #region Context menu and form variables

        private ToolStripMenuItem mReconnect;
        private ToolStripMenuItem mDisconnect;
        private ToolStripMenuItem mOpenInYahoo;
        private ToolStripMenuItem mOpenLogFile;

        private ContextMenuStrip mContextMenu;

        #endregion

        #endregion

        public YDataSource(string config)
            : base(config)
        {
            #region Context menu

            // main menu
            mReconnect = new ToolStripMenuItem("Connect", null, new EventHandler(mReconnect_Click));
            mDisconnect = new ToolStripMenuItem("Disconnect", null, new EventHandler(mDisconnect_Click));
            mOpenInYahoo = new ToolStripMenuItem("Open in Yahoo", null, new EventHandler(mOpenInYahoo_Click));
            mOpenLogFile = new ToolStripMenuItem("Open Log File", null, new EventHandler(mOpenLogFile_Click));

            ToolStripSeparator mSeparator = new ToolStripSeparator();

            mContextMenu = new ContextMenuStrip();
            mContextMenu.Items.AddRange(new ToolStripItem[] { mReconnect, mDisconnect, mOpenInYahoo, mSeparator, mOpenLogFile });

            SetContextMenuState();

            #endregion
        }

        #region AmiBroker's API calls

        public static new string Configure(string oldSettings, ref InfoSite infoSite)
        {
            YConfiguration configuration = YConfiguration.GetConfigObject(oldSettings);

            YConfigureForm frm = new YConfigureForm(configuration, ref infoSite);
            if (frm.ShowDialog() == DialogResult.OK)
                return YConfiguration.GetConfigString(frm.GetNewSettings());
            else
                return oldSettings;
        }

        public override void GetQuotesEx(string ticker, ref QuotationArray quotes)
        {
            database.GetQuotesEx(ticker, ref quotes);
        }

        public override void GetRecentInfo(string ticker)
        {
            database.UpdateRecentInfo(ticker);
        }

        public override AmiVar GetExtraData(string ticker, string name, Periodicity periodicity, int arraySize)
        {
            return database.GetExtraData(ticker, name, periodicity, arraySize);
        }

        public override PluginStatus GetStatus()
        {
            PluginStatus status = new PluginStatus();

            if (database.IsConnected)
            {
                status.Status = StatusCode.OK;
                status.Color = System.Drawing.Color.ForestGreen;
                status.ShortMessage = "Ready";
            }
            else
            {
                status.Status = StatusCode.Warning;
                status.Color = System.Drawing.Color.Red;
                status.ShortMessage = "Error";
            }

            status.LongMessage = LogAndMessage.GetMessages();

            // if there is no message, we show short message
            if (string.IsNullOrEmpty(status.LongMessage))
            {
                status.LongMessage = status.ShortMessage;
                // save as the last shown message to avoid status popup
                lastLongMessage = status.ShortMessage;
            }

            // if new message we use a new lastLongMessageTime value to cause status popup
            if (lastLongMessage != status.LongMessage)
            {
                lastLongMessage = status.LongMessage;
                lastLongMessageTime = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
            }

            // set status and "timestamp"
            status.Status = (StatusCode)((int)status.Status + lastLongMessageTime);

            SetContextMenuState();

            return status;
        }

        public override bool SetTimeBase(Periodicity timeBase)
        {
            // Yahoo can return 1 min, 5 min and daily bars
            return timeBase == Periodicity.OneMinute || timeBase == Periodicity.FiveMinutes || timeBase == Periodicity.EndOfDay;
        }

        public override int GetSymbolLimit()
        {
            // limit the symbols to handle concurrently
            return 20;
        }

        public override bool Notify(ref PluginNotification notifyData)
        {
            bool result = true;

            switch (notifyData.Reason)
            {
                case Reason.DatabaseLoaded:

                    // if database is loaded
                    if (database != null)
                    {
                        // disconnect from data provider and reset all data
                        database.Disconnect();
                    }

                    // start logging the opening of the database
                    LogAndMessage.Log(MessageType.Info, "Database: " + notifyData.DatabasePath);

                    allowMixedEodIntra = notifyData.Workspace.AllowMixedEODIntra != 0;
                    LogAndMessage.Log(MessageType.Info, "Mixed EOD/Intra: " + allowMixedEodIntra);

                    numBars = notifyData.Workspace.NumBars;
                    LogAndMessage.Log(MessageType.Info, "Number of bars: " + numBars);

                    LogAndMessage.Log(MessageType.Info, "Database config: " + Settings);

                    // create the config object
                    config = YConfiguration.GetConfigObject(Settings);

                    // create new database object
                    database = new YDatabase(config, notifyData.Workspace);

                    // connect database to data provider
                    database.Connect();

                    break;

                // user changed the db
                case Reason.DatabaseUnloaded:

                    // disconnect from data provider
                    if (database != null)
                        database.Disconnect();

                    // clean up
                    database = null;

                    break;

                // seams to be obsolete
                case Reason.SettingsChanged:

                    break;

                // user right clicks data plugin area in AB
                case Reason.RightMouseClick:

                    if (database != null)
                    {
                        currentTicker = notifyData.CurrentSI.ShortName;

                        SetContextMenuState();

                        ShowContextMenu(mContextMenu);
                    }

                    break;

                default: result = false;

                    break;
            }
            return result;
        }

        #endregion

        #region Context menu

        private void mReconnect_Click(object sender, EventArgs e)
        {
            LogAndMessage.Log(MessageType.Info, "Manually reconnected.");

            database.Connect();
        }

        private void mDisconnect_Click(object sender, EventArgs e)
        {
            LogAndMessage.Log(MessageType.Info, "Manually disconnected.");

            database.Disconnect();
        }

        private void mOpenInYahoo_Click(object sender, EventArgs e)
        {
            if (database != null && !string.IsNullOrEmpty(currentTicker))
            {
                Type shellType = Type.GetTypeFromProgID("Wscript.Shell");
                object shell = Activator.CreateInstance(shellType);

                shellType.InvokeMember("Run", BindingFlags.InvokeMethod, null, shell, new object[] { "http://finance.yahoo.com/q/bc?t=5d&s=" + currentTicker });
            }
        }

        private void mOpenLogFile_Click(object sender, EventArgs e)
        {
            const string npp = @"C:\Program Files (x86)\Notepad++\notepad++.exe";

            ProcessStartInfo psi;

            try
            {
                // check if notepad++ is installed
                if (File.Exists(npp))
                    // start notepad++ to open the log file
                    psi = new ProcessStartInfo(npp);
                else
                    // start notepad to open the log file
                    psi = new ProcessStartInfo("notepad.exe");

                psi.WorkingDirectory = Path.GetDirectoryName(DataSourceBase.DotNetLogFile);
                psi.Arguments = DataSourceBase.DotNetLogFile;

                // start log file viewer
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start notepad.exe to open instace log file:" + Environment.NewLine + ex);
            }
        }

        private void SetContextMenuState()
        {
            if (database == null)
            {
                mReconnect.Enabled = false;
                mDisconnect.Enabled = false;
            }
            else
            {
                mReconnect.Enabled = !database.IsConnected;
                mDisconnect.Enabled = database.IsConnected;
                mOpenInYahoo.Enabled = !string.IsNullOrEmpty(currentTicker);
            }
        }

        #endregion
    }
}
