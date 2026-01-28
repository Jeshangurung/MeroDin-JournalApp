using System;

namespace Journal.Models
{
    public class JournalEntryDisplayModel
    {
        public int Id { get; set; }

        public DateTime EntryDate { get; set; }

        // Short preview for list/timeline
        public string ContentPreview { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string PrimaryMood { get; set; } = string.Empty;

        public int WordCount { get; set; }
        public bool IsPublic { get; set; }
        public string Author { get; set; } = string.Empty;
    }
}
