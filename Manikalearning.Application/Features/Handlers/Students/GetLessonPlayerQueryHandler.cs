using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetLessonPlayerQueryHandler : IRequestHandler<GetLessonPlayerQuery, LessonPlayerViewModel>
    {
        private readonly IRepository<CourseLecture> _lectureRepository;
        private readonly IRepository<Bookmark> _bookmarkRepository;

        public GetLessonPlayerQueryHandler(IRepository<CourseLecture> lectureRepository, IRepository<Bookmark> bookmarkRepository)
        {
            _lectureRepository = lectureRepository;
            _bookmarkRepository = bookmarkRepository;
        }

        public async Task<LessonPlayerViewModel> Handle(GetLessonPlayerQuery request, CancellationToken cancellationToken)
        {
            var lecture = await _lectureRepository.Query()
                .AsNoTracking()
                .Include(l => l.CourseSection)
                .FirstOrDefaultAsync(l => l.LectureId == request.LessonId && l.CourseSection.CourseId == request.CourseId, cancellationToken);

            if (lecture == null)
            {
                throw new KeyNotFoundException($"Lecture ID {request.LessonId} not found in Course {request.CourseId}.");
            }
            var isBookmarked = await _bookmarkRepository.Query()
                .AsNoTracking()
                .AnyAsync(b => b.UserId == request.UserId && b.LectureId == request.LessonId, cancellationToken);
            return new LessonPlayerViewModel
            {
                CourseId = request.CourseId,
                IsFree = lecture.IsFreePreview,
                LessonId = lecture.LectureId,
                LessonTitle = lecture.LectureTitle,
                VideoUrl = lecture.VideoUrl,
                NotesUrl = lecture.NotesUrl,
                Notes = lecture.Notes,
                IsBookmarked = isBookmarked
            };
        }
    }
}
