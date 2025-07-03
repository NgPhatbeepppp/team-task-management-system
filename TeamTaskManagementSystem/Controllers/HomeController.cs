using Microsoft.AspNetCore.Mvc;

namespace TeamTaskManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
