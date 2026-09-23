using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Topics
{
    public class DeleteTopicCommandHandler : IRequestHandler<DeleteTopicCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Topic> _topicRepo;
        private readonly ILogger<DeleteTopicCommandHandler> _logger;

        public DeleteTopicCommandHandler(IRepository<Topic> topicRepo, ILogger<DeleteTopicCommandHandler> logger)
        {
            _topicRepo = topicRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteTopicCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var topic = await _topicRepo.Entity().FirstOrDefaultAsync(x => x.TopicId == request.TopicId, cancellationToken);

                if (topic == null)
                {
                    return ResponseDto<bool>.Failure("Topic not found.");
                }

                 _topicRepo.Remove(topic);
                await _topicRepo.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Topic deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting topic ID: {TopicId}", request.TopicId);
                return ResponseDto<bool>.Failure($"An error occurred while deleting the topic: {ex.Message}");
            }
        }
    }
}
