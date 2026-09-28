using Manikalearning.Application.Features.Commands.Assessments.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search = "")
        {
            ViewBag.SearchTerm = search;
            var users = await _mediator.Send(new GetUsersQuery(search));
            return View(users);
        }

        [HttpPost("ToggleStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Invalid user ID.";
                return RedirectToAction(nameof(Index));
            }

            var isSuccess = await _mediator.Send(new ToggleUserStatusCommand(userId));

            if (isSuccess)
            {
                TempData["SuccessMessage"] = "User status updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update user status.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
