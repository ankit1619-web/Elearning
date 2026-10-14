using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Features.Handlers.Students;
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
         
        [HttpGet]
        public async Task<IActionResult> Index(string filter = "All", string search = "", CancellationToken ct = default)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var viewModel = await _mediator.Send(new GetUserCoursesQuery(userId, filter, search), ct);
             
            ViewBag.AccessibleCourseIds = await _mediator.Send(new GetAccessibleCourseIdsQuery(userId), ct);

            return View("Index", viewModel);
        }
    }
}