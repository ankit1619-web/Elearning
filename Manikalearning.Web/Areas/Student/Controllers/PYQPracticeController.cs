using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Web.Areas.Student.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            var subjects = await _mediator.Send(new GetSubjectsByStudentQuery{ UserId = userId }, ct);
            var selectedSubjectId = subjectId ?? subjects.FirstOrDefault()?.SubjectId;

            List<SectionDto> sections = new();
            if (selectedSubjectId.HasValue)
            {
                var response = await _mediator.Send(new GetSectionsBySubjectQuery(selectedSubjectId.Value), ct);
                sections = response.Data;
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
            Guid? userId = null;
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var parsedId))
            {
                userId = parsedId;
            }
            var result = await _mediator.Send(new GetPYQQuestionsQuery(sectionId, userId), ct);

            return PartialView("_TopicPyqsTestPartial", result.Data ?? new TopicPyqListDto()); 
        }
    }
}
