using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
