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

        public GetLessonPlayerQueryHandler(IRepository<CourseLecture> lectureRepository)
        {
            _lectureRepository = lectureRepository;
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

            return new LessonPlayerViewModel
            {
                CourseId = request.CourseId,
                LessonId = lecture.LectureId,
                LessonTitle = lecture.LectureTitle,
                VideoUrl = lecture.VideoUrl,
                Notes = lecture.NotesUrl
            };
        }
    }
}
