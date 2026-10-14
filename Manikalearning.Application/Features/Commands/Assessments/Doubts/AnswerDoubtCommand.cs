using FluentValidation;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Doubts
{
    public record AnswerDoubtCommand(Guid StaffUserId, Guid DoubtId, string Text, bool Close) : IRequest<Guid>;

    public class AnswerDoubtCommandValidator : AbstractValidator<AnswerDoubtCommand>
    {
        public AnswerDoubtCommandValidator()
        {
            RuleFor(x => x.StaffUserId).NotEmpty();
            RuleFor(x => x.DoubtId).NotEmpty();
            RuleFor(x => x.Text).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Please write the answer.")
                .Must(t => t.Trim().Length <= 2000).WithMessage("The answer can be at most 2000 characters.");
        }
    }

}
