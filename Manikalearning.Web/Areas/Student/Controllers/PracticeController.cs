using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    public class PracticeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
