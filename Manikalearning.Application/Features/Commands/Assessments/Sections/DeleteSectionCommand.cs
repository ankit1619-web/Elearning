using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Sections
{
    public record DeleteSectionCommand(Guid SectionId) : IRequest<ResponseDto<bool>>;
    public class DeleteSectionCommandValidator : AbstractValidator<DeleteSectionCommand>
    {
        public DeleteSectionCommandValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Section ID.");
        }
    }
}
