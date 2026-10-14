using Manikalearning.Application.Features.Commands.Students;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FluentValidation;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]")]
    public class PerformanceController : Controller
    {
        private readonly IMediator _mediator;
        public PerformanceController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(int days = 30, CancellationToken ct = default)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                return View(await _mediator.Send(new GetPerformanceQuery(userId, days), ct));
            }
            catch (ValidationException)
            {
                return RedirectToAction(nameof(Index), new { days = 30 });   
            }
        }
    }
}
