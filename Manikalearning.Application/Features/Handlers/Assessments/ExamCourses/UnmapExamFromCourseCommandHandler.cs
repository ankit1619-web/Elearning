using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.ExamCourse;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.ExamCourses
{
    public class UnmapExamFromCourseCommandHandler : IRequestHandler<UnmapExamFromCourseCommand, ResponseDto<bool>>
    {
        private readonly IRepository<ExamCourse> _examCourseRepository;
        private readonly ILogger<UnmapExamFromCourseCommandHandler> _logger;

        public UnmapExamFromCourseCommandHandler(
            IRepository<ExamCourse> examCourseRepository,
            ILogger<UnmapExamFromCourseCommandHandler> logger)
        {
            _examCourseRepository = examCourseRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(UnmapExamFromCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var allMappings = await _examCourseRepository.GetAllAsync();
                var mapping = allMappings.FirstOrDefault(x => x.ExamId == request.ExamId && x.CourseId == request.CourseId);

                if (mapping == null)
                {
                    _logger.LogWarning("ExamCourse mapping not found for ExamId: {ExamId}, CourseId: {CourseId}", request.ExamId, request.CourseId);
                    return ResponseDto<bool>.Failure("Mapping not found.");
                }

                  _examCourseRepository.Remove(mapping);
                await _examCourseRepository.SaveChangesAsync();

                _logger.LogInformation("Unmapped ExamId: {ExamId} from CourseId: {CourseId} successfully.", request.ExamId, request.CourseId);

                return ResponseDto<bool>.Success(true, "Exam unmapped from course successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while unmapping ExamId: {ExamId} from CourseId: {CourseId}", request.ExamId, request.CourseId);
                return ResponseDto<bool>.Failure("An unexpected error occurred while unmapping the exam.", new List<string> { ex.Message });
            }
        }
    }
}
