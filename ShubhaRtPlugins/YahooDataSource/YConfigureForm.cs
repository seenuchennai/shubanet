using System.Windows.Forms;
using AmiBroker.Data;

namespace AmiBroker.Samples.YahooDataSource
{
    public partial class YConfigureForm : Form
    {
        private InfoSite infoSite;

        // constructor
        internal YConfigureForm(YConfiguration oldSettings, ref InfoSite infoSite)
        {
            this.infoSite = infoSite;

            InitializeComponent();

            if (oldSettings == null)
                oldSettings = YConfiguration.GetDefaultConfigObject();

            // read and set values in controlls
            numericUpDownRefreshInterval.Value = oldSettings.RefreshPeriod;
        }

        // build config string from the dialog data
        internal YConfiguration GetNewSettings()
        {
            YConfiguration newSettings = new YConfiguration();

            newSettings.RefreshPeriod = (int)numericUpDownRefreshInterval.Value;

            return newSettings;
        }

        private void buttonCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
