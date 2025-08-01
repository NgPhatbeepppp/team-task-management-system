using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces.IAuth_User;
using TeamTaskManagementSystem.Interfaces.Iinvitation;
using TeamTaskManagementSystem.Interfaces.ITeam;

namespace TeamTaskManagementSystem.Services
{
    public class ProjectInvitationService : IProjectInvitationService
    {
        private readonly IProjectInvitationRepository _invitationRepo;
        private readonly IUserRepository _userRepo;
        private readonly ITeamRepository _teamRepo;

        public ProjectInvitationService(
            IProjectInvitationRepository invitationRepo,
            IUserRepository userRepo,
            ITeamRepository teamRepo) 
        {
            _invitationRepo = invitationRepo;
            _userRepo = userRepo;
            _teamRepo = teamRepo; 
        }

        public async Task<bool> InviteUserAsync(int projectId, string identifier, int invitedByUserId)
        {
            var user = await _userRepo.GetUserByEmailOrUsernameAsync(identifier);
            if (user == null) return false;

            var exists = await _invitationRepo.InvitationExistsAsync(projectId, user.Id, null);
            if (exists) return false;

            var invitation = new ProjectInvitation
            {
                ProjectId = projectId,
                InvitedUserId = user.Id,
                InvitedByUserId = invitedByUserId,
                Status = "Pending"
            };

            await _invitationRepo.AddInvitationAsync(invitation);
            await _invitationRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InviteTeamAsync(int projectId, string teamKeyCode, int invitedByUserId)
        {
            // 1. Tìm team bằng KeyCode
            var team = await _teamRepo.GetByKeyCodeAsync(teamKeyCode);
            if (team == null) return false; // Không tìm thấy team

            // 2. Kiểm tra xem lời mời đã tồn tại chưa (sử dụng team.Id)
            var exists = await _invitationRepo.InvitationExistsAsync(projectId, null, team.Id);
            if (exists) return false;

            // 3. Tạo lời mời với team.Id
            var invitation = new ProjectInvitation
            {
                ProjectId = projectId,
                InvitedTeamId = team.Id, // Sử dụng ID nội bộ để tạo mối quan hệ
                InvitedByUserId = invitedByUserId,
                Status = "Pending"
            };

            await _invitationRepo.AddInvitationAsync(invitation);
            await _invitationRepo.SaveChangesAsync();
            return true;
        }
    }
}

