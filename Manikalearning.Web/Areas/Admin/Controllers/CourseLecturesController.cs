using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.CourseLecture;
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
    public class CourseLecturesController : Controller
    {
        private readonly IMediator _mediator;

        public CourseLecturesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{sectionId:guid}")]
        [HttpGet("Index/{sectionId:guid}")]
        public async Task<IActionResult> Index(Guid sectionId)
        {
            var lecturesResponse = await _mediator.Send(new GetCourseLecturesBySectionIdQuery(sectionId));
            var lectures = lecturesResponse.IsSuccess ? lecturesResponse.Data : new List<CourseLectureDto>();

            var sectionResponse = await _mediator.Send(new GetCourseSectionByIdQuery(sectionId));
            if (sectionResponse.IsSuccess && sectionResponse.Data != null)
            {
                ViewData["SectionTitle"] = sectionResponse.Data.SectionName;
                ViewData["CourseId"] = sectionResponse.Data.CourseId;
            }

            ViewData["SectionId"] = sectionId;
            return View(lectures ?? new());
        }

        [HttpGet("CreatePartial/{sectionId:guid}")]
        public IActionResult CreatePartial(Guid sectionId)
        {
            var viewModel = new CourseLectureViewModel
            {
                CourseSectionId = sectionId,
                DisplayOrder = 1,
                IsActive = true
            };
            return PartialView("_CreateUpdateLecture", viewModel);
        }

        [HttpGet("EditPartial/{id:guid}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetCourseLectureByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var viewModel = new CourseLectureViewModel
            {
                LectureId = response.Data.LectureId,
                CourseSectionId = response.Data.CourseSectionId,
                Title = response.Data.LectureTitle,
                VideoUrl = response.Data.VideoUrl,
                PdfUrl = response.Data.NotesUrl,
                DurationInMinutes = response.Data.VideoDurationSeconds ?? 0,
                DisplayOrder = response.Data.DisplayOrder,
                IsPreview = response.Data.IsFreePreview,
                IsActive = response.Data.IsPublished
            };

            return PartialView("_CreateUpdateLecture", viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseLectureViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdateLecture", model);

            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseLectureViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdateLecture", model);

            var response = await _mediator.Send(model.ToUpdateCommand());
            return Json(response);
        }

        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteCourseLectureCommand(id));
            return Json(response);
        }
    }
}