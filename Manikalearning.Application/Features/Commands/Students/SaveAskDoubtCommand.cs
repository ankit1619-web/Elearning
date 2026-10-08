using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{ 
    public record SaveAskDoubtCommand(
        Guid UserId,
        Guid? LectureId, 
        string QuestionText) : IRequest<ResponseDto<Guid>>;
     
    public class AskDoubtCommandValidator : AbstractValidator<SaveAskDoubtCommand>
    {
        public AskDoubtCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User identification is required.");

            RuleFor(x => x.QuestionText)
                .NotEmpty().WithMessage("Please enter your doubt or question.")
                .MinimumLength(10).WithMessage("Doubt must be at least 10 characters long.")
                .MaximumLength(1000).WithMessage("Doubt cannot exceed 1000 characters.");
        }
    }
}
