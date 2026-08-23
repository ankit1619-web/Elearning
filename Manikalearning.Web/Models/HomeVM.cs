using Manikalearning.Web.Areas.Student.ViewModels;

namespace Manikalearning.Web.Models
{
    public class HomeVM
    {
        public string SiteTitle { get; set; } = string.Empty;

        public string? HeroTitle { get; set; }

        public string? HeroDescription { get; set; }

        public string? HeroImageUrl { get; set; }

        public string? HeroButtonText { get; set; }

        public string? HeroButtonUrl { get; set; }


        // Exams
        public List<HomeExamVM> PopularExams { get; set; } = new();


        // Courses
        public List<HomeCourseVM> FeaturedCourses { get; set; } = new();


        // PYQ
        public List<HomePYQVM> PopularPYQs { get; set; } = new();


        // Tests
        public List<HomeTestVM> PopularTests { get; set; } = new();


        // Statistics
        public int TotalStudents { get; set; }

        public int TotalQuestions { get; set; }

        public int TotalPYQs { get; set; }

        public int TotalTests { get; set; }


        // Features
        public List<FeatureVM> Features { get; set; } = new();


        // Testimonials
        public List<TestimonialVM> Testimonials { get; set; } = new();


        // FAQs
        public List<FAQVM> FAQs { get; set; } = new();
    }
    public class HomeExamVM
    {
        public Guid ExamId { get; set; }

        public string ExamName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public int TotalQuestions { get; set; }

        public int TotalPYQs { get; set; }

        public int TotalTests { get; set; }
    }


    public class HomeCourseVM
    {
        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }

        public string? ShortDescription { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public int TotalLectures { get; set; }

        public int TotalHours { get; set; }
    }


    public class HomePYQVM
    {
        public Guid QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public string? ExamName { get; set; }

        public string? SubjectName { get; set; }

        public int? Year { get; set; }
    }


    public class HomeTestVM
    {
        public Guid TestId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public string? ExamName { get; set; }

        public int TotalQuestions { get; set; }

        public int DurationMinutes { get; set; }

        public decimal TotalMarks { get; set; }
    }
    public class FeatureVM
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Icon { get; set; }

        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; }
    }
    public class TestimonialVM
    {
        public Guid TestimonialId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }

        public string? ExamName { get; set; }

        public string Review { get; set; } = string.Empty;

        public int Rating { get; set; }

        public int DisplayOrder { get; set; }
    }
    public class FAQVM
    {
        public Guid FAQId { get; set; }

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
