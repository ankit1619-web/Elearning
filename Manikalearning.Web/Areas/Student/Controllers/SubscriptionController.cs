using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Plans;
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
    public class SubscriptionController : Controller
    {
        private readonly IMediator _mediator;
        public SubscriptionController(IMediator mediator) => _mediator = mediator;



        [HttpGet("")]
        public async Task<IActionResult> Index(string? code, int page = 1, string? payment = null, CancellationToken ct = default)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                return View(new SubscriptionPageVm
                {
                    Plans = await _mediator.Send(new GetPlanQuery(code), ct),
                    Payments = await _mediator.Send(new GetPaymentsQuery(userId.Value, page), ct),
                    Mine = await _mediator.Send(new GetMySubscriptionsQuery(userId.Value), ct),
                    PaymentResult = payment
                });
            }
            catch (ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid request.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Checkout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Guid planId, string? code, CancellationToken ct)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                var dto = await _mediator.Send(new CreateCheckoutCommand(userId.Value, planId, code), ct);

                if (dto.IsFree)                                  
                    return RedirectToAction(nameof(Index), new { payment = "success" });

                return View("Pay", new PayPageVm
                {
                    Checkout = dto,
                    CallbackUrl = Url.Action(nameof(PaymentCallback), "Subscription", new { area = "Student" }, Request.Scheme)!,
                    CancelUrl = Url.Action(nameof(Cancelled), "Subscription", new { area = "Student" }, Request.Scheme)!,
                    Name = User.Identity?.Name,
                    Email = User.FindFirstValue(ClaimTypes.Email)
                });
            }
            catch (ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "Could not start the payment.";
                return RedirectToAction(nameof(Index), new { code });
            }
            catch (InvalidOperationException)                   
            {
                TempData["ErrorMessage"] = "Payment service is not available right now. Please try again in a moment.";
                return RedirectToAction(nameof(Index), new { code });
            }
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [HttpPost("PaymentCallback")]
        public async Task<IActionResult> PaymentCallback([FromForm(Name = "razorpay_payment_id")] string? paymentId,[FromForm(Name = "razorpay_order_id")] string? orderId,
            [FromForm(Name = "razorpay_signature")] string? signature,CancellationToken ct)
        {
            try
            {
                var r = await _mediator.Send(new ConfirmPaymentCommand(orderId ?? "", paymentId ?? "", signature ?? ""), ct);
                return RedirectToAction(nameof(Index), new { payment = r.Success ? "success" : "failed" });
            }
            catch (ValidationException)
            {
                return RedirectToAction(nameof(Index), new { payment = "failed" });
            }
        }
         
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [AcceptVerbs("GET", "POST")]
        [Route("Cancelled")]
        public IActionResult Cancelled() => RedirectToAction(nameof(Index), new { payment = "cancelled" });

        private Guid? CurrentUserId() =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    }

    public class SubscriptionPageVm
    {
        public PlansResultDto Plans { get; set; } = new();
        public PaymentListDto Payments { get; set; } = new();
        public List<MySubscriptionDto> Mine { get; set; } = new();
        public string? PaymentResult { get; set; }
    }

    public class PayPageVm
    {
        public CheckoutDto Checkout { get; set; } = new();
        public string CallbackUrl { get; set; } = "";
        public string CancelUrl { get; set; } = "";
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
