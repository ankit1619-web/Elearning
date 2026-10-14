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
    public class SubscriptionController : Controller
    {
        private readonly IMediator _mediator;
        public SubscriptionController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(string? code, int page = 1, CancellationToken ct = default)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                return View(new SubscriptionPageVm
                {
                    Plans = await _mediator.Send(new GetPlanQuery(code), ct),
                    Payments = await _mediator.Send(new GetPaymentsQuery(userId, page), ct)
                });
            }
            catch (ValidationException ex)
            { 
                TempData["ErrorMessage"] = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid request.";
                return RedirectToAction(nameof(Index));
            }
        }
    }

    public class SubscriptionPageVm
    {
        public PlansResultDto Plans { get; set; } = new();
        public PaymentListDto Payments { get; set; } = new();
    }
}
