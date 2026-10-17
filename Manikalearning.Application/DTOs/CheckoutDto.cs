namespace Manikalearning.Application.DTOs
{
    public static class SubStatus { public const string Pending = "Pending", Active = "Active", Expired = "Expired"; }
    public static class PayStatus { public const string Created = "Created", Success = "Success", Failed = "Failed"; }

    public class CheckoutDto
    {
        public bool IsFree { get; set; } // 100% coupon / free plan: already activated, no payment page
        public Guid PaymentId { get; set; }
        public string OrderId { get; set; } = "";
        public string KeyId { get; set; } = "";
        public long AmountPaise { get; set; }
        public string Currency { get; set; } = "INR";
        public string PlanName { get; set; } = "";
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalAmount { get; set; }
    }

    public class ConfirmPaymentResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
    }

    public class MySubscriptionDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = "";
        public string CourseName { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int DaysLeft { get; set; }
    }
}
