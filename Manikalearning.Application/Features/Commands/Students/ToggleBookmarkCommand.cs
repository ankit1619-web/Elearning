using FluentValidation;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public record ToggleBookmarkCommand(Guid UserId, Guid ItemId, string ItemType) : IRequest<bool>;
    public class ToggleBookmarkCommandValidator : AbstractValidator<ToggleBookmarkCommand>
    {
        public ToggleBookmarkCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ItemId).NotEmpty();
            RuleFor(x => x.ItemType).NotEmpty().Must(x => x == "Lecture" || x == "Question");
        }
    }
}
