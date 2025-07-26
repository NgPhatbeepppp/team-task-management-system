using System.Threading.Tasks;
using TeamTaskManagementSystem.Entities;

namespace TeamTaskManagementSystem.Interfaces.IAuth_User
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        void Update(User user);
        void Delete(User user);
        Task<bool> SaveChangesAsync();
        //BE-2/Feature-QL-Thanh-vien
        Task<User?> GetUserByEmailOrUsernameAsync(string identifier);

        Task<bool> IsUsernameTakenAsync(string username);
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> IsPhoneNumberTakenAsync(string phoneNumber); 


        // Mở rộng cho xác thực
        Task<bool> IsUsernameOrEmailTakenAsync(string username, string email);
        Task AddUserAsync(User user);
        Task AddUserProfileAsync(UserProfile profile);
        // Quản lý hồ sơ
        Task<UserProfile?> GetUserProfileByUserIdAsync(int userId);

        Task<IEnumerable<User>> SearchUsersAsync(string query);

    }
}