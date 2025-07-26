using Microsoft.EntityFrameworkCore;
using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Interfaces.ITeam;

namespace TeamTaskManagementSystem.Repositories
{
    public class TeamMemberRepository : ITeamMemberRepository
    {
        private readonly AppDbContext _context;

        public TeamMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetUserIdsByTeamIdAsync(int teamId)
        {
            return await _context.TeamMembers
                .Where(tm => tm.TeamId == teamId)
                .Select(tm => tm.UserId)
                .ToListAsync();
        }
    }
}
