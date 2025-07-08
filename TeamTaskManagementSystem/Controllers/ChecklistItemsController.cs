using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagementSystem.DTOs;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChecklistItemsController : ControllerBase
    {
        private readonly IChecklistItemService _service;

        public ChecklistItemsController(IChecklistItemService service)
        {
            _service = service;
        }

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetByTask(int taskId)
        {
            var items = await _service.GetByTaskIdAsync(taskId);
            var result = items.Select(i => new ChecklistItemGetDto
            {
                Id = i.Id,
                Content = i.Content,
                IsCompleted = i.IsCompleted
            });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChecklistItemGetDto dto)
        {
            var item = new ChecklistItem
            {
                Content = dto.Content,
                IsCompleted = dto.IsCompleted,
                TaskId = dto.TaskId,
                Order =dto.Order
            };

            var created = await _service.CreateAsync(item);
            return CreatedAtAction(nameof(GetByTask), new { taskId = item.TaskId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ChecklistItemGetDto dto)
        {
            if (dto.Id == null || id != dto.Id.Value)
                return BadRequest("ID không khôp.");

            var item = new ChecklistItem
            {
                Id = dto.Id.Value,
                Content = dto.Content,
                IsCompleted = dto.IsCompleted,
                TaskId = dto.TaskId
            };

            var success = await _service.UpdateAsync(item);
            return success ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
