
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
        public ScenarioConfig scenario_temp = new ScenarioConfig();
        public List<string> recipe_names = new List<string>();
        public bool shouldSave = false;


        public ScenarioEditWindow(ScenarioConfig scenario)
        {
            CloneScenario(scenario);

            InitializeComponent();

            RecipeesListBox.ItemsSource = ToListRecipees();
            ScenarioDescriptionTxtBox.Text = scenario_temp.Description;
            ScenarioNameTxtBox.Text = scenario_temp.Name;
            recipe_names = ToListRecipeNames();
            ActiveRecipeComboBox.ItemsSource = recipe_names;
            ActiveRecipeComboBox.SelectedIndex = scenario_temp.ActiveRecipe;
        }

        private void CloneScenario(ScenarioConfig scenario)
        {
            List<Recipe> recipees = new List<Recipe>();
            List<Datapoint> datapoints = new List<Datapoint>();

            scenario_temp.Id = scenario.Id;
            scenario_temp.Name = scenario.Name;
            scenario_temp.Description = scenario.Description;
            scenario_temp.FinalStatus = scenario.FinalStatus;
            scenario_temp.ActiveRecipe = scenario.ActiveRecipe;
            scenario_temp.StatusDatapointId = scenario.StatusDatapointId;
            scenario_temp.EngineStateDatapointId = scenario.EngineStateDatapointId;
            scenario_temp.TimerDatapointId = scenario.TimerDatapointId;
            scenario_temp.CurrentStageDatapointId = scenario.CurrentStageDatapointId;

            scenario_temp.Recipees = recipees.ToArray();
            for (int idx = 0; idx < scenario.Recipees.Length; idx++)
            {
                List<Stage> stages = new List<Stage>();
                Recipe newRecipe = new Recipe();
                newRecipe.Id = scenario.Recipees[idx].Id;
                newRecipe.Name = scenario.Recipees[idx].Name;
                newRecipe.Description = scenario.Recipees[idx].Description;
                newRecipe.Stages = stages.ToArray();

                for (int kdx = 0; kdx < scenario.Recipees[idx].Stages.Length; kdx++)
                {
                    Stage newStage = new Stage();
                    newStage.Duration = scenario.Recipees[idx].Stages[kdx].Duration;
                    newStage.Id = scenario.Recipees[idx].Stages[kdx].Id;
                    newStage.Name = scenario.Recipees[idx].Stages[kdx].Name;
                    newStage.Description = scenario.Recipees[idx].Stages[kdx].Description;

                    List<string> values = new List<string>(scenario.Recipees[idx].Stages[kdx].Values.ToList());
                    newStage.Values = values.ToArray();
                    stages.Add(newStage);
                }
                newRecipe.Stages = stages.ToArray();
                recipees.Add(newRecipe);
            }
            scenario_temp.Recipees = recipees.ToArray();

            scenario_temp.Datapoints = datapoints.ToArray();
            for (int idx = 0; idx < scenario.Datapoints.Length; idx++)
            {
                Datapoint newDatapoint = new Datapoint();
                newDatapoint.Id = scenario.Datapoints[idx].Id;
                newDatapoint.SvcId = scenario.Datapoints[idx].SvcId;
                newDatapoint.Name = scenario.Datapoints[idx].Name;
                newDatapoint.Precision = scenario.Datapoints[idx].Precision;
                datapoints.Add(newDatapoint);
            }
            scenario_temp.Datapoints = datapoints.ToArray();
        }

        private List<string> ToListRecipees()
        {
            var list = new List<string>();

            foreach (var recipe in scenario_temp.Recipees)
            {
                list.Add(recipe.Name);
            }

            return list;
        }

        private List<string> ToListRecipeNames()
        {
            var list = new List<string>();

            foreach (var recipe in scenario_temp.Recipees)
            {
                list.Add(recipe.Name);
            }

            return list;
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
            scenario_temp.Name = ScenarioNameTxtBox.Text;
        }

        private void ScenarioDescriptionTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            scenario_temp.Description = ScenarioDescriptionTxtBox.Text;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            shouldSave = true;
            Close();
        }

        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            List<Recipe> recipees = new List<Recipe>();
            List<Stage> stages = new List<Stage>();

            recipees = scenario_temp.Recipees.ToList();

            Recipe newRecipe = new Recipe();

            var newIndex = 1;
            if (recipees != null && recipees.Count > 0)
            {
                var topRecipe = recipees.OrderByDescending(item => item.Id).ToList().First();
                newIndex = topRecipe.Id + 1;
            }

            newRecipe.Name = "Recipe " + newIndex.ToString();
            newRecipe.Id = newIndex;
            newRecipe.Stages = stages.ToArray();

            recipees.Add(newRecipe);
            scenario_temp.Recipees = recipees.ToArray();
            log.InfoFormat("Recipe {0} successfully added!", scenario_temp.Recipees[scenario_temp.Recipees.Length - 1].Name);

            RecipeesListBox.ItemsSource = ToListRecipees();
            recipe_names = ToListRecipeNames();
            ActiveRecipeComboBox.ItemsSource = recipe_names;
        }

        private void RemoveRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            List<Recipe> recipees = new List<Recipe>();
            // validate if any recipe has been selected
            if (RecipeesListBox.SelectedIndex < 0) return;

            recipees = scenario_temp.Recipees.ToList();

            if (recipees.Remove(recipees[RecipeesListBox.SelectedIndex]))
            {
                log.InfoFormat("Recipe index {0} successfully removed!", RecipeesListBox.SelectedIndex);
                scenario_temp.Recipees = recipees.ToArray();
                RecipeesListBox.ItemsSource = ToListRecipees();
                recipe_names = ToListRecipeNames();
                ActiveRecipeComboBox.ItemsSource = recipe_names;
            }
            else
            {
                log.InfoFormat("Recipe index {0} does not exist!", RecipeesListBox.SelectedIndex);
            }
        }

        private void EditRecipeButtonClick(object sender, RoutedEventArgs e)
        {
            // validate if any recipe has been selected
            if (RecipeesListBox.SelectedIndex < 0) return;

            RecipeEditWindow recipeEditWindow = new RecipeEditWindow(scenario_temp.Recipees[RecipeesListBox.SelectedIndex]);

            if (recipeEditWindow.ShowDialog() == false && recipeEditWindow.shouldSave == true)
            {
                // replace the selected item with the new value
                scenario_temp.Recipees[RecipeesListBox.SelectedIndex] = recipeEditWindow.recipeTemp;
                RecipeesListBox.ItemsSource = ToListRecipees();
                recipe_names = ToListRecipeNames();
                ActiveRecipeComboBox.ItemsSource = recipe_names;
            }
        }

        private void RecipeesBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ActiveRecipeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            scenario_temp.ActiveRecipe = ActiveRecipeComboBox.SelectedIndex;
        }
    }
}
