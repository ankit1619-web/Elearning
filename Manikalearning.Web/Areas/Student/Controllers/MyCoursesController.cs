using Manikalearning.Application.Features.Commands.Students;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]")]
    public class MyCoursesController : Controller
    {
        private readonly IMediator _mediator;

        public MyCoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /Student/MyCourses?filter=InProgress&search=net
        [HttpGet]
        public async Task<IActionResult> Index(string filter = "All", string search = "")
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
             
            var query = new GetUserCoursesQuery(userId, filter, search);
            var viewModel = await _mediator.Send(query);

            return View("Index", viewModel);
        }
    }
}