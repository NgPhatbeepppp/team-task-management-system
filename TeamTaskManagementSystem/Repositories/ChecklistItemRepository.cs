using Microsoft.EntityFrameworkCore;
using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Repositories
{
    public class ChecklistItemRepository : IChecklistItemRepository
    {
        private readonly AppDbContext _context;

        public ChecklistItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChecklistItem>> GetByTaskIdAsync(int taskId)
        {
            return await _context.ChecklistItems
                .Where(c => c.TaskId == taskId)
                .ToListAsync();
        }

        public async Task<ChecklistItem?> GetByIdAsync(int id)
        {
            return await _context.ChecklistItems.FindAsync(id);
        }

        public async Task AddAsync(ChecklistItem item)
        {
            await _context.ChecklistItems.AddAsync(item);
        }

        public async Task UpdateAsync(ChecklistItem item)
        {
            _context.ChecklistItems.Update(item);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(ChecklistItem item)
        {
            _context.ChecklistItems.Remove(item);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
