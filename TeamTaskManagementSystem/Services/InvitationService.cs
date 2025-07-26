// TeamTaskManagementSystem/Services/InvitationService.cs

using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces.Iinvitation;
using TeamTaskManagementSystem.Interfaces.IProject;
using TeamTaskManagementSystem.Interfaces.ITeam;

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

            bool isAllowed = (invitation.InvitedUserId.HasValue && invitation.InvitedUserId.Value == handlerUserId) ||
                             (invitation.InvitedTeamId.HasValue && await _teamRepository.IsTeamLeaderAsync(invitation.InvitedTeamId.Value, handlerUserId));

            if (!isAllowed) return false;

            invitation.Status = "Accepted";

            if (invitation.InvitedUserId.HasValue)
            {
                await _memberRepo.AddAsync(new ProjectMember
                {
                    ProjectId = invitation.ProjectId,
                    UserId = invitation.InvitedUserId.Value,
                    RoleInProject = "Contributor"
                });
            }

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
                    // <<< GHI CHÚ: Sửa lỗi tại đây
                    // Thay vì gọi `AddIfNotExistsAsync`, chúng ta sẽ thực hiện logic kiểm tra ngay tại đây.
                    var memberExists = await _memberRepo.FindAsync(invitation.ProjectId, userId);
                    if (memberExists == null) // Chỉ thêm nếu thành viên chưa có trong dự án
                    {
                        await _memberRepo.AddAsync(new ProjectMember
                        {
                            ProjectId = invitation.ProjectId,
                            UserId = userId,
                            RoleInProject = "Contributor"
                        });
                    }
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