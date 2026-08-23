using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    public class PapersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
