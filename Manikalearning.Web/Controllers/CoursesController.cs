using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
