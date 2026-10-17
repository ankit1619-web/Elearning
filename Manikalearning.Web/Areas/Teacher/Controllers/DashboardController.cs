using Manikalearning.Application.Features.Handlers.Teacher;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Teacher.Controllers
{
    [Route("Teacher/[controller]")]
    public class DashboardController : TeacherBaseController
    {
        public DashboardController(IMediator mediator) : base(mediator) { }

        [HttpGet("")]
        public async Task<IActionResult> Index()
            => View(await Mediator.Send(new GetTeacherDashboardQuery(TeacherId)));
    }
}
