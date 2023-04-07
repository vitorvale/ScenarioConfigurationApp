
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
    public partial class RecipeEditWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Recipe recipeTemp = new Recipe();
        public bool shouldSave = false;


        public RecipeEditWindow(Recipe recipe)
        {
            CloneRecipe(recipe);

            InitializeComponent();

            StagesListBox.ItemsSource = ToListStages();
            RecipeDescriptionTxtBox.Text = recipeTemp.Description;
            RecipeNameTxtBox.Text = recipeTemp.Name;

        }

        private void CloneRecipe(Recipe recipe)
        {
            List<Stage> stages = new List<Stage>();
            recipeTemp.Id = recipe.Id;
            recipeTemp.Name = recipe.Name;
            recipeTemp.Description = recipe.Description;
            recipeTemp.Stages = stages.ToArray();

            for (int idx = 0; idx < recipe.Stages.Length; idx++)
            {
                Stage newStage = new Stage();
                newStage.Duration = recipe.Stages[idx].Duration;
                newStage.Id = recipe.Stages[idx].Id;
                newStage.Name = recipe.Stages[idx].Name;
                newStage.Description = recipe.Stages[idx].Description;

                List<string> values = new List<string>(recipe.Stages[idx].Values.ToList());
                newStage.Values = values.ToArray();
                stages.Add(newStage);
            }
            recipeTemp.Stages = stages.ToArray();
        }

        private List<string> ToListStages()
        {
            var list = new List<string>();

            if (recipeTemp.Stages != null)
            {
                foreach (var stage in recipeTemp.Stages)
                {
                    list.Add(stage.Name);
                }
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

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            shouldSave = true;
            Close();
            //MessageBox.Show("Receita guardada!");
        }

        private void AddStageButton_Click(object sender, RoutedEventArgs e)
        {
            List<Stage> stages = new List<Stage>();
            
            if (recipeTemp.Stages != null)
            {
                stages = recipeTemp.Stages.ToList();
            }

            Stage newStage = new Stage();
            var newIndex = 1;
            if (stages != null && stages.Count > 0)
            {
                var topStage = stages.OrderByDescending(item => item.Id).ToList().First();
                newIndex = topStage.Id + 1;
            }
            
            newStage.Name = "Stage " + newIndex.ToString();
            newStage.Id = newIndex;

            List<string> values = new List<string>();
            values.Add("0"); //temperature default value
            values.Add("0"); //ventilation default value
            values.Add("0"); //humidity default value

            newStage.Values = values.ToArray();

            newStage.Duration = 0; //default duration value

            stages.Add(newStage);
            recipeTemp.Stages = stages.ToArray();
            log.InfoFormat("Stage {0} successfully added!", recipeTemp.Stages[recipeTemp.Stages.Length - 1].Name);

            StagesListBox.ItemsSource = ToListStages();
        }

        private void RemoveStageButton_Click(object sender, RoutedEventArgs e)
        {
            List<Stage> stages = new List<Stage>();
            // validate if any stage has been selected
            if (StagesListBox.SelectedIndex < 0) return;

            if (recipeTemp.Stages != null)
            {
                stages = recipeTemp.Stages.ToList();
            }
            else
            {
                return;
            }

            if (stages.Remove(stages[StagesListBox.SelectedIndex]))
            {
                log.InfoFormat("Stage index {0} successfully removed!", StagesListBox.SelectedIndex);
                recipeTemp.Stages = stages.ToArray();
                StagesListBox.ItemsSource = ToListStages();
            }
            else
            {
                log.InfoFormat("Stage index {0} does not exist!", StagesListBox.SelectedIndex);
            }
        }

        private void EditStageButtonClick(object sender, RoutedEventArgs e)
        {
            // validate if any recipe has been selected
            if (StagesListBox.SelectedIndex < 0) return;

            ItemEditDialog stageEditWindow = new ItemEditDialog(recipeTemp.Stages[StagesListBox.SelectedIndex]);

            if (stageEditWindow.ShowDialog() == false && stageEditWindow.shouldSave == true)
            {
                // replace the selected item with the new value
                recipeTemp.Stages[StagesListBox.SelectedIndex] = stageEditWindow.stageTemp;
                StagesListBox.ItemsSource = ToListStages();
            }
        }

        private void StagesBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void RecipeNameTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            recipeTemp.Name = RecipeNameTxtBox.Text;
        }

        private void RecipeDescriptionTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            recipeTemp.Description = RecipeDescriptionTxtBox.Text;
        }
    }
}
