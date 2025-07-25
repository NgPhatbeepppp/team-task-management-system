using System.Threading.Tasks;
using TeamTaskManagementSystem.ViewModels;

namespace TeamTaskManagementSystem.Interfaces.IAuth_User
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(AuthRegisterRequest request);
    }
}