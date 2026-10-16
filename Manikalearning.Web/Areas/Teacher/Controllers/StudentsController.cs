using Manikalearning.Application.Features.Handlers.Teacher;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Teacher.Controllers
{
    [Route("Teacher/[controller]")]
    public class StudentsController : TeacherBaseController
    {
        public StudentsController(IMediator mediator) : base(mediator) { }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? search, Guid? courseId = null, int page = 1)
        {
            var model = await Mediator.Send(new GetTeacherStudentsQuery(TeacherId, search, courseId, page));
            ViewData["Courses"] = await Mediator.Send(new GetTeacherCourseOptionsQuery(TeacherId));
            ViewData["CourseId"] = courseId;
            ViewData["Search"] = search;
            return View(model);
        }

        [HttpGet("Details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await Mediator.Send(new GetTeacherStudentDetailQuery(TeacherId, id));
            return model == null ? NotFound() : PartialView("_StudentDetail", model);
        }
    }
}
