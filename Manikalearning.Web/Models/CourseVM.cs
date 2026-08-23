using Manikalearning.Domain.Enums;

namespace Manikalearning.Web.Models
{
    public class CourseVM
    {
        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string? CourseCode { get; set; }

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }


        // Pricing

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }


        // Course Information

        public int TotalSections { get; set; }

        public int TotalLectures { get; set; }

        public int TotalHours { get; set; }

        public string? InstructorName { get; set; }

        public string? InstructorImage { get; set; }


        // Course Content

        public List<PublicCourseSectionVM> Sections { get; set; } = new();


        // Related Courses

        public List<CourseVM> RelatedCourses { get; set; } = new();


        // List

        public List<CourseVM> Items { get; set; } = new();

        public string? Search { get; set; }
    }


    public class PublicCourseSectionVM
    {
        public Guid CourseSectionId { get; set; }

        public string SectionName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int TotalLectures { get; set; }

        public int TotalMinutes { get; set; }

        public List<PublicLectureVM> Lectures { get; set; } = new();
    }


    public class PublicLectureVM
    {
        public Guid CourseLectureId { get; set; }

        public string LectureTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsFreePreview { get; set; }
    }
    public class PYQVM
    {
        // Filters

        public Guid? ExamId { get; set; }

        public Guid? PaperId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? TopicId { get; set; }

        public int? Year { get; set; }


        // Dropdowns

        public List<PublicDropdownVM> Exams { get; set; } = new();

        public List<PublicDropdownVM> Papers { get; set; } = new();

        public List<PublicDropdownVM> Subjects { get; set; } = new();

        public List<PublicDropdownVM> Topics { get; set; } = new();

        public List<int> Years { get; set; } = new();


        // Questions

        public List<PublicPYQQuestionVM> Questions { get; set; } = new();


        // Pagination

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }
    }
    public class PublicPYQQuestionVM
    {
        public Guid QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public string? ExamName { get; set; }

        public string? SubjectName { get; set; }

        public string? TopicName { get; set; }

        public int? Year { get; set; }

        public string? Session { get; set; }

        public DifficultyLevel? Difficulty { get; set; }

        public List<PublicOptionVM> Options { get; set; } = new();

        public bool HasExplanation { get; set; }
    }


    public class PublicOptionVM
    {
        public Guid OptionId { get; set; }

        public string OptionLabel { get; set; } = string.Empty;

        public string OptionText { get; set; } = string.Empty;
    }


    public class PublicDropdownVM
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
    public class PracticeVM
    {
        public Guid? ExamId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? TopicId { get; set; }

        public DifficultyLevel? Difficulty { get; set; }

        public int QuestionCount { get; set; } = 10;


        public List<PublicDropdownVM> Exams { get; set; } = new();

        public List<PublicDropdownVM> Subjects { get; set; } = new();

        public List<PublicDropdownVM> Topics { get; set; } = new();


        public bool IsGuest { get; set; }

        public int RemainingFreeQuestions { get; set; }
    }
}
