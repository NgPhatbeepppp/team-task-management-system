// TeamTaskManagementSystem/Repositories/ProjectMemberRepository.cs
using Microsoft.EntityFrameworkCore;
using TeamTaskManagementSystem.Data;
using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Repositories
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly AppDbContext _context;

        public ProjectMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProjectMember member)
        {
            await _context.ProjectMembers.AddAsync(member);
        }

        public async Task<ProjectMember?> FindAsync(int projectId, int userId)
        {
            return await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        }

        public void Delete(ProjectMember member)
        {
            _context.ProjectMembers.Remove(member);
        }
    }
}