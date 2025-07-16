using System.Threading.Tasks;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface ITeamInvitationService
    {
        Task<bool> InviteUserToTeamAsync(int teamId, int targetUserId, int inviterUserId);
        Task<bool> AcceptInvitationAsync(int invitationId, int handlerUserId);
        Task<bool> RejectInvitationAsync(int invitationId, int handlerUserId);
    }
}