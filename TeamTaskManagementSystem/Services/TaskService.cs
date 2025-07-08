using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;

        public TaskService(ITaskRepository taskRepo, IProjectRepository projectRepo)
        {
            _taskRepository = taskRepo;
            _projectRepository = projectRepo;
        }

        public async Task<TaskItem?> CreateTaskAsync(TaskItem task, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null)
                return null;

            task.CreatedByUserId = userId;
            task.CreatedAt = DateTime.UtcNow;

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(int projectId)
        {
            return await _taskRepository.GetByProjectIdAsync(projectId);
        }

        public async Task<bool> UpdateTaskAsync(TaskItem task, int userId)
        {
            var existing = await _taskRepository.GetByIdAsync(task.Id);
            if (existing == null || existing.CreatedByUserId != userId)
                return false;

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.Priority = task.Priority;
            existing.Deadline = task.Deadline;
            existing.StatusId = task.StatusId;
            existing.AssignedToUserId = task.AssignedToUserId;

            await _taskRepository.UpdateAsync(existing);
            await _taskRepository.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> DeleteTaskAsync(int id, int userId)
        {
            var existing = await _taskRepository.GetByIdAsync(id);
            if (existing == null || existing.CreatedByUserId != userId)
                return false;

            await _taskRepository.DeleteAsync(existing);
            await _taskRepository.SaveChangesAsync();
            return true;
        }
    }
}
