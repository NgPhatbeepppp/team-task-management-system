using Microsoft.EntityFrameworkCore;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IProjectInvitationRepository _invitationRepo;
        private readonly IProjectMemberRepository _memberRepo;
        private readonly IProjectTeamRepository _teamRepo;
        private readonly ITeamMemberRepository _teamMemberRepo;

        public InvitationService(
            IProjectInvitationRepository invitationRepo,
            IProjectMemberRepository memberRepo,
            IProjectTeamRepository teamRepo,
            ITeamMemberRepository teamMemberRepo)
        {
            _invitationRepo = invitationRepo;
            _memberRepo = memberRepo;
            _teamRepo = teamRepo;
            _teamMemberRepo = teamMemberRepo;
        }

        public async Task<bool> AcceptInvitationAsync(int invitationId, int handlerUserId)
        {
            var invitation = await _invitationRepo.GetByIdAsync(invitationId);
            if (invitation == null || invitation.Status != "Pending") return false;

            // Kiểm tra quyền hợp lệ (optional: chỉ cho người được mời chấp nhận)
            if (invitation.InvitedUserId.HasValue && invitation.InvitedUserId != handlerUserId)
                return false;

            invitation.Status = "Accepted";

            // Nếu là người dùng
            if (invitation.InvitedUserId.HasValue)
            {
                await _memberRepo.AddAsync(new ProjectMember
                {
                    ProjectId = invitation.ProjectId,
                    UserId = invitation.InvitedUserId.Value
                });
            }

            // Nếu là team
            if (invitation.InvitedTeamId.HasValue)
            {
                await _teamRepo.AddAsync(new ProjectTeam
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

            if (invitation.InvitedUserId.HasValue && invitation.InvitedUserId != handlerUserId)
                return false;

            invitation.Status = "Rejected";
            await _invitationRepo.SaveChangesAsync();
            return true;
        }
    }
}
