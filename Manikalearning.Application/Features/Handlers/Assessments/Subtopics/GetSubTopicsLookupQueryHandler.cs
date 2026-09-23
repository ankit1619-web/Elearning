using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subtopics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Subtopics
{
    public class GetSubTopicsLookupQueryHandler : IRequestHandler<GetSubTopicsLookupQuery, ResponseDto<List<LookupDto>>>
    {
        private readonly IRepository<SubTopic> _subTopicRepo;

        public GetSubTopicsLookupQueryHandler(IRepository<SubTopic> subTopicRepo)
        {
            _subTopicRepo = subTopicRepo;
        }

        public async Task<ResponseDto<List<LookupDto>>> Handle(GetSubTopicsLookupQuery request, CancellationToken cancellationToken)
        {
            var subTopics = await _subTopicRepo.Entity()
                .Where(st => st.TopicId == request.TopicId && st.IsActive)
                .OrderBy(st => st.SubTopicName)
                .Select(st => new LookupDto
                {
                    Id = st.SubTopicId,
                    Name = st.SubTopicName
                })
                .ToListAsync(cancellationToken);

            return ResponseDto<List<LookupDto>>.Success(subTopics);
        }
    }
}
