using System.Collections.Generic;

namespace ScenariosConfiguration.Models
{
    public class Scenario
    {
        public int Id { get; set; }

        public int StatusDatapointId { get; set; }

        public int EngineStateDatapointId { get; set; }

        public int TimerDatapointId { get; set; }

        public int CurrentStageDatapointId { get; set; }

        public int FinalStatus { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Stage[] Stages { get; set; }

        public Datapoint[] Datapoints { get; set; }
    }
}
