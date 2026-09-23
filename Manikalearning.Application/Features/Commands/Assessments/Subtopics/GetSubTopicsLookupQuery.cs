using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Subtopics
{
    public class GetSubTopicsLookupQuery : IRequest<ResponseDto<List<LookupDto>>>
    {
        public Guid TopicId { get; set; }

        public GetSubTopicsLookupQuery(Guid topicId)
        {
            TopicId = topicId;
        }
    }
}
