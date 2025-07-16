using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IProjectMemberRepository
    {
        Task AddAsync(ProjectMember member);
        Task AddIfNotExistsAsync(int projectId, int userId);
    }
}
