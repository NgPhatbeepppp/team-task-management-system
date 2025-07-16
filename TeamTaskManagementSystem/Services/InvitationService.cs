using Microsoft.EntityFrameworkCore;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IProjectInvitationRepository _invitationRepo;
        private readonly IProjectMemberRepository _memberRepo;
        private readonly IProjectTeamRepository _projectTeamRepo; 
        private readonly ITeamMemberRepository _teamMemberRepo;
        private readonly ITeamRepository _teamRepository; 

        public InvitationService(
            IProjectInvitationRepository invitationRepo,
            IProjectMemberRepository memberRepo,
            IProjectTeamRepository projectTeamRepo,
            ITeamMemberRepository teamMemberRepo,
            ITeamRepository teamRepository) 
        {
            _invitationRepo = invitationRepo;
            _memberRepo = memberRepo;
            _projectTeamRepo = projectTeamRepo; 
            _teamMemberRepo = teamMemberRepo;
            _teamRepository = teamRepository; 
        }

        public async Task<bool> AcceptInvitationAsync(int invitationId, int handlerUserId)
        {
            var invitation = await _invitationRepo.GetByIdAsync(invitationId);
            if (invitation == null || invitation.Status != "Pending") return false;

          
            bool isAllowed = false;
            if (invitation.InvitedUserId.HasValue && invitation.InvitedUserId.Value == handlerUserId)
            {
                isAllowed = true; // Cho phép vì là người được mời trực tiếp
            }
            else if (invitation.InvitedTeamId.HasValue)
            {
                // Kiểm tra xem người xử lý có phải là TeamLeader của team được mời không
                isAllowed = await _teamRepository.IsTeamLeaderAsync(invitation.InvitedTeamId.Value, handlerUserId);
            }

            if (!isAllowed)
            {
                return false; // Không có quyền -> Từ chối
            }

            invitation.Status = "Accepted";

            // Nếu là người dùng
            if (invitation.InvitedUserId.HasValue)
            {
                await _memberRepo.AddAsync(new ProjectMember
                {
                    ProjectId = invitation.ProjectId,
                    UserId = invitation.InvitedUserId.Value,
                    RoleInProject = "Contributor" // Sửa luôn lỗi thiếu Role ở đây
                });
            }

            // Nếu là team
            if (invitation.InvitedTeamId.HasValue)
            {
                await _projectTeamRepo.AddAsync(new ProjectTeam
                {
                    ProjectId = invitation.ProjectId,
                    TeamId = invitation.InvitedTeamId.Value
                });

                var teamMembers = await _teamMemberRepo.GetUserIdsByTeamIdAsync(invitation.InvitedTeamId.Value);
                foreach (var userId in teamMembers)
                {
                    await _memberRepo.AddIfNotExistsAsync(invitation.ProjectId, userId);
                }
            }

            await _invitationRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectInvitationAsync(int invitationId, int handlerUserId)
        {
            var invitation = await _invitationRepo.GetByIdAsync(invitationId);
            if (invitation == null || invitation.Status != "Pending") return false;

            
            bool isAllowed = false;
            if (invitation.InvitedUserId.HasValue && invitation.InvitedUserId.Value == handlerUserId)
            {
                isAllowed = true;
            }
            else if (invitation.InvitedTeamId.HasValue)
            {
                isAllowed = await _teamRepository.IsTeamLeaderAsync(invitation.InvitedTeamId.Value, handlerUserId);
            }

            if (!isAllowed)
            {
                return false;
            }

            invitation.Status = "Rejected";
            await _invitationRepo.SaveChangesAsync();
            return true;
        }
    }
}