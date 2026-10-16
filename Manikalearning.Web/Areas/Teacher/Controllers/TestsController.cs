using Manikalearning.Application.Features.Handlers.Teacher;
using Manikalearning.Web.Areas.Teacher.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Teacher.Controllers
{
    [Route("Teacher/[controller]")]
    public class TestsController : TeacherBaseController
    {
        public TestsController(IMediator mediator) : base(mediator) { }
         
        [HttpGet("")]
        public async Task<IActionResult> Index()
            => View(await Mediator.Send(new GetTeacherTestsQuery(TeacherId)));

        [HttpGet("CreatePartial")]
        public async Task<IActionResult> CreatePartial()
            => PartialView("_TestForm", await BuildVm(new TeacherTestForm()));

        [HttpGet("EditPartial/{id:guid}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var form = await Mediator.Send(new GetTeacherTestFormQuery(TeacherId, id));
            return form == null ? NotFound() : PartialView("_TestForm", await BuildVm(form));
        }

        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TeacherTestForm form)
            => Json(await Mediator.Send(new SaveTeacherTestCommand(TeacherId, form)));

        [HttpPost("TogglePublish/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(Guid id)
            => Json(await Mediator.Send(new ToggleTestPublishCommand(TeacherId, id)));

        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
            => Json(await Mediator.Send(new DeleteTeacherTestCommand(TeacherId, id)));
         
        [HttpGet("Manage/{id:guid}")]
        public async Task<IActionResult> Manage(Guid id)
        {
            var model = await Mediator.Send(new GetTestManageQuery(TeacherId, id));
            return model == null ? NotFound() : View(model);
        }

        [HttpGet("PickPartial/{id:guid}")]
        public async Task<IActionResult> PickPartial(Guid id, string? search, int page = 1)
        {
            var model = await Mediator.Send(new GetPickQuestionsQuery(TeacherId, id, search, page));
            if (model == null) return NotFound();
            ViewData["TestId"] = id;
            ViewData["Search"] = search;
            return PartialView("_PickQuestions", model);
        }

        [HttpPost("AddQuestions/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestions(Guid id, List<Guid> questionIds)
            => Json(await Mediator.Send(new AddQuestionsToTestCommand(TeacherId, id, questionIds ?? new())));

        [HttpPost("RemoveQuestion/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveQuestion(Guid id, Guid questionId)
            => Json(await Mediator.Send(new RemoveTestQuestionCommand(TeacherId, id, questionId)));

        [HttpPost("MoveQuestion/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveQuestion(Guid id, Guid questionId, bool up)
            => Json(await Mediator.Send(new MoveTestQuestionCommand(TeacherId, id, questionId, up)));
         
        private async Task<TestFormViewModel> BuildVm(TeacherTestForm f)
        {
            var vm = new TestFormViewModel { Form = f };
            vm.Exams = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Exams));
            if (f.ExamId != null) vm.Papers = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Papers, f.ExamId));
            if (f.PaperId != null) vm.Subjects = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Subjects, f.PaperId));
            if (f.SubjectId != null) vm.Sections = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Sections, f.SubjectId));
            if (f.SectionId != null) vm.Topics = await Mediator.Send(new GetTeacherLookupQuery(LookupKind.Topics, f.SectionId));
            return vm;
        }
    }
}
