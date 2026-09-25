using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Topics
{
    public class SaveTopicContentCommandHandler : IRequestHandler<SaveTopicContentCommand, ResponseDto<bool>>
    {
        private readonly IRepository<TopicContent> _topicContentRepo;
        private readonly ILogger<SaveTopicContentCommandHandler> _logger;

        public SaveTopicContentCommandHandler(
            IRepository<TopicContent> topicContentRepo,
            ILogger<SaveTopicContentCommandHandler> logger)
        {
            _topicContentRepo = topicContentRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(SaveTopicContentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingContent = await _topicContentRepo.Entity()
                .FirstOrDefaultAsync(x => x.TopicId == request.TopicId, cancellationToken);

                if (existingContent != null)
                {
                    existingContent.ContentText = request.Content;
                    _topicContentRepo.Update(existingContent);
                }
                else
                {
                    var newContent = new TopicContent
                    {
                        ContentId = Guid.NewGuid(),
                        TopicId = request.TopicId,
                        ContentText = request.Content
                    };

                    await _topicContentRepo.AddAsync(newContent);
                }

                await _topicContentRepo.SaveChangesAsync();
                return ResponseDto<bool>.Success(true, "Conent save successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating topic ID: {TopicId}", request.TopicId);
                return ResponseDto<bool>.Failure($"An error occurred while updating the topic: {ex.Message}");

            }
        }
    }
}
