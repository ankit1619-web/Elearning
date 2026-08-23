using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class StudyMaterialController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
