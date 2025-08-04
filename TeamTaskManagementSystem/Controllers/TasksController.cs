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
        public async Task<IActionResult> CreateTask(TaskCreateDto taskDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _taskService.CreateTaskAsync(taskDto, GetUserId());
                if (created == null) return BadRequest("Dự án không tồn tại.");

                var result = await _taskService.GetTaskByIdAsync(created.Id);
                return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateDto taskDto) // Thêm [FromBody] để rõ ràng hơn
        {
            // Dòng kiểm tra ID không còn cần thiết nữa vì ta đã xóa Id khỏi DTO
            // if (id != taskDto.Id) return BadRequest("ID không khớp.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Truyền 'id' từ URL vào service
                var success = await _taskService.UpdateTaskAsync(id, taskDto, GetUserId());
                if (!success)
                {
                    // TaskService sẽ ném ra exception nếu không tìm thấy, nên dòng này có thể không cần thiết
                    return NotFound(new { message = "Không tìm thấy công việc để cập nhật." });
                }
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

        [HttpPut("{taskId}/priority")]
        public async Task<IActionResult> UpdateTaskPriority(int taskId, [FromBody] UpdateTaskPriorityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _taskService.UpdateTaskPriorityAsync(taskId, dto.Priority, GetUserId());

            if (!success)
            {
                return Forbid("Cập nhật thất bại. Công việc không tồn tại hoặc bạn không có quyền.");
            }

            return NoContent();
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
