using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Topics
{
    public class GetTopicContentQueryHandler : IRequestHandler<GetTopicContentQuery, TopicontentDto?>
    {
        private readonly IRepository<TopicContent> _topicContentRepo;
        private readonly IRepository<Topic> _topicRepo;
        private readonly ILogger<GetTopicContentQueryHandler> _logger;

        public GetTopicContentQueryHandler(
            IRepository<TopicContent> topicContentRepo,
            IRepository<Topic> topicRepo,
            ILogger<GetTopicContentQueryHandler> logger)
        {
            _topicContentRepo = topicContentRepo;
            _topicRepo = topicRepo;
            _logger = logger;
        }

        public async Task<TopicontentDto?> Handle(GetTopicContentQuery request, CancellationToken cancellationToken)
        { 
            var topicContent = await _topicContentRepo.Entity()
                .Include(x => x.Topic)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TopicId == request.TopicId, cancellationToken);
             
            if (topicContent != null)
            {
                return new TopicontentDto
                {
                    Id = topicContent.ContentId,
                    TopicId = topicContent.TopicId,
                    TopicName = topicContent.Topic?.TopicName ?? string.Empty,
                    ContentEn = topicContent.ContentText ?? string.Empty 
                };
            }
             
            var topic = await _topicRepo.Entity()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TopicId == request.TopicId, cancellationToken);

            if (topic == null) return null;  

            return new TopicontentDto
            {
                TopicId = topic.TopicId,
                TopicName = topic.TopicName,
                ContentEn = string.Empty 
            };
        }
    }
}
