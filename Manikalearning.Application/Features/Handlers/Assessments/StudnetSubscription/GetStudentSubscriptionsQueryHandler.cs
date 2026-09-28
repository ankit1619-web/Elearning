using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudnetSubscription;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudnetSubscription
{
    public class GetStudentSubscriptionsQueryHandler : IRequestHandler<GetStudentSubscriptionsQuery, ResponseDto<StudentSubscriptionPagedResultDto>>
    {
        private readonly IRepository<StudentSubscription> _subscriptionRepo;

        public GetStudentSubscriptionsQueryHandler(IRepository<StudentSubscription> subscriptionRepo)
        {
            _subscriptionRepo = subscriptionRepo;
        }

        public async Task<ResponseDto<StudentSubscriptionPagedResultDto>> Handle(GetStudentSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _subscriptionRepo.Entity()
                    .Include(s => s.User)
                    .Include(s => s.Plan)
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var term = request.Search.Trim().ToLower();
                    query = query.Where(s =>
                        s.User.FullName.ToLower().Contains(term) ||
                        s.User.Email!.ToLower().Contains(term) ||
                        s.Plan.PlanName.ToLower().Contains(term));
                }

                var today = DateTime.Today;

                if (!string.IsNullOrWhiteSpace(request.StatusFilter) && request.StatusFilter != "All")
                {
                    if (request.StatusFilter == "Active")
                        query = query.Where(s => s.Status == "Active" && s.EndDate >= today);
                    else if (request.StatusFilter == "Expired")
                        query = query.Where(s => s.EndDate < today);
                    else if (request.StatusFilter == "Cancelled")
                        query = query.Where(s => s.Status == "Cancelled" && s.EndDate >= today);
                }

                var totalCount = await query.CountAsync(cancellationToken);
                var activeCount = await _subscriptionRepo.Entity().CountAsync(s => s.Status == "Active" && s.EndDate >= today, cancellationToken);
                var expiredCount = await _subscriptionRepo.Entity().CountAsync(s => s.EndDate < today, cancellationToken);

                var items = await query
                    .OrderByDescending(s => s.CreatedAt)
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(s => new StudentSubscriptionDto
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
                    }).ToListAsync(cancellationToken);

                var result = new StudentSubscriptionPagedResultDto
                {
                    Subscriptions = items,
                    TotalCount = totalCount,
                    ActiveCount = activeCount,
                    ExpiredCount = expiredCount,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize
                };

                return ResponseDto<StudentSubscriptionPagedResultDto>.Success(result, "Subscriptions retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<StudentSubscriptionPagedResultDto>.Failure($"Failed to retrieve subscriptions: {ex.Message}");
            }
        }
    }
}
