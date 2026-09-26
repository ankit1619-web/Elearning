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
    public class GetCouponsQueryHandler : IRequestHandler<GetCouponsQuery, ResponseDto<List<CouponDto>>>
    {
        private readonly IRepository<Coupon> _couponRepo;

        public GetCouponsQueryHandler(IRepository<Coupon> couponRepo)
        {
            _couponRepo = couponRepo;
        }

        public async Task<ResponseDto<List<CouponDto>>> Handle(GetCouponsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _couponRepo.Entity().AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var term = request.Search.Trim().ToLower();
                    query = query.Where(c => c.CouponCode.ToLower().Contains(term));
                }

                var list = await query
                    .OrderByDescending(c => c.StartDate)
                    .Select(c => new CouponDto
                    {
                        CouponId = c.CouponId,
                        CouponCode = c.CouponCode,
                        DiscountType = c.DiscountType,
                        DiscountValue = c.DiscountValue,
                        MinimumAmount = c.MinimumAmount,
                        MaximumDiscount = c.MaximumDiscount,
                        UsageLimit = c.UsageLimit,
                        UsedCount = c.UsedCount,
                        StartDate = c.StartDate,
                        ExpiryDate = c.ExpiryDate,
                        IsActive = c.IsActive
                    }).ToListAsync(cancellationToken);

                return ResponseDto<List<CouponDto>>.Success(list, "Coupons retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<List<CouponDto>>.Failure($"Failed to retrieve coupons: {ex.Message}");
            }
        }
    }
}
