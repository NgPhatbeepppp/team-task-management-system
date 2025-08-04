using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamTaskManagementSystem.DTOs;
using TeamTaskManagementSystem.Exceptions;
using TeamTaskManagementSystem.Interfaces.IProject;

namespace TeamTaskManagementSystem.Controllers
{
    [ApiController]
    [Authorize]
    public class ProjectStatusController : ControllerBase
    {
        private readonly IProjectStatusService _statusService;

        public ProjectStatusController(IProjectStatusService statusService)
        {
            _statusService = statusService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /api/projects/{projectId}/statuses
        [HttpGet("api/projects/{projectId}/statuses")]
        public async Task<IActionResult> GetStatusesForProject(int projectId)
        {
            try
            {
                var statuses = await _statusService.GetStatusesByProjectAsync(projectId, GetUserId());
                return Ok(statuses);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // POST: /api/projects/{projectId}/statuses
        [HttpPost("api/projects/{projectId}/statuses")]
        public async Task<IActionResult> CreateStatus(int projectId, [FromBody] ProjectStatusCreateDto dto)
        {
            if (projectId != dto.ProjectId)
            {
                return BadRequest("ID dự án trong URL và trong body không khớp.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdStatus = await _statusService.CreateStatusAsync(dto, GetUserId());
                return CreatedAtAction(nameof(GetStatusesForProject), new { projectId = createdStatus.ProjectId }, createdStatus);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: /api/statuses/{statusId}
        [HttpPut("api/statuses/{statusId}")]
        public async Task<IActionResult> UpdateStatus(int statusId, [FromBody] ProjectStatusUpdateDto dto)
        {
            if (statusId != dto.Id)
            {
                return BadRequest("ID trong URL và trong body không khớp.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _statusService.UpdateStatusAsync(dto, GetUserId());
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
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: /api/projects/{projectId}/statuses/reorder
        [HttpPut("api/projects/{projectId}/statuses/reorder")]
        public async Task<IActionResult> ReorderStatuses(int projectId, [FromBody] ProjectStatusReorderDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _statusService.ReorderStatusesAsync(projectId, dto.StatusIdsInOrder, GetUserId());
                return Ok(new { message = "Đã cập nhật thứ tự các trạng thái." });
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

        // DELETE: /api/statuses/{statusId}
        [HttpDelete("api/statuses/{statusId}")]
        public async Task<IActionResult> DeleteStatus(int statusId)
        {
            try
            {
                await _statusService.DeleteStatusAsync(statusId, GetUserId());
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
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}