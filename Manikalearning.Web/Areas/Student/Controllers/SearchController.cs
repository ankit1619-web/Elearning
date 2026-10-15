using Manikalearning.Application.Features.Handlers.Students;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]")]
    public class SearchController : Controller
    {
        private readonly IMediator _mediator;
        public SearchController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(string? q, CancellationToken ct)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return RedirectToAction("Login", "Account", new { area = "" });

            if (string.IsNullOrWhiteSpace(q)) return View(new SearchResultsDto());           

            try { return View(await _mediator.Send(new GlobalSearchQuery(userId, q), ct)); }
            catch (ValidationException ex)
            {
                ViewBag.Error = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid search.";
                return View(new SearchResultsDto { Query = q.Trim() });
            }
        }
    }
}
