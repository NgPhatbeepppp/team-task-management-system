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

        public async Task AddIfNotExistsAsync(int projectId, int userId)
        {
            var exists = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
            if (!exists)
            {
                await _context.ProjectMembers.AddAsync(new ProjectMember
                {
                    ProjectId = projectId,
                    UserId = userId
                });
            }
        }
    }
}
