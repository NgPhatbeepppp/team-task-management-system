using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskItem task)
        {
            var created = await _taskService.CreateTaskAsync(task, GetUserId());
            if (created == null) return BadRequest("Dự án không tồn tại.");

            return CreatedAtAction(nameof(GetTaskById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            return Ok(task);
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            var tasks = await _taskService.GetTasksByProjectAsync(projectId);
            return Ok(tasks);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem task)
        {
            if (id != task.Id) return BadRequest("ID không khớp.");

            var success = await _taskService.UpdateTaskAsync(task, GetUserId());
            return success ? Ok() : Forbid();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var success = await _taskService.DeleteTaskAsync(id, GetUserId());
            return success ? NoContent() : Forbid();
        }
    }
}
