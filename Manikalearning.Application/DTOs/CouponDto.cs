using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.DTOs
{
    public class CouponDto
    {
        public Guid CouponId { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public string DiscountType { get; set; } = "Percentage"; // "Percentage" or "Fixed"
        public decimal DiscountValue { get; set; }
        public decimal? MinimumAmount { get; set; }
        public decimal? MaximumDiscount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
