using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record GetPaymentsQuery(Guid UserId, int Page = 1, int PageSize = 10) : IRequest<PaymentListDto>;

    public class GetPaymentsQueryValidator : AbstractValidator<GetPaymentsQuery>
    {
        public GetPaymentsQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(5, 50);
        }
    }

    public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, PaymentListDto>
    {
        private readonly IRepository<Payment> _payments;
        public GetPaymentsQueryHandler(IRepository<Payment> payments) => _payments = payments;

        public async Task<PaymentListDto> Handle(GetPaymentsQuery req, CancellationToken ct)
        {
            var q = _payments.Entity().AsNoTracking().Where(p => p.UserId == req.UserId);
            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(p => p.CreatedAt)
                .Skip((req.Page - 1) * req.PageSize).Take(req.PageSize)
                .Select(p => new PaymentDto    
                {
                    PaymentId = p.PaymentId,
                    Amount = p.Amount,
                    Currency = p.Currency,
                    Status = p.Status,
                    Gateway = p.PaymentGateway,
                    PaidAt = p.PaidAt,
                    CreatedAt = p.CreatedAt
                }).ToListAsync(ct);

            return new PaymentListDto { Items = items, Page = req.Page, PageSize = req.PageSize, TotalCount = total };
        }
    }
}
