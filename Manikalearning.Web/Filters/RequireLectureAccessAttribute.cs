using Manikalearning.Application.Features.Handlers.Students;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;

namespace Manikalearning.Web.Filters
{
    public class RequireLectureAccessAttribute : TypeFilterAttribute
    {
        public RequireLectureAccessAttribute(string parameterName = "lessonId", bool optional = false) : base(typeof(RequireLectureAccessFilter))
            => Arguments = new object[] { parameterName, optional };

        private class RequireLectureAccessFilter : IAsyncActionFilter
        {
            private readonly IMediator _mediator;
            private readonly ITempDataDictionaryFactory _tempData;
            private readonly string _name;
            private readonly bool _optional;

            public RequireLectureAccessFilter(IMediator mediator, ITempDataDictionaryFactory tempData, string parameterName, bool optional)
            { _mediator = mediator; _tempData = tempData; _name = parameterName; _optional = optional; }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (!Guid.TryParse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                { context.Result = new ChallengeResult(); return; }

                Guid lectureId = Guid.Empty;
                if (context.ActionArguments.TryGetValue(_name, out var arg))
                {
                    if (arg is Guid g) lectureId = g;
                    else if (arg is Guid?) lectureId = ((Guid?)arg) ?? Guid.Empty;
                }

                if (lectureId == Guid.Empty)
                {
                    if (_optional) { await next(); return; }
                    context.Result = new BadRequestResult();            
                    return;
                }

                var access = await _mediator.Send(new CheckLectureAccessQuery(userId, lectureId), context.HttpContext.RequestAborted);
                if (access.HasAccess) { await next(); return; }
                if (access.Reason == "NotFound") { context.Result = new NotFoundResult(); return; }

                _tempData.GetTempData(context.HttpContext)["ErrorMessage"] = "This lecture is not included in your plan. Please choose or renew a plan.";
                context.Result = new RedirectToActionResult("Index", "Subscription", new { area = "Student" });
            }
        }
    }
}
