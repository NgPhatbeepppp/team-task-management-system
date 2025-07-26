using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.Iinvitation
{
    public interface IProjectInvitationRepository
    {
        Task<IEnumerable<ProjectInvitation>> GetPendingInvitationsForUserAsync(int userId, List<int> teamIds);
        Task<bool> InvitationExistsAsync(int projectId, int? invitedUserId, int? invitedTeamId);
        Task AddInvitationAsync(ProjectInvitation invitation);
        Task SaveChangesAsync();
        Task<ProjectInvitation?> GetByIdAsync(int invitationId);
    }
}
