using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> ExistsAsync(string username);
        Task AddUserAsync(User user);
        Task AddProfileAsync(UserProfile profile);
        Task SaveChangesAsync();
    }
}
