// TeamTaskManagementSystem/Services/TeamService.cs
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Exceptions;
using TeamTaskManagementSystem.Interfaces.IProject;
using TeamTaskManagementSystem.Interfaces.ITeam;

namespace TeamTaskManagementSystem.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IProjectService _projectService;

        public TeamService(ITeamRepository teamRepository, IProjectService projectService)
        {
            _teamRepository = teamRepository;
            _projectService = projectService;
        }
        private static string GenerateUniqueKeyCode(string prefix)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomPart = new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"{prefix}-{randomPart}";
        }
        public async Task<IEnumerable<Team>> GetTeamsByUserIdAsync(int userId)
        {
            return await _teamRepository.GetTeamsByUserIdAsync(userId);
        }
        
        public async Task CreateTeamAsync(Team team, int creatorUserId)
        {
            team.KeyCode = GenerateUniqueKeyCode("TEAM");
            team.CreatedByUserId = creatorUserId;
            team.CreatedAt = DateTime.UtcNow;

            // Thêm bản thân người tạo làm TeamLeader
            team.Members.Add(new TeamMember
            {
                UserId = creatorUserId,
                RoleInTeam = "TeamLeader"
            });

            await _teamRepository.AddAsync(team);
            await _teamRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await _teamRepository.GetAllAsync();
        }

        public async Task<Team> GetTeamByIdAsync(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
            {
                throw new NotFoundException("Không tìm thấy team.");
            }
            return team;
        }

        public async Task UpdateTeamAsync(Team team, int updaterUserId)
        {
            if (!await _teamRepository.IsTeamLeaderAsync(team.Id, updaterUserId))
            {
                throw new UnauthorizedAccessException("Chỉ trưởng nhóm mới có quyền chỉnh sửa.");
            }
            var existingTeam = await GetTeamByIdAsync(team.Id); // Sử dụng lại GetById để kiểm tra tồn tại

            existingTeam.Name = team.Name;
            existingTeam.Description = team.Description;

            _teamRepository.Update(existingTeam);
            await _teamRepository.SaveChangesAsync();
        }

        public async Task DeleteTeamAsync(int teamId, int userId)
        {
            var team = await _teamRepository.GetByIdWithProjectsAsync(teamId);
            if (team == null) throw new NotFoundException("Không tìm thấy team.");

            if (team.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("Chỉ có người tạo team (trưởng nhóm đầu tiên) mới được quyền xóa team.");
            }

            if (team.Projects != null && team.Projects.Any())
            {
                throw new InvalidOperationException("Team đang tham gia ít nhất một dự án. Bạn cần yêu cầu team rời khỏi tất cả các dự án trước khi xóa.");
            }

            _teamRepository.Delete(team);
            await _teamRepository.SaveChangesAsync();
        }

        
        public async Task LeaveAllProjectsAsync(int teamId, int userId)
        {
            var team = await _teamRepository.GetByIdWithProjectsAsync(teamId);
            if (team == null) throw new NotFoundException("Không tìm thấy team.");

            if (team.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("Chỉ có trưởng nhóm mới có quyền thực hiện hành động này.");
            }

            if (team.Projects == null || !team.Projects.Any())
            {
                return; // Không có gì để làm
            }

            var projectIds = team.Projects.Select(p => p.ProjectId).ToList();
            foreach (var projectId in projectIds)
            {
                await _projectService.RemoveTeamFromProjectAsync(projectId, teamId, userId);
            }
        }

        public async Task AddMemberAsync(int teamId, int targetUserId, int actorUserId)
        {
            if (!await _teamRepository.IsTeamLeaderAsync(teamId, actorUserId))
            {
                throw new UnauthorizedAccessException("Chỉ trưởng nhóm mới có quyền thêm thành viên.");
            }

            if (await _teamRepository.IsMemberAsync(teamId, targetUserId))
            {
                throw new InvalidOperationException("Thành viên đã có trong team.");
            }

            var newMember = new TeamMember { TeamId = teamId, UserId = targetUserId, RoleInTeam = "Member" };
            await _teamRepository.AddMemberAsync(newMember);
            await _teamRepository.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(int teamId, int targetUserId, int actorUserId)
        {
            // Kiểm tra xem thành viên mục tiêu có tồn tại trong nhóm không
            var memberToRemove = await _teamRepository.GetTeamMemberAsync(teamId, targetUserId);
            if (memberToRemove == null)
            {
                throw new NotFoundException("Không tìm thấy thành viên trong team.");
            }

            // Lấy thông tin vai trò của người thực hiện hành động
            bool isActorLeader = await _teamRepository.IsTeamLeaderAsync(teamId, actorUserId);
            bool isSelfRemoval = (actorUserId == targetUserId);

            // Áp dụng logic kiểm tra quyền mới:
            // Hành động hợp lệ nếu:
            // 1. Người thực hiện là Trưởng nhóm.
            // 2. Hoặc người thực hiện đang tự xóa chính mình (rời nhóm).
            if (!isActorLeader && !isSelfRemoval)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền thực hiện hành động này.");
            }

            // Logic bổ sung: Ngăn trưởng nhóm cuối cùng rời khỏi nhóm
            if (isSelfRemoval && memberToRemove.RoleInTeam == "TeamLeader")
            {
                var leaders = await _teamRepository.GetTeamLeadersAsync(teamId);
                if (leaders.Count() <= 1)
                {
                    throw new InvalidOperationException("Bạn là trưởng nhóm cuối cùng. Vui lòng trao quyền cho người khác trước khi rời nhóm.");
                }
            }

            // Nếu tất cả kiểm tra đều qua, tiến hành xóa
            await _teamRepository.RemoveMemberAsync(memberToRemove);
            await _teamRepository.SaveChangesAsync();
        }

        public async Task GrantTeamLeaderAsync(int teamId, int targetUserId, int actorUserId)
        {
            // 1. Kiểm tra quyền của người thực hiện
            var actor = await _teamRepository.GetTeamMemberAsync(teamId, actorUserId);
            if (actor == null || actor.RoleInTeam != "TeamLeader")
            {
                throw new UnauthorizedAccessException("Chỉ trưởng nhóm mới có thể chuyển quyền.");
            }

            // 2. Kiểm tra người được trao quyền
            var targetMember = await _teamRepository.GetTeamMemberAsync(teamId, targetUserId);
            if (targetMember == null)
            {
                throw new NotFoundException("Thành viên không tồn tại trong team để trao quyền.");
            }
            if (targetMember.UserId == actorUserId)
            {
                throw new InvalidOperationException("Bạn đã là trưởng nhóm rồi.");
            }

            // 3. Hạ quyền của trưởng nhóm hiện tại (người thực hiện)
            actor.RoleInTeam = "Member";

            // 4. Nâng quyền cho thành viên mục tiêu
            targetMember.RoleInTeam = "TeamLeader";

            // 5. Lưu tất cả thay đổi vào CSDL
            // SaveChangesAsync sẽ tự động lưu cả 2 sự thay đổi trên trong một transaction
            await _teamRepository.SaveChangesAsync();
        }
    }
}