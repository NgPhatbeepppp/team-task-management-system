using TeamTaskManagementSystem.Entities;

using TeamTaskManagementSystem.DTOs;

namespace TeamTaskManagementSystem.Interfaces.ITask_CheckList
{
    public interface ITaskService
    {
        Task<TaskItem?> CreateTaskAsync(TaskCreateDto taskDto, int userId);
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId);
        Task<bool> UpdateTaskAsync(int taskId, TaskUpdateDto taskDto, int userId);
        Task<bool> DeleteTaskAsync(int id, int userId);
        Task<bool> UpdateTaskStatusAsync(int taskId, int newStatusId, int userId);
        Task<bool> UpdateTaskPriorityAsync(int taskId, string newPriority, int userId);
    }
}
