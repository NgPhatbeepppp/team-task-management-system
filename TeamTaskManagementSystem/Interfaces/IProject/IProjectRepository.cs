using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.IProject
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetProjectsOfUserAsync(int userId);
        Task<Project?> GetByIdAsync(int id);
        Task AddAsync(Project project);
        void Update(Project project);
        void Delete(Project project);
        Task<bool> SaveChangesAsync();
        Task<bool> IsUserProjectLeaderAsync(int projectId, int userId);
    }
}
