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
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ScenariosConfiguration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ScenariosMainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Scenarios Engine
        private const string scenarioManagerDirectory = "C:\\Users\\Vitor\\OneDrive\\Ambiente de Trabalho\\ScenarioManagerApp";
        private const string appSettingsFile = scenarioManagerDirectory + "\\" + "appsettings.json";
        private const string scenariosFile = scenarioManagerDirectory + "\\" + "scenarios.json";

        // Scenarios Configuration App
        private string defaultConfigFile = "scenariosConfiguration.json";
        private string defaultUsersFile = "users.json";

        private static List<Scenario> scenarios = new List<Scenario>();
        private static List<ScenarioConfig> scenariosConfig = new List<ScenarioConfig>();
        private List<User> users = new List<User>();
        private static string basePath = Directory.GetCurrentDirectory();
        private AppSettings appSettings = new AppSettings();
        private string scenariosConfigFileName = null;
        private JObject appSettingsJsonObject = null;
        private User loggedUser = new User();

        public ScenariosMainWindow()
        {
            //_ = log4net.Config.XmlConfigurator.Configure();

            // read configuration from file, if it exists
            var configuration = new ConfigurationBuilder().Build();
            if (configuration == null)
            {
                log.Error("Unable to read configuration file - loading default");
            }

            var ci = new CultureInfo("pt-PT");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            users = GetUsers(defaultUsersFile);
            if (users == null)
            {
                log.Error("Didn't find any users!");
                CloseApplication();
            }
            else
            {
                log.InfoFormat("Loaded {0} users from file", users.Count);
            }

            LoginWindow loginWindow = new LoginWindow(users);
            if (loginWindow.ShowDialog() == false)
            {
                loggedUser.Password = loginWindow.password;
                loggedUser.Name = loginWindow.username;
                loggedUser.Id = loginWindow.loggedUserId;
                log.Info("Login Successful");
            }

            InitializeComponent();

            if (File.Exists(appSettingsFile))
            {
                log.Info("Appsettings file exists!");
                try
                {
                    string jsonString = File.ReadAllText(appSettingsFile);

                    appSettingsJsonObject = JObject.Parse(jsonString);

                    // Check if a property exists
                    if (appSettingsJsonObject["UpdateFrequency"] != null)
                    {
                        appSettings.UpdateFrequency = (int)appSettingsJsonObject["UpdateFrequency"];
                    }

                    if (appSettingsJsonObject["Watchdog"] != null)
                    {
                        var watchdog = appSettingsJsonObject["Watchdog"];
                        JToken token = appSettingsJsonObject["Watchdog"]["UpdateTime"];
                        if (token.Type == JTokenType.Integer)
                        {
                            appSettings.WatchDog.UpdateTime = (int)token;
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.ToString());
                    log.Error("Unable to read appSettings file!");
                }
            }
            else
            {
                log.Error("Appsettings file does not exist!");
            }

            scenarios = GetScenarios(scenariosFile);
            if (scenarios == null)
            {
                log.Error("Didn't find any scenarios!");
            }
            else
            {
                log.InfoFormat("Loaded {0} Scenarios from engine", scenarios.Count);
            }

            scenariosConfig = GetScenariosConfiguration(defaultConfigFile);
            if (scenariosConfig == null)
            {
                log.Error("Didn't find any scenarios config!");
            }
            else
            {
                ScenarioPathTxtBox.Text = defaultConfigFile;
                log.InfoFormat("Loaded {0} Scenarios from configuration", scenariosConfig.Count);
            }

            ScenariosListBox.ItemsSource = ToListScenarios();
        }

        private List<string> ToListScenarios()
        {
            var list = new List<string>();

            foreach (var scenario in scenariosConfig)
            {
                list.Add(scenario.Name);
            }

            return list;
        }

        private static List<Scenario> GetScenarios(string configurationFile)
        {
            var scenarios = new List<Scenario>();

            // read configuration from file, if it exists
            if (File.Exists(configurationFile))
            {
                log.Info("Configuration file exists!");
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
            else
            {
                log.Error("Configuration file does not exist!");
            }

            return scenarios;
        }

        private static List<User> GetUsers(string UsersFile)
        {
            var users = new List<User>();

            // read configuration from file, if it exists
            if (File.Exists(UsersFile))
            {
                log.Info("Configuration file exists!");
                try
                {
                    using (StreamReader r = new StreamReader(UsersFile))
                    {
                        string json = r.ReadToEnd();
                        users = JsonConvert.DeserializeObject<List<User>>(json);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.ToString());
                    log.Error("Unable to read users from file");
                }
            }
            else
            {
                log.Error("Users file does not exist!");
            }

            return users;
        }

        private static List<ScenarioConfig> GetScenariosConfiguration(string configurationFile)
        {
            var scenariosConfig = new List<ScenarioConfig>();

            // read configuration from file, if it exists
            if (File.Exists(configurationFile))
            {
                log.Info("Scenarios configuration file exists!");
                try
                {
                    using (StreamReader r = new StreamReader(configurationFile))
                    {
                        string json = r.ReadToEnd();
                        scenariosConfig = JsonConvert.DeserializeObject<List<ScenarioConfig>>(json);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.ToString());
                    log.Error("Unable to read scenarios configuration file");
                }
            }
            else
            {
                log.Info("Scenarios configuration file does not exist! Will create default one.");
                scenariosConfig = new List<ScenarioConfig> { };
                foreach (var scenario in scenarios)
                {
                    var recipees = new List<Recipe>();
                    recipees.Add(new Recipe
                    {
                        Id = 1,
                        Name = "Recipe 1",
                        Description = "Default recipe",
                        Stages = scenario.Stages
                    });

                    scenariosConfig.Add(
                        new ScenarioConfig()
                        {
                            Id = scenario.Id,
                            Name = scenario.Name,
                            Description = scenario.Description,
                            StatusDatapointId = scenario.StatusDatapointId,
                            EngineStateDatapointId = scenario.EngineStateDatapointId,
                            TimerDatapointId = scenario.TimerDatapointId,
                            CurrentStageDatapointId = scenario.CurrentStageDatapointId,
                            FinalStatus = scenario.FinalStatus,
                            Datapoints = scenario.Datapoints,
                            Recipees = recipees.ToArray(),
                        }
                    );
                }

            }

            return scenariosConfig;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseApplication();
        }

        private void PersistConfiguration(string filePath)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, scenariosConfig);
            }
        }

        private void SaveConfigToEngine()
        {
            //save both the scenarios and appSettings to the respective json files so engine can execute

            for (int idx = 0; idx < scenariosConfig.Count; idx++)
            {
                scenarios[idx].Name = scenariosConfig[idx].Name;
                scenarios[idx].Description = scenariosConfig[idx].Description;
                scenarios[idx].Stages = scenariosConfig[idx].Recipees[scenariosConfig[idx].ActiveRecipe].Stages;
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(scenariosFile))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, scenarios);
            }

            appSettingsJsonObject["UpdateFrequency"] = appSettings.UpdateFrequency;
            appSettingsJsonObject["Watchdog"]["UpdateTime"] = appSettings.WatchDog.UpdateTime;

            using (StreamWriter sw = new StreamWriter(appSettingsFile))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, appSettingsJsonObject);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void CloseApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void EditScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            // validate if any scenario has been selected
            if (ScenariosListBox.SelectedIndex < 0) return;

            ScenarioEditWindow scenarioEditWindow = new ScenarioEditWindow(scenariosConfig[ScenariosListBox.SelectedIndex]);

            if (scenarioEditWindow.ShowDialog() == false && scenarioEditWindow.shouldSave == true)
            {
                // replace the selected item with the new value
                scenariosConfig[ScenariosListBox.SelectedIndex] = scenarioEditWindow.scenario_temp;
                ScenariosListBox.ItemsSource = ToListScenarios();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            PersistConfiguration(defaultConfigFile);
            SaveConfigToEngine();
            MessageBox.Show("Configuração guardada!");
        }

        private void EditEngineConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            ItemEditConfigDialog itemEditConfigDialog = new ItemEditConfigDialog(appSettings);
            if (itemEditConfigDialog.ShowDialog() == false && itemEditConfigDialog.shouldSave == true)
            {
                appSettings = itemEditConfigDialog.appSettings_temp;
            }
        }

        private void ScenariosListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ScenarioPathButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: instead of getting a folder it should be getting a .json file
            var dialog = new OpenFileDialog() {
                Filter = "json files (*.json) | *.json;",
                RestoreDirectory = true,
                Title = "Selecione o ficheiro de configuração",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ScenarioPathTxtBox.Text = dialog.FileName;
                log.InfoFormat("Configuration File '{0}' was selected!", dialog.FileName);
                defaultConfigFile = dialog.FileName;
                scenariosConfig = GetScenariosConfiguration(defaultConfigFile);
                if (scenarios == null)
                {
                    log.Error("Didn't find any scenarios config!");
                }
                else
                {
                    log.InfoFormat("Loaded {0} Scenarios from configuration", scenarios.Count);
                    ScenarioPathTxtBox.Text = defaultConfigFile;
                    MessageBox.Show("Nova configuração carregada!");
                }
            }
        }

        private void ScenarioPathTxt_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            /*scenarios_config = GetScenariosConfiguration(defaultConfigFile);
            if (scenarios == null)
            {
                log.Error("Didn't find any scenarios config!");
            }
            else
            {
                log.InfoFormat("Loaded {0} Scenarios from configuration", scenarios.Count);
            }*/
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                Filter = "json files (*.json) | *.json;",
                RestoreDirectory = true,
                Title = "Guardar o ficheiro de configuração",
                CheckPathExists = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                log.InfoFormat("Configuration File '{0}' was saved!", dialog.FileName);
                PersistConfiguration(dialog.FileName);
                SaveConfigToEngine();
            }
        }
    }
}
