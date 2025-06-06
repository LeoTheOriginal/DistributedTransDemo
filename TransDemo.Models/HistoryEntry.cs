namespace TransDemo.Models
{
    /// <summary>
    /// Represents a single entry in the history log.
    /// </summary>
    public class HistoryEntry
    {
        /// <summary>
        /// Gets or sets the unique identifier for the history entry.
        /// </summary>
        public int HistoryId { get; set; }

        /// <summary>
        /// Gets or sets the information or description associated with this history entry.
        /// </summary>
        public string Info { get; set; } = "";

        /// <summary>
        /// Gets or sets the date and time when this history entry was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
