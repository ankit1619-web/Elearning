using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Topics
{
    public record CreateTopicCommand(
     Guid SectionId,
     string TopicName,
     string? Description,
     int DisplayOrder,
     bool IsActive = true
 ) : IRequest<ResponseDto<Guid>>;

    public class CreateTopicCommandValidator : AbstractValidator<CreateTopicCommand>
    {
        public CreateTopicCommandValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section selection is required.")
                .NotEqual(Guid.Empty).WithMessage("Invalid Section selection.");

            RuleFor(x => x.TopicName)
                .NotEmpty().WithMessage("Topic Name is required.")
                .MaximumLength(150).WithMessage("Topic Name cannot exceed 150 characters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display Order must be 0 or greater.");
        }
    }
}
