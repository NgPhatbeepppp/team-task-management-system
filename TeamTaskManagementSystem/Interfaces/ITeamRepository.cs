using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllAsync();
        Task<bool> CreateTeamAsync(Team team, int creatorUserId);
        Task<Team?> GetByIdAsync(int id);
        Task AddAsync(Team team);
        void Update(Team team);
        void Delete(Team team);
        Task<bool> SaveChangesAsync();
        Task<bool> IsTeamLeaderAsync(int teamId, int userId);
        Task<bool> AddMemberAsync(int teamId, int userId);
        Task<bool> RemoveMemberAsync(int teamId, int userId);
        Task<bool> GrantTeamLeaderAsync(int teamId, int targetUserId);
        Task<bool> IsMemberAsync(int teamId, int userId);

    }
}
