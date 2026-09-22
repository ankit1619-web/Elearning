namespace Manikalearning.Application.DTOs
{
    public class ExamDto
    {
        public Guid ExamId { get; set; }

        public string ExamName { get; set; } = string.Empty;

        public string? ExamCode { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; }
    }
    public class ExamListDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ShortDescription { get; set; }

        public string? Image { get; set; } 

        public int TotalQuestions { get; set; } 

        public int TotalPYQs { get; set; }

        public int TotalTests { get; set; }

        public int TotalStudents { get; set; }
    }
    public class ExamDetailsDto
    {
        public Guid ExamId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public string? CategoryName { get; set; }

        public int TotalQuestions { get; set; }

        public int TotalPYQs { get; set; }

        public int TotalTests { get; set; }

        public int TotalStudents { get; set; }

        public int Duration { get; set; }

        public List<ExamSubjectDto> Subjects { get; set; } = new();

        public List<ExamPYQDto> PYQs { get; set; } = new();

        public List<ExamTestDto> Tests { get; set; } = new();

        public List<ExamCourseDto> Courses { get; set; } = new();

        public List<ExamPatternDto> ExamPattern { get; set; } = new();

        public List<ExamFAQDto> FAQs { get; set; } = new();
    }


    public class ExamSubjectDto
    {
        public Guid SubjectId { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public int TotalQuestions { get; set; }

        public int TotalPYQs { get; set; }

        public int TotalTests { get; set; }
    }


    public class ExamPYQDto
    {
        public Guid PaperId { get; set; }

        public string PaperName { get; set; } = string.Empty;

        public Guid? SubjectId { get; set; }

        public string? SubjectName { get; set; }

        public int Year { get; set; }

        public string? Session { get; set; }

        public int TotalQuestions { get; set; }
        public Guid Id { get; set; } 
        public string Title { get; set; } = string.Empty;
         
    }


    public class ExamTestDto
    {
        public Guid TestId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public int TotalQuestions { get; set; }

        public int DurationMinutes { get; set; }

        public decimal TotalMarks { get; set; }

        public int TotalAttempts { get; set; }
    }

    public class ExamCourseDto
    {
        public Guid ExamId { get; set; }
        public Guid CourseId { get; set; }
        public decimal Price { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
        public string? ExamCode { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    } 


    public class ExamPatternDto
    {
        public Guid Id { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public int Questions { get; set; }

        public decimal Marks { get; set; }
    }


    public class ExamFAQDto
    {
        public Guid Id { get; set; }

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;
    }
}
