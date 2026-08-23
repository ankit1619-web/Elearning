using Manikalearning.Domain.Enums;

namespace Manikalearning.Application.DTOs
{
    public class TestDto
    {
        public Guid TestId { get; set; }

        public Guid? ExamId { get; set; }

        public Guid? PaperId { get; set; }

        public Guid? SubjectId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TestType TestType { get; set; }

        public int TotalQuestions { get; set; }

        public decimal TotalMarks { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsPublished { get; set; }

        public bool IsActive { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
