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
    public class MistakeBookController : Controller
    {
        private readonly IMediator _mediator;

        public MistakeBookController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var mistakes = await _mediator.Send(new GetStudentMistakesQuery(userId), ct);
            return View(mistakes);
        }

        [HttpPost("Remove/{mistakeId}")]
        public async Task<IActionResult> Remove(Guid mistakeId, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _mediator.Send(new RemoveMistakeCommand(userId, mistakeId), ct);
            return Json(new { success = true });
        }
    }
}
