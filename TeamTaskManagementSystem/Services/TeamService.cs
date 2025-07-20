using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Exceptions; // Đảm bảo bạn đã tạo file này
using TeamTaskManagementSystem.Interfaces;
using TeamTaskManagementSystem.Repositories;

namespace TeamTaskManagementSystem.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IProjectService _projectService; // Thêm project service để xử lý logic rời project

        
        public TeamService(ITeamRepository teamRepository, IProjectService projectService)
        {
            _teamRepository = teamRepository;
            _projectService = projectService;
        }
        public async Task LeaveAllProjectsAsync(int teamId, int userId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                throw new NotFoundException("Không tìm thấy team.");
            }

            if (team.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("Chỉ có trưởng nhóm mới có quyền thực hiện hành động này.");
            }

            if (team.Projects == null || !team.Projects.Any())
            {
                // Không có gì để làm
                return;
            }

            // Tạo một bản sao của danh sách project để duyệt, vì collection gốc sẽ bị thay đổi
            var projectsToRemove = team.Projects.ToList();

            foreach (var projectLink in projectsToRemove)
            {
                // Gọi phương thức đã tạo ở ProjectService
                // userId ở đây được truyền vào để kiểm tra quyền trong từng project
                await _projectService.RemoveTeamFromProjectAsync(projectLink.ProjectId, teamId, userId);
            }
        }
        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await _teamRepository.GetAllAsync();
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _teamRepository.GetByIdAsync(id);
        }

        public async Task CreateTeamAsync(Team team, int creatorUserId)
        {
            // Service nên là void hoặc trả về DTO, không nên trả về bool
            // Lỗi sẽ được xử lý bằng Exception
            await _teamRepository.CreateTeamAsync(team, creatorUserId);
        }

        public async Task UpdateTeamAsync(Team team)
        {
            _teamRepository.Update(team);
            await _teamRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Xóa một team. Chỉ xóa được khi team không còn tham gia dự án nào.
        /// </summary>
        public async Task DeleteTeamAsync(int teamId, int userId)
        {
            var team = await _teamRepository.GetByIdWithProjectsAsync(teamId); // Lấy team kèm thông tin project
            if (team == null)
            {
                throw new NotFoundException("Không tìm thấy team.");
            }

            if (team.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("Chỉ có trưởng nhóm mới được quyền xóa team.");
            }

            // Logic nghiệp vụ: Không cho xóa khi team còn tham gia project
            if (team.Projects != null && team.Projects.Any())
            {
                throw new InvalidOperationException("Team đang tham gia ít nhất một dự án. Bạn cần xóa team khỏi tất cả các dự án trước khi xóa team.");
            }

            _teamRepository.Delete(team);
            await _teamRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Yêu cầu team rời khỏi tất cả các dự án đang tham gia.
        /// </summary>
        public async Task LeaveAllProjectsAsync(int teamId, int userId)
        {
            var team = await _teamRepository.GetByIdWithProjectsAsync(teamId);
            if (team == null)
            {
                throw new NotFoundException("Không tìm thấy team.");
            }

            if (team.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("Chỉ có trưởng nhóm mới có quyền thực hiện hành động này.");
            }

            if (team.Projects == null || !team.Projects.Any())
            {
                return; // Không có gì để làm
            }

            // Tạo một bản sao của danh sách project ID để tránh lỗi "Collection was modified"
            var projectIds = team.Projects.Select(p => p.ProjectId).ToList();

            foreach (var projectId in projectIds)
            {
                // Gọi phương thức từ ProjectService để xử lý logic xóa team khỏi từng project
                await _projectService.RemoveTeamFromProjectAsync(projectId, teamId, userId);
            }
        }

        public async Task AddMemberAsync(int teamId, int userId)
        {
            var exists = await _teamRepository.IsMemberAsync(teamId, userId);
            if (exists)
            {
                // Ném lỗi rõ ràng thay vì trả về false
                throw new InvalidOperationException("Thành viên đã có trong team.");
            }
            await _teamRepository.AddMemberAsync(teamId, userId);
        }

        public async Task RemoveMemberAsync(int teamId, int userId)
        {
            await _teamRepository.RemoveMemberAsync(teamId, userId);
        }

        public async Task GrantTeamLeaderAsync(int teamId, int targetUserId)
        {
            await _teamRepository.GrantTeamLeaderAsync(teamId, targetUserId);
        }
    }
}