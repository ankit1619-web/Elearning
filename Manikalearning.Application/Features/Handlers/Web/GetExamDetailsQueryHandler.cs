using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;

namespace Manikalearning.Application.Features.Handlers.Web
{
    public class GetExamDetailsQueryHandler
        : IRequestHandler<GetExamDetailsQuery, ExamDetailsDto?>
    {
        private readonly IRepository<Exam> _examRepository;
        private readonly IRepository<Question> _questionRepository; 
        private readonly IRepository<Test> _testRepository;
        private readonly IRepository<ExamCourse> _courseRepository; 

        public GetExamDetailsQueryHandler(
            IRepository<Exam> examRepository,
            IRepository<Question> questionRepository, 
            IRepository<Test> testRepository,
            IRepository<ExamCourse> courseRepository  )
        {
            _examRepository = examRepository;
            _questionRepository = questionRepository; 
            _testRepository = testRepository;
            _courseRepository = courseRepository; 
        }

        public async Task<ExamDetailsDto?> Handle(GetExamDetailsQuery request,CancellationToken cancellationToken)
        { 
            var exam = await _examRepository.GetByIdAsync(request.ExamId);

            if (exam == null)
                return null;
             
            var questions = await _questionRepository.FindAsync(
                x => x.ExamId == request.ExamId && x.IsPYQ == false);
              
            var pyqs = await _questionRepository.FindAsync(
                x => x.ExamId == request.ExamId && x.IsPYQ);
             

            var tests = await _testRepository.FindAsync(
                x => x.ExamId == request.ExamId);
              
            var courses = await _courseRepository.FindAsync(
                x => x.ExamId == request.ExamId);
             
              
               
            var dto = new ExamDetailsDto
            {
                ExamId = exam.ExamId,

                Name = exam.ExamName,

                ShortDescription = exam.Description,

                Description = exam.Description,

                Image = exam.ImageUrl, 

                TotalQuestions = questions.Count(),

                TotalPYQs = pyqs.Count(),

                TotalTests = tests.Count() 
                 
            }; 

            return dto;
        }
    }
} 
