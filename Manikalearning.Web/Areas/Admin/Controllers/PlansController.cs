using Manikalearning.Application.DTOs;
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
    public class PlansController : Controller
    {
        private readonly IMediator _mediator;

        public PlansController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string search = "")
        {
            var response = await _mediator.Send(new GetPlansQuery(search));

            var plans = response.IsSuccess ? response.Data : new List<PlanDto>();

            if (!string.IsNullOrWhiteSpace(search) && plans != null)
            {
                plans = plans
                    .Where(p => p.PlanName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!response.IsSuccess)
                TempData["ErrorMessage"] = response.Message;

            ViewData["SearchTerm"] = search;
            return View(plans ?? new());
        }

        [HttpGet("CreatePartial")]
        public IActionResult CreatePartial()
        {
            var viewModel = new PlanViewModel
            {
                IsActive = true,
                DurationDays = 365
            };
            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpGet("EditPartial/{id:guid}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetPlanByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var viewModel = new PlanViewModel
            {
                PlanId = response.Data.PlanId,
                PlanName = response.Data.PlanName,
                Price = response.Data.Price,
                DurationDays = response.Data.DurationDays,
                Description = response.Data.Description,
                IsActive = response.Data.IsActive
            };

            return PartialView("_CreateUpdate", viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdate", model);

            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PlanViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUpdate", model);

            var response = await _mediator.Send(model.ToUpdateCommand());
            return Json(response);
        }

        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeletePlanCommand(id));
            return Json(response);
        }
    }
}
