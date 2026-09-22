using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.CourseLecture;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.CourseLecture
{
    public class GetCourseLectureByIdQueryHandler : IRequestHandler<GetCourseLectureByIdQuery, ResponseDto<CourseLectureDto>>
    {
        private readonly IRepository<Manikalearning.Domain.Entities.CourseLecture> _lectureRepository;
        private readonly ILogger<GetCourseLectureByIdQueryHandler> _logger;

        public GetCourseLectureByIdQueryHandler(
            IRepository<Manikalearning.Domain.Entities.CourseLecture> lectureRepository,
            ILogger<GetCourseLectureByIdQueryHandler> logger)
        {
            _lectureRepository = lectureRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<CourseLectureDto>> Handle(GetCourseLectureByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var x = await _lectureRepository.GetByIdAsync(request.Id);
                if (x == null)
                {
                    _logger.LogWarning("Course lecture not found: {LectureId}", request.Id);
                    return ResponseDto<CourseLectureDto>.Failure("Course lecture not found.");
                }

                var dto = new CourseLectureDto
                {
                    LectureId = x.LectureId,
                    CourseSectionId = x.CourseSectionId,
                    LectureTitle = x.LectureTitle,
                    VideoUrl = x.VideoUrl,
                    NotesUrl = x.NotesUrl,
                    VideoDurationSeconds = x.VideoDurationSeconds,
                    DisplayOrder = x.DisplayOrder,
                    IsFreePreview = x.IsFreePreview,
                    IsPublished = x.IsPublished
                };

                return ResponseDto<CourseLectureDto>.Success(dto, "Lecture retrieved successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving course lecture: {LectureId}", request.Id);
                return ResponseDto<CourseLectureDto>.Failure("An unexpected error occurred while fetching the lecture.", new List<string> { ex.Message });
            }
        }
    }
}
