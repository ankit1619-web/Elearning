using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.ExamCourse;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.ExamCourses
{
    public class MapExamToCourseCommandHandler : IRequestHandler<MapExamToCourseCommand, ResponseDto<bool>>
    {
        private readonly IRepository<ExamCourse> _examCourseRepository;
        private readonly IRepository<Exam> _examRepository;
        private readonly ILogger<MapExamToCourseCommandHandler> _logger;

        public MapExamToCourseCommandHandler(
            IRepository<ExamCourse> examCourseRepository,
            IRepository<Exam> examRepository,
            ILogger<MapExamToCourseCommandHandler> logger)
        {
            _examCourseRepository = examCourseRepository;
            _examRepository = examRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(MapExamToCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exam = await _examRepository.GetByIdAsync(request.ExamId);
                if (exam == null || !exam.IsActive)
                {
                    return ResponseDto<bool>.Failure("Exam does not exist or is inactive.");
                }

                var allMappings = await _examCourseRepository.GetAllAsync();
                var exists = allMappings.Any(x => x.ExamId == request.ExamId && x.CourseId == request.CourseId);

                if (exists)
                {
                    return ResponseDto<bool>.Failure("This exam is already mapped to the selected course.");
                }

                var examCourse = new ExamCourse
                {
                    ExamId = request.ExamId,
                    CourseId = request.CourseId
                };

                await _examCourseRepository.AddAsync(examCourse);
                await _examCourseRepository.SaveChangesAsync();

                _logger.LogInformation("Exam {ExamId} mapped to Course {CourseId} successfully.", request.ExamId, request.CourseId);

                return ResponseDto<bool>.Success(true, "Exam mapped to course successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while mapping ExamId: {ExamId} to CourseId: {CourseId}", request.ExamId, request.CourseId);
                return ResponseDto<bool>.Failure("An unexpected error occurred while mapping the exam.", new List<string> { ex.Message });
            }
        }
    }
}
