// TeamTaskManagementSystem/Services/TeamService.cs
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Exceptions;
using TeamTaskManagementSystem.Interfaces;

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

        // <<< GHI CHÚ: Toàn bộ logic tạo team và gán leader được đưa về đây.
        public async Task CreateTeamAsync(Team team, int creatorUserId)
        {
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

        // <<< GHI CHÚ: Xóa phương thức bị trùng lặp.
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
            if (!await _teamRepository.IsTeamLeaderAsync(teamId, actorUserId))
            {
                throw new UnauthorizedAccessException("Chỉ trưởng nhóm mới có quyền xóa thành viên.");
            }

            var member = await _teamRepository.GetTeamMemberAsync(teamId, targetUserId);
            if (member == null)
            {
                throw new NotFoundException("Không tìm thấy thành viên trong team.");
            }

            await _teamRepository.RemoveMemberAsync(member);
            await _teamRepository.SaveChangesAsync();
        }

        public async Task GrantTeamLeaderAsync(int teamId, int targetUserId, int actorUserId)
        {
            if (!await _teamRepository.IsTeamLeaderAsync(teamId, actorUserId))
            {
                throw new UnauthorizedAccessException("Chỉ trưởng nhóm mới có thể chuyển quyền.");
            }

            var member = await _teamRepository.GetTeamMemberAsync(teamId, targetUserId);
            if (member == null)
            {
                throw new NotFoundException("Thành viên không tồn tại trong team để trao quyền.");
            }

            member.RoleInTeam = "TeamLeader";
            await _teamRepository.SaveChangesAsync();
        }
    }
}