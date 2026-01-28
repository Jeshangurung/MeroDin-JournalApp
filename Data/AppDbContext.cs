using Journal.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Journal.Data
{
    public class AppDbContext : DbContext
    {
        // ===== DbSets (Tables) =====
        public DbSet<JournalEntry> JournalEntries { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
 
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<JournalTag> JournalTags { get; set; } = null!;

        private readonly string _dbPath;

        public AppDbContext()
        {
            // Cross-platform safe location for SQLite database
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _dbPath = Path.Combine(folder, "journal1.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== JournalEntry ↔ Tag (Many-to-Many) =====
            modelBuilder.Entity<JournalTag>()
                .HasKey(jt => new { jt.JournalEntryId, jt.TagId });

            modelBuilder.Entity<JournalTag>()
                .HasOne(jt => jt.JournalEntry)
                .WithMany(j => j.JournalTags)
                .HasForeignKey(jt => jt.JournalEntryId);

            modelBuilder.Entity<JournalTag>()
                .HasOne(jt => jt.Tag)
                .WithMany(t => t.JournalTags)
                .HasForeignKey(jt => jt.TagId);

            // ===== User ↔ JournalEntry (One-to-Many) =====
            modelBuilder.Entity<JournalEntry>()
                .HasOne(j => j.User)
                .WithMany(u => u.JournalEntries)
                .HasForeignKey(j => j.UserId);

            // ===== One Journal Entry Per Day PER USER =====
            modelBuilder.Entity<JournalEntry>()
                .HasIndex(j => new { j.UserId, j.EntryDate })
                .IsUnique();

            // ===== User Username/Email uniqueness =====
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
