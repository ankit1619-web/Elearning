using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subtopics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Subtopics
{
    public class CreateSubTopicCommandHandler : IRequestHandler<CreateSubTopicCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<SubTopic> _subTopicRepo;
        private readonly IRepository<Topic> _topicRepo;
        private readonly ILogger<CreateSubTopicCommandHandler> _logger;

        public CreateSubTopicCommandHandler(
            IRepository<SubTopic> subTopicRepo,
            IRepository<Topic> topicRepo,
            ILogger<CreateSubTopicCommandHandler> logger)
        {
            _subTopicRepo = subTopicRepo;
            _topicRepo = topicRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<Guid>> Handle(CreateSubTopicCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var topicExists = await _topicRepo.Entity().FirstOrDefaultAsync(t => t.TopicId == request.TopicId);
                if (topicExists == null)
                {
                    _logger.LogWarning("Create SubTopic failed: TopicId {TopicId} not found.", request.TopicId);
                    return ResponseDto<Guid>.Failure("Selected Topic does not exist.");
                }

                var entity = new SubTopic
                {
                    SubTopicId = Guid.NewGuid(),
                    SubTopicName = request.Name.Trim(),
                    TopicId = request.TopicId,
                    IsActive = request.IsActive
                };

                await _subTopicRepo.AddAsync(entity);
                await _subTopicRepo.SaveChangesAsync();

                _logger.LogInformation("SubTopic {Name} created successfully with ID {Id}.", entity.SubTopicName, entity.SubTopicId);
                return ResponseDto<Guid>.Success(entity.SubTopicId, "SubTopic created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating SubTopic {Name}.", request.Name);
                return ResponseDto<Guid>.Failure("An error occurred while saving SubTopic.");
            }
        }
    }
}
