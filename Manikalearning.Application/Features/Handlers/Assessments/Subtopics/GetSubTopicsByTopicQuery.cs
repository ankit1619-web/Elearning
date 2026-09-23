using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subtopics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Subtopics
{
    public class GetSubTopicsByTopicQueryHandler : IRequestHandler<GetSubTopicsByTopicQuery, ResponseDto<List<SubTopicDto>>>
    {
        private readonly IRepository<SubTopic> _subTopicRepo;
        private readonly ILogger<GetSubTopicsByTopicQueryHandler> _logger;

        public GetSubTopicsByTopicQueryHandler(
            IRepository<SubTopic> subTopicRepo,
            ILogger<GetSubTopicsByTopicQueryHandler> logger)
        {
            _subTopicRepo = subTopicRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<List<SubTopicDto>>> Handle(GetSubTopicsByTopicQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _subTopicRepo.Entity()
                    .Include(st => st.Topic) 
                    .AsNoTracking();

                if (request.TopicId.HasValue && request.TopicId.Value != Guid.Empty)
                {
                    query = query.Where(st => st.TopicId == request.TopicId.Value);
                }
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    string search = request.SearchTerm.Trim().ToLower();
                    query = query.Where(s => s.SubTopicName.ToLower().Contains(search) ||
                                             (s.Topic != null && s.Topic.TopicName.ToLower().Contains(search)));
                }

                var list = await query
                    .OrderBy(st => st.Topic.TopicName)
                    .Select(st => new SubTopicDto
                    {
                        SubTopicId = st.SubTopicId,
                        SubTopicName = st.SubTopicName,
                        TopicId = st.TopicId,
                        TopicName = st.Topic != null ? st.Topic.TopicName : "", 
                        IsActive = st.IsActive
                    })
                    .ToListAsync(cancellationToken);

                return ResponseDto<List<SubTopicDto>>.Success(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching SubTopics list for TopicId: {TopicId}", request.TopicId);
                return ResponseDto<List<SubTopicDto>>.Failure("Failed to retrieve SubTopics.");
            }
        }
    }
}
