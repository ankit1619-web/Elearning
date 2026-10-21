using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Handlers.Students;
using Manikalearning.Web.Areas.Student.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    [Route("plans")]
    public class PlansController : Controller
    {
        private readonly IMediator _mediator;
        public PlansController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(string? code, Guid? courseId = null, int page = 1, string? payment = null, string? messagetype = null, string? message = null, CancellationToken ct = default)
        { 
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
    }
}
