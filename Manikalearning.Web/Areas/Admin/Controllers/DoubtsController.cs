using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Doubts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Teacher")]
    [Route("Admin/[controller]")]
    public class DoubtsController : Controller
    {
        private readonly IMediator _mediator;
        public DoubtsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(string? status = DoubtStatus.Open, int page = 1, CancellationToken ct = default)
        {
            try { return View(await _mediator.Send(new GetStaffDoubtsQuery(status, page), ct)); }
            catch (ValidationException) { return RedirectToAction(nameof(Index)); }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Thread(Guid id, CancellationToken ct)
        {
            var vm = await _mediator.Send(new GetDoubtThreadQuery(null, id), ct);      
            return vm == null ? NotFound() : View(vm);
        }

        [HttpPost("{id:guid}/Reply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(Guid id, string? text, bool close, CancellationToken ct)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var staffId))
                return Challenge();

            try
            {
                await _mediator.Send(new AnswerDoubtCommand(staffId, id, text ?? "", close), ct);
                TempData["SuccessMessage"] = close ? "Answer sent and doubt closed." : "Answer sent.";
            }
            catch (ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "Could not send the answer.";
            }
            return RedirectToAction(nameof(Thread), new { id });
        }
    }
}
