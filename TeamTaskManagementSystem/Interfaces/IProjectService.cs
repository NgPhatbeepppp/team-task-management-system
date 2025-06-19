using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetProjectsOfUserAsync(int userId);
        Task<Project?> GetByIdAsync(int id);
        Task<bool> CreateProjectAsync(Project project, int creatorUserId);
        Task<bool> UpdateProjectAsync(Project project, int userId);
        Task<bool> DeleteProjectAsync(int id, int userId);
    }
}
