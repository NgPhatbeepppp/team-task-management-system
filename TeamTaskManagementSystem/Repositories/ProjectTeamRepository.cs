using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Repositories
{
    public class ProjectTeamRepository : IProjectTeamRepository
    {
        private readonly AppDbContext _context;

        public ProjectTeamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProjectTeam team)
        {
            await _context.ProjectTeams.AddAsync(team);
        }
    }
}
