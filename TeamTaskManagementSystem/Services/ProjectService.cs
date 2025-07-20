using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Exceptions;
using TeamTaskManagementSystem.Interfaces;
using TeamTaskManagementSystem.Repositories;

namespace TeamTaskManagementSystem.Services
{
    public class ProjectService : IProjectService
    {
       
        private readonly IProjectRepository _projectRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IProjectTeamRepository _projectTeamRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;

        // 2. Constructor duy nhất, nhận tất cả dependency
        public ProjectService(
            IProjectRepository projectRepository,
            ITeamRepository teamRepository,
            IProjectTeamRepository projectTeamRepository,
            IProjectMemberRepository projectMemberRepository)
        {
            _projectRepository = projectRepository;
            _teamRepository = teamRepository;
            _projectTeamRepository = projectTeamRepository;
            _projectMemberRepository = projectMemberRepository;
        }

        public async Task<IEnumerable<Project>> GetProjectsOfUserAsync(int userId)
        {
            return await _projectRepository.GetProjectsOfUserAsync(userId);
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _projectRepository.GetByIdAsync(id);
        }

        // 3. Chuyển sang dùng void và Exception để xử lý lỗi
        public async Task CreateProjectAsync(Project project, int creatorUserId)
        {
            project.CreatedByUserId = creatorUserId;
            project.CreatedAt = DateTime.UtcNow;

            project.Members.Add(new ProjectMember
            {
                UserId = creatorUserId,
                RoleInProject = "ProjectLeader"
            });

            project.Statuses = new List<ProjectStatus>
            {
                new ProjectStatus { Name = "To Do", Color = "#007bff", Order = 0 },
                new ProjectStatus { Name = "In Progress", Color = "#ffc107", Order = 1 },
                new ProjectStatus { Name = "Done", Color = "#28a745", Order = 2 }
            };

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();
        }

        // 4. Chuyển sang dùng void và Exception để xử lý lỗi
        public async Task UpdateProjectAsync(Project project, int userId)
        {
            var isLeader = await _projectRepository.IsUserProjectLeaderAsync(project.Id, userId);
            if (!isLeader)
            {
                throw new UnauthorizedAccessException("Chỉ có trưởng dự án mới có quyền chỉnh sửa.");
            }

            var existingProject = await _projectRepository.GetByIdAsync(project.Id);
            if (existingProject == null)
            {
                throw new NotFoundException("Không tìm thấy dự án để cập nhật.");
            }

            existingProject.Name = project.Name;
            existingProject.Description = project.Description;

            _projectRepository.Update(existingProject); // Update thực thể đang được theo dõi
            await _projectRepository.SaveChangesAsync();
        }

        // 5. Chuyển sang dùng void và Exception để xử lý lỗi
        public async Task DeleteProjectAsync(int projectId, int userId)
        {
            var isLeader = await _projectRepository.IsUserProjectLeaderAsync(projectId, userId);
            if (!isLeader)
            {
                throw new UnauthorizedAccessException("Chỉ có trưởng dự án mới có quyền xóa.");
            }

            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                throw new NotFoundException("Không tìm thấy dự án để xóa.");
            }

            _projectRepository.Delete(project);
            await _projectRepository.SaveChangesAsync();
        }

        // 6. Đặt phương thức ở đúng vị trí, là một phương thức của class
        public async Task RemoveTeamFromProjectAsync(int projectId, int teamId, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null) throw new NotFoundException("Không tìm thấy dự án.");

            var team = await _teamRepository.GetByIdWithMembersAsync(teamId); // Cần lấy team kèm thành viên
            if (team == null) throw new NotFoundException("Không tìm thấy team.");

            if (project.CreatedByUserId != userId && team.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("Chỉ trưởng dự án hoặc trưởng nhóm mới có quyền thực hiện hành động này.");
            }

            var projectTeamLink = await _projectTeamRepository.FindAsync(projectId, teamId);
            if (projectTeamLink != null)
            {
                _projectTeamRepository.Delete(projectTeamLink);
            }

            var projectLeaderId = project.CreatedByUserId;
            foreach (var teamMember in team.Members)
            {
                if (teamMember.UserId == projectLeaderId)
                {
                    continue; // Giữ lại Project Leader
                }

                var projectMemberLink = await _projectMemberRepository.FindAsync(projectId, teamMember.UserId);
                if (projectMemberLink != null)
                {
                    _projectMemberRepository.Delete(projectMemberLink);
                }
            }

            await _projectRepository.SaveChangesAsync();
        }
    }
}