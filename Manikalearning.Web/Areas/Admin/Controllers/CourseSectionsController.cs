using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Coursesection; 
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class CourseSectionsController : Controller
    {
        private readonly IMediator _mediator;

        public CourseSectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{courseId:guid}")]
        [HttpGet("Index/{courseId:guid}")]
        public async Task<IActionResult> Index(Guid courseId, string search = "")
        {
            var response = await _mediator.Send(new GetCourseSectionsByCourseIdQuery(courseId));

            var sections = response.IsSuccess ? response.Data : new List<CourseSectionDto>();

            if (!string.IsNullOrWhiteSpace(search) && sections != null)
            {
                sections = sections
                    .Where(s => s.SectionName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!response.IsSuccess)
                TempData["ErrorMessage"] = response.Message;

            ViewData["CourseId"] = courseId;
            ViewData["SearchTerm"] = search;
            return View(sections ?? new());
        }

        [HttpGet("CreatePartial/{courseId:guid}")]
        public IActionResult CreatePartial(Guid courseId)
        {
            var viewModel = new CourseSectionViewModel
            {
                CourseId = courseId,
                DisplayOrder = 1,
                IsActive = true
            };
            return PartialView("_CreateUpdateSection", viewModel);
        }

        [HttpGet("EditPartial/{id:guid}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetCourseSectionByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var viewModel = new CourseSectionViewModel
            {
                SectionId = response.Data.CourseSectionId,
                CourseId = response.Data.CourseId,
                Title = response.Data.SectionName,
                Description = response.Data.Description,
                DisplayOrder = response.Data.DisplayOrder,
                IsActive = response.Data.IsActive
            };

            return PartialView("_CreateUpdateSection", viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseSectionViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdateSection", model);

            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseSectionViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdateSection", model);

            var response = await _mediator.Send(model.ToUpdateCommand());
            return Json(response);
        }

        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid courseId)
        {
            var response = await _mediator.Send(new DeleteCourseSectionCommand(id));

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
            }
            else
            {
                TempData["ErrorMessage"] = response.Errors?.Count > 0
                    ? string.Join(" | ", response.Errors)
                    : response.Message;
            }

            return RedirectToAction(nameof(Index), new { courseId });
        }
    }
}