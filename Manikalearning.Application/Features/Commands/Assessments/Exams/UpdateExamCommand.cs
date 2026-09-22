using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Exams
{
    public record UpdateExamCommand(
         Guid ExamId,
         string ExamName,
         string? ExamCode,
         string? Description, 
         bool IsActive,
         int DisplayOrder
     ) : IRequest<ResponseDto<bool>>; 
    public class UpdateExamCommandValidator : AbstractValidator<UpdateExamCommand>
    {
        public UpdateExamCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .NotEmpty().WithMessage("Exam ID is required.");

            RuleFor(x => x.ExamName)
                .NotEmpty().WithMessage("Exam name is required.")
                .Length(2, 200).WithMessage("Exam name must be between 2 and 200 characters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be 0 or greater.");
        }
    }
}
