using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Exceptions;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpDelete("{teamId}/leave-all-projects")]
        public async Task<IActionResult> LeaveAllProjects(int teamId)
        {
            try
            {
                await _teamService.LeaveAllProjectsAsync(teamId, GetUserId());
                return Ok(new { message = "Đã xóa team khỏi tất cả các dự án thành công." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }

        }
            [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return Ok(teams);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Team team)
        {
            var userId = GetUserId();
            var success = await _teamService.CreateTeamAsync(team, userId);
            if (!success) return BadRequest("Không thể tạo team.");
            return Ok(team);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Team team)
        {
            if (id != team.Id) return BadRequest("Id không khớp.");
            var success = await _teamService.UpdateTeamAsync(team);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            try
            {
                await _teamService.DeleteTeamAsync(id, GetUserId());
                return NoContent(); // 204 No Content - Thành công
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message }); // 401
            }
            catch (InvalidOperationException ex)
            {
                // Bắt lỗi nghiệp vụ và trả về 400 Bad Request
                return BadRequest(new { message = ex.Message }); // 400
            }
        }
            //[HttpPost("{teamId}/members")]
            //public async Task<IActionResult> AddMember(int teamId, [FromBody] int userId)
            //{
            //    if (!await _teamService.IsTeamLeaderAsync(teamId, GetUserId()))
            //        return Forbid("Bạn không phải TeamLeader của team này.");

            //    var success = await _teamService.AddMemberAsync(teamId, userId);
            //    if (!success) return BadRequest("Không thể thêm thành viên.");
            //    return Ok("Đã thêm thành viên.");
            //}

            [HttpDelete("{teamId}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(int teamId, int userId)
        {
            if (!await _teamService.IsTeamLeaderAsync(teamId, GetUserId()))
                return Forbid("Bạn không phải TeamLeader của team này.");

            var success = await _teamService.RemoveMemberAsync(teamId, userId);
            if (!success) return NotFound("Không tìm thấy thành viên.");
            return Ok("Đã xoá thành viên.");
        }

        [HttpPost("{teamId}/grant-leader")]
        public async Task<IActionResult> GrantLeader(int teamId, [FromBody] int targetUserId)
        {
            if (!await _teamService.IsTeamLeaderAsync(teamId, GetUserId()))
                return Forbid("Chỉ TeamLeader mới có thể chuyển quyền.");

            var success = await _teamService.GrantTeamLeaderAsync(teamId, targetUserId);
            if (!success) return BadRequest("Không thể trao quyền.");
            return Ok("Đã chuyển quyền TeamLeader.");
        }
    }
}
