using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record SaveLectureProgressCommand(Guid UserId, Guid LectureId, int PositionSeconds, int DurationSeconds,
                                             bool NotesRead, bool MarkComplete) : IRequest<LectureProgressResult>;

    public record LectureProgressResult(bool IsCompleted, int LastPositionSeconds);
    public class SaveLectureProgressCommandHandler : IRequestHandler<SaveLectureProgressCommand, LectureProgressResult>
    {
        private const double CompleteAt = 0.90;       
        private readonly IRepository<StudentLectureProgress> _progress;
        private readonly IRepository<CourseLecture> _lectures; 
        public SaveLectureProgressCommandHandler(IRepository<StudentLectureProgress> progress, IRepository<CourseLecture> lectures)
        { _progress = progress; _lectures = lectures; }

        public async Task<LectureProgressResult> Handle(SaveLectureProgressCommand r, CancellationToken ct)
        { 
            var duration = Math.Clamp(r.DurationSeconds, 0, 60 * 60 * 12);
            var position = Math.Clamp(r.PositionSeconds, 0, duration > 0 ? duration : 60 * 60 * 12);

            var hasVideo = await _lectures.Entity().AsNoTracking()
                .Where(l => l.LectureId == r.LectureId)
                .Select(l => l.VideoUrl != null && l.VideoUrl != "").FirstOrDefaultAsync(ct);

            var row = await _progress.Entity()
                .FirstOrDefaultAsync(p => p.UserId == r.UserId && p.LectureId == r.LectureId, ct);
            if (row == null)
            {
                row = new StudentLectureProgress { ProgressId = Guid.NewGuid(), UserId = r.UserId, LectureId = r.LectureId };
                await _progress.AddAsync(row);
            }

            row.WatchedSeconds = Math.Max(row.WatchedSeconds, position);    
            if (duration > 0) row.TotalSeconds = duration;
            row.LastWatchedAt = DateTime.UtcNow;

            var watchedEnough = row.TotalSeconds > 0 && row.WatchedSeconds >= row.TotalSeconds * CompleteAt;
            var readingOnlyDone = !hasVideo && r.NotesRead;  

            if (!row.IsCompleted && (r.MarkComplete || watchedEnough || readingOnlyDone))
                row.IsCompleted = true;

            await _progress.SaveChangesAsync();
            return new LectureProgressResult(row.IsCompleted, row.WatchedSeconds);
        }
    }
     
    public record GetLectureProgressQuery(Guid UserId, Guid LectureId) : IRequest<StudentLectureProgress?>;

    public class GetLectureProgressQueryHandler : IRequestHandler<GetLectureProgressQuery, StudentLectureProgress?>
    {
        private readonly IRepository<StudentLectureProgress> _progress;
        public GetLectureProgressQueryHandler(IRepository<StudentLectureProgress> p) { _progress = p; }

        public Task<StudentLectureProgress?> Handle(GetLectureProgressQuery r, CancellationToken ct) =>
            _progress.Entity().AsNoTracking().FirstOrDefaultAsync(p => p.UserId == r.UserId && p.LectureId == r.LectureId, ct);
    }
}
