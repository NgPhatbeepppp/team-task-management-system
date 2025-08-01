using System.Collections.Generic;
using System.Threading.Tasks;
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.IProject
{
    public interface IProjectStatusRepository
    {
        Task<ProjectStatus?> GetByIdAsync(int id);
        Task<List<ProjectStatus>> GetByProjectIdAsync(int projectId);
        Task<int> GetNextOrderValueForProjectAsync(int projectId);
        Task<bool> IsNameTakenInProjectAsync(int projectId, string name, int? excludeStatusId = null);
        Task AddAsync(ProjectStatus status);
        void Update(ProjectStatus status);
        void Delete(ProjectStatus status);
        Task<bool> IsStatusInUse(int statusId);
        Task<bool> SaveChangesAsync();
    }
}