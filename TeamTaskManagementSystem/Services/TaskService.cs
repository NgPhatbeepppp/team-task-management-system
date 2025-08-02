using TeamTaskManagementSystem.DTOs;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Exceptions;
using TeamTaskManagementSystem.Interfaces.IProject;
using TeamTaskManagementSystem.Interfaces.ITask_CheckList;

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

        public async Task<TaskItem?> CreateTaskAsync(TaskCreateDto taskDto, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(taskDto.ProjectId);
            if (project == null)
                return null; // Dự án không tồn tại

            // --- Logic xác thực người dùng được giao ---
            var projectMemberIds = new HashSet<int>(project.Members.Select(m => m.UserId));
            foreach (var assignedUserId in taskDto.AssignedUserIds)
            {
                if (!projectMemberIds.Contains(assignedUserId))
                {
                    // Ném ra lỗi nếu cố gắng giao cho người không thuộc dự án
                    throw new InvalidOperationException($"Người dùng với ID {assignedUserId} không phải là thành viên của dự án.");
                }
            }

            // 1. Map từ DTO sang Entity
            var task = new TaskItem
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Priority = taskDto.Priority,
                Deadline = taskDto.Deadline,
                StatusId = taskDto.StatusId,
                ProjectId = taskDto.ProjectId,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            // 2. Thêm những người được giao việc
            foreach (var assigneeId in taskDto.AssignedUserIds)
            {
                task.Assignees.Add(new TaskAssignee { UserId = assigneeId });
            }

            // 3. Lưu vào CSDL
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

        public async Task<bool> UpdateTaskAsync(TaskUpdateDto taskDto, int userId)
        {
            var existingTask = await _taskRepository.GetByIdAsync(taskDto.Id);
            if (existingTask == null)
            {
                throw new NotFoundException("Công việc không tồn tại.");
            }

            var project = await _projectRepository.GetByIdAsync(existingTask.ProjectId);
            if (project == null || !project.Members.Any(m => m.UserId == userId))
            {
                throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa công việc trong dự án này.");
            }

            // --- Logic xác thực người dùng được giao ---
            var projectMemberIds = new HashSet<int>(project.Members.Select(m => m.UserId));
            foreach (var assignedUserId in taskDto.AssignedUserIds)
            {
                if (!projectMemberIds.Contains(assignedUserId))
                {
                    throw new InvalidOperationException($"Người dùng với ID {assignedUserId} không phải là thành viên của dự án.");
                }
            }

            // --- Cập nhật thông tin cơ bản của Task ---
            existingTask.Title = taskDto.Title;
            existingTask.Description = taskDto.Description;
            existingTask.Priority = taskDto.Priority;
            existingTask.Deadline = taskDto.Deadline;
            existingTask.StatusId = taskDto.StatusId;

            // --- Logic cập nhật danh sách người được giao ---
            var newAssigneeIds = new HashSet<int>(taskDto.AssignedUserIds);
            var currentAssigneeIds = new HashSet<int>(existingTask.Assignees.Select(a => a.UserId));

            // Xóa những người không còn được giao
            var assigneesToRemove = existingTask.Assignees.Where(a => !newAssigneeIds.Contains(a.UserId)).ToList();
            foreach (var assignee in assigneesToRemove)
            {
                existingTask.Assignees.Remove(assignee);
            }

            // Thêm những người mới được giao
            var userIdsToAdd = newAssigneeIds.Where(id => !currentAssigneeIds.Contains(id)).ToList();
            foreach (var userIdToAdd in userIdsToAdd)
            {
                existingTask.Assignees.Add(new TaskAssignee { TaskId = existingTask.Id, UserId = userIdToAdd });
            }

            await _taskRepository.UpdateAsync(existingTask);
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
        public async Task<bool> UpdateTaskStatusAsync(int taskId, int newStatusId, int userId)
        {
            var existingTask = await _taskRepository.GetByIdAsync(taskId);
            if (existingTask == null)
            {
                return false; // Task không tồn tại
            }

            // Logic nghiệp vụ: Kiểm tra xem người dùng có phải là thành viên của dự án chứa task này không
            var project = await _projectRepository.GetByIdAsync(existingTask.ProjectId);
            if (project == null || !project.Members.Any(m => m.UserId == userId))
            {
                return false; // Không có quyền
            }

            // Cập nhật chỉ statusId
            existingTask.StatusId = newStatusId;

            await _taskRepository.UpdateAsync(existingTask);
            await _taskRepository.SaveChangesAsync();
            return true;
        }
    }
}
