using Manikalearning.Application.Features.Handlers.Teacher;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Teacher.Controllers
{
    [Route("Teacher/[controller]")]
    public class DoubtsController : TeacherBaseController
    {
        public DoubtsController(IMediator mediator) : base(mediator) { }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? status = "Open", Guid? courseId = null, int page = 1)
        {
            var model = await Mediator.Send(new GetTeacherDoubtsQuery(TeacherId, status, courseId, page));
            ViewData["Courses"] = await Mediator.Send(new GetTeacherCourseOptionsQuery(TeacherId));
            ViewData["CourseId"] = courseId;
            return View(model);
        }
         
        [HttpGet("Thread/{id:guid}")]
        public async Task<IActionResult> Thread(Guid id)
        {
            var model = await Mediator.Send(new GetTeacherDoubtThreadQuery(TeacherId, id));
            if (model == null) return NotFound();
            return PartialView("_Thread", model);
        }

        [HttpPost("Reply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(Guid doubtId, string replyText)
        {
            if (string.IsNullOrWhiteSpace(replyText))
                return Json(TeacherActionResult.Fail("Answer cannot be blank.."));
            if (replyText.Length > 2000)
                return Json(TeacherActionResult.Fail("Keep the response under 2000 characters."));

            return Json(await Mediator.Send(new AnswerDoubtCommand(doubtId, TeacherId, replyText)));
        }

        [HttpPost("Close/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(Guid id)
            => Json(await Mediator.Send(new CloseDoubtCommand(id, TeacherId)));
    }
}
