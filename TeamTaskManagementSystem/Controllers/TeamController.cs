// TeamTaskManagementSystem/Controllers/TeamController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Exceptions;
using TeamTaskManagementSystem.Interfaces.ITeam;

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

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyTeams()
        {
            var teams = await _teamService.GetTeamsByUserIdAsync(GetUserId());
            return Ok(teams);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var team = await _teamService.GetTeamByIdAsync(id);
                return Ok(team);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // <<< GHI CHÚ: Hoàn toàn làm lại theo pattern try-catch.
        [HttpPost]
        public async Task<IActionResult> Create(Team team)
        {
            try
            {
                await _teamService.CreateTeamAsync(team, GetUserId());
                return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Team team)
        {
            if (id != team.Id) return BadRequest("Id không khớp.");

            try
            {
                await _teamService.UpdateTeamAsync(team, GetUserId());
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _teamService.DeleteTeamAsync(id, GetUserId());
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Bắt lỗi nghiệp vụ (ví dụ: team còn trong project)
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{teamId}/leave-all-projects")]
        public async Task<IActionResult> LeaveAllProjects(int teamId)
        {
            try
            {
                await _teamService.LeaveAllProjectsAsync(teamId, GetUserId());
                return Ok(new { message = "Đã yêu cầu team rời khỏi tất cả các dự án thành công." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // --- Quản lý thành viên ---

        [HttpPost("{teamId}/members/{targetUserId}")]
        public async Task<IActionResult> AddMember(int teamId, int targetUserId)
        {
            try
            {
                await _teamService.AddMemberAsync(teamId, targetUserId, GetUserId());
                return Ok(new { message = "Thêm thành viên thành công." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{teamId}/members/{targetUserId}")]
        public async Task<IActionResult> RemoveMember(int teamId, int targetUserId)
        {
            try
            {
                await _teamService.RemoveMemberAsync(teamId, targetUserId, GetUserId());
                return Ok(new { message = "Xóa thành viên thành công." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("{teamId}/grant-leader/{targetUserId}")]
        public async Task<IActionResult> GrantLeader(int teamId, int targetUserId)
        {
            try
            {
                await _teamService.GrantTeamLeaderAsync(teamId, targetUserId, GetUserId());
                return Ok(new { message = "Trao quyền trưởng nhóm thành công." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}