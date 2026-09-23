using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Sections;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
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
    public class TopicsController : Controller
    {
        private readonly IMediator _mediator;

        public TopicsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? sectionId = null, string search = "")
        {
            var topicsResponse = await _mediator.Send(new GetTopicsBySectionQuery(sectionId, search));
            var sectionsLookup = await _mediator.Send(new GetSectionsLookupQuery());

            var topics = topicsResponse.IsSuccess ? topicsResponse.Data : new List<TopicDto>();
            var sections = sectionsLookup.IsSuccess ? sectionsLookup.Data : new List<LookupDto>();

            ViewData["SectionId"] = sectionId;
            ViewData["SearchTerm"] = search;
            ViewBag.SectionsList = new SelectList(sections, "Id", "Name", sectionId);

            return View(topics);
        }

        [HttpGet("CreatePartial")]
        public async Task<IActionResult> CreatePartial(Guid? sectionId = null)
        {
            var sectionsLookup = await _mediator.Send(new GetSectionsLookupQuery());
            var sections = sectionsLookup.IsSuccess ? sectionsLookup.Data : new List<LookupDto>();

            var viewModel = new TopicViewModel
            {
                SectionId = sectionId ?? Guid.Empty,
                SectionsList = sections.Select(s => new SelectListItem
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
            var response = await _mediator.Send(new GetTopicByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var sectionsLookup = await _mediator.Send(new GetSectionsLookupQuery());
            var sections = sectionsLookup.IsSuccess ? sectionsLookup.Data : new List<LookupDto>();

            var viewModel = new TopicViewModel
            {
                TopicId = response.Data.TopicId,
                SectionId = response.Data.SectionId,
                TopicName = response.Data.TopicName,
                Description = response.Data.Description,
                IsActive = response.Data.IsActive,
                DisplayOrder = response.Data.DisplayOrder,
                SectionsList = sections.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TopicViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSectionsList(model);
                return PartialView("_CreateUpdate", model);
            }

            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TopicViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSectionsList(model);
                return PartialView("_CreateUpdate", model);
            }

            var response = await _mediator.Send(model.ToUpdateCommand());
            return Json(response);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteTopicCommand(id));
            return Json(response);
        }

        private async Task PopulateSectionsList(TopicViewModel model)
        {
            var sectionsLookup = await _mediator.Send(new GetSectionsLookupQuery());
            var sections = sectionsLookup.IsSuccess ? sectionsLookup.Data : new List<LookupDto>();
            model.SectionsList = sections.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();
        }
    }
}
