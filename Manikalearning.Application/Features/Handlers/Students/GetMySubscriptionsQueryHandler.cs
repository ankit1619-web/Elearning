using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record GetMySubscriptionsQuery(Guid UserId) : IRequest<List<MySubscriptionDto>>;

    public class GetMySubscriptionsQueryValidator : AbstractValidator<GetMySubscriptionsQuery>
    {
        public GetMySubscriptionsQueryValidator() => RuleFor(x => x.UserId).NotEmpty();
    }

    public class GetMySubscriptionsQueryHandler : IRequestHandler<GetMySubscriptionsQuery, List<MySubscriptionDto>>
    {
        private readonly IRepository<StudentSubscription> _subs;
        private readonly IRepository<Plan> _plans;
        public GetMySubscriptionsQueryHandler(IRepository<StudentSubscription> subs, IRepository<Plan> plans) { _subs = subs; _plans = plans; }

        public async Task<List<MySubscriptionDto>> Handle(GetMySubscriptionsQuery req, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var rows = await (from s in _subs.Entity().AsNoTracking()
                              join p in _plans.Entity().AsNoTracking() on s.PlanId equals p.PlanId
                              where s.UserId == req.UserId && s.Status != SubStatus.Pending   
                              orderby s.EndDate descending
                              select new { s.SubscriptionId, s.PlanId, p.PlanName, s.StartDate, s.EndDate, s.Status })
                             .Take(20).ToListAsync(ct);

            return rows.Select(r =>
            {
                var active = r.Status == SubStatus.Active && r.EndDate >= now;
                return new MySubscriptionDto
                {
                    SubscriptionId = r.SubscriptionId,
                    PlanId = r.PlanId,
                    PlanName = r.PlanName,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    IsActive = active,
                    DaysLeft = active ? (int)Math.Ceiling((r.EndDate - now).TotalDays) : 0
                };
            }).ToList();
        }
    }
}
