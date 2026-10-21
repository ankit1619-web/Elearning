using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Web
{
    public class GetLecturePartialQueryHandler : IRequestHandler<GetLecturePartialQuery, LessonPlayerViewModel>
    {

        private readonly IRepository<CourseLecture> _lectureRepository;

        public GetLecturePartialQueryHandler(IRepository<CourseLecture> courseLectureRepository)
        {
            _lectureRepository = courseLectureRepository;
        }

        public async Task<LessonPlayerViewModel> Handle(GetLecturePartialQuery request, CancellationToken cancellationToken)
        {
            var lecture = await _lectureRepository.Query()
                .AsNoTracking()
                .Include(l => l.CourseSection)
                .FirstOrDefaultAsync(l => l.LectureId == request.LectureId && l.CourseSection.CourseId == request.CourseId, cancellationToken);

            if (lecture == null)
            {
                throw new KeyNotFoundException($"Lecture ID {request.LectureId} not found in Course {request.CourseId}.");
            }
            return new LessonPlayerViewModel
            {
                CourseId = request.CourseId,
                IsFree = lecture.IsFreePreview,
                LessonId = lecture.LectureId,
                LessonTitle = lecture.LectureTitle,
                VideoUrl = lecture.VideoUrl,
                NotesUrl = lecture.NotesUrl,
                Notes = lecture.Notes
            }; 
        }
    }
}
