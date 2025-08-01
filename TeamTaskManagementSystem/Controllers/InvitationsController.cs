using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagementSystem.Interfaces.Iinvitation;
using TeamTaskManagementSystem.DTOs;
namespace TeamTaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/invitations")]
    [Authorize]
    public class InvitationsController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public InvitationsController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }
    [HttpGet("pending")]
    public async Task<ActionResult<List<InvitationDto>>> GetMyPendingInvitations()
    {
        var invitations = await _invitationService.GetPendingInvitationsForUserAsync(GetUserId());
        return Ok(invitations);
    }
        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("{invitationId}/accept")]
        public async Task<IActionResult> Accept(int invitationId)
        {
            var result = await _invitationService.AcceptInvitationAsync(invitationId, GetUserId());
            return result ? Ok("Invitation accepted.") : BadRequest("Cannot accept invitation.");
        }

        [HttpPost("{invitationId}/reject")]
        public async Task<IActionResult> Reject(int invitationId)
        {
            var result = await _invitationService.RejectInvitationAsync(invitationId, GetUserId());
            return result ? Ok("Invitation rejected.") : BadRequest("Cannot reject invitation.");
        }
    }
}
