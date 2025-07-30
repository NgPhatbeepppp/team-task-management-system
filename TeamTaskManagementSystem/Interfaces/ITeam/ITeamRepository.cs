// TeamTaskManagementSystem/Interfaces/ITeamRepository.cs

// TeamTaskManagementSystem/Interfaces/ITeamRepository.cs
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.ITeam
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllAsync();
        Task<Team?> GetByIdAsync(int id);
        Task<Team?> GetByIdWithProjectsAsync(int teamId);
        Task<Team?> GetByIdWithMembersAsync(int teamId);

        Task<IEnumerable<Team>> GetTeamsByUserIdAsync(int userId);

        Task AddAsync(Team team);
        void Update(Team team);
        void Delete(Team team);
        Task<bool> SaveChangesAsync();
        Task<Team?> GetByKeyCodeAsync(string keyCode);

        Task<bool> IsTeamLeaderAsync(int teamId, int userId);
        Task<bool> IsMemberAsync(int teamId, int userId);

        Task<IEnumerable<TeamMember>> GetTeamMembersByUserIdsAsync(int teamId, List<int> userIds);
        Task<IEnumerable<TeamMember>> GetTeamLeadersAsync(int teamId);
        Task AddMemberAsync(TeamMember member);
        Task RemoveMemberAsync(TeamMember member);
        Task<TeamMember?> GetTeamMemberAsync(int teamId, int userId);
    }
}