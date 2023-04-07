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

namespace ScenariosConfiguration {
    /// <summary>
    /// Interaction logic for ItemEditDialog.xaml
    /// </summary>
    public partial class ItemEditDialog : Window
    {
        public Stage stageTemp = new Stage();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public bool shouldSave = false;

        public ItemEditDialog(Stage stage)
        {
            CloneStage(stage);

            InitializeComponent();

            DialogDescriptionTxtBox.Text = stageTemp.Description;
            DialogNameTxtBox.Text = stageTemp.Name;

            if(stageTemp.Values != null)
            {
                DialogTemperatureTxtBox.Text = stageTemp.Values[0].ToString();
                DialogVentilationTxtBox.Text = stageTemp.Values[1].ToString();
                if (stageTemp.Values.Length > 2)
                {
                    DialogHumidityTxtBox.Text = stageTemp.Values[2].ToString();
                }
                else{
                    //DialogHumidityTxtBox.IsEnabled = false;
                }
                DialogDurationTxtBox.Text = stageTemp.Duration.ToString();
            }
            
        }

        private void CloneStage(Stage stage)
        {
            stageTemp.Duration = stage.Duration;
            stageTemp.Id = stage.Id;
            stageTemp.Name = stage.Name;
            stageTemp.Description = stage.Description;

            List<string> values = new List<string>(stage.Values.ToList());
            stageTemp.Values = values.ToArray();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            DialogNameTxtBox.SelectAll();
            DialogNameTxtBox.Focus();
        }

        private void DialogSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            shouldSave = true;
            Close();
            //MessageBox.Show("Estágio guardado!");
        }

        private void DialogCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DialogDurationTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(DialogDurationTxtBox.Text != null && DialogDurationTxtBox.Text.Length > 0)
            {
                stageTemp.Duration = long.Parse(DialogDurationTxtBox.Text.Trim());
            }
        }

        private void DialogHumidityTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(stageTemp.Values.Length <= 2)
            {
                var newArray = new string[stageTemp.Values.Length + 1];
                stageTemp.Values.CopyTo( newArray, 0 );
                stageTemp.Values = newArray;
            }
            stageTemp.Values[2] = DialogHumidityTxtBox.Text.Trim();
        }

        private void DialogVentilationTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            stageTemp.Values[1] = DialogVentilationTxtBox.Text.Trim();
        }

        private void DialogTemperatureTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            stageTemp.Values[0] = DialogTemperatureTxtBox.Text.Trim();
        }

        private void DialogDescriptionTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            stageTemp.Description = DialogDescriptionTxtBox.Text.Trim();
        }

        private void DialogNameTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            stageTemp.Name = DialogNameTxtBox.Text.Trim();
        }
    }
}
