using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Services
{
    public class ProjectInvitationService : IProjectInvitationService
    {
        private readonly IProjectInvitationRepository _invitationRepo;
        private readonly IUserRepository _userRepo;

        public ProjectInvitationService(IProjectInvitationRepository invitationRepo, IUserRepository userRepo)
        {
            _invitationRepo = invitationRepo;
            _userRepo = userRepo;
        }

        public async Task<bool> InviteUserAsync(int projectId, string identifier, int invitedByUserId)
        {
            var user = await _userRepo.GetUserByEmailOrUsernameAsync(identifier);
            if (user == null) return false;

            var exists = await _invitationRepo.InvitationExistsAsync(projectId, user.Id, null);
            if (exists) return false;

            var invitation = new ProjectInvitation
            {
                ProjectId = projectId,
                InvitedUserId = user.Id,
                InvitedByUserId = invitedByUserId,
                Status = "Pending"
            };

            await _invitationRepo.AddInvitationAsync(invitation);
            await _invitationRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InviteTeamAsync(int projectId, int invitedTeamId, int invitedByUserId)
        {
            var exists = await _invitationRepo.InvitationExistsAsync(projectId, null, invitedTeamId);
            if (exists) return false;

            var invitation = new ProjectInvitation
            {
                ProjectId = projectId,
                InvitedTeamId = invitedTeamId,
                InvitedByUserId = invitedByUserId,
                Status = "Pending"
            };

            await _invitationRepo.AddInvitationAsync(invitation);
            await _invitationRepo.SaveChangesAsync();
            return true;
        }
    }
}
