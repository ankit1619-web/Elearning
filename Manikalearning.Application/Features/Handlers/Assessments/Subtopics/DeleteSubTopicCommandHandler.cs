using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subtopics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Subtopics
{
    public class DeleteSubTopicCommandHandler : IRequestHandler<DeleteSubTopicCommand, ResponseDto<bool>>
    {
        private readonly IRepository<SubTopic> _subTopicRepo;
        private readonly ILogger<DeleteSubTopicCommandHandler> _logger;

        public DeleteSubTopicCommandHandler(
            IRepository<SubTopic> subTopicRepo,
            ILogger<DeleteSubTopicCommandHandler> logger)
        {
            _subTopicRepo = subTopicRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteSubTopicCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _subTopicRepo.GetByIdAsync(request.Id);
                if (entity == null)
                {
                    _logger.LogWarning("Delete failed: SubTopic ID {Id} not found.", request.Id);
                    return ResponseDto<bool>.Failure("SubTopic not found.");
                }

                  _subTopicRepo.Remove(entity);
                await _subTopicRepo.SaveChangesAsync();

                _logger.LogInformation("SubTopic ID {Id} deleted successfully.", request.Id);
                return ResponseDto<bool>.Success(true, "SubTopic deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting SubTopic ID {Id}.", request.Id);
                return ResponseDto<bool>.Failure("Unable to delete SubTopic.");
            }
        }
    }
}
