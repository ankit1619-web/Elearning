using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    public class TopicsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
