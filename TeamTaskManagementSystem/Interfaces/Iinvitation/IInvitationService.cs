namespace TeamTaskManagementSystem.Interfaces.Iinvitation
{
    public interface IInvitationService
    {
        Task<bool> AcceptInvitationAsync(int invitationId, int handlerUserId);
        Task<bool> RejectInvitationAsync(int invitationId, int handlerUserId);
    }
}
