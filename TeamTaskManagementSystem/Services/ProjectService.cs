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
            // ⚠️ GÁN khóa ngoại bắt buộc
            project.CreatedByUserId = creatorUserId;
            project.CreatedAt = DateTime.UtcNow;

            var projectMember = new ProjectMember
            {
                UserId = creatorUserId,
                Project = project,
                RoleInProject = "ProjectLeader"
            };
            project.Members.Add(projectMember);

            project.Statuses = new List<ProjectStatus>
            {
                new ProjectStatus { Name = "To Do", Color = "#007bff", Order = 0 },
                new ProjectStatus { Name = "In Progress", Color = "#ffc107", Order = 1 },
                new ProjectStatus { Name = "Done", Color = "#28a745", Order = 2 }
            };
            await _repo.AddAsync(project);
            await _repo.SaveChangesAsync();

            return true;
        }


        public async Task<bool> UpdateProjectAsync(Project project, int userId)
        {
            var isLeader = await _repo.IsUserProjectLeaderAsync(project.Id, userId);
            if (!isLeader) return false;

            var existing = await _repo.GetByIdAsync(project.Id);
            if (existing == null) return false;

            // Chỉ update những trường được phép
            existing.Name = project.Name;
            existing.Description = project.Description;

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
