using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ScenariosConfiguration {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        public static App Instance;

        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info("        =============  ScenariosConfiguration Logging  =============        ");
            base.OnStartup(e);
        }

        public App()
        {
            Instance = this;
        }

        public void SwitchLanguage(string locale)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;

            Instance.MainWindow.Hide();
            
            Instance.MainWindow = new ScenariosMainWindow();
            Instance.MainWindow.Show();
        }
    }
}
