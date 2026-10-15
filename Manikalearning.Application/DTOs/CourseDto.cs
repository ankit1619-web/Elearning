namespace Manikalearning.Application.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
         
        public string CourseName { get; set; } = string.Empty;

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }

        public string? ExamName { get; set; }

        public string? CategoryName { get; set; }
        public bool Ispublished { get; set; }

        public decimal Price { get; set; } 
        public int TotalLectures { get; set; } 

        public int TotalStudents { get; set; }

        public int TotalQuestions { get; set; }
        public List<CourseSectionDto> Sections { get; set; } = new();
        public List<CourseLectureDto> Lectures { get; set; } = new();
    }
    public static class PlanFeatureCodes
    {
        public const string Pyq = "Pyq";                  
        public const string MockTests = "MockTests";
        public const string Doubts = "Doubts";      

        public static string Label(string code) => code switch
        {
            Pyq => "Save PYQ results and Mistake Book",
            MockTests => "Mock tests",
            Doubts => "Doubt forum",
            _ => code
        };
    }

    public class CourseAccessDto
    {
        public bool HasAccess { get; set; }
        public string Reason { get; set; } = "";     // Free | Subscription | Enrolled | NoSubscription | NotFound
        public Guid? CourseId { get; set; }
    }
}
