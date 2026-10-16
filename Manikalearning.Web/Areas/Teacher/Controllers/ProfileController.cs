using Manikalearning.Application.Features.Handlers.Teacher;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Teacher.Controllers
{
    [Route("Teacher/[controller]")]
    public class ProfileController : TeacherBaseController
    {
        public ProfileController(IMediator mediator) : base(mediator) { }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var model = await Mediator.Send(new GetTeacherProfileQuery(TeacherId));
            return model == null ? NotFound() : View(model);
        }

        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TeacherProfileForm form)
            => Json(await Mediator.Send(new UpdateTeacherProfileCommand(TeacherId, form)));
    }
}
