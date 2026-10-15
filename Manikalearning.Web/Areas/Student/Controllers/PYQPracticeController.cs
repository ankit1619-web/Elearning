using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Pyq;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Features.Handlers.Assessments.Pyq;
using Manikalearning.Web.Areas.Student.ViewModels;
using Manikalearning.Web.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [AllowAnonymous]                                
    [Route("pyq-bank")]
    [Route("Student/[controller]")]
    public class PYQPracticeController : Controller
    {
        private readonly IMediator _mediator;

        public PYQPracticeController(IMediator mediator) => _mediator = mediator;

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

    public class PyqBankPageVm
    {
        public string ActiveTab { get; set; } = "subject";  
         
        public PyqBankSubjectsDto SubjectData { get; set; } = new();
        public Guid? SelectedSubjectId { get; set; }
        public List<SectionDto> Sections { get; set; } = new();
         
        public PyqYearsDto YearData { get; set; } = new();
    }
}