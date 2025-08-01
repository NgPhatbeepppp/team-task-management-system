// TeamTaskManagementSystem/Services/ProjectService.cs
using TeamTaskManagementSystem.DTOs;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Exceptions;
using TeamTaskManagementSystem.Interfaces.IAuth_User;
using TeamTaskManagementSystem.Interfaces.IProject;
using TeamTaskManagementSystem.Interfaces.ITeam;

namespace TeamTaskManagementSystem.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IProjectTeamRepository _projectTeamRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;
        // <<< GHI CHÚ: Inject thêm IUserRepository để kiểm tra người dùng có tồn tại không.
        private readonly IUserRepository _userRepository;


        public ProjectService(
            IProjectRepository projectRepository,
            ITeamRepository teamRepository,
            IProjectTeamRepository projectTeamRepository,
            IProjectMemberRepository projectMemberRepository,
            IUserRepository userRepository) 
        {
            _projectRepository = projectRepository;
            _teamRepository = teamRepository;
            _projectTeamRepository = projectTeamRepository;
            _projectMemberRepository = projectMemberRepository;
            _userRepository = userRepository; // Gán repo
        }
        public async Task<ProjectDetailsDto> GetProjectDetailsByIdAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                throw new NotFoundException($"Không tìm thấy dự án với ID {id}.");
            }

            var projectDto = new ProjectDetailsDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                CreatedByUserId = project.CreatedByUserId,
                Members = project.Members.Select(pm => new ProjectMemberDto
                {
                    User = new UserDto
                    {
                        Id = pm.User.Id,
                        Username = pm.User.Username,
                        FullName = pm.User.UserProfile?.FullName
                    },
                    RoleInProject = pm.RoleInProject
                }).ToList(),


                Teams = project.Teams.Select(pt => new TeamDto
                {
                    Id = pt.Team.Id,
                    Name = pt.Team.Name
                }).ToList()
            };

            return projectDto;
        }
        private static string GenerateUniqueKeyCode(string prefix)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomPart = new string(Enumerable.Repeat(chars, 4) // Tạo 4 ký tự ngẫu nhiên
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"{prefix}-{randomPart}";
        }


        // <<< GHI CHÚ: Triển khai phương thức mới với đầy đủ logic nghiệp vụ.
        public async Task AddMemberToProjectAsync(int projectId, int targetUserId, int actorUserId)
        {
            // 1. Kiểm tra quyền: Chỉ có Project Leader mới được thêm thành viên
            if (!await _projectRepository.IsUserProjectLeaderAsync(projectId, actorUserId))
            {
                throw new UnauthorizedAccessException("Chỉ trưởng dự án mới có quyền thêm thành viên.");
            }

            // 2. Kiểm tra người dùng được thêm có tồn tại trong hệ thống không
            var userToAdd = await _userRepository.GetByIdAsync(targetUserId);
            if (userToAdd == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng với ID {targetUserId} để thêm vào dự án.");
            }

            // 3. Kiểm tra xem người dùng đã là thành viên của dự án chưa
            var existingMember = await _projectMemberRepository.FindAsync(projectId, targetUserId);
            if (existingMember != null)
            {
                throw new InvalidOperationException("Người dùng này đã là thành viên của dự án.");
            }

            // 4. Nếu tất cả đều hợp lệ, tiến hành thêm
            var newMember = new ProjectMember
            {
                ProjectId = projectId,
                UserId = targetUserId,
                RoleInProject = "Contributor" // Vai trò mặc định khi thêm
            };

            await _projectMemberRepository.AddAsync(newMember);
            await _projectRepository.SaveChangesAsync(); // Lưu thay đổi
        }

       
        public async Task<IEnumerable<Project>> GetProjectsOfUserAsync(int userId)
        {
            return await _projectRepository.GetProjectsOfUserAsync(userId);
        }

        public async Task<Project> GetByIdAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                throw new NotFoundException($"Không tìm thấy dự án với ID {id}.");
            }
            return project;
        }

        public async Task<Project> CreateProjectAsync(ProjectCreateDto projectDto, int creatorUserId)
        {
            // Chuyển đổi từ DTO sang Entity
            var project = new Project
            {
                Name = projectDto.Name,
                Description = projectDto.Description,
                CreatedByUserId = creatorUserId,
                KeyCode = GenerateUniqueKeyCode("PROJ"),
                CreatedAt = DateTime.UtcNow
            };

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

            return project; // Trả về entity Project hoàn chỉnh
        }

        public async Task UpdateProjectAsync(Project project, int userId)
        {
            if (!await _projectRepository.IsUserProjectLeaderAsync(project.Id, userId))
            {
                throw new UnauthorizedAccessException("Chỉ có trưởng dự án mới có quyền chỉnh sửa.");
            }

            var existingProject = await GetByIdAsync(project.Id);

            existingProject.Name = project.Name;
            existingProject.Description = project.Description;

            _projectRepository.Update(existingProject);
            await _projectRepository.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(int projectId, int userId)
        {
            if (!await _projectRepository.IsUserProjectLeaderAsync(projectId, userId))
            {
                throw new UnauthorizedAccessException("Chỉ có trưởng dự án mới có quyền xóa.");
            }

            var project = await GetByIdAsync(projectId);

            _projectRepository.Delete(project);
            await _projectRepository.SaveChangesAsync();
        }
        public async Task RemoveMemberFromProjectAsync(int projectId, int targetUserId, int actorUserId)
        {
            // 1. Kiểm tra quyền: Chỉ Project Leader mới được xóa
            if (!await _projectRepository.IsUserProjectLeaderAsync(projectId, actorUserId))
            {
                throw new UnauthorizedAccessException("Chỉ trưởng dự án mới có quyền xóa thành viên.");
            }

            // 2. Tìm thành viên trong dự án
            var memberToRemove = await _projectMemberRepository.FindAsync(projectId, targetUserId);
            if (memberToRemove == null)
            {
                throw new NotFoundException($"Không tìm thấy người dùng có ID {targetUserId} trong dự án này.");
            }

            // 3. Quy tắc nghiệp vụ: Không cho phép xóa chính người tạo dự án
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project.CreatedByUserId == targetUserId)
            {
                throw new InvalidOperationException("Không thể xóa người sáng lập ra dự án.");
            }

            // 4. Tiến hành xóa
            _projectMemberRepository.Delete(memberToRemove);
            await _projectRepository.SaveChangesAsync();
        }

        public async Task RemoveTeamFromProjectAsync(int projectId, int teamId, int userId)
        {
            var project = await GetByIdAsync(projectId);
            var team = await _teamRepository.GetByIdWithMembersAsync(teamId);
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
            if (team.Members != null)
            {
                foreach (var teamMember in team.Members)
                {
                    if (teamMember.UserId == projectLeaderId)
                    {
                        continue;
                    }

                    var projectMemberLink = await _projectMemberRepository.FindAsync(projectId, teamMember.UserId);
                    if (projectMemberLink != null)
                    {
                        _projectMemberRepository.Delete(projectMemberLink);
                    }
                }
            }

            await _projectRepository.SaveChangesAsync();
        }
    }
}