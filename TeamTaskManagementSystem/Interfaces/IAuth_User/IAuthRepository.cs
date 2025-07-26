using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.IAuth_User
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
