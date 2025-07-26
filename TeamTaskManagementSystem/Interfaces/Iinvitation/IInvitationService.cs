using System.Collections.Generic; 
using TeamTaskManagementSystem.DTOs;

namespace TeamTaskManagementSystem.Interfaces.Iinvitation
{
    public interface IInvitationService
    {
        Task<List<InvitationDto>> GetPendingInvitationsForUserAsync(int userId);
        Task<bool> AcceptInvitationAsync(int invitationId, int handlerUserId);
        Task<bool> RejectInvitationAsync(int invitationId, int handlerUserId);
    }
}
