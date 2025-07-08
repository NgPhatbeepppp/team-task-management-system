using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IChecklistItemRepository
    {
        Task<IEnumerable<ChecklistItem>> GetByTaskIdAsync(int taskId);
        Task<ChecklistItem?> GetByIdAsync(int id);
        Task AddAsync(ChecklistItem item);
        Task UpdateAsync(ChecklistItem item);
        Task DeleteAsync(ChecklistItem item);
        Task SaveChangesAsync();
    }
}
