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
}
