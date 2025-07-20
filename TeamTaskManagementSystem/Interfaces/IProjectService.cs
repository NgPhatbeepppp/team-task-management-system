using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetProjectsOfUserAsync(int userId);

        Task<Project?> GetByIdAsync(int id);

        Task CreateProjectAsync(Project project, int creatorUserId);

        Task UpdateProjectAsync(Project project, int userId);

        Task DeleteProjectAsync(int projectId, int userId);

        Task RemoveTeamFromProjectAsync(int projectId, int teamId, int userId);
    }
}