using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Subtopics
{ 
    public record GetSubTopicsByTopicQuery(Guid? TopicId = null, string SearchTerm = "") : IRequest<ResponseDto<List<SubTopicDto>>>;
}
