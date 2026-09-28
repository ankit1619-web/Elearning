using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudnetSubscription;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudnetSubscription
{
    public class GetStudentSubscriptionByIdQueryHandler : IRequestHandler<GetStudentSubscriptionByIdQuery, ResponseDto<StudentSubscriptionDto>>
    {
        private readonly IRepository<StudentSubscription> _subscriptionRepo;

        public GetStudentSubscriptionByIdQueryHandler(IRepository<StudentSubscription> subscriptionRepo)
        {
            _subscriptionRepo = subscriptionRepo;
        }

        public async Task<ResponseDto<StudentSubscriptionDto>> Handle(GetStudentSubscriptionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var s = await _subscriptionRepo.Entity()
                    .Include(x => x.User)
                    .Include(x => x.Plan)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.SubscriptionId == request.SubscriptionId, cancellationToken);

                if (s == null)
                    return ResponseDto<StudentSubscriptionDto>.Failure("Subscription record not found.");

                var today = DateTime.Today;
                var dto = new StudentSubscriptionDto
                {
                    SubscriptionId = s.SubscriptionId,
                    UserId = s.UserId,
                    UserName = s.User.FullName ?? "N/A",
                    UserEmail = s.User.Email ?? "N/A",
                    UserPhone = s.User.PhoneNumber ?? "N/A",
                    PlanId = s.PlanId,
                    PlanName = s.Plan.PlanName,
                    PlanPrice = s.Plan.Price,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate, 
                    Status = s.EndDate < today ? "Expired" : s.Status,
                    DaysRemaining = s.EndDate >= today ? (s.EndDate - today).Days : 0,
                    PaymentId = s.PaymentId,
                    CreatedAt = s.CreatedAt
                };

                return ResponseDto<StudentSubscriptionDto>.Success(dto, "Subscription details retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<StudentSubscriptionDto>.Failure($"Failed to fetch subscription details: {ex.Message}");
            }
        }
    }
}
