namespace Manikalearning.Application.DTOs
{
   
    public class PlanDto
    {
        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; } 
        public decimal Discount { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int TotalCoursesMapped { get; set; }
        public decimal FinalPrice { get; set; }
        public bool CouponApplied => Discount > 0;
        public List<string> Courses { get; set; } = new();
    }
}
