// TeamTaskManagementSystem/Repositories/ProjectTeamRepository.cs
using Microsoft.EntityFrameworkCore;
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

        public async Task AddAsync(ProjectTeam projectTeam)
        {
            await _context.ProjectTeams.AddAsync(projectTeam);
        }

        // <<< GHI CHÚ: Triển khai phương thức FindAsync còn thiếu.
        public async Task<ProjectTeam?> FindAsync(int projectId, int teamId)
        {
            return await _context.ProjectTeams
                .FirstOrDefaultAsync(pt => pt.ProjectId == projectId && pt.TeamId == teamId);
        }

        // <<< GHI CHÚ: Triển khai phương thức Delete còn thiếu.
        public void Delete(ProjectTeam projectTeam)
        {
            _context.ProjectTeams.Remove(projectTeam);
        }
    }
}