using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class ResultsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
