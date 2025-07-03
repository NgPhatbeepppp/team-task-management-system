using System.Threading.Tasks;
using TeamTaskManagementSystem.ViewModels;

namespace TeamTaskManagementSystem.Interfaces
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(AuthRegisterRequest request);
    }
}