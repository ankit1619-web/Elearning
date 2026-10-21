using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Features.Handlers.Assessments.Pyq;
using Manikalearning.Web.Areas.Student.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    [Route("pyq")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class PYQController : Controller
    {
        private readonly IMediator _mediator;

        public PYQController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(
            string tab = "subject",
            Guid? examId = null,
            Guid? subjectId = null,
            Guid? paperId = null,
            CancellationToken ct = default)
        {
            var vm = new PyqBankPageVm { ActiveTab = tab };

            try
            {
                if (tab.Equals("year", StringComparison.OrdinalIgnoreCase))
                {
                    vm.YearData = await _mediator.Send(new GetPyqYearsQuery(examId, paperId), ct);
                }
                else
                {
                    var data = await _mediator.Send(new GetPyqBankSubjectsQuery(examId), ct);
                    var selected = data.Subjects.Any(s => s.SubjectId == subjectId)
                        ? subjectId
                        : data.Subjects.FirstOrDefault()?.SubjectId;

                    var sections = new List<SectionDto>();
                    if (selected.HasValue)
                    {
                        var response = await _mediator.Send(new GetSectionsBySubjectQuery(selected.Value), ct);
                        sections = response.Data ?? new List<SectionDto>();
                    }

                    vm.SubjectData = data;
                    vm.SelectedSubjectId = selected;
                    vm.Sections = sections;
                }
            }
            catch (ValidationException)
            {
                return RedirectToAction(nameof(Index), new { tab });
            } 
            return View(vm);
        }

        [HttpGet("Start/{sectionId:guid}")]
        public IActionResult Start(Guid sectionId) =>
            RedirectToAction("Start", "PyqTest", new { area = "Student", kind = "section", key = sectionId });
    } 
} 