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
    public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Coupon> _couponRepo;

        public UpdateCouponCommandHandler(IRepository<Coupon> couponRepo)
        {
            _couponRepo = couponRepo;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var coupon = await _couponRepo.Entity()
                    .FirstOrDefaultAsync(c => c.CouponId == request.CouponId, cancellationToken);

                if (coupon == null)
                    return ResponseDto<bool>.Failure("Coupon not found.");

                var couponCodeUpper = request.CouponCode.Trim().ToUpper();
                var duplicate = await _couponRepo.Entity()
                    .AnyAsync(c => c.CouponCode == couponCodeUpper && c.CouponId != request.CouponId, cancellationToken);

                if (duplicate)
                    return ResponseDto<bool>.Failure("Another coupon with this code already exists.");

                coupon.CouponCode = couponCodeUpper;
                coupon.DiscountType = request.DiscountType;
                coupon.DiscountValue = request.DiscountValue;
                coupon.MinimumAmount = request.MinimumAmount;
                coupon.MaximumDiscount = request.MaximumDiscount;
                coupon.UsageLimit = request.UsageLimit;
                coupon.StartDate = request.StartDate;
                coupon.ExpiryDate = request.ExpiryDate;
                coupon.IsActive = request.IsActive;

                 _couponRepo.Update(coupon);
                await _couponRepo.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Coupon updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to update coupon: {ex.Message}");
            }
        }
    }
}
