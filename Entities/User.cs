using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Journal.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // PIN for app locking
        public string PinHash { get; set; } = string.Empty;

        // System-generated timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property for entries
        public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    }
}
