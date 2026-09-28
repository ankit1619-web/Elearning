using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudentLectureProgress;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudentLectureProgress
{
    public class GetLectureProgressByIdQueryHandler : IRequestHandler<GetLectureProgressByIdQuery, ResponseDto<StudentLectureProgressDto>>
    {
        private readonly IRepository<Domain.Entities.StudentLectureProgress> _progressRepo;

        public GetLectureProgressByIdQueryHandler(IRepository<Domain.Entities.StudentLectureProgress> progressRepo)
        {
            _progressRepo = progressRepo;
        }

        public async Task<ResponseDto<StudentLectureProgressDto>> Handle(GetLectureProgressByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var p = await _progressRepo.Entity()
                    .Include(x => x.User)
                    .Include(x => x.Lecture) 
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ProgressId == request.ProgressId, cancellationToken);

                if (p == null)
                    return ResponseDto<StudentLectureProgressDto>.Failure("Progress record not found.");

                var dto = new StudentLectureProgressDto
                {
                    ProgressId = p.ProgressId,
                    UserId = p.UserId,
                    StudentName = p.User.FullName ?? "N/A",
                    StudentEmail = p.User.Email ?? "N/A",
                    LectureId = p.LectureId,
                    LectureTitle = p.Lecture?.LectureTitle ?? "N/A",
                    //CourseTitle = p.Lecture?.Course?.Title ?? "N/A",
                    WatchedSeconds = p.WatchedSeconds,
                    TotalSeconds = p.TotalSeconds,
                    IsCompleted = p.IsCompleted,
                    LastWatchedAt = p.LastWatchedAt
                };

                return ResponseDto<StudentLectureProgressDto>.Success(dto, "Details retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<StudentLectureProgressDto>.Failure($"Error loading progress details: {ex.Message}");
            }
        }
    }
}
