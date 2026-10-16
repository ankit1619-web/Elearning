using Manikalearning.Application.Features.Handlers.Teacher;
using Manikalearning.Application.Interface;
using Manikalearning.Web.Areas.Teacher.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Teacher.Controllers
{
    [Route("Teacher/[controller]")]
    public class QuestionsController : TeacherBaseController
    {
        private readonly ITranslationService _translationService;
        public QuestionsController(IMediator mediator, ITranslationService translationService) : base(mediator)
        {
            _translationService = translationService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            ViewData["Search"] = search;
            return View(await Mediator.Send(new GetTeacherQuestionsQuery(TeacherId, search, null, page)));
        }
         
        [HttpGet("CreatePartial")]
        public async Task<IActionResult> CreatePartial()
            => PartialView("_QuestionForm", await BuildVm(new TeacherQuestionForm()));

        [HttpGet("EditPartial/{id:guid}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var form = await Mediator.Send(new GetTeacherQuestionFormQuery(TeacherId, id));
            return form == null ? NotFound() : PartialView("_QuestionForm", await BuildVm(form));
        }

        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TeacherQuestionForm form)
            => Json(await Mediator.Send(new SaveTeacherQuestionCommand(TeacherId, form)));

        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
            => Json(await Mediator.Send(new DeleteTeacherQuestionCommand(TeacherId, id)));

        private async Task<QuestionFormViewModels> BuildVm(TeacherQuestionForm form)
        {
            var vm = new QuestionFormViewModels { Form = form };
            vm.Subjects = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Subjects));
            vm.Passages = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Passages));
            if (form.PickSubjectId != null)
                vm.Sections = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Sections, form.PickSubjectId));
            if (form.PickSectionId != null)
                vm.Topics = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Topics, form.PickSectionId));
            if (form.TopicId != null)
                vm.SubTopics = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.SubTopics, form.TopicId));
            return vm;
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
