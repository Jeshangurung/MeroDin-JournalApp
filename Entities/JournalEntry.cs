using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Journal.Entities
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }

        // One entry per day (business rule enforced in Service layer)
        [Required]
        public DateTime EntryDate { get; set; }

        // Rich-text / Markdown content (stored as string / HTML / Markdown)
        [Required]
        public string Content { get; set; } = string.Empty;

        // Category (e.g., Personal, Work, Reflection)
        [Required]
        public string Category { get; set; } = string.Empty;

        // Primary Mood (required)
        [Required]
        public string PrimaryMood { get; set; } = string.Empty;

        // Secondary moods (optional, max 2 – enforced in Service layer)
        public string? SecondaryMood1 { get; set; }
        public string? SecondaryMood2 { get; set; }

        // System-generated timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property (many-to-many with Tags)
        public ICollection<JournalTag> JournalTags { get; set; } = new List<JournalTag>();

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public bool IsPublic { get; set; }
    }
}
