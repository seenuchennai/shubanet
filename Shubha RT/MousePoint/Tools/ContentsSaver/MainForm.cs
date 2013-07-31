using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagedWinapi.Windows;
using ManagedWinapi.Windows.Contents;

namespace ShubhaRt
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void crossHair_CrosshairDragging(object sender, EventArgs e)
        {
            
        }

        private void crossHair_CrosshairDragged(object sender, EventArgs e)
        {
            try
            {
                string point = MousePosition.X.ToString() + "," + MousePosition.Y.ToString();
               if(!System.IO.Directory.Exists("C:\\data"))
               {
                   System.IO.Directory.CreateDirectory("C:\\data");
               }
                
                if (!System.IO.File.Exists("C:\\data\\Mousepoint.txt"))
                {
                    System.IO.File.Create("C:\\data\\Mousepoint.txt");
                }
                // System.IO.File.WriteAllText("C:\\data\\Mousepoint.txt", point);
                using (var writer = new System.IO.StreamWriter("C:\\data\\Mousepoint.txt"))
                    writer.WriteLine(point);

                Environment.Exit(0);
            }
            catch
            {
                Environment.Exit(0);

            }
            //System.Windows.Forms.MessageBox.Show(point);

        }

       
        private void MainForm_Load(object sender, EventArgs e)
        {
           
        }

        
    }
}