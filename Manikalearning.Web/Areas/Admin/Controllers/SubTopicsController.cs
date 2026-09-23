using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subtopics;
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
    public class SubTopicsController : Controller
    {
        private readonly IMediator _mediator;

        public SubTopicsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? topicId = null, string search = "")
        {
            var subTopicsResponse = await _mediator.Send(new GetSubTopicsByTopicQuery(topicId, search));
            var topicsLookup = await _mediator.Send(new GetTopicsLookupQuery(Guid.Empty));

            var subTopics = subTopicsResponse.IsSuccess ? subTopicsResponse.Data : new List<SubTopicDto>();
            var topics = topicsLookup.IsSuccess ? topicsLookup.Data : new List<LookupDto>();

            ViewData["TopicId"] = topicId;
            ViewData["SearchTerm"] = search;
            ViewBag.TopicsList = new SelectList(topics, "Id", "Name", topicId);

            return View(subTopics);
        }

        [HttpGet("CreatePartial")]
        public async Task<IActionResult> CreatePartial(Guid? topicId = null)
        {
            var topicsLookup = await _mediator.Send(new GetTopicsLookupQuery(Guid.Empty));
            var topics = topicsLookup.IsSuccess ? topicsLookup.Data : new List<LookupDto>();

            var viewModel = new SubTopicViewModel
            {
                TopicId = topicId ?? Guid.Empty,
                TopicsList = topics.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList()
            };

            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpGet("EditPartial/{id}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetSubTopicByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var topicsLookup = await _mediator.Send(new GetTopicsLookupQuery(Guid.Empty));
            var topics = topicsLookup.IsSuccess ? topicsLookup.Data : new List<LookupDto>();

            var viewModel = new SubTopicViewModel
            {
                SubTopicId = response.Data.SubTopicId,
                TopicId = response.Data.TopicId,
                SubTopicName = response.Data.SubTopicName,
                IsActive = response.Data.IsActive,
                TopicsList = topics.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList()
            };

            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubTopicViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateTopicsList(model);
                return PartialView("_CreateUpdate", model);
            }

            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubTopicViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateTopicsList(model);
                return PartialView("_CreateUpdate", model);
            }

            var response = await _mediator.Send(model.ToUpdateCommand());
            return Json(response);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteSubTopicCommand(id));
            return Json(response);
        }

        private async Task PopulateTopicsList(SubTopicViewModel model)
        {
            var topicsLookup = await _mediator.Send(new GetTopicsLookupQuery(Guid.Empty));
            var topics = topicsLookup.IsSuccess ? topicsLookup.Data : new List<LookupDto>();
            model.TopicsList = topics.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }).ToList();
        }
    }
}
