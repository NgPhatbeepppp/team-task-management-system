using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces.Iinvitation;

namespace TeamTaskManagementSystem.Repositories
{
    public class TeamInvitationRepository : ITeamInvitationRepository
    {
        private readonly AppDbContext _context;

        public TeamInvitationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPendingInvitationAsync(int teamId, int userId)
        {
            return await _context.TeamInvitations
                .AnyAsync(i => i.TeamId == teamId && i.InvitedUserId == userId && i.Status == "Pending");
        }

        public async Task AddAsync(TeamInvitation invitation)
        {
            await _context.TeamInvitations.AddAsync(invitation);
        }

        public async Task<TeamInvitation?> GetByIdAsync(int invitationId)
        {
            return await _context.TeamInvitations.FindAsync(invitationId);
        }

        public void Update(TeamInvitation invitation)
        {
            _context.TeamInvitations.Update(invitation);
        }
       
        public async Task<IEnumerable<TeamInvitation>> GetPendingInvitationsForUsersAsync(int teamId, List<int> userIds)
        {
            return await _context.TeamInvitations
                .Where(i => i.TeamId == teamId && i.Status == "Pending" && userIds.Contains(i.InvitedUserId))
                .ToListAsync();
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}