using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Bookmarks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class BookmarksController : Controller
    {
        private readonly IMediator _mediator;

        public BookmarksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search = "")
        {
            var response = await _mediator.Send(new GetStudentBookmarksQuery(Search: search, PageSize: 1000));

            var bookmarks = response.IsSuccess && response.Data?.Bookmarks != null
                ? response.Data.Bookmarks
                : new List<BookmarkDto>();

            if (!response.IsSuccess)
                TempData["ErrorMessage"] = response.Message;

            ViewData["SearchTerm"] = search;
            return View(bookmarks);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _mediator.Send(new GetBookmarkByIdQuery(id));

            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            return PartialView("_BookmarkDetailsPartial", response.Data);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteBookmarkCommand(id));
            return Json(response);
        }
    }
}
