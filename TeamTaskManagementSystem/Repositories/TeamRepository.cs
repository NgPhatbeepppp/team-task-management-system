using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TeamTaskManagementSystem.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly AppDbContext _context;

        public TeamRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateTeamAsync(Team team, int creatorUserId)
        {
            team.CreatedByUserId = creatorUserId;
            await _context.Teams.AddAsync(team);

            _context.TeamMembers.Add(new TeamMember
            {
                Team = team,
                UserId = creatorUserId,
                RoleInTeam = "TeamLeader"
            });

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams
                                 .Include(t => t.Members)
                                 .ThenInclude(tm => tm.User)
                                 .ToListAsync();
        }

        public async Task<Team?> GetByIdAsync(int id) => await _context.Teams.FindAsync(id);

        public async Task AddAsync(Team team) => await _context.Teams.AddAsync(team);

        public void Update(Team team) => _context.Teams.Update(team);

        public void Delete(Team team) => _context.Teams.Remove(team);
       
        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

        public async Task<bool> IsTeamLeaderAsync(int teamId, int userId)
        {
            return await _context.TeamMembers.AnyAsync(tm =>
                tm.TeamId == teamId &&
                tm.UserId == userId &&
                tm.RoleInTeam == "TeamLeader");
        }
        public async Task<bool> AddMemberAsync(int teamId, int userId)
        {
            var exists = await _context.TeamMembers.AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
            if (exists) return false;

            _context.TeamMembers.Add(new TeamMember
            {
                TeamId = teamId,
                UserId = userId,
                RoleInTeam = "Member"
            });

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveMemberAsync(int teamId, int userId)
        {
            var member = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            if (member == null) return false;

            _context.TeamMembers.Remove(member);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> GrantTeamLeaderAsync(int teamId, int userId)
        {
            var member = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            if (member == null) return false;

            member.RoleInTeam = "TeamLeader";
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsMemberAsync(int teamId, int userId)
        {
            return await _context.TeamMembers.AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        }

    } 
}
