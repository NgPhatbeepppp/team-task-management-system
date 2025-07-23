// TeamTaskManagementSystem/Interfaces/ITeamRepository.cs
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllAsync();
        Task<Team?> GetByIdAsync(int id);
        Task<Team?> GetByIdWithProjectsAsync(int teamId);
        Task<Team?> GetByIdWithMembersAsync(int teamId);

        // <<< GHI CHÚ: Đơn giản hóa, repository chỉ có nhiệm vụ thêm thực thể vào DbContext.
        Task AddAsync(Team team);
        void Update(Team team);
        void Delete(Team team);
        Task<bool> SaveChangesAsync();

        Task<bool> IsTeamLeaderAsync(int teamId, int userId);
        Task<bool> IsMemberAsync(int teamId, int userId);

        Task AddMemberAsync(TeamMember member);
        Task RemoveMemberAsync(TeamMember member);
        Task<TeamMember?> GetTeamMemberAsync(int teamId, int userId);
    }
}