using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Topics
{
    public record DeleteTopicCommand(Guid TopicId) : IRequest<ResponseDto<bool>>;

    public class DeleteTopicCommandValidator : AbstractValidator<DeleteTopicCommand>
    {
        public DeleteTopicCommandValidator()
        {
            RuleFor(x => x.TopicId)
                .NotEmpty().WithMessage("Topic ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Topic ID.");
        }
    }
}
