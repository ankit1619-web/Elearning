using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Exams
{
    public class CreateExamCommand : IRequest<ResponseDto<Guid>>
    {
        public string ExamName { get; set; } = string.Empty;
        public string? ExamCode { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateExamCommandValidator : AbstractValidator<CreateExamCommand>
    {
        public CreateExamCommandValidator()
        {
            RuleFor(x => x.ExamName)
                .NotEmpty().WithMessage("Exam name is required.")
                .Length(2, 200).WithMessage("Exam name must be between 2 and 200 characters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be 0 or greater.");
        }
    }
}
