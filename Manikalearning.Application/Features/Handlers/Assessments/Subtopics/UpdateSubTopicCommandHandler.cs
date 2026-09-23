using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subtopics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Subtopics
{
    public class UpdateSubTopicCommandHandler : IRequestHandler<UpdateSubTopicCommand, ResponseDto<bool>>
    {
        private readonly IRepository<SubTopic> _subTopicRepo;
        private readonly ILogger<UpdateSubTopicCommandHandler> _logger;

        public UpdateSubTopicCommandHandler(
            IRepository<SubTopic> subTopicRepo,
            ILogger<UpdateSubTopicCommandHandler> logger)
        {
            _subTopicRepo = subTopicRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateSubTopicCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _subTopicRepo.GetByIdAsync(request.Id);
                if (entity == null)
                {
                    _logger.LogWarning("Update failed: SubTopic with ID {Id} not found.", request.Id);
                    return ResponseDto<bool>.Failure("SubTopic not found.");
                }

                entity.SubTopicName = request.Name.Trim();
                entity.TopicId = request.TopicId;
                entity.IsActive = request.IsActive;

                  _subTopicRepo.Update(entity);
                await _subTopicRepo.SaveChangesAsync();

                _logger.LogInformation("SubTopic ID {Id} updated successfully.", request.Id);
                return ResponseDto<bool>.Success(true, "SubTopic updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating SubTopic ID {Id}.", request.Id);
                return ResponseDto<bool>.Failure("Failed to update SubTopic.");
            }
        }
    }
}
