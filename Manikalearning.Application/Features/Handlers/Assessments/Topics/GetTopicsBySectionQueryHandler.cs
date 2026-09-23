using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Topics
{
    public class GetTopicsBySectionQueryHandler : IRequestHandler<GetTopicsBySectionQuery, ResponseDto<List<TopicDto>>>
    {
        private readonly IRepository<Topic> _topicRepo;

        public GetTopicsBySectionQueryHandler(IRepository<Topic> topicRepo)
        {
            _topicRepo = topicRepo;
        }

        public async Task<ResponseDto<List<TopicDto>>> Handle(GetTopicsBySectionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _topicRepo.Entity()
                    .AsNoTracking().Include(t => t.Section)
                    .AsQueryable();
                if (request.SectionId.HasValue && request.SectionId.Value != Guid.Empty)
                {
                    query = query.Where(s => s.SectionId == request.SectionId.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    string search = request.SearchTerm.Trim().ToLower();
                    query = query.Where(s => s.TopicName.ToLower().Contains(search) ||
                                             (s.Section != null && s.Section.SectionName.ToLower().Contains(search)));
                }
                var topics = await query
                    .OrderBy(s => s.DisplayOrder)
                   .Select(t => new TopicDto
                   {
                       TopicId = t.TopicId,
                       SectionId = t.SectionId,
                       SectionName = t.Section.SectionName,
                       TopicName = t.TopicName,
                       Description = t.Description ?? "",
                       DisplayOrder = t.DisplayOrder,
                       IsActive = t.IsActive
                   }).ToListAsync(); 

                return ResponseDto<List<TopicDto>>.Success(topics);
            }
            catch (Exception ex)
            { 
                return ResponseDto<List<TopicDto>>.Failure($"An error occurred while retrieving topics: {ex.Message}");
            }
        }
    }
}
