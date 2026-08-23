using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class PracticeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
