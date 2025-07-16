using Microsoft.EntityFrameworkCore;
using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Repositories
{
    public class ProjectInvitationRepository : IProjectInvitationRepository
    {
        private readonly AppDbContext _context;

        public ProjectInvitationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> InvitationExistsAsync(int projectId, int? invitedUserId, int? invitedTeamId)
        {
            return await _context.ProjectInvitations.AnyAsync(i =>
                i.ProjectId == projectId &&
                i.Status == "Pending" &&
                (
                    (invitedUserId.HasValue && i.InvitedUserId == invitedUserId) ||
                    (invitedTeamId.HasValue && i.InvitedTeamId == invitedTeamId)
                ));
        }

        public async Task AddInvitationAsync(ProjectInvitation invitation)
        {
            await _context.ProjectInvitations.AddAsync(invitation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<ProjectInvitation?> GetByIdAsync(int invitationId)
        {
            return await _context.ProjectInvitations
                .FirstOrDefaultAsync(i => i.Id == invitationId);
        }

    }
}
