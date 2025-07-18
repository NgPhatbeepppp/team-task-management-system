﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

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

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var projects = await _service.GetProjectsOfUserAsync(GetUserId());
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            var success = await _service.CreateProjectAsync(project, GetUserId());
            if (!success) return BadRequest("Không thể tạo project.");
            return Ok(project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Project project)
        {
            if (id != project.Id) return BadRequest("Id không khớp.");
            var success = await _service.UpdateProjectAsync(project, GetUserId());
            if (!success) return StatusCode(403, "Bạn không có quyền sửa project.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteProjectAsync(id, GetUserId());
            if (!success) return Forbid("Bạn không có quyền xoá project.");
            return NoContent();
        }
    }
}
