using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.PaymentTransaction;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.PaymentTransaction
{
    public class GetPaymentTransactionsQueryHandler : IRequestHandler<GetPaymentTransactionsQuery, ResponseDto<PaymentTransactionPagedResultDto>>
    {
        private readonly IRepository<Payment> _transactionRepo;

        public GetPaymentTransactionsQueryHandler(IRepository<Payment> transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task<ResponseDto<PaymentTransactionPagedResultDto>> Handle(GetPaymentTransactionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _transactionRepo.Entity().Include(x=>x.User).AsNoTracking();
                 
                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var term = request.Search.Trim().ToLower();
                    query = query.Where(t =>
                        t.GatewayPaymentId.ToLower().Contains(term) ||
                        t.GatewayOrderId.ToLower().Contains(term) ||
                        t.User.UserName.ToLower().Contains(term) ||
                        t.User.Email.ToLower().Contains(term));
                }

                if (!string.IsNullOrWhiteSpace(request.StatusFilter) && request.StatusFilter != "All")
                {
                    query = query.Where(t => t.Status == request.StatusFilter);
                }

                if (request.FromDate.HasValue)
                {
                    query = query.Where(t => t.CreatedAt >= request.FromDate.Value.Date);
                }

                if (request.ToDate.HasValue)
                {
                    var endDate = request.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(t => t.CreatedAt <= endDate);
                }

                var totalCount = await query.CountAsync(cancellationToken);
                var totalRevenue = await query
                    .Where(t => t.Status == "Success")
                    .SumAsync(t => t.Amount, cancellationToken);

                var items = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(t => new PaymentTransactionDto
                    {
                        TransactionId = t.PaymentId,
                        UserId = t.UserId,
                        UserName = t.User.UserName,
                        UserEmail = t.User.Email,
                        UserPhone = t.User.Mobile,
                        RazorpayOrderId = t.GatewayOrderId,
                        RazorpayPaymentId = t.GatewayPaymentId,  
                        Amount = t.Amount, 
                        Currency = t.Currency,
                        PaymentStatus = t.Status 
                    }).ToListAsync(cancellationToken);

                var result = new PaymentTransactionPagedResultDto
                {
                    Transactions = items,
                    TotalCount = totalCount,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    TotalRevenue = totalRevenue
                };

                return ResponseDto<PaymentTransactionPagedResultDto>.Success(result, "Transactions retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<PaymentTransactionPagedResultDto>.Failure($"Failed to load transactions: {ex.Message}");
            }
        }
    }
}
