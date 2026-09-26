using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.PaymentTransaction
{
    public record GetPaymentTransactionsQuery(
        string? Search = null,
        string? StatusFilter = null,
        DateTime? FromDate = null,
        DateTime? ToDate = null,
        int PageIndex = 1,
        int PageSize = 10
    ) : IRequest<ResponseDto<PaymentTransactionPagedResultDto>>;
}
