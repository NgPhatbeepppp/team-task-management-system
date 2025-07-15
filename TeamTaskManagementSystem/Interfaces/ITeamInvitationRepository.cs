using System.Threading.Tasks;
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface ITeamInvitationRepository
    {
        Task<bool> HasPendingInvitationAsync(int teamId, int userId);
        Task AddAsync(TeamInvitation invitation);
        Task<TeamInvitation?> GetByIdAsync(int invitationId);
        Task<bool> SaveChangesAsync();
        void Update(TeamInvitation invitation);
    }
}