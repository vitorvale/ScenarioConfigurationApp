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

namespace ScenariosConfiguration {
    /// <summary>
    /// Interaction logic for ItemEditDialog.xaml
    /// </summary>
    public partial class ItemEditDialog : Window
    {
        public ItemEditDialog(string title, string label, string value)
        {
            InitializeComponent();
            this.Title = title;
            DialogNameLbl.Content = label;
            DialogNameTxtBox.Text = value;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            DialogNameTxtBox.SelectAll();
            DialogNameTxtBox.Focus();
        }

        public string Value
        {
            get { return DialogNameTxtBox.Text; }
        }

        private void DialogSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void DialogCancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DialogDurationTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DialogHumidityTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DialogVentilationTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DialogTemperatureTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DialogDescriptionTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DialogNameTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
