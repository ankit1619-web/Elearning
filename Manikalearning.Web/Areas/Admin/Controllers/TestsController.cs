using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    public class TestsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
