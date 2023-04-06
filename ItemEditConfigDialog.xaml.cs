using log4net;
using ScenariosConfiguration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScenariosConfiguration
{
    /// <summary>
    /// Interaction logic for ItemEditConfigDialog.xaml
    /// </summary>
    public partial class ItemEditConfigDialog : Window
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public AppSettings appSettings_temp = new AppSettings();
        public string scenariosFileName_temp = null;
        public bool shouldSave = false;

        public ItemEditConfigDialog(AppSettings appSettings)
        {
            appSettings_temp.UpdateFrequency = appSettings.UpdateFrequency;
            appSettings_temp.WatchDog = new WatchDog();
            appSettings_temp.WatchDog.UpdateTime = appSettings.WatchDog.UpdateTime;
            appSettings_temp.WatchDog.Id = appSettings.WatchDog.Id;


            InitializeComponent();

            DialogPingFrequencyTxtBox.Text = appSettings_temp.WatchDog.UpdateTime.ToString();
            DialogUpdateFrequencyTxtBox.Text = appSettings_temp.UpdateFrequency.ToString();

        }

        private void DialogPingFrequencyTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DialogPingFrequencyTxtBox.Text != null & DialogPingFrequencyTxtBox.Text.Length > 0)
            {
                appSettings_temp.WatchDog.UpdateTime = int.Parse(DialogPingFrequencyTxtBox.Text);
            }
        }

        private void DialogUpdateFrequencyTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DialogUpdateFrequencyTxtBox.Text != null && DialogUpdateFrequencyTxtBox.Text.Length > 0)
            {
                appSettings_temp.UpdateFrequency = int.Parse(DialogUpdateFrequencyTxtBox.Text);
            }
        }

        private void DialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DialogSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            shouldSave = true;
            Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

        }
    }
}
