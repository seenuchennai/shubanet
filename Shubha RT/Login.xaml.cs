using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Web;
using System.IO;
using System.Net;
using Microsoft.Win32;
namespace ShubhaRt
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void frame1_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //Login Page
        }
        public void SetRegKey()
        {

            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.CreateSubKey(@"Software\");
            regKey.SetValue("ApplicationID", "0");

        }
        private void Loginbtn_Click(object sender, RoutedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();

            try
            {
                string loginUri = "http://shubhalabha.in/community/wp-login.php";

                string reqString = "log=" + username.Text + "&pwd=" + password.Password;
                byte[] requestData = Encoding.UTF8.GetBytes(reqString);

                CookieContainer cc = new CookieContainer();
                var request = (HttpWebRequest)WebRequest.Create(loginUri);
                request.Proxy = null;
                request.AllowAutoRedirect = false;
                request.CookieContainer = cc;
                request.Method = "post";

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = requestData.Length;
                using (Stream s = request.GetRequestStream())
                    s.Write(requestData, 0, requestData.Length);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    int count = 1;
                    foreach (Cookie c in response.Cookies)
                    {
                        //responce 2 contain loggen in or not 
                        if (count == 2)
                        {
                            if (c.ToString().Contains("wordpress_logged_in_17e90d9fdb1ef2a442ed2d6aeb707f54"))
                            {
                                System.Windows.MessageBox.Show("Login Successful");
                                try
                                {
                                    SetRegKey();
                                    this.Hide();
                                    StockD.MainWindow newwin = new StockD.MainWindow();
                                    CommandManager.InvalidateRequerySuggested();
                                    
                                    newwin.InitializeComponent();
                                    CommandManager.InvalidateRequerySuggested();

                                    newwin.ShowDialog();
                                    CommandManager.InvalidateRequerySuggested();

                                }
                                catch
                                {
                                    CommandManager.InvalidateRequerySuggested();

                                }
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Please Enter Valid UserName & Password ");

                            }
                        }
                        else
                        {
                            count++;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.CreateSubKey(@"Software\");
            object unm = regKey.GetValue("ApplicationID");

            string a = "0";

            if (unm != null)
            {   //username .Text = regKey.GetValue("UserName").ToString();

                string b = unm.ToString();

                if (b != a)
                {


                }
                else
                {
                    try
                    {
                        this.Hide();
                        StockD.MainWindow newwin = new StockD.MainWindow();
                        newwin.InitializeComponent();

                        newwin.ShowDialog();
                    }
                    catch
                    {
                    }


                    // System.Diagnostics.Process.Start(@"C:\Documents and Settings\maheshwar\My Documents\GitHub\shubanet\Shubha RT\bin\Debug\ShubhaRt.exe");

                }
            }
        }

        private void Regiser_btn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://shubhalabha.in/community/wp-login.php?action=register");
        }

        private void Cancle_btn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        
    }
}
