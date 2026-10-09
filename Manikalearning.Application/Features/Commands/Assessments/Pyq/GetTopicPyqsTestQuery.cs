using FluentValidation;
using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Pyq
{
    public record GetTopicPyqsTestQuery(Guid LectureId, Guid? UserId) : IRequest<ResponseDto<TopicPyqListDto>>;
    public class GetTopicPyqsTestQueryValidator : AbstractValidator<GetTopicPyqsTestQuery>
    {
        public GetTopicPyqsTestQueryValidator()
        {
            RuleFor(x => x.LectureId)
                .NotEmpty().WithMessage("Invalid Lecture ID.");
        }
    }
}
