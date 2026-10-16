using Manikalearning.Application.Features.Commands.Assessments.Tests;
using Manikalearning.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{ 
    public class TestViewModel
    {
        public Guid TestId { get; set; }

        [Required(ErrorMessage = "Test Name is required.")]
        [StringLength(200, ErrorMessage = "Test Name cannot exceed 200 characters.")]
        public string TestName { get; set; } = string.Empty;

        public Guid? ExamId { get; set; }
        public Guid? PaperId { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? TopicId { get; set; }

        [Required(ErrorMessage = "Test Type is required.")]
        public string  TestType { get; set; }

        [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes.")]
        public int? DurationMinutes { get; set; } = 60;

        [Range(0, 1000, ErrorMessage = "Total questions cannot be negative.")]
        public int TotalQuestions { get; set; }

        [Range(0, 5000, ErrorMessage = "Total marks cannot be negative.")]
        public decimal TotalMarks { get; set; }

        [Range(0, 100, ErrorMessage = "Negative marking must be between 0 and 100.")]
        public decimal NegativeMarking { get; set; } = 0.25m;

        public bool IsRandom { get; set; }
        public bool ShowResultImmediately { get; set; } = true;
        public bool IsPublished { get; set; }

        public List<SelectListItem> SubjectsList { get; set; } = new();
        public List<SelectListItem> SectionsList { get; set; } = new();
        public List<SelectListItem> TopicsList { get; set; } = new();

        public CreateTestCommand ToCreateCommand() => new()
        {
            TestName = TestName,
            ExamId = ExamId,
            PaperId = PaperId,
            SubjectId = SubjectId,
            SectionId = SectionId,
            TopicId = TopicId,
            TestType = TestType,
            DurationMinutes = DurationMinutes,
            TotalQuestions = TotalQuestions,
            TotalMarks = TotalMarks,
            NegativeMarking = NegativeMarking,
            IsRandom = IsRandom,
            ShowResultImmediately = ShowResultImmediately,
            IsPublished = IsPublished
        };

        public UpdateTestCommand ToUpdateCommand() => new()
        {
            TestId = TestId,
            TestName = TestName,
            ExamId = ExamId,
            PaperId = PaperId,
            SubjectId = SubjectId,
            SectionId = SectionId,
            TopicId = TopicId,
            TestType = TestType,
            DurationMinutes = DurationMinutes,
            TotalQuestions = TotalQuestions,
            TotalMarks = TotalMarks,
            NegativeMarking = NegativeMarking,
            IsRandom = IsRandom,
            ShowResultImmediately = ShowResultImmediately,
            IsPublished = IsPublished
        };
    }
}
