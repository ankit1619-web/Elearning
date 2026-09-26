using Manikalearning.Application.Features.Commands.Assessments.Plans;
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class PlanCoursesController : Controller
    {
        private readonly IMediator _mediator;

        public PlanCoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("Manage")]
        [HttpGet("Manage/{planId:guid}")]
        public async Task<IActionResult> Manage(Guid planId)
        {
            var response = await _mediator.Send(new GetPlanCourseMappingQuery(planId));

            if (!response.IsSuccess || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to load plan courses.";
                return RedirectToAction("Index", "Plans");
            }

            var viewModel = new PlanCourseMappingViewModel
            {
                PlanId = response.Data.PlanId,
                PlanName = response.Data.PlanName,
                SelectedCourseIds = response.Data.SelectedCourseIds,
                AvailableCourses = response.Data.AvailableCourses
            };

            return View(viewModel);
        }

        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromBody] PlanCourseMappingViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { isSuccess = false, message = "Invalid data submitted." });

            var response = await _mediator.Send(model.ToSaveCommand());
            return Json(response);
        }
    }
}
