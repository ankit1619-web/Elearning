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
    public class MistakeBookController : Controller
    {
        private readonly IMediator _mediator;

        public MistakeBookController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var userId = Guid.Parse(userIdClaim);
            var response = await _mediator.Send(new GetStudentMistakesQuery(userId), ct);
              
            return View(response);
            
        }

        [HttpPost("Remove/{mistakeId}")]
        public async Task<IActionResult> Remove(Guid mistakeId, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _mediator.Send(new RemoveMistakeCommand(userId, mistakeId), ct);
            return View(nameof(Index)) ;
        }
    }
}
