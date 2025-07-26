using System.Threading.Tasks;
using TeamTaskManagementSystem.DTOs.TeamInvitation;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface ITeamInvitationService
    {
        Task<bool> InviteUserToTeamAsync(int teamId, int targetUserId, int inviterUserId);
        Task<bool> AcceptInvitationAsync(int invitationId, int handlerUserId);
        Task<bool> RejectInvitationAsync(int invitationId, int handlerUserId);
        Task<IEnumerable<UserSearchResponseDto>> SearchUsersForInvitationAsync(int teamId, string query);
    }
}