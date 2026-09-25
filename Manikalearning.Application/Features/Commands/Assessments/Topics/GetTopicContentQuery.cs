using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Topics
{
    public record GetTopicContentQuery(Guid TopicId) : IRequest<TopicontentDto?>;
}
