using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Web.Areas.Student.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]")]
    public class PYQPracticeController : Controller
    {
        private readonly IMediator _mediator;

        public PYQPracticeController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(Guid? subjectId, CancellationToken ct)
        {
            var subjects = await _mediator.Send(new GetAllSubjectsQuery(), ct);
            var selectedSubjectId = subjectId ?? subjects.FirstOrDefault()?.SubjectId;

            List<SectionDto> sections = new();
            if (selectedSubjectId.HasValue)
            {
                sections = await _mediator.Send(new GetSectionsBySubjectQuery(selectedSubjectId.Value), ct);
            }

            var vm = new PYQPracticeViewModel
            {
                Subjects = subjects,
                SelectedSubjectId = selectedSubjectId,
                Sections = sections
            };

            return View(vm);
        }

        [HttpGet("Practice/{sectionId}")]
        public async Task<IActionResult> Practice(Guid sectionId, CancellationToken ct)
        {
            var questions = await _mediator.Send(new GetPYQQuestionsQuery(sectionId), ct);
            return View(questions);
        }
    }
}
