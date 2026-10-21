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
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class SubscriptionController : Controller
    {
        private readonly IMediator _mediator;
        public SubscriptionController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(string? code, Guid? courseId = null, int page = 1, string? payment = null, string? messagetype = null, string? message = null, CancellationToken ct = default)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                var courses = await _mediator.Send(new GetPlanCoursesQuery(), ct);
                var selected = courseId == null ? null : courses.FirstOrDefault(c => c.CourseId == courseId);

                return View(new SubscriptionPageVm
                {
                    Courses = courses,
                    SelectedCourseId = selected?.CourseId,
                    SelectedCourseName = selected?.CourseName,
                    Plans = selected == null ? new PlansResultDto() : await _mediator.Send(new GetPlanQuery(code, selected.CourseId), ct),
                    Payments = await _mediator.Send(new GetPaymentsQuery(userId.Value, page), ct),
                    Mine = await _mediator.Send(new GetMySubscriptionsQuery(userId.Value), ct),
                    PaymentResult = payment,
                    MessageType = messagetype,
                    Message = message
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
        public async Task<IActionResult> Checkout(Guid planId, string? code, Guid? courseId, CancellationToken ct)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });
            if (planId == Guid.Empty) return RedirectToAction(nameof(Index), new { code, messagetype = "error", message = "Please select a plan." });
            if(courseId == null || courseId == Guid.Empty) return RedirectToAction(nameof(Index), new { code, messagetype = "error", message = "Please select a course." });
            try
            {
                var dto = await _mediator.Send(new CreateCheckoutCommand(userId.Value, planId, (Guid)courseId, code), ct);

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
                return RedirectToAction(nameof(Index), new { code, courseId, messagetype = "error", message = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "Could not start the payment." });
            }
            catch (InvalidOperationException)
            {
                return RedirectToAction(nameof(Index), new { code, courseId, messagetype = "error", message = "Payment service is not available right now. Please try again in a moment." });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index), new { code, courseId, messagetype = "error", message = ex.Message });
            }
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [HttpPost("PaymentCallback")]
        public async Task<IActionResult> PaymentCallback([FromForm(Name = "razorpay_payment_id")] string? paymentId, [FromForm(Name = "razorpay_order_id")] string? orderId,
            [FromForm(Name = "razorpay_signature")] string? signature, CancellationToken ct)
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
        public List<PlanCourseOptionDto> Courses { get; set; } = new();
        public Guid? SelectedCourseId { get; set; }
        public string? SelectedCourseName { get; set; }
        public PlansResultDto Plans { get; set; } = new();
        public PaymentListDto Payments { get; set; } = new();
        public List<MySubscriptionDto> Mine { get; set; } = new();
        public string? PaymentResult { get; set; }
        public string? MessageType { get; set; }
        public string? Message { get; set; }
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
