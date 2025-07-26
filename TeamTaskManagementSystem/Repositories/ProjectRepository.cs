using Microsoft.EntityFrameworkCore;
using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces.IProject;

namespace TeamTaskManagementSystem.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetProjectsOfUserAsync(int userId)
        {
            var projectIds = await _context.ProjectMembers
                .Where(pm => pm.UserId == userId)
                .Select(pm => pm.ProjectId)
                .ToListAsync();

            return await _context.Projects
                .Where(p => projectIds.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
        }

        public void Update(Project project)
        {
            _context.Projects.Update(project);
        }

        public void Delete(Project project)
        {
            _context.Projects.Remove(project);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsUserProjectLeaderAsync(int projectId, int userId)
        {
            return await _context.ProjectMembers.AnyAsync(pm =>
                pm.ProjectId == projectId &&
                pm.UserId == userId &&
                pm.RoleInProject == "ProjectLeader");
        }
    }
}
