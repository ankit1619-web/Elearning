using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Features.Handlers.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    [Route("pyq-bank")]
    public class PYQController : Controller
    {
        private const int MinCount = 5;
        private const int MaxCount = 50;
        private const int MaxAnswers = 100;

        private readonly IMediator _mediator;

        public PYQController(IMediator mediator) => _mediator = mediator;
         
        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var model = await _mediator.Send(new GetPyqBankQuery(), ct);
            return View("PyqIndex", model);
        }
         
        [HttpGet("practice")]
        public async Task<IActionResult> Practice(int? subjectId, int? year, int count = 10, CancellationToken ct = default)
        {
            count = Math.Clamp(count, MinCount, MaxCount);

            var model = await _mediator.Send(new GetPyqPracticeSetQuery(subjectId, year, count), ct);

            if (model is null)
            {
                TempData["PyqMessage"] = "Is selection ke liye abhi questions available nahi hain.";
                return RedirectToAction(nameof(Index));
            }

            return View("PyqPractice", model);
        }
         
        [HttpPost("practice/evaluate")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Evaluate([FromBody] List<AnswerInput>? answers, CancellationToken ct)
        {
            if (answers is null || answers.Count == 0 || answers.Count > MaxAnswers)
                return BadRequest(new { error = "Invalid answers." });

            var result = await _mediator.Send(new EvaluatePyqAnswersQuery(answers), ct);
            return Json(result);
        }
    }
}
