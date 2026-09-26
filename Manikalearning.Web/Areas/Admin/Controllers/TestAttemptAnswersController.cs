
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
    public class TestAttemptAnswersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IViewRenderService _viewRender;

        public TestAttemptAnswersController(IMediator mediator, IViewRenderService viewRender)
        {
            _mediator = mediator;
            _viewRender = viewRender;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid attemptId)
        {
            var attemptDetail = await _mediator.Send(new GetTestAttemptDetailQuery(attemptId));

            if (attemptDetail == null)
            {
                return NotFound();
            }

            return View(attemptDetail);
        }
    }
}
