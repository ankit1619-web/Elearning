namespace Manikalearning.Application.DTOs
{
    public  class StudentSubscriptionDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;

        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal PlanPrice { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled
        public int DaysRemaining { get; set; }

        public Guid? PaymentId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class StudentSubscriptionPagedResultDto
    {
        public List<StudentSubscriptionDto> Subscriptions { get; set; } = new();
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int ExpiredCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
