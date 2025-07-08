using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Services
{
    public class ChecklistItemService : IChecklistItemService
    {
        private readonly IChecklistItemRepository _repo;

        public ChecklistItemService(IChecklistItemRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ChecklistItem>> GetByTaskIdAsync(int taskId)
        {
            return await _repo.GetByTaskIdAsync(taskId);
        }

        public async Task<ChecklistItem?> CreateAsync(ChecklistItem item)
        {
            await _repo.AddAsync(item);
            await _repo.SaveChangesAsync();
            return item;
        }

        public async Task<bool> UpdateAsync(ChecklistItem item)
        {
            var existing = await _repo.GetByIdAsync(item.Id);
            if (existing == null) return false;

            existing.Content = item.Content;
            existing.IsCompleted = item.IsCompleted;

            await _repo.UpdateAsync(existing);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return false;

            await _repo.DeleteAsync(item);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
