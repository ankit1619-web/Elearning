using FluentValidation;
using FluentValidation.Results;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Services
{
    internal static class Guard
    {
        public static ValidationException Fail(string property, string message) =>
            new(new[] { new ValidationFailure(property, message) });
    }

    public static class SubscriptionActivator
    { 
        public static async Task ActivateAsync(IRepository<StudentSubscription> subs, StudentSubscription sub,
                                               int durationDays, DateTime now, CancellationToken ct)
        {
            var latestEnd = await subs.Entity()
                .Where(s => s.UserId == sub.UserId && s.PlanId == sub.PlanId && s.Status == SubStatus.Active
                            && s.EndDate > now && s.SubscriptionId != sub.SubscriptionId)
                .MaxAsync(s => (DateTime?)s.EndDate, ct);

            var start = latestEnd ?? now;
            sub.StartDate = start;
            sub.EndDate = start.AddDays(durationDays);
            sub.Status = SubStatus.Active;
        }
    }
}
