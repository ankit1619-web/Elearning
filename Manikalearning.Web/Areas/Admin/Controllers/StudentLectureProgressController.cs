using Manikalearning.Application.Features.Commands.Assessments.StudentLectureProgress;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class StudentLectureProgressController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Course> _courseRepo;

        public StudentLectureProgressController(IMediator mediator, IRepository<Course> courseRepo)
        {
            _mediator = mediator;
            _courseRepo = courseRepo;
        }

        [HttpGet]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(
            string? search = null,
            Guid? courseId = null,
            bool? isCompleted = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = new GetStudentLectureProgressQuery(search, courseId, isCompleted, pageIndex, pageSize);
            var response = await _mediator.Send(query);

            var courses = await _courseRepo.Entity()
                .AsNoTracking()
                .Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = c.CourseName
                }).ToListAsync();

            var viewModel = new StudentLectureProgressListViewModel
            {
                SearchTerm = search,
                CourseIdFilter = courseId,
                IsCompletedFilter = isCompleted,
                CourseOptions = courses,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            if (response.IsSuccess && response.Data != null)
            {
                viewModel.ProgressRecords = response.Data.ProgressRecords;
                viewModel.TotalCount = response.Data.TotalCount;
                viewModel.CompletedCount = response.Data.CompletedCount;
                viewModel.InProgressCount = response.Data.InProgressCount;
                viewModel.AverageCompletionRate = response.Data.AverageCompletionRate;
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to load progress records.";
            }

            return View(viewModel);
        }

        [HttpGet("DetailsPartial/{id:guid}")]
        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            var response = await _mediator.Send(new GetLectureProgressByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message ?? "Record not found.");

            return PartialView("_ProgressDetailsPartial", response.Data);
        }

        [HttpPost("ResetProgress/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetProgress(Guid id)
        {
            var response = await _mediator.Send(new ResetLectureProgressCommand(id));
            return Json(response);
        }
    }
}
