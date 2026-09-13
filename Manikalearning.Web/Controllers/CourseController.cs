using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    public class CourseController : BaseController
    {
        public CourseController(IMediator mediator)
        : base(mediator)
        {
        }
        public async Task<IActionResult> Index(Guid? examId,CancellationToken cancellationToken)
        {
            var dto = await _mediator.Send( new GetCoursesPageQuery(examId),cancellationToken);
            var firstCourse = dto.FirstOrDefault();
            var model = new CourseVM
            {
                ExamName = firstCourse?.ExamName ?? (examId.HasValue ? "Exam Preparation" : "All Courses"),
                Items = dto.Select(x => new CourseVM
                {
                    CourseId = x.Id,
                    CourseName = x.CourseName,  
                    Description = x.Description,
                    ThumbnailUrl = x.ThumbnailUrl, 
                    Price = x.Price, 
                    TotalLectures = x.TotalLectures,  
                }).ToList()
            };

            return View(model);
        }
        public async Task<IActionResult> Details(Guid? course, CancellationToken cancellationToken)
        {
            if (!course.HasValue)
            {
                return BadRequest();
            }

            var dto = await _mediator.Send(new GetCourseDetailsQuery(course.Value), cancellationToken);
            var model = new CourseVM
            {
                CourseId = dto.Id,
                CourseName = dto.CourseName,
                Description = dto.Description,
                Price = dto.Price,
                TotalLectures = dto.TotalLectures,
                Sections = dto.Sections.Select(s => new PublicCourseSectionVM
                {
                    CourseSectionId = s.CourseSectionId,
                    SectionName = s.SectionName,
                    Lectures = s.Lectures.Select(l => new PublicLectureVM
                    {
                        LectureId = l.LectureId,
                        LectureTitle = l.LectureTitle,
                        Description = l.Description,
                        VideoUrl = l.VideoUrl,
                        VideoDurationSeconds = l.VideoDurationSeconds,
                        NotesUrl = l.NotesUrl,
                        IsFreePreview = l.IsFreePreview,
                        DisplayOrder = l.DisplayOrder
                    }).ToList()
                }).ToList()
            };

            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetLecturePartial(Guid lectureId)
        {
            var l = await _mediator.Send(new GetLecturePartialQuery(lectureId));

            if (l == null)
                return NotFound();
            var model = new PublicLectureVM
            {                 
                LectureId = l.LectureId,
                LectureTitle = l.LectureTitle,
                Description = l.Description,
                VideoUrl = l.VideoUrl,
                VideoDurationSeconds = l.VideoDurationSeconds,
                NotesUrl = l.NotesUrl,
                IsFreePreview = l.IsFreePreview,
                DisplayOrder = l.DisplayOrder
                    
            };
            return PartialView("_LectureViewerPartial", model);
        }
    }
}
