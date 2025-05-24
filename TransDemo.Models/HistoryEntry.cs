namespace TransDemo.Models
{
    public class HistoryEntry
    {
        public int HistoryId { get; set; }
        public string Info { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
