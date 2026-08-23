using Manikalearning.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class ExamsController : Controller
    {
        public IActionResult Index()
        {
            var model = new List<ExamViewModel>();

            return View(model);
        }
        public IActionResult Details(Guid id)
        {
            return View();
        }
    }
}
