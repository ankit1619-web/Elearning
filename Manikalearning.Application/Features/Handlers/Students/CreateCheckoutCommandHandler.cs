using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Application.Services;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record CreateCheckoutCommand(Guid UserId, Guid PlanId, string? CouponCode) : IRequest<CheckoutDto>;

    public class CreateCheckoutCommandValidator : AbstractValidator<CreateCheckoutCommand>
    {
        public CreateCheckoutCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.PlanId).NotEmpty();
            RuleFor(x => x.CouponCode).MaximumLength(50)
                .Matches("^[A-Za-z0-9_-]*$").WithMessage("Coupon code can only have letters, numbers, - and _.");
        }
    }

    public class CreateCheckoutCommandHandler : IRequestHandler<CreateCheckoutCommand, CheckoutDto>
    {
        private readonly IRepository<Plan> _plans;
        private readonly IRepository<Coupon> _coupons;
        private readonly IRepository<Payment> _payments;
        private readonly IRepository<StudentSubscription> _subs;
        private readonly IRazorpayClient _razorpay;

        public CreateCheckoutCommandHandler(IRepository<Plan> plans, IRepository<Coupon> coupons, IRepository<Payment> payments,
                                            IRepository<StudentSubscription> subs, IRazorpayClient razorpay)
        { _plans = plans; _coupons = coupons; _payments = payments; _subs = subs; _razorpay = razorpay; }

        public async Task<CheckoutDto> Handle(CreateCheckoutCommand req, CancellationToken ct)
        { 
            var plan = await _plans.Entity().AsNoTracking().FirstOrDefaultAsync(p => p.PlanId == req.PlanId && p.IsActive, ct)
                       ?? throw Guard.Fail("PlanId", "This plan is not available.");

            var now = DateTime.UtcNow;
            var code = req.CouponCode?.Trim();
            Coupon? coupon = null;
            decimal discount = 0;

            if (!string.IsNullOrEmpty(code))
            {
                coupon = await _coupons.Entity().FirstOrDefaultAsync(c => c.CouponCode == code, ct);    
                if (coupon == null || !CouponCalculator.IsUsable(coupon, now)) throw Guard.Fail("CouponCode", "This coupon is not valid.");
                discount = CouponCalculator.Discount(coupon, plan.Price);
                if (discount <= 0) throw Guard.Fail("CouponCode", "This coupon does not apply to this plan.");
            }

            var final = plan.Price - discount;
            if (final > 0 && final < 1) throw Guard.Fail("CouponCode", "The final amount is too small for online payment.");

            var paymentId = Guid.NewGuid();
            var sub = new StudentSubscription
            {
                SubscriptionId = Guid.NewGuid(),
                UserId = req.UserId,
                PlanId = plan.PlanId,
                PaymentId = paymentId,
                StartDate = now,
                EndDate = now.AddDays(plan.DurationDays),
                Status = SubStatus.Pending
            };

            var dto = new CheckoutDto
            {
                PaymentId = paymentId,
                PlanName = plan.PlanName,
                Price = plan.Price,
                Discount = discount,
                FinalAmount = final,
                AmountPaise = (long)Math.Round(final * 100m),
                Currency = "INR",
                KeyId = _razorpay.KeyId
            };
             
            if (final <= 0)
            {
                await SubscriptionActivator.ActivateAsync(_subs, sub, plan.DurationDays, now, ct);
                if (coupon != null) coupon.UsedCount++;

                await _payments.AddAsync(new Payment
                {
                    PaymentId = paymentId,
                    UserId = req.UserId,
                    Amount = 0,
                    Currency = "INR",
                    PaymentGateway = "Coupon",
                    GatewayOrderId = $"FREE-{paymentId:N}",
                    GatewayPaymentId = null,
                    Status = PayStatus.Success,
                    PaidAt = now,
                    CreatedAt = now
                });
                await _subs.AddAsync(sub);
                await _payments.SaveChangesAsync();

                dto.IsFree = true;
                return dto;
            }
             
            var notes = new Dictionary<string, string>
            {
                ["userId"] = req.UserId.ToString(),
                ["planId"] = plan.PlanId.ToString(),
                ["coupon"] = coupon?.CouponCode ?? ""
            };
            var order = await _razorpay.CreateOrderAsync(dto.AmountPaise, $"rcpt_{paymentId:N}", notes, ct);

            await _payments.AddAsync(new Payment
            {
                PaymentId = paymentId,
                UserId = req.UserId,
                Amount = final,
                Currency = "INR",
                PaymentGateway = "Razorpay",
                GatewayOrderId = order.Id,
                GatewayPaymentId = null,
                Status = PayStatus.Created,
                PaidAt = null,
                CreatedAt = now
            });
            await _subs.AddAsync(sub); //   "Pending" until the payment is confirmed
            await _payments.SaveChangesAsync();

            dto.OrderId = order.Id;
            return dto;
        }
    }
}
