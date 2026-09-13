using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Web
{
    public class GetLecturePartialQueryHandler : IRequestHandler<GetLecturePartialQuery, CourseLectureDto>
    {

        private readonly IRepository<CourseLecture> _courseLectureRepository;

        public GetLecturePartialQueryHandler(IRepository<CourseLecture> courseLectureRepository)
        {
            _courseLectureRepository = courseLectureRepository;
        }

        public async Task<CourseLectureDto> Handle(GetLecturePartialQuery request, CancellationToken cancellationToken)
        { 
            var lecture = await _courseLectureRepository.Entity()
                .Where(l => l.LectureId == request.LectureId)
                .Select(l => new CourseLectureDto
                {
                    LectureId = l.LectureId,
                    CourseSectionId = l.CourseSectionId,
                    LectureTitle = l.LectureTitle,
                    Description = l.Description,
                    VideoUrl = l.VideoUrl,
                    VideoDurationSeconds = l.VideoDurationSeconds,
                    NotesUrl = l.NotesUrl,
                    IsFreePreview = l.IsFreePreview,
                    DisplayOrder = l.DisplayOrder,
                    IsPublished = l.IsPublished
                     
                })
                .FirstOrDefaultAsync(cancellationToken);

            return lecture;
        }
    }
}
