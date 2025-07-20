using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IProjectTeamRepository
    {
        Task AddAsync(ProjectTeam team);
        Task<ProjectMember?> FindAsync(int projectId, int userId);
        void Delete(ProjectMember member);
    }
}
