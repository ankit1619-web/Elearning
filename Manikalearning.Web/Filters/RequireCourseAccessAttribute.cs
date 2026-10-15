using Manikalearning.Application.Features.Handlers.Students;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;

namespace Manikalearning.Web.Filters
{
    public class RequireCourseAccessAttribute : TypeFilterAttribute
    { 
        public RequireCourseAccessAttribute(string parameterName = "courseId", string? skipIfPresent = null) : base(typeof(RequireCourseAccessFilter))
            => Arguments = new object[] { parameterName, skipIfPresent! };

        private class RequireCourseAccessFilter : IAsyncActionFilter
        {
            private readonly IMediator _mediator;
            private readonly ITempDataDictionaryFactory _tempData;
            private readonly string _parameterName;
            private readonly string? _skipIfPresent;

            public RequireCourseAccessFilter(IMediator mediator, ITempDataDictionaryFactory tempData, string parameterName, string? skipIfPresent)
            { _mediator = mediator; _tempData = tempData; _parameterName = parameterName; _skipIfPresent = skipIfPresent; }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (!Guid.TryParse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                {
                    context.Result = new ChallengeResult();
                    return;
                }

                if (_skipIfPresent != null && context.ActionArguments.TryGetValue(_skipIfPresent, out var lesson) &&
                    ((lesson is Guid lg && lg != Guid.Empty) || (lesson is Guid? && ((Guid?)lesson).GetValueOrDefault() != Guid.Empty)))
                { await next(); return; }                      
                 
                Guid courseId = Guid.Empty;
                if (context.ActionArguments.TryGetValue(_parameterName, out var arg))
                {
                    if (arg is Guid g) courseId = g;
                    else if (arg is Guid?) courseId = ((Guid?)arg) ?? Guid.Empty;
                }
                if (courseId == Guid.Empty)
                {
                    var raw = context.RouteData.Values[_parameterName]?.ToString() ?? context.HttpContext.Request.Query[_parameterName].ToString();
                    Guid.TryParse(raw, out courseId);
                }
                if (courseId == Guid.Empty)
                {
                    context.Result = new BadRequestResult();        
                    return;
                }

                var access = await _mediator.Send(new CheckCourseAccessQuery(userId, courseId), context.HttpContext.RequestAborted);
                if (access.HasAccess) { await next(); return; }

                if (access.Reason == "NotFound") { context.Result = new NotFoundResult(); return; }

                _tempData.GetTempData(context.HttpContext)["ErrorMessage"] =
                    "This course is not included in your plan. Please choose or renew a plan.";
                context.Result = new RedirectToActionResult("Index", "Subscription", new { area = "Student" });
            }
        }
    }
}
