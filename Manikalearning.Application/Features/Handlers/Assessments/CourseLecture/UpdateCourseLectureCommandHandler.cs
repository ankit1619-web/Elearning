using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.CourseLecture;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.CourseLecture
{
    public class UpdateCourseLectureCommandHandler : IRequestHandler<UpdateCourseLectureCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Manikalearning.Domain.Entities.CourseLecture> _lectureRepository;
        private readonly ILogger<UpdateCourseLectureCommandHandler> _logger;

        public UpdateCourseLectureCommandHandler(
            IRepository<Manikalearning.Domain.Entities.CourseLecture> lectureRepository,
            ILogger<UpdateCourseLectureCommandHandler> logger)
        {
            _lectureRepository = lectureRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateCourseLectureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var lecture = await _lectureRepository.GetByIdAsync(request.LectureId);
                if (lecture == null)
                {
                    _logger.LogWarning("Course lecture not found for update: {LectureId}", request.LectureId);
                    return ResponseDto<bool>.Failure("Course lecture not found.");
                }

                lecture.CourseSectionId = request.CourseSectionId;
                lecture.LectureTitle = request.Title;
                lecture.VideoUrl = request.VideoUrl ?? "";
                lecture.NotesUrl = request.PdfUrl ?? "";
                lecture.Notes = request.Notes ?? "";
                lecture.VideoDurationSeconds = request.DurationInSeconds;
                lecture.DisplayOrder = request.DisplayOrder;
                lecture.IsFreePreview = request.IsPreview;
                lecture.IsPublished = request.IsActive;
                lecture.TopicId = request.TopicId;

                _lectureRepository.Update(lecture);
                await _lectureRepository.SaveChangesAsync();

                _logger.LogInformation("Course lecture updated successfully: {LectureId}", lecture.LectureId);

                return ResponseDto<bool>.Success(true, "Course lecture updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating course lecture: {LectureId}", request.LectureId);
                return ResponseDto<bool>.Failure("An unexpected error occurred while updating the lecture.", new List<string> { ex.Message });
            }
        }
    }
}
