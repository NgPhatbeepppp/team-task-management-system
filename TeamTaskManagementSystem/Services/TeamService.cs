using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces;

namespace TeamTaskManagementSystem.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _repo;

        public TeamService(ITeamRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Team>> GetAllTeamsAsync() => await _repo.GetAllAsync();

        public async Task<Team?> GetTeamByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task<bool> CreateTeamAsync(Team team, int creatorUserId)
        {
            return await _repo.CreateTeamAsync(team, creatorUserId);
        }


        public async Task<bool> UpdateTeamAsync(Team team)
        {
            _repo.Update(team);
            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team = await _repo.GetByIdAsync(id);
            if (team == null) return false;
            _repo.Delete(team);
            return await _repo.SaveChangesAsync();
        }
        public async Task<bool> IsTeamLeaderAsync(int teamId, int userId)
        {
            return await _repo.IsTeamLeaderAsync(teamId, userId);
        }
        public async Task<bool> AddMemberAsync(int teamId, int userId)
        {
            var exists = await _repo.IsMemberAsync(teamId, userId);
            if (exists) return false;

            return await _repo.AddMemberAsync(teamId, userId);
        }

        public async Task<bool> RemoveMemberAsync(int teamId, int userId)
        {
            return await _repo.RemoveMemberAsync(teamId, userId);
        }

        public async Task<bool> GrantTeamLeaderAsync(int teamId, int targetUserId)
        {
            return await _repo.GrantTeamLeaderAsync(teamId, targetUserId);
        }

    }
}
