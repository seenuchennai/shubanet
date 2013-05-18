using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Threading;
using log4net;
using log4net.Config;
namespace StockD
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string url1 = "http://www.goog";
        public MainWindow()
        {
            
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
           // Lbl_internet.Content = DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
            url1 = "http://www.google.com";
            Check_internet_connetion(url1);

        }
        private void Check_internet_connetion(string url)
        {
            //Check Internet Connection Is Present Or Not
            DispatcherTimer DispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();

            try
            {
                System.Net.WebRequest myRequest = System.Net.WebRequest.Create(url);
                System.Net.WebResponse myResponse = myRequest.GetResponse();
                Net_Connection.Fill = new SolidColorBrush(Colors.Green);
                //Connection is ok time stop
                DispatcherTimer1.Stop();
            }
            catch (System.Net.WebException)
            {
                Net_Connection.Fill = new SolidColorBrush(Colors.Red);
                DispatcherTimer1.Tick += new EventHandler(dispatcherTimer_Tick);
                DispatcherTimer1.Interval = new TimeSpan(0, 0, 10);
                DispatcherTimer1.Start();
            }
        }

        private void wMain_Loaded(object sender, RoutedEventArgs e)
        {
            Check_internet_connetion(url1);
        }

        private void wMain_Closed(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(typeof(MainWindow));
            log.Debug("Application Close ");
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

       

     }
}
