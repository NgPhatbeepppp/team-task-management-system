using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.IProject
{
    public interface IProjectMemberRepository
    {

        Task<ProjectMember?> FindAsync(int projectId, int userId);

        Task AddAsync(ProjectMember member);

        void Delete(ProjectMember member);
    }
}