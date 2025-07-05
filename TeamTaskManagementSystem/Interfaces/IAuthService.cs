using TeamTaskManagementSystem.ViewModels;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResult> RegisterAsync(AuthRegisterRequest request);
    }
}
