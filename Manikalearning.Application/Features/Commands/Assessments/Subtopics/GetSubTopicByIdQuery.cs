using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Subtopics
{
    public record GetSubTopicByIdQuery(Guid Id) : IRequest<ResponseDto<SubTopicDto>>;
}
