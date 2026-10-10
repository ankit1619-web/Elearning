using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.CourseLecture;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.CourseLecture
{
    public class GetCourseLecturesBySectionIdQueryHandler : IRequestHandler<GetCourseLecturesBySectionIdQuery, ResponseDto<List<CourseLectureDto>>>
    {
        private readonly IRepository<Manikalearning.Domain.Entities.CourseLecture> _lectureRepository;
        private readonly ILogger<GetCourseLecturesBySectionIdQueryHandler> _logger;

        public GetCourseLecturesBySectionIdQueryHandler(
            IRepository<Manikalearning.Domain.Entities.CourseLecture> lectureRepository,
            ILogger<GetCourseLecturesBySectionIdQueryHandler> logger)
        {
            _lectureRepository = lectureRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<List<CourseLectureDto>>> Handle(GetCourseLecturesBySectionIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var lectures = await _lectureRepository.Entity().AsNoTracking() 
                    .Where(x => x.CourseSectionId == request.SectionId)
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new CourseLectureDto
                    {
                        LectureId = x.LectureId,
                        CourseSectionId = x.CourseSectionId,
                        LectureTitle = x.LectureTitle, 
                        NotesUrl = x.NotesUrl,
                        VideoDurationSeconds = x.VideoDurationSeconds,
                        DisplayOrder = x.DisplayOrder,
                        IsFreePreview = x.IsFreePreview,
                        IsPublished = x.IsPublished,
                        TopicId = x.TopicId,
                    })
                    .ToListAsync();

                return ResponseDto<List<CourseLectureDto>>.Success(lectures, "Lectures retrieved successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving lectures for section: {SectionId}", request.SectionId);
                return ResponseDto<List<CourseLectureDto>>.Failure("An unexpected error occurred while fetching lectures.", new List<string> { ex.Message });
            }
        }
    }
}
