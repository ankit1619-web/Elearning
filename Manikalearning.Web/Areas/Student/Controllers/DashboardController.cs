using Manikalearning.Application.DTOs;
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
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;
        public DashboardController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return RedirectToAction("Login", "Account", new { area = "" });

            return View(new DashboardPageVm
            {
                Data = await _mediator.Send(new GetMyDashboardQuery(userId), ct), 
                Courses = await _mediator.Send(new GetUserCoursesQuery(userId, "InProgress", ""), ct)
            });
        }
    }

    public class DashboardPageVm
    {
        public MyDashboardDto Data { get; set; } = new();
        public MyCoursesViewModel Courses { get; set; } = new();
    }
}
