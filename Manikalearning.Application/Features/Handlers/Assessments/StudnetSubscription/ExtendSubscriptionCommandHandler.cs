using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudnetSubscription;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudnetSubscription
{
    public class ExtendSubscriptionCommandHandler : IRequestHandler<ExtendSubscriptionCommand, ResponseDto<bool>>
    {
        private readonly IRepository<StudentSubscription> _subscriptionRepo;
        private readonly IRepository<StudentCourse> _studentCourseRepo;

        public ExtendSubscriptionCommandHandler(
            IRepository<StudentSubscription> subscriptionRepo,
            IRepository<StudentCourse> studentCourseRepo)
        {
            _subscriptionRepo = subscriptionRepo;
            _studentCourseRepo = studentCourseRepo;
        }
       
        public async Task<ResponseDto<bool>> Handle(ExtendSubscriptionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sub = await _subscriptionRepo.Entity()
                    .FirstOrDefaultAsync(s => s.SubscriptionId == request.SubscriptionId, cancellationToken);

                if (sub == null)
                    return ResponseDto<bool>.Failure("Subscription record not found.");

                // 2. Calculate New End/Expiry Date
                var baseDate = sub.EndDate < DateTime.Today ? DateTime.Today : sub.EndDate;
                var newEndDate = baseDate.AddDays(request.AdditionalDays);

                sub.EndDate = newEndDate;
                _subscriptionRepo.Update(sub);

                var studentCourses = await _studentCourseRepo.Entity()
                    .Where(sc => sc.UserId == sub.UserId)
                    .ToListAsync(cancellationToken);

                foreach (var course in studentCourses)
                { 
                    course.ExpiryDate = newEndDate;
                    course.IsActive = newEndDate > DateTime.UtcNow;

                    _studentCourseRepo.Update(course);
                }
                await _subscriptionRepo.SaveChangesAsync();
                return ResponseDto<bool>.Success(true, $"Subscription successfully extended by {request.AdditionalDays} days.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to extend subscription: {ex.Message}");
            }
        }
    }
}
