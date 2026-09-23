using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Subjects
{
    public record DeleteSubjectCommand(Guid SubjectId) : IRequest<ResponseDto<bool>>;
    public class DeleteSubjectCommandValidator : AbstractValidator<DeleteSubjectCommand>
    {
        public DeleteSubjectCommandValidator()
        {
            RuleFor(x => x.SubjectId)
                .NotEmpty().WithMessage("Subject ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Subject ID.");
        }
    }
}
