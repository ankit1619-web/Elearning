using System;
using System.Collections.Generic;
using System.Text;

namespace Manikalearning.Application.DTOs
{
    public class HomeDto
    {
        public string SiteTitle { get; set; } = string.Empty;

        public string? HeroTitle { get; set; }

        public string? HeroDescription { get; set; }

        public string? HeroImageUrl { get; set; }

        public string? HeroButtonText { get; set; }

        public string? HeroButtonUrl { get; set; }

        public List<HomeExamDto> PopularExams { get; set; } = new();

        public List<HomeCourseDto> FeaturedCourses { get; set; } = new();

        public List<HomePYQDto> PopularPYQs { get; set; } = new();

        public List<HomeTestDto> PopularTests { get; set; } = new();

        public int TotalStudents { get; set; }

        public int TotalQuestions { get; set; }

        public int TotalPYQs { get; set; }

        public int TotalTests { get; set; }

        public List<FeatureDto> Features { get; set; } = new();

        public List<TestimonialDto> Testimonials { get; set; } = new();

        public List<FAQDto> FAQs { get; set; } = new();
    }


    public class HomeExamDto
    {
        public Guid ExamId { get; set; }

        public string ExamName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public int TotalQuestions { get; set; }

        public int TotalPYQs { get; set; }

        public int TotalTests { get; set; }
    }


    public class HomeCourseDto
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


    public class HomePYQDto
    {
        public Guid QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public string? ExamName { get; set; }

        public string? SubjectName { get; set; }

        public int? Year { get; set; }
    }


    public class HomeTestDto
    {
        public Guid TestId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public string? ExamName { get; set; }

        public int TotalQuestions { get; set; }

        public int DurationMinutes { get; set; }

        public decimal TotalMarks { get; set; }
    }


    public class FeatureDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Icon { get; set; }

        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; }
    }


    public class TestimonialDto
    {
        public Guid TestimonialId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }

        public string? ExamName { get; set; }

        public string Review { get; set; } = string.Empty;

        public int Rating { get; set; }

        public int DisplayOrder { get; set; }
    }


    public class FAQDto
    {
        public Guid FAQId { get; set; }

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
