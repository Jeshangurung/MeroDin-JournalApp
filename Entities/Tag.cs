using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Journal.Entities
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<JournalTag> JournalTags { get; set; } = new List<JournalTag>();
    }
}

