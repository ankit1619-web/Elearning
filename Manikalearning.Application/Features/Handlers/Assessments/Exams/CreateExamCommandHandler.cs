using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Exams;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Exams
{
    public class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<Exam> _examRepository;
        private readonly ILogger<CreateExamCommandHandler> _logger;

        public CreateExamCommandHandler(
            IRepository<Exam> examRepository,
            ILogger<CreateExamCommandHandler> logger)
        {
            _examRepository = examRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<Guid>> Handle(CreateExamCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exam = new Exam
                {
                    ExamId = Guid.NewGuid(),
                    ExamName = request.ExamName,
                    ExamCode = request.ExamCode ?? "",
                    Description = request.Description, 
                    DisplayOrder = request.DisplayOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _examRepository.AddAsync(exam);
                await _examRepository.SaveChangesAsync();

                _logger.LogInformation("Exam created successfully in Exams table: {ExamId}", exam.ExamId);

                return ResponseDto<Guid>.Success(exam.ExamId, "Exam created successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating exam: {ExamName}", request.ExamName);
                return ResponseDto<Guid>.Failure("An unexpected error occurred while creating the exam.", new List<string> { ex.Message });
            }
        }
    }
}
