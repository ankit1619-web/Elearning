using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
