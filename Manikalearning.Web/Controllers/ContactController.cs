using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
