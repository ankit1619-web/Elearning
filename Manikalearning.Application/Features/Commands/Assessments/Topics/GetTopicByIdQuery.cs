using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Topics
{
    public record GetTopicByIdQuery(Guid TopicId) : IRequest<ResponseDto<TopicDto>>;
}
