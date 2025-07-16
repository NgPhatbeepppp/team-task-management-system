// TeamTaskManagementSystem/Controllers/ProjectInvitationsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagementSystem.Dtos.ProjectInvitations;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/invitations")]
    [Authorize]
    public class ProjectInvitationsController : ControllerBase
    {
        private readonly IProjectInvitationService _invitationService;
        private readonly IProjectRepository _projectRepo; // <<< BỔ SUNG REPO ĐỂ KIỂM TRA QUYỀN

        public ProjectInvitationsController(IProjectInvitationService invitationService, IProjectRepository projectRepo) // <<< CẬP NHẬT CONSTRUCTOR
        {
            _invitationService = invitationService;
            _projectRepo = projectRepo; // <<< GÁN REPO
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("user")]
        public async Task<IActionResult> InviteUser(int projectId, [FromBody] InviteUserDto dto)
        {
            // --- KIỂM TRA QUYỀN TRƯỚC KHI MỜI ---
            var isLeader = await _projectRepo.IsUserProjectLeaderAsync(projectId, GetUserId());
            if (!isLeader)
            {
                return Forbid("Bạn không có quyền mời thành viên vào dự án này.");
            }
            // ------------------------------------

            var success = await _invitationService.InviteUserAsync(projectId, dto.Identifier, GetUserId());
            return success ? Ok("Đã gửi lời mời tới người dùng.") : BadRequest("Gửi lời mời thất bại hoặc lời mời đã tồn tại.");
        }

        [HttpPost("team")]
        public async Task<IActionResult> InviteTeam(int projectId, [FromBody] InviteTeamDto dto)
        {
            // --- KIỂM TRA QUYỀN TRƯỚC KHI MỜI ---
            var isLeader = await _projectRepo.IsUserProjectLeaderAsync(projectId, GetUserId());
            if (!isLeader)
            {
                return Forbid("Bạn không có quyền mời nhóm vào dự án này.");
            }
            // ------------------------------------

            var success = await _invitationService.InviteTeamAsync(projectId, dto.InvitedTeamId, GetUserId());
            return success ? Ok("Đã gửi lời mời tới nhóm.") : BadRequest("Gửi lời mời thất bại hoặc lời mời đã tồn tại.");
        }
    }
}