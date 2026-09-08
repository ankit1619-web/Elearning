using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Web
{
    public class GetExamDetailsQueryHandler
        : IRequestHandler<GetExamDetailsQuery, ExamDetailsDto?>
    {
        private readonly IRepository<Exam> _examRepository;
        private readonly IRepository<Question> _questionRepository; 
        private readonly IRepository<Test> _testRepository;
        private readonly IRepository<ExamCourse> _examCourseRepository;
        private readonly IRepository<Subject> _subjectRepository;
        private readonly IRepository<Paper> _paperRepository;
        private readonly ILogger<GetExamDetailsQueryHandler> _logger;
        public GetExamDetailsQueryHandler(
            IRepository<Exam> examRepository,
            IRepository<Question> questionRepository, 
            IRepository<Test> testRepository,
            IRepository<Paper> paperRepository,
            IRepository<ExamCourse> courseRepository,ILogger<GetExamDetailsQueryHandler> logger, IRepository<Subject> subjectRepository)
        {
            _examRepository = examRepository;
            _questionRepository = questionRepository; 
            _testRepository = testRepository;
            _examCourseRepository = courseRepository;
            _paperRepository = paperRepository;
            _logger = logger;
            _subjectRepository = subjectRepository;
        }

        public async Task<ExamDetailsDto?> Handle(GetExamDetailsQuery request,CancellationToken cancellationToken)
        {
            try
            { 

                var exam = await _examRepository
                    .Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.ExamId == request.ExamId,
                        cancellationToken);

                if (exam == null)
                    return null;
                 
                var questions = await _questionRepository
                    .Query()
                    .AsNoTracking()
                    .Where(x =>
                        x.ExamId == request.ExamId &&
                        !x.IsPYQ)
                    .ToListAsync(cancellationToken); 

                var pyqs = await _questionRepository
                    .Query()
                    .AsNoTracking() 
                    .Where(x =>
                        x.ExamId == request.ExamId &&
                        x.IsPYQ)
                    .CountAsync(); 

                var tests = await _testRepository
                    .Query()
                    .AsNoTracking()
                    .Where(x => x.ExamId == request.ExamId)
                    .ToListAsync(cancellationToken); 

                var courses = await _examCourseRepository
                    .Query()
                    .AsNoTracking()
                    .Where(x => x.ExamId == request.ExamId)
                    .Include(x => x.Course)
                    .Select(x => x.Course)
                    .Where(x => x != null)
                    .ToListAsync(cancellationToken);

                var papers = await _paperRepository
                   .Query()
                   .AsNoTracking()
                   .Include(x => x.Subjects)
                   .Include(x => x.Questions)
                   .Where(x => x.ExamId == request.ExamId)
                   .ToListAsync(cancellationToken);

                var subjects = papers.Select(x=>x.Subjects).SelectMany(x => x)
                    .ToList();

                return new ExamDetailsDto
                {
                    ExamId = exam.ExamId,

                    Name = exam.ExamName,

                    ShortDescription = exam.Description,

                    Description = exam.Description,

                    Image = exam.ImageUrl, 

                    TotalQuestions = questions.Count,

                    TotalPYQs = pyqs,

                    TotalTests = tests.Count, 

                    Subjects = subjects
                        .Select(x => new ExamSubjectDto
                        {
                            SubjectId = x.SubjectId,
                            SubjectName = x.SubjectName
                        })
                        .ToList(),

                    PYQs = papers
                     .Where(x => x.Questions.Any(q => q.IsPYQ)) 
                     .Take(10)
                     .Select(x => new ExamPYQDto
                     {
                         PaperId = x.PaperId,
                    
                         PaperName = x.PaperName,
                     
                         TotalQuestions = x.Questions
                             .Count(q => q.IsPYQ)
                     })
                     .ToList(),

                    Tests = tests
                        .OrderByDescending(x => x.CreatedAt)
                        .Take(6)
                        .Select(x => new ExamTestDto
                        {
                            TestId = x.TestId,
                            TestName = x.TestName,
                            TotalQuestions = x.TotalQuestions,
                            DurationMinutes = x.DurationMinutes,
                            TotalMarks = x.TotalMarks
                        })
                        .ToList(), 

                    Courses = courses
                        .Select(x => new ExamCourseDto
                        {
                            CourseId = x.CourseId,
                            CourseName = x.CourseName,  
                            Price = x.Price 
                        })
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching exam details for ExamId: {ExamId}",
                    request.ExamId);

                throw;
            }
        }
    }
} 
