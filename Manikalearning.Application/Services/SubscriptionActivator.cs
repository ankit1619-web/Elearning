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
        // If the student already has this plan running, the new period starts when the old one ends (renewal).
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

    public static class EnrollmentActivator
    {
        // Plan active -> student is enrolled in EVERY course of that plan (table StudentCourses), valid until the plan ends.
        public static async Task EnrollAsync(IRepository<PlanCourse> planCourses, IRepository<StudentCourse> enrollments,
                                             Guid userId, Guid planId, DateTime start, DateTime end, DateTime now, CancellationToken ct)
        {
            var courseIds = await planCourses.Entity().AsNoTracking().Where(pc => pc.PlanId == planId).Select(pc => pc.CourseId).ToListAsync(ct);
            if (courseIds.Count == 0) return;

            var existing = await enrollments.Entity().Where(e => e.UserId == userId && courseIds.Contains(e.CourseId)).ToListAsync(ct);

            foreach (var courseId in courseIds)
            {
                var e = existing.FirstOrDefault(x => x.CourseId == courseId);
                if (e == null)
                {
                    await enrollments.AddAsync(new StudentCourse
                    {
                        StudentCourseId = Guid.NewGuid(),
                        UserId = userId,
                        CourseId = courseId,
                        EnrolledAt = now,
                        StartDate = start,
                        ExpiryDate = end,
                        IsActive = true
                    });
                }
                else
                {
                    e.IsActive = true;
                    if (e.ExpiryDate != null)                              // null = lifetime access given by admin: never shorten it
                    {
                        if (e.ExpiryDate < now) e.StartDate = start;       // old access had ended -> new period starts now
                        e.ExpiryDate = e.ExpiryDate > end ? e.ExpiryDate : end;
                    }
                }
            }
        }
    }
}
