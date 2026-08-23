using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
