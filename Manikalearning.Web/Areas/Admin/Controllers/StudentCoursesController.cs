using Manikalearning.Application.Features.Commands.Assessments.StudentCourses;
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class StudentCoursesController : Controller
    {
        private readonly IMediator _mediator;

        public StudentCoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(
            string? search = null,
            Guid? courseId = null,
            bool? isActive = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = new GetStudentCoursesQuery(search, courseId, isActive, pageIndex, pageSize);
            var response = await _mediator.Send(query);

            var dropdownRes = await _mediator.Send(new GetEnrollmentDropdownsQuery());

            var viewModel = new StudentCourseListViewModel
            {
                SearchTerm = search,
                CourseIdFilter = courseId,
                IsActiveFilter = isActive,
                PageIndex = pageIndex,
                PageSize = pageSize,
                CourseFilterOptions = dropdownRes.IsSuccess ? dropdownRes.Data.Courses
                    .Select(c => new SelectListItem { Value = c.CourseId.ToString(), Text = c.Title, Selected = c.CourseId == courseId })
                    .ToList() : new()
            };

            if (response.IsSuccess && response.Data != null)
            {
                viewModel.Enrollments = response.Data.Enrollments;
                viewModel.TotalCount = response.Data.TotalCount;
                viewModel.ActiveCount = response.Data.ActiveCount;
                viewModel.InactiveCount = response.Data.InactiveCount;
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve course enrollments.";
            }

            return View(viewModel);
        }

        [HttpGet("DetailsPartial/{id:guid}")]
        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            var response = await _mediator.Send(new GetStudentCourseByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message ?? "Enrollment record not found.");

            return PartialView("_StudentCourseDetailsPartial", response.Data);
        }

        [HttpGet("EnrollPartial")]
        public async Task<IActionResult> EnrollPartial()
        {
            var response = await _mediator.Send(new GetEnrollmentDropdownsQuery());
            if (!response.IsSuccess)
                return BadRequest(response.Message);

            var vm = new EnrollStudentViewModel
            {
                UserOptions = response.Data.Users
                    .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.DisplayText })
                    .ToList(),
                CourseOptions = response.Data.Courses
                    .Select(c => new SelectListItem { Value = c.CourseId.ToString(), Text = c.Title })
                    .ToList()
            };

            return PartialView("_EnrollStudentPartial", vm);
        }

        [HttpPost("Enroll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(EnrollStudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var dropdownRes = await _mediator.Send(new GetEnrollmentDropdownsQuery());
                if (dropdownRes.IsSuccess)
                {
                    model.UserOptions = dropdownRes.Data.Users
                        .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.DisplayText })
                        .ToList();
                    model.CourseOptions = dropdownRes.Data.Courses
                        .Select(c => new SelectListItem { Value = c.CourseId.ToString(), Text = c.Title })
                        .ToList();
                }
                return PartialView("_EnrollStudentPartial", model);
            }

            var response = await _mediator.Send(model.ToCommand());
            return Json(response);
        }

        [HttpPost("ToggleStatus/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var response = await _mediator.Send(new ToggleEnrollmentStatusCommand(id));
            return Json(response);
        }

        [HttpPost("Unenroll/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unenroll(Guid id)
        {
            var response = await _mediator.Send(new UnenrollStudentCommand(id));
            return Json(response);
        }
    }
}
