using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Topics
{
    public class CreateTopicCommandHandler : IRequestHandler<CreateTopicCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<Topic> _topicRepo;
        private readonly ILogger<CreateTopicCommandHandler> _logger;

        public CreateTopicCommandHandler(IRepository<Topic> topicRepo, ILogger<CreateTopicCommandHandler> logger)
        {
            _topicRepo = topicRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<Guid>> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var topic = new Topic
                {
                    TopicId = Guid.NewGuid(),
                    SectionId = request.SectionId,
                    TopicName = request.TopicName.Trim(),
                    Description = request.Description?.Trim() ?? "",
                    DisplayOrder = request.DisplayOrder,
                    IsActive = request.IsActive
                };

                await _topicRepo.AddAsync(topic);
                await _topicRepo.SaveChangesAsync();

                return ResponseDto<Guid>.Success(topic.TopicId, "Topic created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating topic: {TopicName}", request.TopicName);
                return ResponseDto<Guid>.Failure($"An error occurred while creating the topic: {ex.Message}");
            }
        }
    }
}
