using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IChecklistItemService
    {
        Task<IEnumerable<ChecklistItem>> GetByTaskIdAsync(int taskId);
        Task<ChecklistItem?> CreateAsync(ChecklistItem item);
        Task<bool> UpdateAsync(ChecklistItem item);
        Task<bool> DeleteAsync(int id);
    }
}
