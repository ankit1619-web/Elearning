using Manikalearning.Application.Features.Commands.Assessments.Coupans;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class CouponViewModel
    {
        public Guid? CouponId { get; set; }

        [Required(ErrorMessage = "Coupon code is required.")]
        [StringLength(50, ErrorMessage = "Coupon code cannot exceed 50 characters.")]
        [Display(Name = "Coupon Code")]
        public string CouponCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Discount type is required.")]
        [Display(Name = "Discount Type")]
        public string DiscountType { get; set; } = "Percentage"; // "Percentage" or "Fixed"

        [Required(ErrorMessage = "Discount value is required.")]
        [Range(0.01, 100000, ErrorMessage = "Enter a valid discount value.")]
        [Display(Name = "Discount Value")]
        public decimal DiscountValue { get; set; }

        [Display(Name = "Minimum Order Amount (₹)")]
        [Range(0, 100000, ErrorMessage = "Please enter a valid minimum amount.")]
        public decimal? MinimumAmount { get; set; }

        [Display(Name = "Max Discount Amount (₹)")]
        [Range(0, 100000, ErrorMessage = "Please enter a valid maximum discount.")]
        public decimal? MaximumDiscount { get; set; }

        [Display(Name = "Usage Limit")]
        [Range(1, 1000000, ErrorMessage = "Usage limit must be at least 1.")]
        public int? UsageLimit { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Expiry Date is required.")]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; } = DateTime.Today.AddDays(30);

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;

        // Command Conversion Helpers
        public CreateCouponCommand ToCreateCommand()
        {
            return new CreateCouponCommand(
                CouponCode,
                DiscountType,
                DiscountValue,
                MinimumAmount,
                MaximumDiscount,
                UsageLimit,
                StartDate,
                ExpiryDate,
                IsActive
            );
        }

        public UpdateCouponCommand ToUpdateCommand()
        {
            return new UpdateCouponCommand(
                CouponId ?? Guid.Empty,
                CouponCode,
                DiscountType,
                DiscountValue,
                MinimumAmount,
                MaximumDiscount,
                UsageLimit,
                StartDate,
                ExpiryDate,
                IsActive
            );
        }
    }
}
