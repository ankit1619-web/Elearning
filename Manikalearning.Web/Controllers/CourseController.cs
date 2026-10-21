using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Features.Handlers.Students;
using Manikalearning.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class CourseController : BaseController
    {
        public CourseController(IMediator mediator)
        : base(mediator)
        {
        }
        public async Task<IActionResult> Index(Guid? examId,CancellationToken cancellationToken)
        {
            var model = await _mediator.Send( new GetCoursesPageQuery(examId),cancellationToken); 
            return View(model);
        }
        public async Task<IActionResult> Details(Guid? course, CancellationToken cancellationToken)
        {
            if (!course.HasValue)
            {
                return BadRequest();
            }

            var overview = await _mediator.Send(new GetCourseDetailsQuery(course.Value), cancellationToken); 
            var viewModel = new WatchIndexViewModel
            {
                CourseOverview = overview,
                CurrentLesson = new LessonPlayerViewModel()
            };  
            return View(viewModel);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetLesson(Guid courseId, Guid lessonId, CancellationToken cancellationToken)
        {
            var model = await _mediator.Send(new GetLecturePartialQuery(lessonId, courseId));
            await ApplyPlanAccessAsync(model, cancellationToken);
            return PartialView("_LectureViewerPartial", model);
        }
        private async Task ApplyPlanAccessAsync(LessonPlayerViewModel m, CancellationToken ct)
        {
            if (m.LessonId == Guid.Empty) return;
            m.HasNotesPdf = !string.IsNullOrEmpty(m.NotesUrl);

            if (m.IsFree) return;
             
            m.CanWatchVideo = false;
            m.CanOpenNotes = false;
            m.CanAskDoubt = false;

            if (!m.CanWatchVideo) m.VideoUrl = null;
            if (!m.CanOpenNotes) { m.NotesUrl = null; m.Notes = null; }
        }
    }
}
