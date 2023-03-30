using System;
using System.Collections.Generic;
using System.Linq;
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
        public ItemEditConfigDialog()
        {
            InitializeComponent();

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ValueTxtBox.SelectAll();
            ValueTxtBox.Focus();
        }

        public string Value
        {
            get { return ValueTxtBox.Text; }
        }

        private void DialogPingFrequencyTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DialogUpdateFrequencyTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DialogCloseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DialogSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void ScenariosFileTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
