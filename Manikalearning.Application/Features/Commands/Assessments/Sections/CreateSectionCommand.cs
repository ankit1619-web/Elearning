using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Sections
{
    public record CreateSectionCommand(
        Guid SubjectId,
        string SectionName,
        bool IsActive,
        int DisplayOrder
    ) : IRequest<ResponseDto<Guid>>; 
    public class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
    {
        public CreateSectionCommandValidator()
        {
            RuleFor(x => x.SubjectId)
                .NotEmpty().WithMessage("Subject selection is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Subject selection.");

            RuleFor(x => x.SectionName)
                .NotEmpty().WithMessage("Section Name is required.")
                .MaximumLength(150).WithMessage("Section Name cannot exceed 150 characters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display Order must be 0 or greater.");
        }
    }
}
