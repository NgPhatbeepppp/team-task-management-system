using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllAsync();

        Task<Team?> GetByIdAsync(int id);

      
        Task<Team?> GetByIdWithMembersAsync(int teamId);

        Task<Team?> GetByIdWithProjectsAsync(int teamId);

        Task CreateTeamAsync(Team team, int creatorUserId);

        void Update(Team team);

        void Delete(Team team);

        Task<bool> IsTeamLeaderAsync(int teamId, int userId);

        Task<bool> IsMemberAsync(int teamId, int userId);

        Task AddMemberAsync(int teamId, int userId);

        Task RemoveMemberAsync(int teamId, int userId);

        Task GrantTeamLeaderAsync(int teamId, int targetUserId);

        Task<bool> SaveChangesAsync();
    }
}