using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IProjectInvitationRepository
    {
        Task<bool> InvitationExistsAsync(int projectId, int? invitedUserId, int? invitedTeamId);
        Task AddInvitationAsync(ProjectInvitation invitation);
        Task SaveChangesAsync();
        Task<ProjectInvitation?> GetByIdAsync(int invitationId);
    }
}
