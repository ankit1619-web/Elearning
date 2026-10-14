using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manikalearning.Domain.Entities
{
    [Table("Plans")]
    public class Plan
    {
        [Key]
        public Guid PlanId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150)]
        public string PlanName { get; set; }

        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
         
        public ICollection<PlanCourse> PlanCourses { get; set; }
        public ICollection<StudentSubscription> StudentSubscriptions { get; set; }
    }


    [Table("PlanCourses")]
    public class PlanCourse
    {
        public Guid PlanId { get; set; }
        [ForeignKey("PlanId")]
        public Plan Plan { get; set; }

        public Guid CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }


    [Table("StudentSubscriptions")]
    public class StudentSubscription
    {
        [Key]
        public Guid SubscriptionId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public Guid PlanId { get; set; }
        [ForeignKey("PlanId")]
        public Plan Plan { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }

        public Guid? PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    


    [Table("Payments")]
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public decimal Amount { get; set; }
        [MaxLength(10)]
        public string Currency { get; set; } = "INR";

        [MaxLength(50)]
        public string PaymentGateway { get; set; }

        [MaxLength(200)]
        public string GatewayOrderId { get; set; }
        [MaxLength(200)]
        public string? GatewayPaymentId { get; set; }

        [MaxLength(30)]
        public string Status { get; set; }

        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<StudentSubscription> StudentSubscriptions { get; set; }
    }



    [Table("Coupons")]
    public class Coupon
    {
        [Key]
        public Guid CouponId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string CouponCode { get; set; }

        [Required, MaxLength(20)]
        public string DiscountType { get; set; } // Percentage, Fixed

        public decimal DiscountValue { get; set; }
        public decimal? MinimumAmount { get; set; }
        public decimal? MaximumDiscount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0;

        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

     
}
