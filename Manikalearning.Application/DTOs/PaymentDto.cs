namespace Manikalearning.Application.DTOs
{
    public class PaymentDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Status { get; set; } = "";
        public string Gateway { get; set; } = "";
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
     

    public class PlansResultDto
    {
        public List<PlanDto> Plans { get; set; } = new();
        public string? CouponCode { get; set; }
        public bool? CouponValid { get; set; }          // null = no code typed
        public string? CouponMessage { get; set; }
    }
   

    public class PaymentListDto
    {
        public List<PaymentDto> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    }
}
