using FluentValidation;
using Manikalearning.Application.Features.Handlers.Assessments.Pyq;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [AllowAnonymous]
    [Route("pyq-bank/years")]
    public class PyqYearsController : Controller
    {
        private readonly IMediator _mediator;
        public PyqYearsController(IMediator mediator) => _mediator = mediator;
         
        [HttpGet("")]
        public async Task<IActionResult> Index(Guid? examId, Guid? paperId, CancellationToken ct)
        {
            try { return View(await _mediator.Send(new GetPyqYearsQuery(examId, paperId), ct)); }
            catch (ValidationException) { return RedirectToAction(nameof(Index)); }
        }
    }
}
