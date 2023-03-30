namespace ScenariosConfiguration.Models
{
    public class Datapoint
    {
        public int Id { get; set; }

        public long SvcId { get; set; } = -1;

        public string Name { get; set; } = string.Empty;

        public int Precision { get; set; }

        public string Value { get; set; }
    }
}
