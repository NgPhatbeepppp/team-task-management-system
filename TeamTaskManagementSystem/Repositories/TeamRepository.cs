// TeamTaskManagementSystem/Repositories/TeamRepository.cs
using Microsoft.EntityFrameworkCore;
using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces.ITeam;

namespace TeamTaskManagementSystem.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly AppDbContext _context;

        public TeamRepository(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
        }

        public async Task<IEnumerable<Team>> GetTeamsByUserIdAsync(int userId)
        {
            
            var teamIds = await _context.TeamMembers
                .Where(tm => tm.UserId == userId)
                .Select(tm => tm.TeamId)
                .ToListAsync();

            
            return await _context.Teams
                .Where(t => teamIds.Contains(t.Id))
                .Include(t => t.Members) 
                .ThenInclude(tm => tm.User) 
                .ToListAsync();
        }
        public async Task<Team?> GetByKeyCodeAsync(string keyCode)
        {
            return await _context.Teams
                .FirstOrDefaultAsync(t => t.KeyCode == keyCode);
        }
        public async Task<IEnumerable<TeamMember>> GetTeamLeadersAsync(int teamId)
        {
            return await _context.TeamMembers
                .Where(tm => tm.TeamId == teamId && tm.RoleInTeam == "TeamLeader")
                .ToListAsync();
        }
        public void Delete(Team team) => _context.Teams.Remove(team);

        public async Task<IEnumerable<Team>> GetAllAsync() => await _context.Teams.Include(t => t.Members).ThenInclude(tm => tm.User).ToListAsync();

        public async Task<Team?> GetByIdAsync(int id) => await _context.Teams.FindAsync(id);

        public async Task<Team?> GetByIdWithProjectsAsync(int teamId)
        {
            return await _context.Teams
                .Include(t => t.Projects)
                .FirstOrDefaultAsync(t => t.Id == teamId);
        }

        public async Task<Team?> GetByIdWithMembersAsync(int teamId)
        {
            return await _context.Teams
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == teamId);
        }

        public async Task<bool> IsMemberAsync(int teamId, int userId)
        {
            return await _context.TeamMembers.AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        }

        public async Task<bool> IsTeamLeaderAsync(int teamId, int userId)
        {
            return await _context.TeamMembers.AnyAsync(tm =>
                tm.TeamId == teamId &&
                tm.UserId == userId &&
                tm.RoleInTeam == "TeamLeader");
        }

        public async Task AddMemberAsync(TeamMember member)
        {
            await _context.TeamMembers.AddAsync(member);
        }

        public async Task RemoveMemberAsync(TeamMember member)
        {
            _context.TeamMembers.Remove(member);
            await Task.CompletedTask;
        }

        public async Task<TeamMember?> GetTeamMemberAsync(int teamId, int userId)
        {
            return await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        }
        
        public async Task<IEnumerable<TeamMember>> GetTeamMembersByUserIdsAsync(int teamId, List<int> userIds)
        {
            return await _context.TeamMembers
                .Where(tm => tm.TeamId == teamId && userIds.Contains(tm.UserId))
                .ToListAsync();
        }
        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

        public void Update(Team team) => _context.Teams.Update(team);
    }
}