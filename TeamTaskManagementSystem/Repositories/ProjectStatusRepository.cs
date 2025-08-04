using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces.IProject;

namespace TeamTaskManagementSystem.Repositories
{
    public class ProjectStatusRepository : IProjectStatusRepository
    {
        private readonly AppDbContext _context;

        public ProjectStatusRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectStatus?> GetByIdAsync(int id)
        {
            return await _context.ProjectStatuses.FindAsync(id);
        }

        public async Task<List<ProjectStatus>> GetByProjectIdAsync(int projectId)
        {
            return await _context.ProjectStatuses
                .Where(ps => ps.ProjectId == projectId)
                .OrderBy(ps => ps.Order)
                .ToListAsync();
        }

        public async Task<int> GetNextOrderValueForProjectAsync(int projectId)
        {
            var maxOrder = await _context.ProjectStatuses
                .Where(ps => ps.ProjectId == projectId)
                .MaxAsync(ps => (int?)ps.Order);

            return (maxOrder ?? -1) + 1;
        }

        public async Task<bool> IsNameTakenInProjectAsync(int projectId, string name, int? excludeStatusId = null)
        {
            var query = _context.ProjectStatuses
                .Where(ps => ps.ProjectId == projectId && ps.Name.ToLower() == name.ToLower());

            if (excludeStatusId.HasValue)
            {
                query = query.Where(ps => ps.Id != excludeStatusId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task AddAsync(ProjectStatus status)
        {
            await _context.ProjectStatuses.AddAsync(status);
        }

        public void Update(ProjectStatus status)
        {
            _context.ProjectStatuses.Update(status);
        }

        public void Delete(ProjectStatus status)
        {
            _context.ProjectStatuses.Remove(status);
        }

        public async Task<bool> IsStatusInUse(int statusId)
        {
            return await _context.Tasks.AnyAsync(t => t.StatusId == statusId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}