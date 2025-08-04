using TeamTaskManagementSystem.ViewModels;

namespace TeamTaskManagementSystem.Interfaces.IAuth_User
{
    public interface IAuthService
    {
        Task<RegisterResult> RegisterAsync(AuthRegisterRequest request);
        Task<bool> ForgotPasswordAsync(string email); 
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}
