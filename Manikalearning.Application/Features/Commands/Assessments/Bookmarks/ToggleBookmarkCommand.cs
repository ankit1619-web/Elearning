using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Bookmarks
{
    public record ToggleBookmarkCommand(Guid UserId, Guid? LectureId , Guid? QuestionId) : IRequest<ResponseDto<bool>>;
    public class ToggleBookmarkCommandValidator : AbstractValidator<ToggleBookmarkCommand>
    {
        public ToggleBookmarkCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID missing hai.");

            RuleFor(x => x)
                .Must(x => x.LectureId.HasValue || x.QuestionId.HasValue)
                .WithMessage("At least LectureId ya QuestionId me se ek hona zaroori hai.");
        }
    }
}
