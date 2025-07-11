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

        public ProjectInvitationsController(IProjectInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("user")]
        public async Task<IActionResult> InviteUser(int projectId, [FromBody] InviteUserDto dto)
        {
            var success = await _invitationService.InviteUserAsync(projectId, dto.Identifier, GetUserId());
            return success ? Ok("Invitation sent.") : BadRequest("Invitation failed or already exists.");
        }

        [HttpPost("team")]
        public async Task<IActionResult> InviteTeam(int projectId, [FromBody] InviteTeamDto dto)
        {
            var success = await _invitationService.InviteTeamAsync(projectId, dto.InvitedTeamId, GetUserId());
            return success ? Ok("Team invitation sent.") : BadRequest("Invitation failed or already exists.");
        }
    }
}
