using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagementSystem.DTOs;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Exceptions;
using TeamTaskManagementSystem.Interfaces.ITask_CheckList;

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
        public async Task<IActionResult> UpdateTask(int id, TaskUpdateDto taskDto)
        {
            if (id != taskDto.Id) return BadRequest("ID không khớp.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var success = await _taskService.UpdateTaskAsync(taskDto, GetUserId());
                return Ok(new { message = "Cập nhật công việc thành công." });
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
        [HttpPut("{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, [FromBody] UpdateTaskStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _taskService.UpdateTaskStatusAsync(taskId, dto.NewStatusId, GetUserId());

            if (!success)
            {
                return Forbid("Cập nhật thất bại. Công việc không tồn tại hoặc bạn không có quyền.");
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var success = await _taskService.DeleteTaskAsync(id, GetUserId());
            return success ? NoContent() : Forbid();
        }
    }
}
