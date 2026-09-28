using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudentLectureProgress;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudentLectureProgress
{
    public class GetStudentLectureProgressQueryHandler : IRequestHandler<GetStudentLectureProgressQuery, ResponseDto<ProgressPagedResultDto>>
    {
        private readonly IRepository<Domain.Entities.StudentLectureProgress> _progressRepo;

        public GetStudentLectureProgressQueryHandler(IRepository<Domain.Entities.StudentLectureProgress> progressRepo)
        {
            _progressRepo = progressRepo;
        }

        public async Task<ResponseDto<ProgressPagedResultDto>> Handle(GetStudentLectureProgressQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _progressRepo.Entity()
                    .Include(p => p.User)
                    .Include(p => p.Lecture)
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var term = request.Search.Trim().ToLower();
                    query = query.Where(p =>
                        p.User.FullName.ToLower().Contains(term) ||
                        p.User.Email!.ToLower().Contains(term) ||
                        p.Lecture.LectureTitle.ToLower().Contains(term) );
                }

                if (request.CourseId.HasValue && request.CourseId != Guid.Empty)
                {
                    //query = query.Where(p => p.Lecture.CourseId == request.CourseId);
                }

                if (request.IsCompleted.HasValue)
                {
                    query = query.Where(p => p.IsCompleted == request.IsCompleted.Value);
                }

                var totalCount = await query.CountAsync(cancellationToken);
                var completedCount = await _progressRepo.Entity().CountAsync(p => p.IsCompleted, cancellationToken);
                var inProgressCount = await _progressRepo.Entity().CountAsync(p => !p.IsCompleted, cancellationToken);

                var items = await query
                    .OrderByDescending(p => p.LastWatchedAt)
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(p => new StudentLectureProgressDto
                    {
                        ProgressId = p.ProgressId,
                        UserId = p.UserId,
                        StudentName = p.User.FullName ?? "N/A",
                        StudentEmail = p.User.Email ?? "N/A",
                        LectureId = p.LectureId,
                        LectureTitle = p.Lecture.LectureTitle ?? "N/A",
                        //CourseTitle = p.Lecture.Course != null ? p.Lecture.Course.Title : "N/A",
                        WatchedSeconds = p.WatchedSeconds,
                        TotalSeconds = p.TotalSeconds,
                        IsCompleted = p.IsCompleted,
                        LastWatchedAt = p.LastWatchedAt
                    }).ToListAsync(cancellationToken);

                var avgRate = items.Any() ? items.Average(x => x.CompletionPercentage) : 0m;

                var result = new ProgressPagedResultDto
                {
                    ProgressRecords = items,
                    TotalCount = totalCount,
                    CompletedCount = completedCount,
                    InProgressCount = inProgressCount,
                    AverageCompletionRate = Math.Round(avgRate, 2),
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize
                };

                return ResponseDto<ProgressPagedResultDto>.Success(result, "Progress records loaded successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<ProgressPagedResultDto>.Failure($"Failed to fetch lecture progress: {ex.Message}");
            }
        }
    }
}
