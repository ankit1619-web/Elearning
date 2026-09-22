using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.ExamCourse;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.ExamCourses
{
    public class GetExamsByCourseIdQueryHandler : IRequestHandler<GetExamsByCourseIdQuery, ResponseDto<List<ExamCourseDto>>>
    {
        private readonly IRepository<ExamCourse> _examCourseRepository;
        private readonly IRepository<Exam> _examRepository;
        private readonly ILogger<GetExamsByCourseIdQueryHandler> _logger;

        public GetExamsByCourseIdQueryHandler(
            IRepository<ExamCourse> examCourseRepository,
            IRepository<Exam> examRepository,
            ILogger<GetExamsByCourseIdQueryHandler> logger)
        {
            _examCourseRepository = examCourseRepository;
            _examRepository = examRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<List<ExamCourseDto>>> Handle(GetExamsByCourseIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var examCourses = await _examCourseRepository.GetAllAsync();
                var exams = await _examRepository.GetAllAsync();

                var result = (from ec in examCourses
                              join e in exams on ec.ExamId equals e.ExamId
                              where ec.CourseId == request.CourseId && e.IsActive
                              orderby e.DisplayOrder
                              select new ExamCourseDto
                              {
                                  ExamId = ec.ExamId,
                                  CourseId = ec.CourseId,
                                  ExamName = e.ExamName,
                                  ExamCode = e.ExamCode,
                                  Description = e.Description,
                                  ImageUrl = e.ImageUrl,
                                  IsActive = e.IsActive
                              }).ToList();

                return ResponseDto<List<ExamCourseDto>>.Success(result, "Exams fetched successfully for the given course.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching exams for Course ID: {CourseId}", request.CourseId);
                return ResponseDto<List<ExamCourseDto>>.Failure("An unexpected error occurred while fetching mapped exams.", new List<string> { ex.Message });
            }
        }
    }
}
