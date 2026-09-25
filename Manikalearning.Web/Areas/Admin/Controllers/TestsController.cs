using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Questions;
using Manikalearning.Application.Features.Commands.Assessments.Sections;
using Manikalearning.Application.Features.Commands.Assessments.Tests;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Manikalearning.Application.Interface;
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Security.Claims;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class TestsController : Controller
    { 
        private readonly IMediator _mediator;
        private readonly IViewRenderService _viewRender;

        public TestsController(IMediator mediator, IViewRenderService viewRender)
        {
            _mediator = mediator;
            _viewRender = viewRender;
        }
       

        [HttpGet]
        public async Task<IActionResult> Index(Guid? subjectId, string? search)
        {
            ViewData["SearchTerm"] = search;
            ViewData["SubjectId"] = subjectId;

            var response = await _mediator.Send(new GetTestsQuery(subjectId, search));
            var testsList = response.IsSuccess ? response.Data : new List<TestDto>();

            return View(testsList);
        }

        [HttpGet("CreatePartial")]
        public async Task<IActionResult> CreatePartial()
        {
            var viewModel = new TestViewModel
            {
                DurationMinutes = 60,
                NegativeMarking = 0.25m,
                ShowResultImmediately = true,
                IsPublished = false
            };

            await PopulateDropdowns(viewModel);
            string htmlContent = await _viewRender.RenderToStringAsync("_CreateUpdate", viewModel);
            return Json(new { success = true, html = htmlContent });
        }

        [HttpGet("EditPartial/{id}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetTestByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var t = response.Data;
            var viewModel = new TestViewModel
            {
                TestId = t.TestId,
                TestName = t.TestName,
                ExamId = t.ExamId,
                PaperId = t.PaperId,
                SubjectId = t.SubjectId,
                SectionId = t.SectionId,
                TopicId = t.TopicId,
                TestType = t.TestType,
                DurationMinutes = t.DurationMinutes,
                TotalQuestions = t.TotalQuestions,
                TotalMarks = t.TotalMarks,
                NegativeMarking = t.NegativeMarking,
                IsRandom = t.IsRandom,
                ShowResultImmediately = t.ShowResultImmediately,
                IsPublished = t.IsPublished
            };

            await PopulateDropdowns(viewModel);
            string htmlContent = await _viewRender.RenderToStringAsync("_CreateUpdate", viewModel);
            return Json(new { success = true, html = htmlContent });
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return PartialView("_CreateUpdate", model);
            } 
            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return PartialView("_CreateUpdate", model);
            }

            var response = await _mediator.Send(model.ToUpdateCommand());
            return Json(response);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteTestCommand(id));
            return Json(response);
        }
         

        [HttpGet("ManageQuestions/{id}")] 
        public async Task<IActionResult> ManageQuestions(Guid id)
        {
            var testResponse = await _mediator.Send(new GetTestByIdQuery(id));
            if (!testResponse.IsSuccess || testResponse.Data == null)
            {
                TempData["ErrorMessage"] = "Test not found.";
                return RedirectToAction(nameof(Index));
            }

            var mappedQuestionsResponse = await _mediator.Send(new GetMappedTestQuestionsQuery(id));

            ViewBag.Test = testResponse.Data;
            var mappedQuestions = mappedQuestionsResponse.IsSuccess ? mappedQuestionsResponse.Data : new List<TestQuestionDto>();

            return View(mappedQuestions);
        }

        [HttpPost("SaveQuestions")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveQuestions([FromBody] SaveTestQuestionsCommand command)
        {
            if (command == null || command.TestId == Guid.Empty)
            {
                return Json(new { success = false, message = "Invalid request payload." });
            }

            var response = await _mediator.Send(command);
            return Json(new { success = response.IsSuccess, message = response.Message });
        }
           

        private async Task PopulateDropdowns(TestViewModel model)
        {
            var subjects = await _mediator.Send(new GetSubjectsLookupQuery());
            var sections = await _mediator.Send(new GetSectionsLookupQuery(model.SubjectId ?? Guid.Empty));
            var topics = await _mediator.Send(new GetTopicsLookupQuery(model.SectionId ?? Guid.Empty));

            model.SubjectsList = subjects.Data?.Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList() ?? new();
            model.SectionsList = sections.Data?.Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList() ?? new();
            model.TopicsList = topics.Data?.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList() ?? new();
        }

        [HttpGet("GetSections")]
        public async Task<IActionResult> GetSections(Guid subjectId)
        {
            if (subjectId == Guid.Empty)
                return Json(Enumerable.Empty<object>());

            var result = await _mediator.Send(new GetSectionsLookupQuery(subjectId));
            return Json(result.Data?.Select(s => new { id = s.Id, name = s.Name }) ?? Enumerable.Empty<object>());
        }

        [HttpGet("GetTopics")]
        public async Task<IActionResult> GetTopics(Guid sectionId)
        {
            if (sectionId == Guid.Empty)
                return Json(Enumerable.Empty<object>());

            var result = await _mediator.Send(new GetTopicsLookupQuery(sectionId));
            return Json(result.Data?.Select(t => new { id = t.Id, name = t.Name }) ?? Enumerable.Empty<object>());
        } 

    }
}
