// TeamTaskManagementSystem/Services/InvitationService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamTaskManagementSystem.DTOs;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces.Iinvitation;
using TeamTaskManagementSystem.Interfaces.IProject;
using TeamTaskManagementSystem.Interfaces.ITeam;

namespace TeamTaskManagementSystem.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IProjectInvitationRepository _projectInvitationRepo;
        private readonly ITeamInvitationRepository _teamInvitationRepo;
        private readonly IProjectMemberRepository _memberRepo;
        private readonly IProjectTeamRepository _projectTeamRepo;
        private readonly ITeamMemberRepository _teamMemberRepo;
        private readonly ITeamRepository _teamRepository;

        public InvitationService(
            IProjectInvitationRepository projectInvitationRepo,
            ITeamInvitationRepository teamInvitationRepo,
            IProjectMemberRepository memberRepo,
            IProjectTeamRepository projectTeamRepo,
            ITeamMemberRepository teamMemberRepo,
            ITeamRepository teamRepository)
        {
            _projectInvitationRepo = projectInvitationRepo;
            _teamInvitationRepo = teamInvitationRepo;
            _memberRepo = memberRepo;
            _projectTeamRepo = projectTeamRepo;
            _teamMemberRepo = teamMemberRepo;
            _teamRepository = teamRepository;
        }

        public async Task<List<InvitationDto>> GetPendingInvitationsForUserAsync(int userId)
        {
            var allInvitations = new List<InvitationDto>();

            // 1. Lấy lời mời vào Nhóm
            var teamInvitations = await _teamInvitationRepo.GetPendingInvitationsByUserIdAsync(userId);
            allInvitations.AddRange(teamInvitations.Select(i => new InvitationDto
            {
                InvitationId = i.Id,
                InvitationType = "Team",
                TargetId = i.TeamId,
                TargetName = i.Team?.Name ?? "N/A",
                InviterName = i.InvitedByUser?.Username ?? "N/A",
                SentAt = i.CreatedAt
            }));

            // 2. Lấy lời mời vào Dự án
            var userTeamIds = (await _teamRepository.GetTeamsByUserIdAsync(userId)).Select(t => t.Id).ToList();
            var projectInvitations = await _projectInvitationRepo.GetPendingInvitationsForUserAsync(userId, userTeamIds);

            allInvitations.AddRange(projectInvitations.Select(i => new InvitationDto
            {
                InvitationId = i.Id,
                InvitationType = "Project",
                TargetId = i.ProjectId,
                TargetName = i.Project?.Name ?? "N/A",
                InviterName = i.InvitedByUser?.Username ?? "N/A",
                SentAt = i.CreatedAt
            }));

            // 3. Hợp nhất và sắp xếp
            return allInvitations.OrderByDescending(i => i.SentAt).ToList();
        }

        public async Task<bool> AcceptInvitationAsync(int invitationId, int handlerUserId)
        {
            var invitation = await _projectInvitationRepo.GetByIdAsync(invitationId);
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
                    var memberExists = await _memberRepo.FindAsync(invitation.ProjectId, userId);
                    if (memberExists == null)
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

            await _projectInvitationRepo.SaveChangesAsync();
            return true;
        }


        public async Task<bool> RejectInvitationAsync(int invitationId, int handlerUserId)
        {
            var invitation = await _projectInvitationRepo.GetByIdAsync(invitationId);
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
            await _projectInvitationRepo.SaveChangesAsync();
            return true;
        }
    }
}