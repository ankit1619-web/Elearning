using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Sections
{
    public record UpdateSectionCommand(
         Guid SectionId,
         Guid SubjectId,
         string SectionName,
         bool IsActive,
         int DisplayOrder
     ) : IRequest<ResponseDto<bool>>;
    public class UpdateSectionCommandValidator : AbstractValidator<UpdateSectionCommand>
    {
        public UpdateSectionCommandValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Section ID.");

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
