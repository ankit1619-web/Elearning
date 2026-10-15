using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Doubts;
using Manikalearning.Application.Features.Handlers.Assessments.Dropdowns;
using Manikalearning.Application.Features.Handlers.Students;
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
    public class DoubtsController : Controller
    {
        private readonly IMediator _mediator;
        public DoubtsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(string? status, int page = 1, CancellationToken ct = default)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            try { return View(await BuildPageAsync(userId.Value, status, page, new AskDoubtInput(), ct)); }
            catch (ValidationException) { return RedirectToAction(nameof(Index)); }
        }

        [HttpPost("Ask")]
        [RequireFeature(PlanFeatureCodes.Doubts)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ask([Bind(Prefix = "Input")] AskDoubtInput input, CancellationToken ct)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                await _mediator.Send(new AskDoubtCommand(userId.Value, input.CourseId, input.QuestionText ?? ""), ct);
                TempData["SuccessMessage"] = "Your doubt was sent. We will reply soon.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            { 
                foreach (var f in ex.Errors) ModelState.AddModelError($"Input.{f.PropertyName}", f.ErrorMessage);
                return View("Index", await BuildPageAsync(userId.Value, null, 1, input, ct));
            }
        }

        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, string? status, int page = 1, CancellationToken ct = default)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var ok = await _mediator.Send(new DeleteDoubtCommand(userId.Value, id), ct);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Doubt deleted." : "Only unanswered doubts can be deleted.";
            return RedirectToAction(nameof(Index), new { status, page });
        }
         
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Thread(Guid id, CancellationToken ct)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var vm = await _mediator.Send(new GetDoubtThreadQuery(userId.Value, id), ct);   
            return vm == null ? NotFound() : View(vm);
        }

        [HttpPost("{id:guid}/Reply")]
        [RequireFeature(PlanFeatureCodes.Doubts)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(Guid id, string? text, CancellationToken ct)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                await _mediator.Send(new ReplyToDoubtCommand(userId.Value, id, text ?? ""), ct);
                TempData["SuccessMessage"] = "Message sent.";
            }
            catch (ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "Could not send your message.";
            }
            return RedirectToAction(nameof(Thread), new { id });
        }

        private async Task<DoubtsPageVm> BuildPageAsync(Guid userId, string? status, int page, AskDoubtInput input, CancellationToken ct) => new()
        {
            List = await _mediator.Send(new GetMyDoubtsQuery(userId, status, page), ct),
            Courses = await _mediator.Send(new GetCourseOptionsQuery(), ct),
            Input = input
        };

        private Guid? CurrentUserId() =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    }

    public class AskDoubtInput
    {
        public Guid? CourseId { get; set; }
        public string? QuestionText { get; set; }
    }

    public class DoubtsPageVm
    {
        public DoubtListDto List { get; set; } = new();
        public List<CourseOptionDto> Courses { get; set; } = new();
        public AskDoubtInput Input { get; set; } = new();
    }
}
