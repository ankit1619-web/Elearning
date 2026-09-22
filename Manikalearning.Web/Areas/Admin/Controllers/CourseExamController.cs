using Manikalearning.Application.Features.Commands.Assessments.Courses;
using Manikalearning.Application.Features.Commands.Assessments.ExamCourse;
using Manikalearning.Application.Features.Commands.Assessments.Exams;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class CourseExamController : Controller
    {
        private readonly IMediator _mediator;

        public CourseExamController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Manage")]
        [HttpGet("Manage/{courseId:guid}")]
        public async Task<IActionResult> Manage(Guid courseId)
        {
            var courseResult = await _mediator.Send(new GetCourseByIdQuery(courseId));
            ViewBag.Course = courseResult.Data;

            var mappedResult = await _mediator.Send(new GetExamsByCourseIdQuery(courseId));
             
            var allExamsResult = await _mediator.Send(new GetAllExamsQuery());

            ViewBag.CourseId = courseId;
            ViewBag.MappedExams = mappedResult.Data ?? new();

            var mappedExamIds = mappedResult.Data?.Select(x => x.ExamId).ToList() ?? new List<Guid>();
            var unmappedExams = allExamsResult.Data?.Where(e => !mappedExamIds.Contains(e.ExamId)).ToList() ?? new();

            ViewBag.UnmappedExams = unmappedExams;

            return View();
        }

        [HttpPost("MapExam")]
        public async Task<IActionResult> MapExam([FromBody] MapExamToCourseCommand command)
        {
            var result = await _mediator.Send(command);
            return Json(result);
        }

        [HttpPost("UnmapExam")]
        public async Task<IActionResult> UnmapExam([FromBody] UnmapExamFromCourseCommand command)
        {
            var result = await _mediator.Send(command);
            return Json(result);
        }
    }
}
