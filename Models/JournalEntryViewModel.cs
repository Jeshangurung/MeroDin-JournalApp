using System.ComponentModel.DataAnnotations;

namespace Journal.Models
{
    public class JournalEntryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Journal content is required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;

        // Primary mood is required
        [Required(ErrorMessage = "Primary mood is required")]
        public string PrimaryMood { get; set; } = string.Empty;

        // Optional secondary moods (max 2 enforced in Service)
        public string? SecondaryMood1 { get; set; }
        public string? SecondaryMood2 { get; set; }

        public DateTime EntryDate { get; set; } = DateTime.Today;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPublic { get; set; }
        public List<string> SelectedTags { get; set; } = new();
    }
}
