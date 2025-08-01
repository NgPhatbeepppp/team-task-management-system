// TeamTaskManagementSystem/Interfaces/IProjectService.cs

// TeamTaskManagementSystem/Interfaces/IProjectService.cs
using TeamTaskManagementSystem.DTOs;
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.IProject
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetProjectsOfUserAsync(int userId);
        Task<Project> GetByIdAsync(int id);

        Task<Project> CreateProjectAsync(ProjectCreateDto projectDto, int creatorUserId);
        Task UpdateProjectAsync(Project project, int userId);
        Task DeleteProjectAsync(int projectId, int userId);

        Task RemoveTeamFromProjectAsync(int projectId, int teamId, int userId);

        Task RemoveMemberFromProjectAsync(int projectId, int targetUserId, int actorUserId);
        
        Task AddMemberToProjectAsync(int projectId, int targetUserId, int actorUserId);
    }
}