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
    public class GetPaymentTransactionByIdQueryHandler : IRequestHandler<GetPaymentTransactionByIdQuery, ResponseDto<PaymentTransactionDto>>
    {
        private readonly IRepository<Payment> _transactionRepo;

        public GetPaymentTransactionByIdQueryHandler(IRepository<Payment> transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task<ResponseDto<PaymentTransactionDto>> Handle(GetPaymentTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var t = await _transactionRepo.Entity().Include(x=>x.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.PaymentId == request.TransactionId, cancellationToken);

                if (t == null)
                    return ResponseDto<PaymentTransactionDto>.Failure("Transaction record not found.");

                var dto = new PaymentTransactionDto
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
                };

                return ResponseDto<PaymentTransactionDto>.Success(dto, "Transaction details retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<PaymentTransactionDto>.Failure($"Failed to fetch transaction details: {ex.Message}");
            }
        }
    }
}
