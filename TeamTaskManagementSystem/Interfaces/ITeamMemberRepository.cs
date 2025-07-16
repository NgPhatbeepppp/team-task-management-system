namespace TeamTaskManagementSystem.Interfaces
{
    public interface ITeamMemberRepository
    {
        Task<List<int>> GetUserIdsByTeamIdAsync(int teamId);
    }
}
