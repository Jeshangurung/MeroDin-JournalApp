using Journal.Models;

namespace Journal.Services
{
    public interface IJournalService
    {
        // CREATE / UPDATE
        Task<bool> CreateOrUpdateTodayAsync(JournalEntryViewModel model);
        Task<bool> CreateOrUpdateAsync(JournalEntryViewModel model, int? id = null);

        // READ
        Task<JournalEntryViewModel?> GetTodayAsync();
        Task<JournalEntryViewModel?> GetByIdAsync(int id);

        // LIST (basic)
        Task<List<JournalEntryDisplayModel>> GetAllAsync();

        // 🔍 SEARCH + FILTER + PAGINATION (NEW)
        Task<PagedResult<JournalEntryDisplayModel>> SearchAsync(
            string? searchText,
            DateTime? fromDate,
            DateTime? toDate,
            string? mood,
            string? tag,
            int page,
            int pageSize
        );
        Task<PagedResult<JournalEntryDisplayModel>> GetPublicEntriesPagedAsync(string? searchText, int page, int pageSize);
        Task<List<string>> GetAllTagsAsync();
        Task<byte[]> ExportPdfAsync(DateTime from, DateTime to);


        // DELETE
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteTodayAsync();
    }
}
