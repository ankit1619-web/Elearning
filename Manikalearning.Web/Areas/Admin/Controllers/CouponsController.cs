using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Coupans;
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class CouponsController : Controller
    {
        private readonly IMediator _mediator;

        public CouponsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string search = "")
        {
            var response = await _mediator.Send(new GetCouponsQuery(search));

            var coupons = response.IsSuccess ? response.Data : new List<CouponDto>();

            if (!string.IsNullOrWhiteSpace(search) && coupons != null)
            {
                coupons = coupons
                    .Where(c => c.CouponCode.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!response.IsSuccess)
                TempData["ErrorMessage"] = response.Message;

            ViewData["SearchTerm"] = search;
            return View(coupons ?? new());
        }

        [HttpGet("CreatePartial")]
        public IActionResult CreatePartial()
        {
            var viewModel = new CouponViewModel
            {
                IsActive = true,
                DiscountType = "Percentage",
                StartDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddDays(30)
            };
            return PartialView("_CreateEditCouponPartial", viewModel);
        }

        [HttpGet("EditPartial/{id:guid}")]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            var response = await _mediator.Send(new GetCouponByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message);

            var viewModel = new CouponViewModel
            {
                CouponId = response.Data.CouponId,
                CouponCode = response.Data.CouponCode,
                DiscountType = response.Data.DiscountType,
                DiscountValue = response.Data.DiscountValue,
                MinimumAmount = response.Data.MinimumAmount,
                MaximumDiscount = response.Data.MaximumDiscount,
                UsageLimit = response.Data.UsageLimit,
                StartDate = response.Data.StartDate,
                ExpiryDate = response.Data.ExpiryDate,
                IsActive = response.Data.IsActive
            };

            return PartialView("_CreateEditCouponPartial", viewModel);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateEditCouponPartial", model);

            var response = await _mediator.Send(model.ToCreateCommand());
            return Json(response);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CouponViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateEditCouponPartial", model);

            var response = await _mediator.Send(model.ToUpdateCommand());
            return Json(response);
        }

        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteCouponCommand(id));
            return Json(response);
        }
    }
}
