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
        public List<string> Features { get; set; } = new();
    }
    public static class PlanFeatureCodes
    { 
        public const string MockTests = "MockTests";
        public const string Pyq = "Pyq";
        public const string Doubts = "Doubts";
        public const string Lectures = "Lectures";
        public const string Notes = "Notes";
        public const string BookMark = "BookMark";
        public const string Analytics = "Analytics";
         
        public static readonly string[] All = { MockTests, Pyq, Doubts, Lectures, Notes, BookMark, Analytics };

        public static string Label(string code) => code switch
        {
            MockTests => "Mock Tests",
            Pyq => "Previous Year Questions (PYQ)",
            Doubts => "Ask Doubts",
            Lectures => "Video Lectures",
            Notes => "Notes & PDFs",
            BookMark => "Bookmark Lessons & Questions",
            Analytics => "Performance Analytics",
            _ => code
        };

        public static string Description(string code) => code switch
        {
            MockTests => "Attempt full-length and topic-wise tests.",
            Pyq => "Practise real previous year questions.",
            Doubts => "Ask doubts and get answers from your teacher.",
            Lectures => "Watch recorded course lectures.",
            Notes => "Read and download lecture notes and PDFs.",
            Analytics => "See your score trend and topic-wise accuracy.",
            _ => ""
        };
    }
}
