namespace Manikalearning.Web.Models
{
    public class ExamViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ShortDescription { get; set; }

        public string? Image { get; set; } 
        public string? CategorySlug { get; set; } 

        public int TotalQuestions { get; set; }
          
        public int TotalPYQs { get; set; }

        public int TotalTests { get; set; }

        public int TotalStudents { get; set; }
    }
    public class ExamVM
    {
        public Guid ExamId { get; set; }

        public string ExamName { get; set; } = string.Empty;

        public string? ExamCode { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; } 

        public int TotalQuestions { get; set; }

        public int TotalPYQs { get; set; }

        public int TotalTests { get; set; }

        public int TotalSubjects { get; set; }
         
        public List<PublicSubjectVM> Subjects { get; set; } = new();
          
        public List<PublicTestVM> Tests { get; set; } = new();
         
        public List<PublicCourseVM> Courses { get; set; } = new();
          
        public List<ExamVM> RelatedExams { get; set; } = new();
          
        public List<ExamVM> Items { get; set; } = new();

        public string? Search { get; set; }
    }
    public class PublicSubjectVM
    {
        public Guid SubjectId { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public int TotalQuestions { get; set; }

        public int TotalPYQs { get; set; }
    }


    public class PublicTestVM
    {
        public Guid TestId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public string? TestType { get; set; }

        public int TotalQuestions { get; set; }

        public int DurationMinutes { get; set; }

        public decimal TotalMarks { get; set; }
    }


    public class PublicCourseVM
    {
        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }
    }
}
