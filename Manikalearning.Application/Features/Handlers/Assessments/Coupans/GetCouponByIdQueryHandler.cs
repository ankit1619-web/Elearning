using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Coupans;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Coupans
{
    public class GetCouponByIdQueryHandler : IRequestHandler<GetCouponByIdQuery, ResponseDto<CouponDto>>
    {
        private readonly IRepository<Coupon> _couponRepo;

        public GetCouponByIdQueryHandler(IRepository<Coupon> couponRepo)
        {
            _couponRepo = couponRepo;
        }

        public async Task<ResponseDto<CouponDto>> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var coupon = await _couponRepo.Entity()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CouponId == request.CouponId, cancellationToken);

                if (coupon == null)
                    return ResponseDto<CouponDto>.Failure("Coupon not found.");

                var dto = new CouponDto
                {
                    CouponId = coupon.CouponId,
                    CouponCode = coupon.CouponCode,
                    DiscountType = coupon.DiscountType,
                    DiscountValue = coupon.DiscountValue,
                    MinimumAmount = coupon.MinimumAmount,
                    MaximumDiscount = coupon.MaximumDiscount,
                    UsageLimit = coupon.UsageLimit,
                    UsedCount = coupon.UsedCount,
                    StartDate = coupon.StartDate,
                    ExpiryDate = coupon.ExpiryDate,
                    IsActive = coupon.IsActive
                };

                return ResponseDto<CouponDto>.Success(dto, "Coupon retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<CouponDto>.Failure($"Failed to retrieve coupon: {ex.Message}");
            }
        }
    }
}
