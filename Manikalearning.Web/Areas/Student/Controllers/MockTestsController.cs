using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Web.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]")]
    [RequireSubscription]
    public class MockTestsController : Controller
    {
        private readonly IMediator _mediator;

        public MockTestsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("Index")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var tests = await _mediator.Send(new GetAvailableMockTestsQuery(userId), ct);
            return View(tests);
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitTestAttemptCommand command, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var fullCommand = command with { UserId = userId };

            var result = await _mediator.Send(fullCommand, ct);
            return Json(new { success = true, data = result });
        }
    }
}
