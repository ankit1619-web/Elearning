using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExamsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
