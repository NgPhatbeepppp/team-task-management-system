namespace TeamTaskManagementSystem.Interfaces
{
    public interface IProjectInvitationService
    {
        Task<bool> InviteUserAsync(int projectId, string identifier, int invitedByUserId);
        Task<bool> InviteTeamAsync(int projectId, int invitedTeamId, int invitedByUserId);
    }
}
