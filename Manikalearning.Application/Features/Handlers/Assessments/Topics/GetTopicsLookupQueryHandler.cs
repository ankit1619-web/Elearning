using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Topics
{ 
    public class GetTopicsLookupQueryHandler : IRequestHandler<GetTopicsLookupQuery, ResponseDto<List<LookupDto>>>
    {
        private readonly IRepository<Topic> _topicRepo;

        public GetTopicsLookupQueryHandler(IRepository<Topic> topicRepo)
        {
            _topicRepo = topicRepo;
        }

        public async Task<ResponseDto<List<LookupDto>>> Handle(GetTopicsLookupQuery request, CancellationToken cancellationToken)
        {
            var query = _topicRepo.Entity()
                  .AsNoTracking()
                  .Where(t => t.IsActive);
            if (request.SectionId.HasValue && request.SectionId.Value != Guid.Empty)
            {
                query = query.Where(t => t.SectionId == request.SectionId.Value);
            }
            var topicsLookup = await query
               .OrderBy(t => t.TopicName)
               .Select(t => new LookupDto
               {
                   Id = t.TopicId,
                   Name = t.TopicName
               })
               .ToListAsync(cancellationToken); 
            return ResponseDto<List<LookupDto>>.Success(topicsLookup);
        }
    }
}
