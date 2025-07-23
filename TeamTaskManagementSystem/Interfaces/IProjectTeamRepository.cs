// TeamTaskManagementSystem/Interfaces/IProjectTeamRepository.cs
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IProjectTeamRepository
    {
        // <<< GHI CHÚ: Sửa lại tham số và kiểu dữ liệu cho đúng với ProjectTeam
        Task<ProjectTeam?> FindAsync(int projectId, int teamId);

        Task AddAsync(ProjectTeam projectTeam);

        // <<< GHI CHÚ: Sửa lại tham số cho đúng với ProjectTeam
        void Delete(ProjectTeam projectTeam);
    }
}