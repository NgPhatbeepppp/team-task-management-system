// TeamTaskManagementSystem/Controllers/ProjectController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Exceptions;
using TeamTaskManagementSystem.Interfaces.IProject;

namespace TeamTaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectController(IProjectService service)
        {
            _service = service;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ... (các endpoint khác giữ nguyên) ...

        // <<< GHI CHÚ: Thêm endpoint mới để thêm thành viên.
        [HttpPost("{projectId}/members/{targetUserId}")]
        public async Task<IActionResult> AddMemberToProject(int projectId, int targetUserId)
        {
            try
            {
                await _service.AddMemberToProjectAsync(projectId, targetUserId, GetUserId());
                return Ok(new { message = "Thêm thành viên vào dự án thành công." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var projects = await _service.GetProjectsOfUserAsync(GetUserId());
            return Ok(projects);
        }

        [HttpDelete("{projectId}/teams/{teamId}")]
        public async Task<IActionResult> RemoveTeamFromProject(int projectId, int teamId)
        {
            try
            {
                await _service.RemoveTeamFromProjectAsync(projectId, teamId, GetUserId());
                return NoContent();
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

        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            try
            {
                await _service.CreateProjectAsync(project, GetUserId());
                return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var project = await _service.GetByIdAsync(id);
                return Ok(project);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Project project)
        {
            if (id != project.Id) return BadRequest("Id trong route và trong body không khớp.");
            try
            {
                await _service.UpdateProjectAsync(project, GetUserId());
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
                await _service.DeleteProjectAsync(id, GetUserId());
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
    }
}