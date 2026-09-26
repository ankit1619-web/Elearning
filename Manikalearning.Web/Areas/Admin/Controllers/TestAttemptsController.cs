using Manikalearning.Application.Features.Commands.Assessments.TestAttempts;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class TestAttemptsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IViewRenderService _viewRender;

        public TestAttemptsController(IMediator mediator, IViewRenderService viewRender)
        {
            _mediator = mediator;
            _viewRender = viewRender;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? testId, string? search)
        {
            ViewBag.SelectedTestId = testId;
            ViewBag.SearchTerm = search;
            var attempts = await _mediator.Send(new GetStudentAttemptsQuery(testId, search));
            return View(attempts);
        }

        [HttpPost("Reset/{id}")]
        public async Task<IActionResult> Reset(Guid id)
        {
            var result = await _mediator.Send(new ResetTestAttemptCommand(id));
            if (result)
                return Json(new { success = true, message = "Student attempt has been reset successfully." });

            return Json(new { success = false, message = "Unable to reset test attempt." });
        }

        [HttpPost("ForceSubmit/{id}")]
        public async Task<IActionResult> ForceSubmit(Guid id)
        {
            var result = await _mediator.Send(new ForceSubmitTestAttemptCommand(id));
            if (result)
                return Json(new { success = true, message = "Test attempt force submitted successfully." });

            return Json(new { success = false, message = "Failed to force submit test." });
        }

    }
}
