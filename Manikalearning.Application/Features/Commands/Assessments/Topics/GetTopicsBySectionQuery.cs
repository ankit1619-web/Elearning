using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Topics
{
    public record GetTopicsBySectionQuery(Guid? SectionId = null, string SearchTerm = "") : IRequest<ResponseDto<List<TopicDto>>>; 
     
}
