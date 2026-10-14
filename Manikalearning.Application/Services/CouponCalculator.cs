using Manikalearning.Domain.Entities;

namespace Manikalearning.Application.Services
{
    public static class CouponCalculator
    {
        public static bool IsUsable(Coupon c, DateTime nowUtc) =>
            c.IsActive && nowUtc >= c.StartDate && nowUtc <= c.ExpiryDate &&
            (c.UsageLimit == null || c.UsedCount < c.UsageLimit);

        public static decimal Discount(Coupon c, decimal amount)
        {
            if (amount < c.MinimumAmount) return 0;

            var type = (c.DiscountType ?? "").Trim().ToLowerInvariant();
            var isPercent = type.StartsWith("per") || type == "%";
            var d = isPercent ? amount * c.DiscountValue / 100m : c.DiscountValue;

            if (c.MaximumDiscount > 0)
                d = Math.Min(d, (decimal)c.MaximumDiscount);  

            d = Math.Min(d, amount);
            return Math.Round(Math.Max(0, d), 2);
        }
    }
}
