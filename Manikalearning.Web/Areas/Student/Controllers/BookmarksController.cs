using FluentValidation;
using Manikalearning.Application.DTOs;
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
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class BookmarksController : Controller
    {
        private readonly IMediator _mediator;
        public BookmarksController(IMediator mediator) => _mediator = mediator;
         
        [HttpGet("")]
        public async Task<IActionResult> Index(string? type, string? q, int page = 1, CancellationToken ct = default)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var kind = string.Equals(type, BookmarkType.Lecture, StringComparison.OrdinalIgnoreCase) ? BookmarkType.Lecture : BookmarkType.Question;

            try
            {
                var vm = await _mediator.Send(new GetBookmarksQuery(userId.Value, kind, q, page), ct);
                 
                if (vm.Items.Count == 0 && page > 1 && vm.TotalCount > 0)
                    return RedirectToAction(nameof(Index), new { type = kind, q, page = vm.TotalPages });

                return View(vm);
            }
            catch (ValidationException)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Remove/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid id, string? type, string? q, int page = 1, CancellationToken ct = default)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                var removed = await _mediator.Send(new RemoveBookmarkCommand(userId.Value, id), ct);
                TempData[removed ? "SuccessMessage" : "ErrorMessage"] = removed ? "Removed from Saved." : "Item not found.";
            }
            catch (ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid request.";
            }
            return RedirectToAction(nameof(Index), new { type, q, page });
        }

        private Guid? CurrentUserId() =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    }
} 
