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
    public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<Coupon> _couponRepo;

        public CreateCouponCommandHandler(IRepository<Coupon> couponRepo)
        {
            _couponRepo = couponRepo;
        }

        public async Task<ResponseDto<Guid>> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var couponCodeUpper = request.CouponCode.Trim().ToUpper();
                var exists = await _couponRepo.Entity()
                    .AnyAsync(c => c.CouponCode == couponCodeUpper, cancellationToken);

                if (exists)
                    return ResponseDto<Guid>.Failure("A promo coupon with this code already exists.");

                var coupon = new Coupon
                {
                    CouponId = Guid.NewGuid(),
                    CouponCode = couponCodeUpper,
                    DiscountType = request.DiscountType,
                    DiscountValue = request.DiscountValue,
                    MinimumAmount = request.MinimumAmount,
                    MaximumDiscount = request.MaximumDiscount,
                    UsageLimit = request.UsageLimit,
                    UsedCount = 0,
                    StartDate = request.StartDate,
                    ExpiryDate = request.ExpiryDate,
                    IsActive = request.IsActive
                };

                await _couponRepo.AddAsync(coupon);
                await _couponRepo.SaveChangesAsync();

                return ResponseDto<Guid>.Success(coupon.CouponId, "Coupon created successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<Guid>.Failure($"Failed to create coupon: {ex.Message}");
            }
        }
    }
}
