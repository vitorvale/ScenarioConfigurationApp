using System.Collections.Generic;

namespace ScenariosConfiguration.Models
{
    public class Scenario_Config
    {
        public int Id { get; set; }

        public int StatusDatapointId { get; set; }

        public int EngineStateDatapointId { get; set; }

        public int TimerDatapointId { get; set; }

        public int CurrentStageDatapointId { get; set; }

        public int FinalStatus { get; set; }

        public int ActiveRecipe { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Recipe[] Recipees { get; set; }

        public Datapoint[] Datapoints { get; set; }
    }
}
