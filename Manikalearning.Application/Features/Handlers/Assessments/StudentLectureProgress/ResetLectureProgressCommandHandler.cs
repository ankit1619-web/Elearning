using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudentLectureProgress;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudentLectureProgress
{
    public class ResetLectureProgressCommandHandler : IRequestHandler<ResetLectureProgressCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Domain.Entities.StudentLectureProgress> _progressRepo;

        public ResetLectureProgressCommandHandler(IRepository<Domain.Entities.StudentLectureProgress> progressRepo)
        {
            _progressRepo = progressRepo;
        }

        public async Task<ResponseDto<bool>> Handle(ResetLectureProgressCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var p = await _progressRepo.Entity()
                    .FirstOrDefaultAsync(x => x.ProgressId == request.ProgressId, cancellationToken);

                if (p == null)
                    return ResponseDto<bool>.Failure("Progress record not found.");

                p.WatchedSeconds = 0;
                p.IsCompleted = false;
                p.LastWatchedAt = DateTime.UtcNow;

                 _progressRepo.Remove(p);
                await _progressRepo.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Student progress for this lecture has been reset.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to reset lecture progress: {ex.Message}");
            }
        }
    }
}
