using Manikalearning.Application.Features.Handlers.Students;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;

namespace Manikalearning.Web.Filters
{
    public class RequireSubscriptionAttribute : TypeFilterAttribute
    {
        public RequireSubscriptionAttribute() : base(typeof(RequireSubscriptionFilter)) { }

        private class RequireSubscriptionFilter : IAsyncActionFilter
        {
            private readonly IMediator _mediator;
            private readonly ITempDataDictionaryFactory _tempData;

            public RequireSubscriptionFilter(IMediator mediator, ITempDataDictionaryFactory tempData)
            { _mediator = mediator; _tempData = tempData; }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (!Guid.TryParse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                {
                    context.Result = new ChallengeResult();                   
                    return;
                }

                var ok = await _mediator.Send(new HasActiveSubscriptionQuery(userId), context.HttpContext.RequestAborted);
                if (ok) { await next(); return; }

                _tempData.GetTempData(context.HttpContext)["ErrorMessage"] = "Please choose a plan to continue.";
                context.Result = new RedirectToActionResult("Index", "Subscription", new { area = "Student" });
            }
        }
    }
     
}
