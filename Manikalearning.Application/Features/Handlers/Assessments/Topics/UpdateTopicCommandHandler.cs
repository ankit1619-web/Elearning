using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Topics
{
    public class UpdateTopicCommandHandler : IRequestHandler<UpdateTopicCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Topic> _topicRepo;
        private readonly ILogger<UpdateTopicCommandHandler> _logger;

        public UpdateTopicCommandHandler(IRepository<Topic> topicRepo, ILogger<UpdateTopicCommandHandler> logger)
        {
            _topicRepo = topicRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateTopicCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var topic = await _topicRepo.Entity().FirstOrDefaultAsync(x=>x.TopicId == request.TopicId, cancellationToken);

                if (topic == null)
                {
                    return ResponseDto<bool>.Failure("Topic not found.");
                }

                topic.SectionId = request.SectionId;
                topic.TopicName = request.TopicName.Trim();
                topic.Description = request.Description?.Trim() ?? "";
                topic.DisplayOrder = request.DisplayOrder;
                topic.IsActive = request.IsActive;

                  _topicRepo.Update(topic);
                await _topicRepo.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Topic updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating topic ID: {TopicId}", request.TopicId);
                return ResponseDto<bool>.Failure($"An error occurred while updating the topic: {ex.Message}");
            }
        }
    }
}
