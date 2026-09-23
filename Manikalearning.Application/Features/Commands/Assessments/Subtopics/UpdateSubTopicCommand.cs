using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Subtopics
{
    public class UpdateSubTopicCommand : IRequest<ResponseDto<bool>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid TopicId { get; set; } 
        public bool IsActive { get; set; }
    }
    public class UpdateSubTopicCommandValidator : AbstractValidator<UpdateSubTopicCommand>
    {
        public UpdateSubTopicCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty).WithMessage("Please select a valid SubTopic.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("SubTopic Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.TopicId)
                .NotEqual(Guid.Empty).WithMessage("Please select a valid Topic.");
        }
    }
}
