using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repo;

        public ProjectService(IProjectRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Project>> GetProjectsOfUserAsync(int userId)
        {
            return await _repo.GetProjectsOfUserAsync(userId);
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<bool> CreateProjectAsync(Project project, int creatorUserId)
        {
            var projectMember = new ProjectMember
            {
                UserId = creatorUserId,
                Project = project,
                RoleInProject = "ProjectLeader"
            };

            await _repo.AddAsync(project);
            await _repo.SaveChangesAsync(); // Đảm bảo project có Id
            return true;
        }

        public async Task<bool> UpdateProjectAsync(Project project, int userId)
        {
            var isLeader = await _repo.IsUserProjectLeaderAsync(project.Id, userId);
            if (!isLeader) return false;

            _repo.Update(project);
            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> DeleteProjectAsync(int id, int userId)
        {
            var isLeader = await _repo.IsUserProjectLeaderAsync(id, userId);
            if (!isLeader) return false;

            var project = await _repo.GetByIdAsync(id);
            if (project == null) return false;

            _repo.Delete(project);
            return await _repo.SaveChangesAsync();
        }
    }
}
