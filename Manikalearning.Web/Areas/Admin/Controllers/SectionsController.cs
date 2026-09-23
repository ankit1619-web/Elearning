using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Sections;
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
    public class SectionsController : Controller
    {
        private readonly IMediator _mediator;

        public SectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? subjectId = null, string search = "")
        {
            var sectionsResponse = await _mediator.Send(new GetSectionsBySubjectQuery(subjectId, search));
            var subjectsLookup = await _mediator.Send(new GetSubjectsLookupQuery());

            var sections = sectionsResponse.IsSuccess ? sectionsResponse.Data : new List<SectionDto>();
            var subjects = subjectsLookup.IsSuccess ? subjectsLookup.Data : new List<LookupDto>();

            ViewData["SubjectId"] = subjectId;
            ViewData["SearchTerm"] = search;
            ViewBag.SubjectsList = new SelectList(subjects, "Id", "Name", subjectId);

            return View(sections);
        }

        [HttpGet("CreatePartial")]
        public async Task<IActionResult> CreatePartial(Guid? subjectId = null)
        {
            var subjectsLookup = await _mediator.Send(new GetSubjectsLookupQuery());
            var subjects = subjectsLookup.IsSuccess ? subjectsLookup.Data : new List<LookupDto>();

            var viewModel = new SectionViewModel
            {
                SubjectId = subjectId ?? Guid.Empty,
                SubjectsList = subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpGet("EditPartial/{id}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetSectionByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var subjectsLookup = await _mediator.Send(new GetSubjectsLookupQuery());
            var subjects = subjectsLookup.IsSuccess ? subjectsLookup.Data : new List<LookupDto>();

            var viewModel = new SectionViewModel
            {
                SectionId = response.Data.SectionId,
                SubjectId = response.Data.SubjectId,
                SectionName = response.Data.SectionName,
                IsActive = response.Data.IsActive,
                DisplayOrder = response.Data.DisplayOrder,
                SubjectsList = subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSubjectsList(model);
                return PartialView("_CreateUpdate", model);
            }

            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSubjectsList(model);
                return PartialView("_CreateUpdate", model);
            }

            var response = await _mediator.Send(model.ToUpdateCommand());
            return Json(response);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteSectionCommand(id));
            return Json(response);
        }

        private async Task PopulateSubjectsList(SectionViewModel model)
        {
            var subjectsLookup = await _mediator.Send(new GetSubjectsLookupQuery());
            var subjects = subjectsLookup.IsSuccess ? subjectsLookup.Data : new List<LookupDto>();
            model.SubjectsList = subjects.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();
        }
    }
}
