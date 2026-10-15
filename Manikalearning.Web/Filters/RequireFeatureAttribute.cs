using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Handlers.Students;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;

namespace Manikalearning.Web.Filters
{
    public class RequireFeatureAttribute : TypeFilterAttribute
    {
        public RequireFeatureAttribute(string feature) : base(typeof(RequireFeatureFilter)) => Arguments = new object[] { feature };

        private class RequireFeatureFilter : IAsyncActionFilter
        {
            private readonly IMediator _mediator;
            private readonly ITempDataDictionaryFactory _tempData;
            private readonly string _feature;

            public RequireFeatureFilter(IMediator mediator, ITempDataDictionaryFactory tempData, string feature)
            { _mediator = mediator; _tempData = tempData; _feature = feature; }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (!Guid.TryParse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                { context.Result = new ChallengeResult(); return; }

                if (await _mediator.Send(new HasFeatureQuery(userId, _feature), context.HttpContext.RequestAborted))
                { await next(); return; }

                _tempData.GetTempData(context.HttpContext)["ErrorMessage"] =
                    $"Your plan does not include \"{PlanFeatureCodes.Label(_feature)}\". Choose a plan that has it.";
                context.Result = new RedirectToActionResult("Index", "Subscription", new { area = "Student" });
            }
        }
    }
}
