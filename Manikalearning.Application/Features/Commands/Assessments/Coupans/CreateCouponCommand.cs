using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.Coupans
{
    public record CreateCouponCommand(
         string CouponCode,
         string DiscountType,
         decimal DiscountValue,
         decimal? MinimumAmount,
         decimal? MaximumDiscount,
         int? UsageLimit,
         DateTime StartDate,
         DateTime ExpiryDate,
         bool IsActive = true
     ) : IRequest<ResponseDto<Guid>>;
}
