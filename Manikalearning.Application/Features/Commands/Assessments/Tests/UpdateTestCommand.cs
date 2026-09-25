using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Tests
{
    public class UpdateTestCommand : IRequest<ResponseDto<bool>>
    {
        public Guid TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public Guid? ExamId { get; set; }
        public Guid? PaperId { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? TopicId { get; set; }
        public string TestType { get; set; } = "MockTest";
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal NegativeMarking { get; set; }
        public bool IsRandom { get; set; }
        public bool ShowResultImmediately { get; set; }
        public bool IsPublished { get; set; }
    }

    // 2. Validator
    public class UpdateTestCommandValidator : AbstractValidator<UpdateTestCommand>
    {
        public UpdateTestCommandValidator()
        {
            RuleFor(x => x.TestId)
                .NotEmpty().WithMessage("Test ID is required.");

            RuleFor(x => x.TestName)
                .NotEmpty().WithMessage("Test name is required.")
                .MaximumLength(200).WithMessage("Test name must not exceed 200 characters.");

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.");

            RuleFor(x => x.NegativeMarking)
                .GreaterThanOrEqualTo(0).WithMessage("Negative marking cannot be negative.");
        }
    }
}
