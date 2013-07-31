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
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Threading;
using System.IO;
using log4net;
using log4net.Config;
namespace RTDFINAL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow :System.Windows. Window
    {
        int flag = 0;
        int i=0;
        object y = "";
        List<string> yahoortname = new List<String>();
        List<string> yahoortdata = new List<String>();
        System.Windows.Threading.DispatcherTimer DispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
           CommandManager.InvalidateRequerySuggested();
            rtddata();
            RtdataRecall();

        }

        private void RtdataRecall()
        {           
                DispatcherTimer1.Tick += new EventHandler(dispatcherTimer_Tick);
                DispatcherTimer1.Interval = new TimeSpan(0, 0,5);
                DispatcherTimer1.Start();
            
        } 
     
        public void rtddata()
        {
            try
            {

            yahoortdata.Clear();

            using (var reader = new StreamReader("c:\\ShubhaRtsymbollist.txt"))
            {
                string line = null;
                int i = 0;

                while ((line = reader.ReadLine()) != null)
                {

                    yahoortname.Add(line);
                    Array retval;
                    MethodInfo method;
                    Type type = Type.GetTypeFromProgID("now.scriprtd");


                    IRtdServer m_server = (IRtdServer)Activator.CreateInstance(type);

                    int j = m_server.Heartbeat();
                    if (flag == 0)
                    {
                        bool bolGetNewValue = true;
                        object[] array = new object[3];





                        array[0] = "MktWatch";
                            array[1] = line;
                            array[2] = "Last Trade Time";

                            Array sysArrParams = (Array)array;
                            m_server.ConnectData(i, sysArrParams, bolGetNewValue);

                            i++;
                            object[] array1 = new object[3];





                            array1[0] = "MktWatch";
                            array1[1] = line;
                            array1[2] = "Best Buy Rate";

                            Array sysArrParams1 = (Array)array1;
                            m_server.ConnectData(i, sysArrParams1, bolGetNewValue);

                            i++;

                            object[] array2 = new object[3];





                            array2[0] = "MktWatch";
                            array2[1] = line;
                            array2[2] = "Best Buy Qty";

                            Array sysArrParams2 = (Array)array2;
                            m_server.ConnectData(i, sysArrParams2, bolGetNewValue);

                            retval = m_server.RefreshData(10);

                            i++;    //imp it change topic id 
                            foreach (var item in retval)
                            {
                                yahoortdata.Add(item.ToString());

                            }

                   }

                }
                string tempfilepath = "C:\\YahooRealTimeData.txt";
                log4net.Config.XmlConfigurator.Configure();
                ILog log = LogManager.GetLogger(typeof(MainWindow));
                log.Debug("Data Capturing At"+DateTime.Now.TimeOfDay);
                using (var writer = new StreamWriter(tempfilepath))
                    for (int c = 1; c <= yahoortdata.Count - 1; c = c + 2)
                    {
                        writer.WriteLine(yahoortdata[c].ToString());
                    }

            }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message );

            }
       }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            RtdataRecall();
                   
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer1.Stop();
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(typeof(MainWindow));
            log.Debug("Data Capturing Stop... ");
           
        }
    }
}
