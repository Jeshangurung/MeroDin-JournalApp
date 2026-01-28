using Journal.Models;

namespace Journal.Services
{
    /// <summary>
    /// Defines the contract for journal entry management operations.
    /// This interface supports CRUD operations, advanced search capabilities,
    /// document export, and community features with pagination.
    /// </summary>
    public interface IJournalService
    {
        // CREATE / UPDATE operations for creating new entries or modifying existing ones.
        Task<bool> CreateOrUpdateTodayAsync(JournalEntryViewModel model);
        Task<bool> CreateOrUpdateAsync(JournalEntryViewModel model, int? id = null);

        // READ
        Task<JournalEntryViewModel?> GetTodayAsync();
        Task<JournalEntryViewModel?> GetByIdAsync(int id);

        // LIST (basic)
        Task<List<JournalEntryDisplayModel>> GetAllAsync();

        // Advanced search and filtering with server-side pagination for scalability.
        Task<PagedResult<JournalEntryDisplayModel>> SearchAsync(
            string? searchText,
            DateTime? fromDate,
            DateTime? toDate,
            string? mood,
            string? tag,
            int page,
            int pageSize
        );
        
        // Community features for retrieving public entries shared by all users.
        Task<PagedResult<JournalEntryDisplayModel>> GetPublicEntriesPagedAsync(string? searchText, int page, int pageSize);
        
        // Utility methods for tag management and document generation.
        Task<List<string>> GetAllTagsAsync();
        Task<byte[]> ExportPdfAsync(DateTime from, DateTime to);


        // DELETE
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteTodayAsync();
    }
}
