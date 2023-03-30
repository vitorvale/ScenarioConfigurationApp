namespace ScenariosConfiguration.Models
{
    public class Stage
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public long Duration { get; set; } = -1L; // seconds

        public string[] Values { get; set; }
    }
}
