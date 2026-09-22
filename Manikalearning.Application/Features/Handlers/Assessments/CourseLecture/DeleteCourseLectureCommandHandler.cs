using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.CourseLecture;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.CourseLecture
{
    public class DeleteCourseLectureCommandHandler : IRequestHandler<DeleteCourseLectureCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Manikalearning.Domain.Entities.CourseLecture> _lectureRepository;
        private readonly ILogger<DeleteCourseLectureCommandHandler> _logger;

        public DeleteCourseLectureCommandHandler(
            IRepository<Manikalearning.Domain.Entities.CourseLecture> lectureRepository,
            ILogger<DeleteCourseLectureCommandHandler> logger)
        {
            _lectureRepository = lectureRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteCourseLectureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var lecture = await _lectureRepository.GetByIdAsync(request.Id);
                if (lecture == null)
                {
                    _logger.LogWarning("Course lecture not found for deletion: {LectureId}", request.Id);
                    return ResponseDto<bool>.Failure("Course lecture not found.");
                }

                  _lectureRepository.Remove(lecture);
                await _lectureRepository.SaveChangesAsync();

                _logger.LogInformation("Course lecture deleted successfully: {LectureId}", request.Id);

                return ResponseDto<bool>.Success(true, "Course lecture deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting course lecture: {LectureId}", request.Id);
                return ResponseDto<bool>.Failure("An unexpected error occurred while deleting the lecture.", new List<string> { ex.Message });
            }
        }
    }
}
