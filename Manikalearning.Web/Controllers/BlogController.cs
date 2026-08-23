using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
