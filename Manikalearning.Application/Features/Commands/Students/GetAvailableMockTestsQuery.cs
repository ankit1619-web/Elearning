using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public class MockTestDto
    {
        public Guid TestId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int? DurationMinutes { get; set; }
        public decimal TotalMarks { get; set; }
        public bool IsAttempted { get; set; }
        public int? LastScore { get; set; }
    }
    public class MockTestDetailDto
    {
        public Guid TestId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? DurationMinutes { get; set; }
        public decimal TotalMarks { get; set; }

        // Explicitly using Application.DTOs.QuestionDto
        public List<QuestionDto> Questions { get; set; } = new();
    }

    public class QuestionDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? QuestionTextHindi { get; set; }
        public int DisplayOrder { get; set; }
        public List<QuestionOptionDto> Options { get; set; } = new();
    }

    public class QuestionOptionDto
    {
        public Guid OptionId { get; set; }
        public string OptionLabel { get; set; } = string.Empty;
        public string OptionText { get; set; } = string.Empty;
        public string OptionTextHindi { get; set; } = string.Empty;
    }
    public record GetAvailableMockTestsQuery(Guid UserId) : IRequest<List<MockTestDto>>;
}
