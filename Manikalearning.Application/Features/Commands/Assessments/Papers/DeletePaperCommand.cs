using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Papers
{
    public record DeletePaperCommand(Guid PaperId) : IRequest<ResponseDto<bool>>;
    public class DeletePaperCommandValidator : AbstractValidator<DeletePaperCommand>
    {
        public DeletePaperCommandValidator()
        {
            RuleFor(x => x.PaperId)
                .NotEmpty().WithMessage("Paper ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Paper ID.");
        }
    }
}
