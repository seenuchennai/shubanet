using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
namespace StockD
{
    class StartUp
    {

        [STAThread]
        static void Main()
        {
            App app = new App();
            StockDViewModel sm = new StockDViewModel();

            app.InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
              ILog log = LogManager.GetLogger(typeof(MainWindow ));
              log.Debug("Application Strated Successfully");
            
            app.Run();
  
        }
    }
}
