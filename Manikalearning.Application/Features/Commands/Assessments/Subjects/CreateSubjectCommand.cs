using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Subjects
{
    public record CreateSubjectCommand(
         Guid PaperId,
         string SubjectName,
         string? Description,
         int DisplayOrder,
         bool IsActive
     ) : IRequest<ResponseDto<Guid>>;
    public class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
    {
        public CreateSubjectCommandValidator()
        {
            RuleFor(x => x.PaperId)
                .NotEmpty().WithMessage("Paper selection is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Paper selection.");

            RuleFor(x => x.SubjectName)
                .NotEmpty().WithMessage("Subject Name is required.")
                .MaximumLength(150).WithMessage("Subject Name cannot exceed 150 characters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display Order must be 0 or greater.");
        }
    }
}
