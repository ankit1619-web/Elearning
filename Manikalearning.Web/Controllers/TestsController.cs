using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class TestsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
