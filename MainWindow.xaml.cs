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
using Microsoft.VisualBasic.Logging;
using log4net.Core;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using ScenariosConfiguration.Models;

namespace ScenariosConfiguration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ScenariosMainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string scenarioManagerDirectory = "C:\\Users\\vitor\\Documents\\GitHub\\scenariomanager\\ScenarioManagerApp\\";
        private const string appSettingsFile = scenarioManagerDirectory + "appsettings.json";
        private const string defaultConfigFile = "scenariosConfiguration.json";
        private const string scenariosFile = "scenarios.json";
        private static List<Scenario> scenarios = new List<Scenario>();
        private static List<Scenario_Config> scenarios_config = new List<Scenario_Config>();
        private static string basePath = Directory.GetCurrentDirectory();
        private static AppSettings appSettings = new AppSettings();

        public ScenariosMainWindow()
        {
            _ = log4net.Config.XmlConfigurator.Configure();

            // read configuration from file, if it exists
            var configuration = new ConfigurationBuilder().Build();
            if (configuration == null)
            {
                log.Error("Unable to read configuration file - loading default");
            }

            if (File.Exists(appSettingsFile))
            {
                try
                {
                    using (StreamReader r = new StreamReader(appSettingsFile))
                    {
                        string json = r.ReadToEnd();
                        appSettings = JsonConvert.DeserializeObject<AppSettings>(json);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.ToString());
                    log.Error("Unable to read appSettings file!");
                }
            }

            scenarios = GetConfiguration(scenariosFile);
            if (scenarios == null)
            {
                log.Error("Didn't find any scenarios!");
            }
            else
            {
                log.InfoFormat("Loaded {0} Scenarios from configuration", scenarios.Count);
            }

            scenarios_config = GetScenariosConfiguration(defaultConfigFile);
            if (scenarios == null)
            {
                log.Error("Didn't find any scenarios config!");
            }
            else
            {
                ScenarioPathTxtBox.Text = basePath + defaultConfigFile;
                log.InfoFormat("Loaded {0} Scenarios from configuration", scenarios.Count);
            }


            var ci = new CultureInfo("pt-PT");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            InitializeComponent();

            ScenariosListBox.ItemsSource = ToListScenarios();
        }

        private List<string> ToListScenarios()
        {
            var list = new List<string>();

            foreach (var scenario in scenarios_config)
            {
                list.Add(scenario.Name);
            }

            return list;
        }

        private static List<Scenario> GetConfiguration(string configurationFile)
        {
            var scenarios = new List<Scenario>();

            // read configuration from file, if it exists
            if (File.Exists(configurationFile))
            {
                try
                {
                    using (StreamReader r = new StreamReader(configurationFile))
                    {
                        string json = r.ReadToEnd();
                        scenarios = JsonConvert.DeserializeObject<List<Scenario>>(json);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.ToString());
                    log.Error("Unable to read scenarios configuration file");
                }
            }

            return scenarios;
        }

        private static List<Scenario_Config> GetScenariosConfiguration(string configurationFile)
        {
            var scenarios = new List<Scenario_Config>();

            // read configuration from file, if it exists
            if (File.Exists(configurationFile))
            {
                try
                {
                    using (StreamReader r = new StreamReader(configurationFile))
                    {
                        string json = r.ReadToEnd();
                        scenarios = JsonConvert.DeserializeObject<List<Scenario_Config>>(json);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.ToString());
                    log.Error("Unable to read scenarios configuration file");
                }
            }

            return scenarios;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseApplication();
        }

        private void PersistConfiguration()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(appSettingsFile))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, scenarios_config);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void CloseApplication()
        {
            PersistConfiguration();

            System.Windows.Application.Current.Shutdown();
        }

        private void RemoveScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            // validate if any scenario has been selected
            if (ScenariosListBox.SelectedIndex < 0) return;

            if (scenarios_config.Remove(scenarios_config[ScenariosListBox.SelectedIndex]))
            {
                log.InfoFormat("Scenario index {0} successfully removed!", ScenariosListBox.SelectedIndex);
            }
            else
            {
                log.InfoFormat("Scenario index {0} does not exist!", ScenariosListBox.SelectedIndex);
            }
        }

        private void EditScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            // validate if any scenario has been selected
            if (ScenariosListBox.SelectedIndex < 0) return;

            ScenarioEditWindow scenarioEditWindow = new ScenarioEditWindow(scenarios_config[ScenariosListBox.SelectedIndex]);

            if (scenarioEditWindow.ShowDialog() == true)
            {
                // replace the selected item with the new value
                scenarios_config[ScenariosListBox.SelectedIndex] = scenarioEditWindow.scenario_temp;
                ScenariosListBox.ItemsSource = scenarios_config;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddScenarioBtn_Click(object sender, RoutedEventArgs e)
        {
            // validate if any scenario file is loaded

            Scenario_Config new_scenario = new Scenario_Config();
            var new_index = scenarios_config.Count + 1;
            new_scenario.Name = "scenario " + new_index.ToString();
            new_scenario.Id = new_index;

            scenarios_config.Add(new_scenario);
            log.InfoFormat("Scenario {0} successfully added!", scenarios_config[scenarios_config.Count - 1]);
            
        }

        private void EditEngineConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            ItemEditConfigDialog itemEditConfigDialog = new ItemEditConfigDialog();
            if (itemEditConfigDialog.ShowDialog() == true)
            {
                // replace the new parameters in the appsettings.json file
                
            }
        }

        private void ScenariosListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ScenarioPathButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "SelectDestinationFolder",
                UseDescriptionForTitle = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + Path.DirectorySeparatorChar,
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ScenarioPathTxtBox.Text = dialog.SelectedPath;
                log.InfoFormat("Folder '{0}' was selected as destination", dialog.SelectedPath);
            }
        }

        private void ScenarioPathTxt_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            scenarios_config = GetScenariosConfiguration(defaultConfigFile);
            if (scenarios == null)
            {
                log.Error("Didn't find any scenarios config!");
            }
            else
            {
                log.InfoFormat("Loaded {0} Scenarios from configuration", scenarios.Count);
            }
        }

        private void NewScenarioFileButton_Click(object sender, RoutedEventArgs e)
        {
            scenarios_config = new List<Scenario_Config>();
        }
    }
}
