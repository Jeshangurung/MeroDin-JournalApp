using Journal.Data;
using Journal.Entities;
using Journal.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace Journal.Services
{
    public class JournalService : IJournalService
    {
        private readonly AppDbContext _context;
        private readonly AuthStateService _auth;

        public JournalService(AppDbContext context, AuthStateService auth)
        {
            _context = context;
            _auth = auth;
        }

        private int CurrentUserId => _auth.CurrentUser?.Id ?? throw new Exception("Unauthorized access");

        // CREATE or UPDATE a journal entry
        public async Task<bool> CreateOrUpdateAsync(JournalEntryViewModel model, int? id = null)
        {
            JournalEntry? entry = null;

            if (id.HasValue)
            {
                entry = await _context.JournalEntries
                    .FirstOrDefaultAsync(j => j.Id == id.Value && j.UserId == CurrentUserId);
            }
            else
            {
                var today = DateTime.Today;
                entry = await _context.JournalEntries
                    .Include(j => j.JournalTags)
                    .ThenInclude(jt => jt.Tag)
                    .FirstOrDefaultAsync(j => j.EntryDate == today && j.UserId == CurrentUserId);
            }

            if (entry == null)
            {
                entry = new JournalEntry
                {
                    UserId = CurrentUserId,
                    EntryDate = DateTime.Today,
                    CreatedAt = DateTime.Now
                };
                _context.JournalEntries.Add(entry);
            }

            entry.Content = model.Content;
            entry.Category = model.Category;
            entry.PrimaryMood = model.PrimaryMood;
            entry.SecondaryMood1 = model.SecondaryMood1;
            entry.SecondaryMood2 = model.SecondaryMood2;
            entry.IsPublic = model.IsPublic;
            entry.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            // ===== Tag Handling =====
            var existingTags = _context.JournalTags.Where(jt => jt.JournalEntryId == entry.Id);
            _context.JournalTags.RemoveRange(existingTags);

            if (model.SelectedTags.Any())
            {
                foreach (var name in model.SelectedTags)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
                    if (tag == null)
                    {
                        tag = new Tag { Name = name };
                        _context.Tags.Add(tag);
                        await _context.SaveChangesAsync();
                    }
                    _context.JournalTags.Add(new JournalTag { JournalEntryId = entry.Id, TagId = tag.Id });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // CREATE or UPDATE today's journal entry
        public async Task<bool> CreateOrUpdateTodayAsync(JournalEntryViewModel model)
        {
            return await CreateOrUpdateAsync(model);
        }

        // READ today's journal entry (for EditForm)
        public async Task<JournalEntryViewModel?> GetTodayAsync()
        {
            var today = DateTime.Today;
            var entry = await _context.JournalEntries
                .Include(j => j.JournalTags)
                .ThenInclude(jt => jt.Tag)
                .FirstOrDefaultAsync(j => j.EntryDate == today && j.UserId == CurrentUserId);

            return MapToViewModel(entry);
        }

        public async Task<JournalEntryViewModel?> GetByIdAsync(int id)
        {
            var entry = await _context.JournalEntries
                .Include(j => j.JournalTags)
                .ThenInclude(jt => jt.Tag)
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == CurrentUserId);
            return MapToViewModel(entry);
        }

        private JournalEntryViewModel? MapToViewModel(JournalEntry? entry)
        {
            if (entry == null) return null;

            return new JournalEntryViewModel
            {
                Id = entry.Id,
                Content = entry.Content,
                Category = entry.Category,
                PrimaryMood = entry.PrimaryMood,
                SecondaryMood1 = entry.SecondaryMood1,
                SecondaryMood2 = entry.SecondaryMood2,
                EntryDate = entry.EntryDate,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt,
                IsPublic = entry.IsPublic,
                SelectedTags = entry.JournalTags.Select(jt => jt.Tag.Name).ToList()
            };
        }

        // READ all journal entries (timeline / list)
        public async Task<List<JournalEntryDisplayModel>> GetAllAsync()
        {
            var entries = await _context.JournalEntries
                .Where(j => j.UserId == CurrentUserId)
                .Include(j => j.JournalTags)
                .ThenInclude(jt => jt.Tag)
                .OrderByDescending(j => j.EntryDate)
                .ToListAsync();

            return entries.Select(j => new JournalEntryDisplayModel
            {
                Id = j.Id,
                EntryDate = j.EntryDate,
                Category = j.Category,
                PrimaryMood = j.PrimaryMood,
                ContentPreview = GetPreview(j.Content),
                IsPublic = j.IsPublic,
                WordCount = j.Content
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Length,
                Tags = j.JournalTags.Select(jt => jt.Tag.Name).ToList()
            }).ToList();
        }

        private string GetPreview(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            var plainText = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
            return plainText.Length > 100 ? plainText.Substring(0, 100) + "..." : plainText;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == CurrentUserId);
            if (entry == null) return false;

            _context.JournalEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE today's journal entry
        public async Task<bool> DeleteTodayAsync()
        {
            var today = DateTime.Today;

            var entry = await _context.JournalEntries
                .Include(j => j.JournalTags) // Added Include as per instruction
                .ThenInclude(jt => jt.Tag)    // Added ThenInclude as per instruction
                .FirstOrDefaultAsync(j => j.EntryDate == today && j.UserId == CurrentUserId);

            if (entry == null)
                return false;

            _context.JournalEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> ExportPdfAsync(DateTime from, DateTime to)
        {
            var entries = await _context.JournalEntries
                .Where(e => e.UserId == CurrentUserId && e.EntryDate >= from.Date && e.EntryDate <= to.Date)
                .OrderBy(e => e.EntryDate)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Header()
                        .Text($"Journal Entries ({from:dd MMM yyyy} – {to:dd MMM yyyy})")
                        .FontSize(18)
                        .SemiBold();

                    page.Content().Column(col =>
                    {
                        foreach (var entry in entries)
                        {
                            col.Item().PaddingBottom(15).BorderBottom(1).Column(e =>
                            {
                                e.Item().Text(entry.EntryDate.ToString("dddd, dd MMM yyyy"))
                                    .Bold();

                                e.Item().Text($"Mood: {entry.PrimaryMood}");
                                e.Item().Text($"Category: {entry.Category}");

                                e.Item().PaddingTop(5)
                                    .Text(RemoveHtml(entry.Content));
                            });
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on ");
                            x.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm"));
                        });
                });
            });

            return document.GeneratePdf();
        }

        /// <summary>
        /// Retrieves a paginated and optionally filtered list of public journal entries.
        /// </summary>
        /// <param name="searchText">Optional text to filter entries by (checks category and content).</param>
        /// <param name="page">The current page number (1-indexed).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paged result containing entry display models.</returns>
        public async Task<PagedResult<JournalEntryDisplayModel>> GetPublicEntriesPagedAsync(string? searchText, int page, int pageSize)
        {
            // Base query for public entries
            var query = _context.JournalEntries
                .Include(j => j.User)
                .Include(j => j.JournalTags)
                .ThenInclude(jt => jt.Tag)
                .Where(j => j.IsPublic);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var search = searchText.ToLower();
                query = query.Where(j => j.Category.ToLower().Contains(search) || 
                                       j.Content.ToLower().Contains(search));
            }

            // Get total count for pagination metadata
            var totalCount = await query.CountAsync();

            // Fetch paged data
            var entries = await query
                .OrderByDescending(j => j.EntryDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map entities to display models
            return new PagedResult<JournalEntryDisplayModel>
            {
                Items = entries.Select(j => new JournalEntryDisplayModel
                {
                    Id = j.Id,
                    EntryDate = j.EntryDate,
                    Category = j.Category,
                    PrimaryMood = j.PrimaryMood,
                    ContentPreview = GetPreview(j.Content),
                    IsPublic = j.IsPublic,
                    Author = j.User?.Username ?? "Anonymous",
                    WordCount = j.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
                    Tags = j.JournalTags.Select(jt => jt.Tag.Name).ToList()
                }).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private string RemoveHtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return "";
            return System.Text.RegularExpressions.Regex
                .Replace(html, "<.*?>", string.Empty);
        }


        public async Task<PagedResult<JournalEntryDisplayModel>> SearchAsync(
    string? searchText,
    DateTime? fromDate,
    DateTime? toDate,
    string? mood,
    string? tag,
    int page,
    int pageSize)
        {
            var query = _context.JournalEntries
                .Include(j => j.JournalTags)
                .ThenInclude(jt => jt.Tag)
                .Where(j => j.UserId == CurrentUserId)
                .AsQueryable();

            // 🔍 Search by content OR category
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var term = searchText.ToLower();
                query = query.Where(j => 
                    j.Content.ToLower().Contains(term) || 
                    j.Category.ToLower().Contains(term) ||
                    j.JournalTags.Any(jt => jt.Tag.Name.ToLower().Contains(term)));
            }

            // 📅 Date range filter
            if (fromDate.HasValue)
                query = query.Where(j => j.EntryDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(j => j.EntryDate <= toDate.Value);

            // 😊 Mood filter
            if (!string.IsNullOrWhiteSpace(mood))
                query = query.Where(j =>
                    j.PrimaryMood == mood ||
                    j.SecondaryMood1 == mood ||
                    j.SecondaryMood2 == mood);

            // 🏷️ Tag filter (NEW dedicated field)
            if (!string.IsNullOrWhiteSpace(tag))
            {
                var tagTerm = tag.ToLower();
                query = query.Where(j => j.JournalTags.Any(jt => jt.Tag.Name.ToLower().Contains(tagTerm)));
            }

            var totalCount = await query.CountAsync();

            var entries = await query
                .OrderByDescending(j => j.EntryDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<JournalEntryDisplayModel>
            {
                Items = entries.Select(j => new JournalEntryDisplayModel
                {
                    Id = j.Id,
                    EntryDate = j.EntryDate,
                    Category = j.Category,
                    PrimaryMood = j.PrimaryMood,
                    ContentPreview = GetPreview(j.Content),
                    IsPublic = j.IsPublic,
                    WordCount = j.Content
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Length,
                    Tags = j.JournalTags.Select(jt => jt.Tag.Name).ToList()
                }).ToList(),

                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<List<string>> GetAllTagsAsync()
        {
            return await _context.JournalTags
                .Where(jt => jt.JournalEntry.UserId == CurrentUserId)
                .Select(jt => jt.Tag.Name)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();
        }

    }
}
