using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Passages
{
    public class GetPassagesLookupQuery : IRequest<ResponseDto<List<LookupDto>>>
    {
        public Guid? TopicId { get; set; }

        public GetPassagesLookupQuery(Guid? topicId = null)
        {
            TopicId = topicId;
        }
    }
}
