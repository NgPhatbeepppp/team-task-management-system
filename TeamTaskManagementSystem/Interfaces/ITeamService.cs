using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<Team?> GetTeamByIdAsync(int id);
        Task<bool> CreateTeamAsync(Team team, int creatorUserId);
        Task<bool> UpdateTeamAsync(Team team);
        Task DeleteTeamAsync(int teamId, int userId);
        Task LeaveAllProjectsAsync(int teamId, int userId);
        Task<bool> IsTeamLeaderAsync(int teamId, int userId);
        Task<bool> AddMemberAsync(int teamId, int userId);
        Task<bool> RemoveMemberAsync(int teamId, int userId);
        Task<bool> GrantTeamLeaderAsync(int teamId, int targetUserId);


    }
}
