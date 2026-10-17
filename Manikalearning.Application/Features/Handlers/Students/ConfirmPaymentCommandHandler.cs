using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Application.Services;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record ConfirmPaymentCommand(string OrderId, string PaymentId, string Signature, bool FromWebhook = false) : IRequest<ConfirmPaymentResultDto>;

    public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
    {
        public ConfirmPaymentCommandValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_]+$");
            RuleFor(x => x.PaymentId).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_]+$"); 
            RuleFor(x => x.Signature).NotEmpty().MaximumLength(200).Matches("^[A-Fa-f0-9]+$").When(x => !x.FromWebhook);
        }
    }

    public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, ConfirmPaymentResultDto>
    {
        private readonly IRepository<Payment> _payments;
        private readonly IRepository<StudentSubscription> _subs;
        private readonly IRepository<Plan> _plans;
        private readonly IRepository<Coupon> _coupons;
        private readonly IRazorpayClient _razorpay;

        private readonly IRepository<PlanCourse> _planCourses;
        private readonly IRepository<StudentCourse> _enrollments;

        public ConfirmPaymentCommandHandler(IRepository<Payment> payments, IRepository<StudentSubscription> subs,
                                            IRepository<Plan> plans, IRepository<Coupon> coupons, IRazorpayClient razorpay,
                                            IRepository<PlanCourse> planCourses, IRepository<StudentCourse> enrollments)
        { _payments = payments; _subs = subs; _plans = plans; _coupons = coupons; _razorpay = razorpay; _planCourses = planCourses; _enrollments = enrollments; }

        private static ConfirmPaymentResultDto Fail(string m) => new() { Success = false, Message = m };

        public async Task<ConfirmPaymentResultDto> Handle(ConfirmPaymentCommand req, CancellationToken ct)
        {
            var payment = await _payments.Entity()
                .FirstOrDefaultAsync(p => p.GatewayOrderId == req.OrderId && p.PaymentGateway == "Razorpay", ct);
            if (payment == null) return Fail("Payment not found.");

            if (payment.Status == PayStatus.Success)                   
                return new ConfirmPaymentResultDto { Success = true, Message = "Payment already confirmed." };
             
            if (!req.FromWebhook && !_razorpay.VerifySignature(req.OrderId, req.PaymentId, req.Signature))
                return Fail("Payment verification failed.");
             
            var order = await _razorpay.GetOrderAsync(req.OrderId, ct);
            if (order == null || order.Status != "paid" || order.Amount != (long)Math.Round(payment.Amount * 100m))
                return Fail("Payment could not be confirmed.");

            var now = DateTime.UtcNow;
            payment.Status = PayStatus.Success;
            payment.GatewayPaymentId = req.PaymentId;
            payment.PaidAt = now;

            var sub = await _subs.Entity().FirstOrDefaultAsync(s => s.PaymentId == payment.PaymentId, ct);
            if (sub != null)
            {
                var plan = await _plans.Entity().AsNoTracking().FirstAsync(p => p.PlanId == sub.PlanId, ct);
                await SubscriptionActivator.ActivateAsync(_subs, sub, plan.DurationDays, now, ct);
                await EnrollmentActivator.EnrollAsync(_enrollments, sub.UserId, sub.PlanId,sub.CourseId, sub.StartDate, sub.EndDate, now, ct);
            }
             
            if (order.Notes.TryGetValue("coupon", out var code) && !string.IsNullOrWhiteSpace(code))
            {
                var coupon = await _coupons.Entity().FirstOrDefaultAsync(c => c.CouponCode == code, ct);
                if (coupon != null) coupon.UsedCount++;
            }

            await _payments.SaveChangesAsync();                        
            return new ConfirmPaymentResultDto { Success = true, Message = "Payment successful." };
        }
    }
}
