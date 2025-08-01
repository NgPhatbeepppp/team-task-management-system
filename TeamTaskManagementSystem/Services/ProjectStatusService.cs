using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamTaskManagementSystem.DTOs;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Exceptions;
using TeamTaskManagementSystem.Interfaces.IProject;

namespace TeamTaskManagementSystem.Services
{
    public class ProjectStatusService : IProjectStatusService
    {
        private readonly IProjectStatusRepository _statusRepo;
        private readonly IProjectRepository _projectRepo;

        public ProjectStatusService(IProjectStatusRepository statusRepo, IProjectRepository projectRepo)
        {
            _statusRepo = statusRepo;
            _projectRepo = projectRepo;
        }

        public async Task<List<ProjectStatus>> GetStatusesByProjectAsync(int projectId, int userId)
        {
            // Logic nghiệp vụ: Chỉ thành viên của dự án mới được xem các trạng thái
            var project = await _projectRepo.GetByIdAsync(projectId);
            if (project == null || !project.Members.Any(m => m.UserId == userId))
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xem các trạng thái của dự án này.");
            }
            return await _statusRepo.GetByProjectIdAsync(projectId);
        }

        public async Task<ProjectStatus> CreateStatusAsync(ProjectStatusCreateDto dto, int userId)
        {
            // Logic nghiệp vụ: Chỉ trưởng dự án mới có quyền tạo trạng thái mới
            if (!await _projectRepo.IsUserProjectLeaderAsync(dto.ProjectId, userId))
            {
                throw new UnauthorizedAccessException("Chỉ trưởng dự án mới có quyền tạo trạng thái mới.");
            }

            // Logic nghiệp vụ: Tên trạng thái không được trùng trong cùng một dự án
            if (await _statusRepo.IsNameTakenInProjectAsync(dto.ProjectId, dto.Name))
            {
                throw new InvalidOperationException("Tên trạng thái đã tồn tại trong dự án.");
            }

            var newStatus = new ProjectStatus
            {
                Name = dto.Name,
                Color = dto.Color ?? "#808080", // Màu xám mặc định nếu không được cung cấp
                ProjectId = dto.ProjectId,
                Order = await _statusRepo.GetNextOrderValueForProjectAsync(dto.ProjectId)
            };

            await _statusRepo.AddAsync(newStatus);
            await _statusRepo.SaveChangesAsync();

            return newStatus;
        }

        public async Task UpdateStatusAsync(ProjectStatusUpdateDto dto, int userId)
        {
            var status = await _statusRepo.GetByIdAsync(dto.Id);
            if (status == null)
            {
                throw new NotFoundException("Không tìm thấy trạng thái cần cập nhật.");
            }

            // Logic nghiệp vụ: Chỉ trưởng dự án mới có quyền cập nhật
            if (!await _projectRepo.IsUserProjectLeaderAsync(status.ProjectId, userId))
            {
                throw new UnauthorizedAccessException("Chỉ trưởng dự án mới có quyền cập nhật trạng thái.");
            }

            // Logic nghiệp vụ: Kiểm tra tên trùng, loại trừ chính nó
            if (await _statusRepo.IsNameTakenInProjectAsync(status.ProjectId, dto.Name, dto.Id))
            {
                throw new InvalidOperationException("Tên trạng thái đã tồn tại trong dự án.");
            }

            status.Name = dto.Name;
            status.Color = dto.Color;

            _statusRepo.Update(status);
            await _statusRepo.SaveChangesAsync();
        }

        public async Task ReorderStatusesAsync(int projectId, List<int> orderedStatusIds, int userId)
        {
            // Logic nghiệp vụ: Chỉ trưởng dự án mới có quyền sắp xếp lại
            if (!await _projectRepo.IsUserProjectLeaderAsync(projectId, userId))
            {
                throw new UnauthorizedAccessException("Chỉ trưởng dự án mới có quyền sắp xếp lại trạng thái.");
            }

            var statuses = await _statusRepo.GetByProjectIdAsync(projectId);
            if (statuses.Count != orderedStatusIds.Count || statuses.Any(s => !orderedStatusIds.Contains(s.Id)))
            {
                throw new InvalidOperationException("Danh sách ID trạng thái không hợp lệ.");
            }

            for (int i = 0; i < orderedStatusIds.Count; i++)
            {
                var statusToUpdate = statuses.First(s => s.Id == orderedStatusIds[i]);
                statusToUpdate.Order = i;
                _statusRepo.Update(statusToUpdate);
            }

            await _statusRepo.SaveChangesAsync();
        }

        public async Task DeleteStatusAsync(int statusId, int userId)
        {
            var status = await _statusRepo.GetByIdAsync(statusId);
            if (status == null)
            {
                throw new NotFoundException("Không tìm thấy trạng thái cần xóa.");
            }

            // Logic nghiệp vụ: Chỉ trưởng dự án mới có quyền xóa
            if (!await _projectRepo.IsUserProjectLeaderAsync(status.ProjectId, userId))
            {
                throw new UnauthorizedAccessException("Chỉ trưởng dự án mới có quyền xóa trạng thái.");
            }

            // Logic nghiệp vụ: Không cho phép xóa nếu trạng thái đang được sử dụng bởi bất kỳ task nào
            if (await _statusRepo.IsStatusInUse(statusId))
            {
                throw new InvalidOperationException("Không thể xóa trạng thái đang được sử dụng bởi một hoặc nhiều công việc.");
            }

            _statusRepo.Delete(status);
            await _statusRepo.SaveChangesAsync();
        }
    }
}