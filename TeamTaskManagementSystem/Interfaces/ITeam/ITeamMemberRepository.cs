namespace TeamTaskManagementSystem.Interfaces.ITeam
{
    public interface ITeamMemberRepository
    {
        Task<List<int>> GetUserIdsByTeamIdAsync(int teamId);
    }
}
