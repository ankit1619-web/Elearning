using FluentValidation;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Doubts
{
    public record ReplyToDoubtCommand(Guid UserId, Guid DoubtId, string Text) : IRequest<Guid>;

    public class ReplyToDoubtCommandValidator : AbstractValidator<ReplyToDoubtCommand>
    {
        public ReplyToDoubtCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.DoubtId).NotEmpty();
            RuleFor(x => x.Text).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Please write your message.")
                .Must(t => t.Trim().Length <= 1000).WithMessage("Your message can be at most 1000 characters.");
        }
    }
}
