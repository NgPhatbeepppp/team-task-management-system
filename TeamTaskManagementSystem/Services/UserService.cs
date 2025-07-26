using TeamTaskManagementSystem.Entities;
using TeamTaskManagementSystem.Interfaces.IAuth_User;

namespace TeamTaskManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            await _repo.AddAsync(user);
            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _repo.Update(user);
            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;
            _repo.Delete(existing);
            return await _repo.SaveChangesAsync();
        }
        public async Task<IEnumerable<User>> SearchUsersAsync(string query)
        {
            return await _repo.SearchUsersAsync(query);
        }
    }
}
