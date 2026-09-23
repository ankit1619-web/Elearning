using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subtopics;
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

namespace Manikalearning.Application.Features.Handlers.Assessments.Subtopics
{
    public class GetSubTopicByIdQueryHandler : IRequestHandler<GetSubTopicByIdQuery, ResponseDto<SubTopicDto>>
    {
        private readonly IRepository<SubTopic> _subTopicRepo;
        private readonly ILogger<GetSubTopicByIdQueryHandler> _logger;

        public GetSubTopicByIdQueryHandler(
            IRepository<SubTopic> subTopicRepo,
            ILogger<GetSubTopicByIdQueryHandler> logger)
        {
            _subTopicRepo = subTopicRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<SubTopicDto>> Handle(GetSubTopicByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var subTopic = await _subTopicRepo.Entity()
                    .Include(st => st.Topic) 
                    .AsNoTracking()
                    .FirstOrDefaultAsync(st => st.SubTopicId == request.Id, cancellationToken);

                if (subTopic == null)
                {
                    _logger.LogWarning("SubTopic with ID {Id} was not found.", request.Id);
                    return ResponseDto<SubTopicDto>.Failure("SubTopic not found.");
                }

                var dto = new SubTopicDto
                {
                    SubTopicId = subTopic.SubTopicId,
                    SubTopicName = subTopic.SubTopicName,
                    TopicId = subTopic.TopicId,
                    TopicName = subTopic.Topic != null ? subTopic.Topic.TopicName : string.Empty,
                    IsActive = subTopic.IsActive 
                };

                return ResponseDto< SubTopicDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching SubTopic with ID {Id}.", request.Id);
                return ResponseDto<SubTopicDto>.Failure("Failed to retrieve SubTopic details.");
            }
        }
    }
}
