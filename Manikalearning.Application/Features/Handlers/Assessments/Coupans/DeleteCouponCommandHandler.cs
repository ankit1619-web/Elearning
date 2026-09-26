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
    public class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Coupon> _couponRepo;

        public DeleteCouponCommandHandler(IRepository<Coupon> couponRepo)
        {
            _couponRepo = couponRepo;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var coupon = await _couponRepo.Entity()
                    .FirstOrDefaultAsync(c => c.CouponId == request.CouponId, cancellationToken);

                if (coupon == null)
                    return ResponseDto<bool>.Failure("Coupon not found.");

                  _couponRepo.Remove(coupon);
                await _couponRepo.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Coupon deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to delete coupon: {ex.Message}");
            }
        }
    }
}
