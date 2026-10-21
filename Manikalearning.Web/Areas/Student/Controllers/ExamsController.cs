using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Exams;
using Manikalearning.Application.Features.Handlers.Assessments.Exams;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ExamsController : Controller
    {
        private readonly IMediator _mediator;
        public ExamsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(string? q, CancellationToken ct)
        {
            try { return View(new ExamPageVM { Search = q, Exams = await _mediator.Send(new GetAllExamsQuery(q), ct) }); }
            catch (ValidationException) { return RedirectToAction(nameof(Index)); }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var vm = await _mediator.Send(new GetExamDetailsQuery(id), ct);
            return vm == null ? NotFound() : View(vm);
        }
    }

    public class ExamPageVM
    {
        public string? Search { get; set; }
        public ResponseDto<List<ExamDto>> Exams { get; set; } = new();
    }
}
