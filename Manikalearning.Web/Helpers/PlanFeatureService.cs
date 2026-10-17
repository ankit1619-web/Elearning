using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Handlers.Students;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Services
{
    public class PlanFeatureService : IPlanFeatureService
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _http;
        private HashSet<string>? _cache;

        public PlanFeatureService(IMediator mediator, IHttpContextAccessor http) { _mediator = mediator; _http = http; }

        public async Task<HashSet<string>> AllAsync()
        {
            if (_cache != null) return _cache;
            var id = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _cache = Guid.TryParse(id, out var uid)
                ? await _mediator.Send(new GetMyFeaturesQuery(uid))
                : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            return _cache;
        }

        public async Task<bool> HasAsync(string featureCode) => (await AllAsync()).Contains(featureCode);
    }
     
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePlanFeatureAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _code;
        public RequirePlanFeatureAttribute(string featureCode) { _code = featureCode; }

        public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
        {
            var svc = ctx.HttpContext.RequestServices.GetRequiredService<IPlanFeatureService>();
            if (await svc.HasAsync(_code)) { await next(); return; }

            var label = PlanFeatureCodes.Label(_code);
            ctx.Result = new RedirectToActionResult("Index", "Subscription", new
            {
                area = "Student",
                messagetype = "error",
                message = $"{label} is not included in your current plan. Choose a plan that includes it."
            });
        }
    }
}
