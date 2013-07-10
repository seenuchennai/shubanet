﻿using System;
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

using System.IO;
using System.Collections;
using System.Threading;

using Microsoft.Win32;

namespace ShubhaRt
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        System.Windows.Threading.DispatcherTimer DispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();

        public Login()
        {
            InitializeComponent();
        }

        private void frame1_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //Login Page








        }

        private void RtdataRecall()
        {
            DispatcherTimer1.Tick += new EventHandler(dispatcherTimerForRT_Tick);
            DispatcherTimer1.Interval = new TimeSpan(0, 0, 5);
            DispatcherTimer1.Start();

        }
        private void dispatcherTimerForRT_Tick(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            validate();
            RtdataRecall();

        }

        private void Login_btn_Click(object sender, RoutedEventArgs e)
        {

            Uri a = new Uri("http://shubhalabha.in/community/wp-login.php");
            Uri a1 = new Uri("http://shubhalabha.in/community/wp-admin/profile.php");


            if (LoginAunthenticate.Source == a)
            {
                System.Windows.MessageBox.Show("Please Valicated Acc First ");

            }
            else if (LoginAunthenticate.Source == a1)
            {
                System.Windows.MessageBox.Show("Valid USer ");

            }
            else
            {
                LoginAunthenticate.Source = a;
                System.Windows.MessageBox.Show("Please Valicated Acc First ");

            }



        }

        public void SetRegKey()
        {

            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.CreateSubKey(@"Software\");
            regKey.SetValue("ApplicationID", "1");

        }
        public void validate()
        {
            Uri a = new Uri("http://shubhalabha.in/community/wp-login.php");
            Uri a1 = new Uri("http://shubhalabha.in/community/");
            Uri a2 = new Uri("http://shubhalabha.in/community/wp-admin/profile.php");


            if (LoginAunthenticate.Source == a1)
            {
                LoginAunthenticate.Source = a;
                System.Windows.MessageBox.Show("Please Valicated Acc First ");
                RtdataRecall();
            }
            if (LoginAunthenticate.Source == a2)
            {
                System.Windows.MessageBox.Show("Valid USer ");
                SetRegKey();
                this.Hide();
                StockD.MainWindow newwin = new StockD.MainWindow();
                newwin.InitializeComponent();

                newwin.ShowDialog();
                   
                   


            }


        }




        private void Register_btn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://shubhalabha.in/community/wp-login.php?action=register");

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.CreateSubKey(@"Software\");
            object unm = regKey.GetValue("ApplicationID");

            string a = "1";

            if (unm != null)
            {   //username .Text = regKey.GetValue("UserName").ToString();

                string b = unm.ToString();

                if (b != a)
                {


                }
                else
                {

                    this.Hide();
                    StockD.MainWindow newwin = new StockD.MainWindow();
                    newwin.InitializeComponent();

                    newwin.ShowDialog();



                    // System.Diagnostics.Process.Start(@"C:\Documents and Settings\maheshwar\My Documents\GitHub\shubanet\Shubha RT\bin\Debug\ShubhaRt.exe");

                }
            }
            validate();
            RtdataRecall();

        }
    }
}
