using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Controllers
{
    // === CONTROLLER ĐỂ GỬI LỜI MỜI ===
    [ApiController]
    [Route("api/teams/{teamId}/invitations")]
    [Authorize]
    public class TeamInvitationsController : ControllerBase
    {
        private readonly ITeamInvitationService _invitationService;

        public TeamInvitationsController(ITeamInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Mời một người dùng tham gia vào nhóm (chỉ TeamLeader có thể thực hiện).
        /// </summary>
        /// <param name="teamId">ID của nhóm muốn mời vào.</param>
        /// <param name="targetUserId">ID của người dùng được mời.</param>
        [HttpPost]
        public async Task<IActionResult> InviteUserToTeam(int teamId, [FromBody] int targetUserId)
        {
            var inviterId = GetUserId();
            var success = await _invitationService.InviteUserToTeamAsync(teamId, targetUserId, inviterId);

            if (!success)
            {
                return BadRequest("Không thể gửi lời mời. Người dùng có thể đã là thành viên hoặc đã có lời mời đang chờ.");
            }

            return Ok("Đã gửi lời mời thành công.");
        }
    }

    // === CONTROLLER ĐỂ XỬ LÝ (CHẤP NHẬN/TỪ CHỐI) LỜI MỜI ===
    [ApiController]
    [Route("api/team-invitations")]
    [Authorize]
    public class HandleTeamInvitationsController : ControllerBase
    {
        private readonly ITeamInvitationService _invitationService;

        public HandleTeamInvitationsController(ITeamInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Người dùng được mời chấp nhận lời mời tham gia nhóm.
        /// </summary>
        [HttpPost("{invitationId}/accept")]
        public async Task<IActionResult> Accept(int invitationId)
        {
            var result = await _invitationService.AcceptInvitationAsync(invitationId, GetUserId());
            if (!result) return BadRequest("Không thể chấp nhận lời mời.");
            return Ok("Đã chấp nhận lời mời và tham gia vào nhóm.");
        }

        /// <summary>
        /// Người dùng được mời từ chối lời mời tham gia nhóm.
        /// </summary>
        [HttpPost("{invitationId}/reject")]
        public async Task<IActionResult> Reject(int invitationId)
        {
            var result = await _invitationService.RejectInvitationAsync(invitationId, GetUserId());
            if (!result) return BadRequest("Không thể từ chối lời mời.");
            return Ok("Đã từ chối lời mời.");
        }
    }
}