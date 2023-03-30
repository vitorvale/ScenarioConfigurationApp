
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Globalization;
using Path = System.IO.Path;
using log4net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MessageBox = System.Windows.MessageBox;
using Newtonsoft.Json;
using System.Collections;
using System.Threading;
using SystemFonts = System.Windows.SystemFonts;
using System.Windows.Media;
using ScenariosConfiguration.Models;

namespace ScenariosConfiguration
{
    /// <summary>
    /// Interaction logic for ScenarioEditWindow.xaml
    /// </summary>
    public partial class ScenarioEditWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Scenario_Config scenario_temp = new Scenario_Config();


        public ScenarioEditWindow(Scenario_Config scenario)
        {
            scenario_temp = scenario;
            _ = log4net.Config.XmlConfigurator.Configure();   

            InitializeComponent();

        }      

        private void ScenarioEditWindow_Closed(object sender, EventArgs e)
        {
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ScenarioNameTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void ScenarioDescriptionTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditDatapointsButton_Click(object sender, System.Windows.Input.TouchEventArgs e)
        {

        }

        private void TimerIdTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void StageIdTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void StatusIdTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void EngineStateIdTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveRecipeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditRecipeButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void RecipeesBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ActiveRecipeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
