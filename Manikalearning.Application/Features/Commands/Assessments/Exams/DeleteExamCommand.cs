using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Exams
{
    public record DeleteExamCommand(Guid ExamId) : IRequest<ResponseDto<bool>>;
     
    public class DeleteExamCommandValidator : AbstractValidator<DeleteExamCommand>
    {
        public DeleteExamCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .NotEmpty().WithMessage("Exam ID is required.");
        }
    }
}
