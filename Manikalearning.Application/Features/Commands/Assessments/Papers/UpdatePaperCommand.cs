using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Papers
{
    public record UpdatePaperCommand(
        Guid PaperId,
        Guid ExamId,
        string PaperName,
        string PaperCode,
        string? Description,
        int DisplayOrder,
        bool IsActive
    ) : IRequest<ResponseDto<bool>>;
    public class UpdatePaperCommandValidator : AbstractValidator<UpdatePaperCommand>
    {
        public UpdatePaperCommandValidator()
        {
            RuleFor(x => x.PaperId)
                .NotEmpty().WithMessage("Paper ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Paper ID.");

            RuleFor(x => x.ExamId)
                .NotEmpty().WithMessage("Exam selection is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Exam selection.");

            RuleFor(x => x.PaperName)
                .NotEmpty().WithMessage("Paper Name is required.")
                .MaximumLength(150).WithMessage("Paper Name cannot exceed 150 characters.");

            RuleFor(x => x.PaperCode)
                .NotEmpty().WithMessage("Paper Code is required.")
                .MaximumLength(50).WithMessage("Paper Code cannot exceed 50 characters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display Order must be 0 or greater.");
        }
    }
}
