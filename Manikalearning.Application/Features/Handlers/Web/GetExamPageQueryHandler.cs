using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Web
{ 
    public class GetExamPageQueryHandler  : IRequestHandler<GetExamPageQuery, List<ExamListDto>>
    {
        private readonly IRepository<Exam> _examRepository;
        private readonly IRepository<Question> _questionRepository; 
        private readonly IRepository<Test> _testRepository;
        private readonly IRepository<User> _studentRepository;

        public GetExamPageQueryHandler(
            IRepository<Exam> examRepository,
            IRepository<Question> questionRepository, 
            IRepository<Test> testRepository,
            IRepository<User> studentRepository)
        {
            _examRepository = examRepository;
            _questionRepository = questionRepository; 
            _testRepository = testRepository;
            _studentRepository = studentRepository;
        }

        public async Task<List<ExamListDto>> Handle(
            GetExamPageQuery request,
            CancellationToken cancellationToken)
        {
            var exams = await _examRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.IsActive) 
                .ToListAsync(cancellationToken);

            var result = new List<ExamListDto>();

            foreach (var exam in exams)
            {
                var totalQuestions = await _questionRepository
                    .Query()
                    .CountAsync(  x => x.ExamId == exam.ExamId,
                        cancellationToken);
                var totalPyq = await _questionRepository
                    .Query()
                    .CountAsync(  x => x.ExamId == exam.ExamId && x.IsPYQ,
                        cancellationToken);
                var totalTest = await _testRepository
                    .Query()
                    .CountAsync(  x => x.ExamId == exam.ExamId,
                        cancellationToken);
                    
                var totalStudents = await _studentRepository
                    .Query()
                    .CountAsync(cancellationToken);

                result.Add(new ExamListDto
                {
                    Id = exam.ExamId,

                    Name = exam.ExamName, 
                    ShortDescription = exam.Description, 
                    Image = exam.ImageUrl, 
                    TotalQuestions = totalQuestions,  
                    TotalPYQs = totalPyq,  
                    TotalTests = totalTest,  
                    TotalStudents = totalStudents
                });
            }

            return result;
        }
    }
}
