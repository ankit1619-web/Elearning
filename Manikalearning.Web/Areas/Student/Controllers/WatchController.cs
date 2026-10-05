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
    public class WatchController : Controller
    {
        private readonly IMediator _mediator;

        public WatchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Index/{courseId}/{lessonId?}")]
        public async Task<IActionResult> Index(Guid courseId, Guid? lessonId, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
             
            var overview = await _mediator.Send(new GetWatchOverviewQuery(userId, courseId), cancellationToken);
             
            var activeLessonId = lessonId ?? overview.Modules.FirstOrDefault()?.Lessons.FirstOrDefault()?.LessonId;

            LessonPlayerViewModel? currentLesson = null;
            if (activeLessonId.HasValue && activeLessonId.Value != Guid.Empty)
            {
                currentLesson = await _mediator.Send(new GetLessonPlayerQuery(userId, courseId, activeLessonId.Value), cancellationToken);
            } 
            var viewModel = new WatchIndexViewModel
            {
                CourseOverview = overview,
                CurrentLesson = currentLesson ?? new LessonPlayerViewModel()
            };

            return View(viewModel);
        }

        [HttpGet("GetLesson")]
        public async Task<IActionResult> GetLesson(Guid courseId, Guid lessonId, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var lessonModel = await _mediator.Send(new GetLessonPlayerQuery(userId, courseId, lessonId), cancellationToken);

            return PartialView("_LessonPlayerPartial", lessonModel);
        }
    }
}
