// TeamTaskManagementSystem/Interfaces/ITeamService.cs
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<Team> GetTeamByIdAsync(int id);

        // <<< GHI CHÚ: Đổi kiểu trả về sang Task. Service sẽ ném Exception nếu có lỗi.
        Task CreateTeamAsync(Team team, int creatorUserId);
        Task UpdateTeamAsync(Team team, int updaterUserId);
        Task DeleteTeamAsync(int teamId, int userId);

        Task AddMemberAsync(int teamId, int targetUserId, int actorUserId);
        Task RemoveMemberAsync(int teamId, int targetUserId, int actorUserId);
        Task GrantTeamLeaderAsync(int teamId, int targetUserId, int actorUserId);

        Task LeaveAllProjectsAsync(int teamId, int userId);
    }
}