// TeamTaskManagementSystem/Interfaces/IProjectService.cs

// TeamTaskManagementSystem/Interfaces/IProjectService.cs
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.IProject
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetProjectsOfUserAsync(int userId);
        Task<Project> GetByIdAsync(int id);

        Task CreateProjectAsync(Project project, int creatorUserId);
        Task UpdateProjectAsync(Project project, int userId);
        Task DeleteProjectAsync(int projectId, int userId);

        Task RemoveTeamFromProjectAsync(int projectId, int teamId, int userId);

        // <<< GHI CHÚ: Bổ sung phương thức mới để xử lý việc thêm thành viên.
        Task AddMemberToProjectAsync(int projectId, int targetUserId, int actorUserId);
    }
}