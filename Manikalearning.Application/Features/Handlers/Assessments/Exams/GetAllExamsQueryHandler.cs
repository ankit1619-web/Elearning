using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Exams;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Exams
{
    public class GetAllExamsQueryHandler : IRequestHandler<GetAllExamsQuery, ResponseDto<List<ExamDto>>>
    { 
        private readonly IRepository<Exam> _exams;
        private readonly IRepository<ExamCourse> _examCourses;
        private readonly IRepository<Course> _courses;
        private readonly ILogger<GetAllExamsQueryHandler> _logger;

        public GetAllExamsQueryHandler(IRepository<Exam> exams, IRepository<ExamCourse> examCourses, IRepository<Course> courses, ILogger<GetAllExamsQueryHandler> logger)
        { _exams = exams; _examCourses = examCourses; _courses = courses; _logger = logger; }

        public async Task<ResponseDto<List<ExamDto>>> Handle(GetAllExamsQuery request, CancellationToken ct)
        {
            try
            {
                var s = request.SearchTerm?.Trim();
                var q = _exams.Entity().AsNoTracking().Where(e => e.IsActive);
                if (!string.IsNullOrEmpty(s)) q = q.Where(e => e.ExamName.Contains(s) || e.ExamCode.Contains(s));

                var exams = await q.OrderBy(e => e.DisplayOrder).ThenBy(e => e.ExamName).ToListAsync(ct);
                var ids = exams.Select(e => e.ExamId).ToList();
                var counts = await (from ec in _examCourses.Entity().AsNoTracking()
                                    join c in _courses.Entity().AsNoTracking() on ec.CourseId equals c.CourseId
                                    where ids.Contains(ec.ExamId) && c.IsPublished
                                    group ec by ec.ExamId into g
                                    select new { ExamId = g.Key, Count = g.Count() }).ToListAsync(ct);

                var countById = counts.ToDictionary(x => x.ExamId, x => x.Count);
               
               

                var examslist =  exams
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new ExamDto
                    {
                        ExamId = x.ExamId,
                        ExamName = x.ExamName,
                        ExamCode = x.ExamCode,
                        Description = x.Description,
                        ImageUrl = x.ImageUrl,
                        DisplayOrder = x.DisplayOrder,
                        IsActive = x.IsActive,
                        CourseCount = countById.TryGetValue(x.ExamId, out var n) ? n : 0
                    }).ToList() ;

                return ResponseDto<List<ExamDto>>.Success(examslist, "Exams retrieved successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching exams list.");
                return ResponseDto<List<ExamDto>>.Failure("An unexpected error occurred while fetching exams.", new List<string> { ex.Message });
            }
        }
    }
}
