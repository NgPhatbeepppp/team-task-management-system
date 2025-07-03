using Microsoft.AspNetCore.Mvc;

namespace TeamTaskManagementSystem.Controllers
{
    public class UserProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
