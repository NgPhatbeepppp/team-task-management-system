using Microsoft.EntityFrameworkCore;
using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(int projectId)
        {
            return await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task AddAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(TaskItem task)
        {
            _context.Tasks.Remove(task);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
