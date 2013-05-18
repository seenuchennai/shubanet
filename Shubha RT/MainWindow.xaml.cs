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
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
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
            BinaryFormatter bf = new BinaryFormatter();
            //Use For Serialization Data Get Save In Fileio.txt file 


            if (File.Exists(@"C:\Fileio.txt"))
            {
                FileStream fs = new FileStream(@"C:\Fileio.txt", FileMode.Open, FileAccess.Read);
                target1 t1 = (target1)bf.Deserialize(fs);
                txtTargetFolder.Text = t1.target;
                dtStartDate.Text = t1.fromdate.ToShortDateString();
                dtEndDate.Text = t1.todate.ToShortDateString();
                fs.Close();



            }
            else
            {
                dtStartDate.Text = DateTime.Today.Date.ToString();
                dtEndDate.Text = DateTime.Today.Date.ToString();
                textBox1.Text = "";
            }
           
            Check_internet_connetion(url1);
        }

        private void wMain_Closed(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(typeof(MainWindow));
            log.Debug("Application Close ");
            savechanges();
           
        }
        private void savechanges()
        {
            if (dtStartDate.Text.ToString() == "")
            {

            }
            else
            {
                target1 t = new target1();
                t.fromdate = Convert.ToDateTime(dtStartDate.Text);
                t.todate = Convert.ToDateTime(dtEndDate.Text);
                t.target = txtTargetFolder.Text;

                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(@"C:\Fileio.txt", FileMode.Create, FileAccess.Write);
                bf.Serialize(fs, t);

                fs.Close();
            }
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
           
           
        }

        private void tabItem2_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            savechanges();
        }

       

     }
    [Serializable]
    public class target1
    {
        public string target;
        public DateTime fromdate;
        public DateTime todate;
        public string checkboxevent;
    }
}
