// TeamTaskManagementSystem/Services/TeamInvitationService.cs
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;
using System.Threading.Tasks;
using TeamTaskManagementSystem.Interfaces.IAuth_User;
using TeamTaskManagementSystem.Interfaces.Iinvitation;
using TeamTaskManagementSystem.Interfaces.ITeam;
using TeamTaskManagementSystem.DTOs.TeamInvitation;

namespace TeamTaskManagementSystem.Services
{
    public class TeamInvitationService : ITeamInvitationService
    {
        private readonly ITeamRepository _teamRepo;
        private readonly IUserRepository _userRepo;
        private readonly ITeamInvitationRepository _invitationRepo;

        public TeamInvitationService(ITeamRepository teamRepo, IUserRepository userRepo, ITeamInvitationRepository invitationRepo)
        {
            _teamRepo = teamRepo;
            _userRepo = userRepo;
            _invitationRepo = invitationRepo;
        }

        public async Task<bool> AcceptInvitationAsync(int invitationId, int handlerUserId)
        {
            var invitation = await _invitationRepo.GetByIdAsync(invitationId);

            if (invitation == null || invitation.Status != "Pending" || invitation.InvitedUserId != handlerUserId)
                return false;

            invitation.Status = "Accepted";
            _invitationRepo.Update(invitation);

           
            // Tạo một đối tượng TeamMember mới và truyền vào phương thức AddMemberAsync.
            var newMember = new TeamMember
            {
                TeamId = invitation.TeamId,
                UserId = invitation.InvitedUserId,
                RoleInTeam = "Member" 
            };
            await _teamRepo.AddMemberAsync(newMember);

            // Lưu tất cả các thay đổi (cả update status và add member)
            return await _teamRepo.SaveChangesAsync();
        }

        
        public async Task<bool> InviteUserToTeamAsync(int teamId, int targetUserId, int inviterUserId)
        {
            if (!await _teamRepo.IsTeamLeaderAsync(teamId, inviterUserId))
                return false;

            if (await _userRepo.GetByIdAsync(targetUserId) == null)
                return false;

            var isAlreadyMember = await _teamRepo.IsMemberAsync(teamId, targetUserId);
            var hasPendingInvitation = await _invitationRepo.HasPendingInvitationAsync(teamId, targetUserId);

            if (isAlreadyMember || hasPendingInvitation)
                return false;

            var invitation = new TeamInvitation
            {
                TeamId = teamId,
                InvitedUserId = targetUserId,
                InvitedByUserId = inviterUserId
            };

            await _invitationRepo.AddAsync(invitation);
            return await _invitationRepo.SaveChangesAsync();
        }

        public async Task<bool> RejectInvitationAsync(int invitationId, int handlerUserId)
        {
            var invitation = await _invitationRepo.GetByIdAsync(invitationId);

            if (invitation == null || invitation.Status != "Pending" || invitation.InvitedUserId != handlerUserId)
                return false;

            invitation.Status = "Rejected";
            _invitationRepo.Update(invitation);

            return await _invitationRepo.SaveChangesAsync();
        }
        // ✨ TRIỂN KHAI LOGIC TÌM KIẾM VÀ KIỂM TRA TRẠNG THÁI
        public async Task<IEnumerable<UserSearchResponseDto>> SearchUsersForInvitationAsync(int teamId, string query)
        {
            // 1. Tìm kiếm user theo query
            var foundUsers = await _userRepo.SearchUsersAsync(query);
            if (!foundUsers.Any())
            {
                return Enumerable.Empty<UserSearchResponseDto>();
            }

            var userIds = foundUsers.Select(u => u.Id).ToList();

            // 2. Lấy thông tin thành viên và lời mời đang chờ của những user này cho team cụ thể
            var teamMembers = await _teamRepo.GetTeamMembersByUserIdsAsync(teamId, userIds);
            var pendingInvitations = await _invitationRepo.GetPendingInvitationsForUsersAsync(teamId, userIds);

            var memberUserIds = new HashSet<int>(teamMembers.Select(m => m.UserId));
            var pendingUserIds = new HashSet<int>(pendingInvitations.Select(i => i.InvitedUserId));

            // 3. Xây dựng kết quả trả về
            var results = foundUsers.Select(user => new UserSearchResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                StatusInTeam = memberUserIds.Contains(user.Id) ? "Member"
                             : pendingUserIds.Contains(user.Id) ? "Pending"
                             : "NotInvited"
            }).ToList();

            return results;
        }
    }
}