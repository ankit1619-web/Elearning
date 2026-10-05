using FluentValidation;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public class TestResultDto
    {
        public Guid AttemptId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int WrongCount { get; set; }
    }
    public record SubmitTestAttemptCommand(
     Guid UserId,
     Guid TestId,
     Dictionary<Guid, string> SelectedAnswers,
     int TimeTakenSeconds) : IRequest<TestResultDto>;
    public class SubmitTestAttemptCommandValidator : AbstractValidator<SubmitTestAttemptCommand>
    {
        public SubmitTestAttemptCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.TestId).NotEmpty().WithMessage("Test ID is required.");
            RuleFor(x => x.SelectedAnswers).NotNull().WithMessage("Answers payload cannot be null.");
        }
    }
}
