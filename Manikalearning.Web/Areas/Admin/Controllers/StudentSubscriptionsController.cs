using Manikalearning.Application.Features.Commands.Assessments.StudnetSubscription;
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class StudentSubscriptionsController : Controller
    {
        private readonly IMediator _mediator;

        public StudentSubscriptionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(
            string? search = null,
            string status = "All",
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = new GetStudentSubscriptionsQuery(search, status, pageIndex, pageSize);
            var response = await _mediator.Send(query);

            if (!response.IsSuccess || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to load student subscriptions.";
                return View(new StudentSubscriptionListViewModel());
            }

            var viewModel = new StudentSubscriptionListViewModel
            {
                Subscriptions = response.Data.Subscriptions,
                SearchTerm = search,
                StatusFilter = status,
                PageIndex = response.Data.PageIndex,
                PageSize = response.Data.PageSize,
                TotalCount = response.Data.TotalCount,
                ActiveCount = response.Data.ActiveCount,
                ExpiredCount = response.Data.ExpiredCount
            };

            return View(viewModel);
        }

        [HttpGet("DetailsPartial/{id:guid}")]
        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            var response = await _mediator.Send(new GetStudentSubscriptionByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message ?? "Subscription details not found.");

            return PartialView("_SubscriptionDetailsPartial", response.Data);
        }

        [HttpGet("ExtendPartial/{id:guid}")]
        public async Task<IActionResult> ExtendPartial(Guid id)
        {
            var response = await _mediator.Send(new GetStudentSubscriptionByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message ?? "Subscription not found.");

            var vm = new ExtendSubscriptionViewModel
            {
                SubscriptionId = response.Data.SubscriptionId,
                StudentName = response.Data.UserName,
                PlanName = response.Data.PlanName,
                CurrentEndDate = response.Data.EndDate,
                AdditionalDays = 30
            };

            return PartialView("_ExtendSubscriptionPartial", vm);
        }

        [HttpPost("Extend")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Extend(ExtendSubscriptionViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ExtendSubscriptionPartial", model);

            var response = await _mediator.Send(model.ToCommand());
            return Json(response);
        }

        [HttpPost("Cancel/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var response = await _mediator.Send(new CancelSubscriptionCommand(id));
            return Json(response);
        }
    }
}
