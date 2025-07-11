using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IProjectTeamRepository
    {
        Task AddAsync(ProjectTeam team);
    }
}
