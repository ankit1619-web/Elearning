using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.CourseLecture;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities; 
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.CourseLecture
{
    public class CreateCourseLectureCommandHandler : IRequestHandler<CreateCourseLectureCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<Manikalearning.Domain.Entities.CourseLecture> _lectureRepository;
        private readonly ILogger<CreateCourseLectureCommandHandler> _logger;

        public CreateCourseLectureCommandHandler(
            IRepository<Manikalearning.Domain.Entities.CourseLecture> lectureRepository,
            ILogger<CreateCourseLectureCommandHandler> logger)
        {
            _lectureRepository = lectureRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<Guid>> Handle(CreateCourseLectureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var lecture = new Manikalearning.Domain.Entities.CourseLecture
                {
                    LectureId = Guid.NewGuid(),
                    CourseSectionId = request.CourseSectionId,
                    LectureTitle = request.Title,
                    VideoUrl = request.VideoUrl ?? "",
                    NotesUrl = request.PdfUrl ?? "",
                    VideoDurationSeconds = request.DurationInSeconds,
                    DisplayOrder = request.DisplayOrder,
                    IsFreePreview = request.IsPreview,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _lectureRepository.AddAsync(lecture);
                await _lectureRepository.SaveChangesAsync();

                _logger.LogInformation("Course lecture created successfully: {LectureId}", lecture.LectureId);

                return ResponseDto<Guid>.Success(lecture.LectureId, "Course lecture created successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating course lecture: {Title}", request.Title);
                return ResponseDto<Guid>.Failure("An unexpected error occurred while creating the lecture.", new List<string> { ex.Message });
            }
        }
    }
}
