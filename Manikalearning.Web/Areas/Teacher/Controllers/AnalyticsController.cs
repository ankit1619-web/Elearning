using Manikalearning.Application.Features.Handlers.Teacher;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Teacher.Controllers
{
    [Route("Teacher/[controller]")]
    public class AnalyticsController : TeacherBaseController
    {
        public AnalyticsController(IMediator mediator) : base(mediator) { }

        [HttpGet("")]
        public async Task<IActionResult> Index(Guid? testId = null)
            => View(await Mediator.Send(new GetTeacherAnalyticsQuery(TeacherId, testId)));
    }
}
