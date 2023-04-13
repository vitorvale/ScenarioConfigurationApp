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
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

namespace ScenariosConfiguration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ScenariosMainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Scenarios Engine
        private string scenarioManagerDirectory = string.Empty;
        private string engineAppSettingsFile = string.Empty;
        private string engineScenariosFile = string.Empty;

        // Scenarios Configuration App
        private string defaultConfigFile = "scenariosConfiguration.json";
        private string defaultAppSettingsFile = "appsettings.json";
        private string defaultDatFile = "scenarioConfigurationApp.dat";

        private static List<Scenario> scenarios = new List<Scenario>();
        private static List<ScenarioConfig> scenariosConfig = new List<ScenarioConfig>();
        private List<User> users = new List<User>();
        private static string basePath = Directory.GetCurrentDirectory();
        private AppSettings appSettings = new AppSettings();
        private string scenariosConfigFileName = null;
        private JObject appSettingsJsonObject = null;
        private JObject engineAppSettingsJsonObject = null;
        private User loggedUser = new User();
        private string usersDecryptedFile = string.Empty;
        private string passwordDecryptedFile = string.Empty;
        private static string defaultEncryptingPassword = "password123";

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

            if (File.Exists(defaultAppSettingsFile))
            {
                log.Info("Appsettings file exists!");
                try
                {
                    string jsonString = File.ReadAllText(defaultAppSettingsFile);

                    appSettingsJsonObject = JObject.Parse(jsonString);

                    if (appSettingsJsonObject["scenarioManagerDirectory"] != null)
                    {
                        scenarioManagerDirectory = (string)appSettingsJsonObject["scenarioManagerDirectory"];
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

            engineAppSettingsFile = scenarioManagerDirectory + "\\" + "appsettings.json";
            engineScenariosFile = scenarioManagerDirectory + "\\" + "scenarios.json";

            usersDecryptedFile = DecryptJsonFile(defaultDatFile);
            if (usersDecryptedFile != null)
            {
                users = GetUsers(usersDecryptedFile);
                log.InfoFormat("Loaded {0} users from file", users.Count);
            }
            else
            {
                log.Error("Users encrypted file does not exist, creating new one!");
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

            if (File.Exists(engineAppSettingsFile))
            {
                log.Info("Engine Appsettings file exists!");
                try
                {
                    string jsonString = File.ReadAllText(engineAppSettingsFile);

                    engineAppSettingsJsonObject = JObject.Parse(jsonString);

                    // Check if a property exists
                    if (engineAppSettingsJsonObject["UpdateFrequency"] != null)
                    {
                        appSettings.UpdateFrequency = (int)engineAppSettingsJsonObject["UpdateFrequency"];
                    }

                    if (engineAppSettingsJsonObject["Watchdog"] != null)
                    {
                        var watchdog = engineAppSettingsJsonObject["Watchdog"];
                        JToken token = engineAppSettingsJsonObject["Watchdog"]["UpdateTime"];
                        if (token.Type == JTokenType.Integer)
                        {
                            appSettings.WatchDog.UpdateTime = (int)token;
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.ToString());
                    log.Error("Unable to read Engine appSettings file!");
                }
            }
            else
            {
                log.Error("Engine Appsettings file does not exist!");
            }

            scenarios = GetScenarios(engineScenariosFile);
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
                log.Error("Engine configuration file does not exist!");
            }

            return scenarios;
        }

        private static List<User> GetUsers(string UsersFile)
        {
            var users = new List<User>();

            // read configuration from file, if it exists

            try
            {
                users = JsonConvert.DeserializeObject<List<User>>(UsersFile);
                log.Info("users file deserialized!");


                /*using (StreamReader r = new StreamReader(UsersFile))
                {
                    string json = r.ReadToEnd();
                    users = JsonConvert.DeserializeObject<List<User>>(json);
                }*/
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
                log.Error("Unable to read users from file");
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
                    var recipees = new List<Recipe>
                    {
                        new Recipe
                        {
                            Id = 1,
                            Name = "Recipe 1",
                            Description = "Default recipe",
                            Stages = scenario.Stages,
                        }
                    };

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
                            ActiveRecipe = 0,
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

            EncryptJsonFile(defaultDatFile);
        }

        private void SaveConfigToEngine()
        {
            //save both the scenarios and appSettings to the respective json files so engine can execute

            for (int idx = 0; idx < scenariosConfig.Count; idx++)
            {
                scenarios[idx].Name = scenariosConfig[idx].Name;
                scenarios[idx].Description = scenariosConfig[idx].Description;
                if(scenariosConfig[idx].Recipees.Count() > 0)
                {
                    scenarios[idx].Stages = scenariosConfig[idx].Recipees[scenariosConfig[idx].ActiveRecipe].Stages;
                }
                
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(engineScenariosFile))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, scenarios);
            }

            engineAppSettingsJsonObject["UpdateFrequency"] = appSettings.UpdateFrequency;
            engineAppSettingsJsonObject["Watchdog"]["UpdateTime"] = appSettings.WatchDog.UpdateTime;

            using (StreamWriter sw = new StreamWriter(engineAppSettingsFile))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, engineAppSettingsJsonObject);
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
            RestartScenarioManagerService();
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
                    log.InfoFormat("Loaded {0} Scenarios from configuration", scenariosConfig.Count);
                    ScenarioPathTxtBox.Text = defaultConfigFile;
                    ScenariosListBox.ItemsSource = ToListScenarios();
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
                RestartScenarioManagerService();
            }
        }

        private void EditPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(loggedUser);
            if (changePasswordWindow.ShowDialog() == false && changePasswordWindow.shouldSave == true)
            {
                loggedUser.Password = changePasswordWindow.newPassword;
                SaveUserPassword(loggedUser);
            }
        }


        private void SaveUserPassword(User loggedUser)
        {
            foreach(var user in users)
            {
                if (loggedUser.Id.Equals(user.Id) && loggedUser.Name.Equals(user.Name))
                {
                    user.Password = loggedUser.Password;
                }
            }
        }

        private string DecryptJsonFile(string encryptedFilePath)
        {

            if (File.Exists(encryptedFilePath))
            {
                log.Info("Users encrypted file exists!");

                byte[] encryptedData = File.ReadAllBytes(encryptedFilePath);

                byte[] salt = encryptedData.Take(16).ToArray();
                byte[] cipherData = encryptedData.Skip(16).ToArray();

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(defaultEncryptingPassword, salt, 100000);

                byte[] key = pdb.GetBytes(32);
                byte[] iv = pdb.GetBytes(16);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.IV = iv;

                    using (MemoryStream msDecrypt = new MemoryStream(cipherData))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            else
            {
                users = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "user1",
                        Password = "password123"
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "user2",
                        Password = "password456"
                    }
                };
                EncryptJsonFile(encryptedFilePath);
                return null;
            }
        }

        private void EncryptJsonFile(string encryptedFilePath)
        {
            string jsonString = JsonConvert.SerializeObject(users);

            byte[] jsonData = Encoding.UTF8.GetBytes(jsonString);
            byte[] salt = new byte[] {
            0x72, 0x69, 0x76, 0x65, 0x72, 0x72, 0x75, 0x6e,
            0x2c, 0x20, 0x70, 0x61, 0x73, 0x74, 0x20, 0x45 };

            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(defaultEncryptingPassword, salt, 100000);

            byte[] key = pdb.GetBytes(32);
            byte[] iv = pdb.GetBytes(16);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                using (FileStream fsEncrypt = new FileStream(encryptedFilePath, FileMode.Create))
                {
                    fsEncrypt.Write(salt, 0, salt.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(fsEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(jsonData, 0, jsonData.Length);
                    }
                }
            }
        }

        private void RestartScenarioManagerService()
        {
            string bat = basePath + "\\" + "StartScenarioManagerService.bat";
            var psi = new ProcessStartInfo();
            psi.CreateNoWindow = true; //This hides the dos-style black window that the command prompt usually shows
            psi.FileName = @"cmd.exe";
            psi.Verb = "runas"; //This is what actually runs the command as administrator
            psi.Arguments = "/C " + bat;
            psi.UseShellExecute = true;
            try
            {
                var process = new Process();
                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
                log.Info("Restarted ScenarioManager Service!");
            }
            catch (Exception e)
            {
                //If you are here the user clicked decline to grant admin privileges (or he's not administrator)
                log.Error(e.Message);
            }

            
        }
    }
}
