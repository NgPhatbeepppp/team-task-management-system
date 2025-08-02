using TeamTaskManagementSystem.DTOs;
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.ITask_CheckList
{
    public interface ITaskService
    {
        Task<TaskItem?> CreateTaskAsync(TaskItem task, int userId);
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId);
        Task<bool> UpdateTaskAsync(TaskUpdateDto taskDto, int userId);
        Task<bool> DeleteTaskAsync(int id, int userId);
        Task<bool> UpdateTaskStatusAsync(int taskId, int newStatusId, int userId);
    }
}
