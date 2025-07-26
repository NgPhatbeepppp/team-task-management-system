using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamTaskManagementSystem.DTOs.TeamInvitation;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Controllers
{
    // =================================================================
    // CONTROLLER NÀY QUẢN LÝ VIỆC MỜI VÀ TÌM KIẾM CHO MỘT TEAM CỤ THỂ
    // =================================================================
    [ApiController]
    [Route("api/team/{teamId}/invitations")] 
    [Authorize]
    public class TeamInvitationsController : ControllerBase
    {
        private readonly ITeamInvitationService _invitationService;

        public TeamInvitationsController(ITeamInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // POST /api/team/{teamId}/invitations
        [HttpPost]
        public async Task<IActionResult> InviteUserToTeam(int teamId, [FromBody] InviteUserRequestDto request)
        {
            var inviterId = GetUserId();
            var success = await _invitationService.InviteUserToTeamAsync(teamId, request.TargetUserId, inviterId);

            if (!success)
            {
                return BadRequest("Không thể gửi lời mời. Người dùng có thể đã là thành viên hoặc đã có lời mời đang chờ.");
            }
            return Ok("Đã gửi lời mời thành công.");
        }

        // GET /api/team/{teamId}/invitations/search-users
        [HttpGet("search-users")]
        public async Task<ActionResult<IEnumerable<UserSearchResponseDto>>> SearchUsers(int teamId, [FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query) || query.Length < 2)
            {
                return Ok(Enumerable.Empty<UserSearchResponseDto>());
            }
            var users = await _invitationService.SearchUsersForInvitationAsync(teamId, query);
            return Ok(users);
        }
    }

    // =================================================================
    // CONTROLLER NÀY XỬ LÝ CÁC LỜI MỜI ĐÃ TỒN TẠI (CHUNG)
    // =================================================================
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

        // POST /api/team-invitations/{invitationId}/accept
        [HttpPost("{invitationId}/accept")]
        public async Task<IActionResult> Accept(int invitationId)
        {
            var result = await _invitationService.AcceptInvitationAsync(invitationId, GetUserId());
            if (!result) return BadRequest("Không thể chấp nhận lời mời.");
            return Ok("Đã chấp nhận lời mời và tham gia vào nhóm.");
        }

        // POST /api/team-invitations/{invitationId}/reject
        [HttpPost("{invitationId}/reject")]
        public async Task<IActionResult> Reject(int invitationId)
        {
            var result = await _invitationService.RejectInvitationAsync(invitationId, GetUserId());
            if (!result) return BadRequest("Không thể từ chối lời mời.");
            return Ok("Đã từ chối lời mời.");
        }
    }
}
