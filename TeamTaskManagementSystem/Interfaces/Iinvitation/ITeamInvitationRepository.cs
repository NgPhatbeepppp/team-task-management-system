using System.Threading.Tasks;
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.Iinvitation
{
    public interface ITeamInvitationRepository
    {
        Task<bool> HasPendingInvitationAsync(int teamId, int userId);
        Task AddAsync(TeamInvitation invitation);
        Task<TeamInvitation?> GetByIdAsync(int invitationId);
        Task<bool> SaveChangesAsync();
        void Update(TeamInvitation invitation);

        Task<IEnumerable<TeamInvitation>> GetPendingInvitationsByUserIdAsync(int userId);
        Task<IEnumerable<TeamInvitation>> GetPendingInvitationsForUsersAsync(int teamId, List<int> userIds);
    }
}