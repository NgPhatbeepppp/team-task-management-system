using System.Threading.Tasks;
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        void Update(User user);
        void Delete(User user);
        Task<bool> SaveChangesAsync();
        Task<bool> IsUsernameTakenAsync(string username);
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> IsPhoneNumberTakenAsync(string phoneNumber); // nếu cần

        // Mở rộng cho xác thực
        Task<bool> IsUsernameOrEmailTakenAsync(string username, string email);
        Task AddUserAsync(User user);
        Task AddUserProfileAsync(UserProfile profile);
        // Quản lý hồ sơ
        Task<UserProfile?> GetUserProfileByUserIdAsync(int userId);
    }
}