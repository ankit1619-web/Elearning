using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Topics
{
    public class GetTopicByIdQueryHandler : IRequestHandler<GetTopicByIdQuery, ResponseDto<TopicDto>>
    {
        private readonly IRepository<Topic> _topicRepo;
        private readonly ILogger<GetTopicByIdQueryHandler> _logger;

        public GetTopicByIdQueryHandler(IRepository<Topic> topicRepo, ILogger<GetTopicByIdQueryHandler> logger)
        {
            _topicRepo = topicRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<TopicDto>> Handle(GetTopicByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var topic = await _topicRepo.Entity().Where(t => t.TopicId == request.TopicId)
                    .Select(t => new TopicDto
                    {
                        TopicId = t.TopicId,
                        SectionId = t.SectionId,
                        SectionName = t.Section != null ? t.Section.SectionName : string.Empty,
                        TopicName = t.TopicName,
                        Description = t.Description ?? "",
                        DisplayOrder = t.DisplayOrder,
                        IsActive = t.IsActive
                    })
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (topic == null)
                {
                    return ResponseDto<TopicDto>.Failure("Topic not found.");
                }

                return ResponseDto<TopicDto>.Success(topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching topic by ID: {TopicId}", request.TopicId);
                return ResponseDto<TopicDto>.Failure($"An error occurred while retrieving topic details: {ex.Message}");
            }
        }
    }
}
