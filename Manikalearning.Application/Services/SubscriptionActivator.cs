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
        public static async Task EnrollAsync(
            IRepository<StudentCourse> enrollments,
            Guid userId,
            Guid planId,
            Guid courseId,
            DateTime start,
            DateTime end,
            DateTime now,
            CancellationToken ct)
        { 
            var existing = await enrollments.Entity()
                .Where(e => e.UserId == userId && e.CourseId == courseId)
                .ToListAsync(ct);

            var enrollment = existing.FirstOrDefault();

            if (enrollment == null)
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
                await enrollments.SaveChangesAsync();
            }
            else
            {
                enrollment.IsActive = true;

                if (enrollment.ExpiryDate != null) 
                {
                    if (enrollment.ExpiryDate < now)
                    {
                        enrollment.StartDate = start; 
                    }

                    enrollment.ExpiryDate = enrollment.ExpiryDate > end ? enrollment.ExpiryDate : end;
                }
                await enrollments.SaveChangesAsync();
            }

        }
    }
}
