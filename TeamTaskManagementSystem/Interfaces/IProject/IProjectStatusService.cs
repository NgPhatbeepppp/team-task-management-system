using System.Collections.Generic;
using System.Threading.Tasks;
using TeamTaskManagementSystem.DTOs;
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.IProject
{
    public interface IProjectStatusService
    {
        Task<List<ProjectStatus>> GetStatusesByProjectAsync(int projectId, int userId);
        Task<ProjectStatus> CreateStatusAsync(ProjectStatusCreateDto dto, int userId);
        Task UpdateStatusAsync(ProjectStatusUpdateDto dto, int userId);
        Task ReorderStatusesAsync(int projectId, List<int> orderedStatusIds, int userId);
        Task DeleteStatusAsync(int statusId, int userId);
    }
}