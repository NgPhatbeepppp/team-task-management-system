using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;
using System.Threading.Tasks;

namespace TeamTaskManagementSystem.Services
{
    public class TeamInvitationService : ITeamInvitationService
    {
        private readonly ITeamRepository _teamRepo;
        private readonly IUserRepository _userRepo;
        private readonly ITeamInvitationRepository _invitationRepo; // ✨ THAY THẾ DbContext BẰNG REPO

        public TeamInvitationService(ITeamRepository teamRepo, IUserRepository userRepo, ITeamInvitationRepository invitationRepo)
        {
            _teamRepo = teamRepo;
            _userRepo = userRepo;
            _invitationRepo = invitationRepo; // ✨ GÁN REPO
        }

        public async Task<bool> InviteUserToTeamAsync(int teamId, int targetUserId, int inviterUserId)
        {
            if (!await _teamRepo.IsTeamLeaderAsync(teamId, inviterUserId))
                return false;

            if (await _userRepo.GetByIdAsync(targetUserId) == null)
                return false;

            var isAlreadyMember = await _teamRepo.IsMemberAsync(teamId, targetUserId);
            var hasPendingInvitation = await _invitationRepo.HasPendingInvitationAsync(teamId, targetUserId); // ✨ SỬ DỤNG REPO

            if (isAlreadyMember || hasPendingInvitation)
                return false;

            var invitation = new TeamInvitation
            {
                TeamId = teamId,
                InvitedUserId = targetUserId,
                InvitedByUserId = inviterUserId
            };

            await _invitationRepo.AddAsync(invitation); // ✨ SỬ DỤNG REPO
            return await _invitationRepo.SaveChangesAsync(); // ✨ SỬ DỤNG REPO
        }

        public async Task<bool> AcceptInvitationAsync(int invitationId, int handlerUserId)
        {
            var invitation = await _invitationRepo.GetByIdAsync(invitationId); // ✨ SỬ DỤNG REPO

            if (invitation == null || invitation.Status != "Pending" || invitation.InvitedUserId != handlerUserId)
                return false;

            invitation.Status = "Accepted";
            _invitationRepo.Update(invitation); // Báo cho EF Core biết là có thay đổi

            // Thêm thành viên vào team
            await _teamRepo.AddMemberAsync(invitation.TeamId, invitation.InvitedUserId);

            // SaveChanges trong AddMember của TeamRepository đã xử lý việc lưu
            // nên ta không cần gọi lại. Nếu AddMember không có SaveChanges,
            // bạn sẽ cần gọi await _invitationRepo.SaveChangesAsync(); ở đây.

            return true;
        }

        public async Task<bool> RejectInvitationAsync(int invitationId, int handlerUserId)
        {
            var invitation = await _invitationRepo.GetByIdAsync(invitationId); // ✨ SỬ DỤNG REPO

            if (invitation == null || invitation.Status != "Pending" || invitation.InvitedUserId != handlerUserId)
                return false;

            invitation.Status = "Rejected";
            _invitationRepo.Update(invitation); // Báo cho EF Core biết là có thay đổi

            return await _invitationRepo.SaveChangesAsync(); // ✨ SỬ DỤNG REPO
        }
    }
}