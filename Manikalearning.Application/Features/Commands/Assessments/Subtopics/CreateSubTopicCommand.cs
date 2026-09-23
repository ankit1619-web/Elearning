using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Subtopics
{
    public class CreateSubTopicCommand : IRequest<ResponseDto<Guid>>
    {
        public string Name { get; set; } = string.Empty;
        public Guid TopicId { get; set; } 
        public bool IsActive { get; set; } = true;
    }

    public class CreateSubTopicCommandValidator : AbstractValidator<CreateSubTopicCommand>
    {
        public CreateSubTopicCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("SubTopic Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.TopicId)
                .NotEqual(Guid.Empty).WithMessage("Please select a valid Topic.");
        }
    }
}
