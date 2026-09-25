using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Passages;
using Manikalearning.Application.Features.Commands.Assessments.Questions;
using Manikalearning.Application.Features.Commands.Assessments.Sections;
using Manikalearning.Application.Features.Commands.Assessments.Subtopics;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Manikalearning.Application.Interface;
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
    public class QuestionsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMediator _mediator;
        private readonly IViewRenderService _viewRender;
        private readonly ITranslationService _translationService;
        public QuestionsController(IMediator mediator, IWebHostEnvironment webHostEnvironment, IViewRenderService viewRender, ITranslationService translationService)
        {
            _webHostEnvironment = webHostEnvironment;
            _mediator = mediator; 
            _viewRender = viewRender;
            _translationService = translationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? topicId = null, Guid? subTopicId = null, string search = "")
        {
            var response = await _mediator.Send(new GetQuestionsQuery(topicId, subTopicId, search));
            var questions = response.IsSuccess ? response.Data : new List<QuestionDto>();

            ViewData["TopicId"] = topicId;
            ViewData["SubTopicId"] = subTopicId;
            ViewData["SearchTerm"] = search;

            return View(questions);
        }

        [HttpGet("CreatePartial")]
        public async Task<IActionResult> CreatePartial(Guid? topicId = null, Guid? subTopicId = null)
        {
            var viewModel = new QuestionViewModel
            {
                TopicId = topicId ?? Guid.Empty,
                SubTopicId = subTopicId,
                Options = new List<QuestionOptionViewModel>
                {
                    new() { OptionLabel = "A", DisplayOrder = 1 },
                    new() { OptionLabel = "B", DisplayOrder = 2 },
                    new() { OptionLabel = "C", DisplayOrder = 3 },
                    new() { OptionLabel = "D", DisplayOrder = 4 }
                }
            };

            await PopulateDropdowns(viewModel);
            string htmlContent = await _viewRender.RenderToStringAsync("_CreateUpdate", viewModel);
            return Json(new { success = true, html = htmlContent });
        }

        [HttpGet("EditPartial/{id}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetQuestionByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var q = response.Data;
            var viewModel = new QuestionViewModel
            {
                QuestionId = q.QuestionId,
                SubjectId = q.SubjectId,
                SectionId = q.SectionId,
                TopicId = q.TopicId,
                SubTopicId = q.SubTopicId,
                ExamId = q.ExamId,
                PaperId = q.PaperId,
                PassageId = q.PassageId,
                QuestionType = q.QuestionType,
                 
                QuestionText = q.QuestionText,
                QuestionTextHindi = q.QuestionTextHindi,
                Explanation = q.Explanation,
                ExplanationHindi = q.ExplanationHindi,
                ShortExplanation = q.ShortExplanation,
                ShortExplanationHindi = q.ShortExplanationHindi,

                Difficulty = q.Difficulty,
                Marks = q.Marks,
                NegativeMarks = q.NegativeMarks,
                IsPYQ = q.IsPYQ,
                PYQYear = q.PYQYear,
                PYQSession = q.PYQSession,
                SourceName = q.SourceName,
                IsApproved = q.IsApproved,
                IsActive = q.IsActive,
                 
                Options = q.Options.Select(o => new QuestionOptionViewModel
                {
                    OptionId = o.OptionId,
                    OptionLabel = o.OptionLabel,
                    OptionText = o.OptionText,
                    OptionTextHindi = o.OptionTextHindi,  
                    IsCorrect = o.IsCorrect,
                    DisplayOrder = o.DisplayOrder
                }).ToList()
            };

            await PopulateDropdowns(viewModel);
            string htmlContent = await _viewRender.RenderToStringAsync("_CreateUpdate", viewModel);
            return Json(new { success = true, html = htmlContent });
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionViewModel model)
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
        public async Task<IActionResult> Edit(QuestionViewModel model)
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
            var response = await _mediator.Send(new DeleteQuestionCommand(id));
            return Json(response);
        }

        private async Task PopulateDropdowns(QuestionViewModel model)
        {
            var subjects = await _mediator.Send(new GetSubjectsLookupQuery());
            var sections = await _mediator.Send(new GetSectionsLookupQuery(Guid.Empty));
            var topics = await _mediator.Send(new GetTopicsLookupQuery(Guid.Empty));
            var subtopics = await _mediator.Send(new GetSubTopicsLookupQuery(Guid.Empty));
            var passages = await _mediator.Send(new GetPassagesLookupQuery());

            model.SubjectsList = subjects.Data?.Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList() ?? new();
            model.SectionsList = sections.Data?.Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList() ?? new();
            model.TopicsList = topics.Data?.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList() ?? new();
            model.SubTopicsList = subtopics.Data?.Select(st => new SelectListItem(st.Name, st.Id.ToString())).ToList() ?? new();
            model.PassagesList = passages.Data?.Select(p => new SelectListItem(p.Name, p.Id.ToString())).ToList() ?? new();
        }

        [HttpGet("GetSections")]
        public async Task<IActionResult> GetSections(Guid subjectId)
        {
            if (subjectId == Guid.Empty)
                return Json(Enumerable.Empty<object>());

            var result = await _mediator.Send(new GetSectionsLookupQuery(subjectId));
            var sections = result.Data?.Select(s => new { id = s.Id, name = s.Name }) ?? Enumerable.Empty<object>();

            return Json(sections);
        }
         
        [HttpGet("GetTopics")]
        public async Task<IActionResult> GetTopics(Guid sectionId)
        {
            if (sectionId == Guid.Empty)
                return Json(Enumerable.Empty<object>());

            var result = await _mediator.Send(new GetTopicsLookupQuery(sectionId));
            var topics = result.Data?.Select(t => new { id = t.Id, name = t.Name }) ?? Enumerable.Empty<object>();

            return Json(topics);
        }
         
        [HttpGet("GetSubTopics")]
        public async Task<IActionResult> GetSubTopics(Guid topicId)
        {
            if (topicId == Guid.Empty)
                return Json(Enumerable.Empty<object>());

            var result = await _mediator.Send(new GetSubTopicsLookupQuery(topicId));
            var subTopics = result.Data?.Select(st => new { id = st.Id, name = st.Name }) ?? Enumerable.Empty<object>();

            return Json(subTopics);
        }
         
        [HttpGet("GetPassages")]
        public async Task<IActionResult> GetPassages(Guid topicId)
        {
            if (topicId == Guid.Empty)
                return Json(Enumerable.Empty<object>());

            var result = await _mediator.Send(new GetPassagesLookupQuery(topicId));
            var passages = result.Data?.Select(p => new { id = p.Id, name = p.Name }) ?? Enumerable.Empty<object>();

            return Json(passages);
        }
         
        [HttpPost("AutoTranslate")]
        public async Task<IActionResult> AutoTranslate(string text, string targetLang = "hi")
        {
            if (string.IsNullOrWhiteSpace(text))
                return Json(new { translatedText = "" });

            try
            { 
                  string translated = await _translationService.TranslateAsync(text, targetLang);

                string translatedText = translated;  

                return Json(new { translatedText });
            }
            catch (Exception ex)
            {
                return Json(new { translatedText = "", error = ex.Message });
            }
        }
         
    }
}
