using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Exams;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Exams
{
    public class DeleteExamCommandHandler : IRequestHandler<DeleteExamCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Exam> _examRepository;
        private readonly ILogger<DeleteExamCommandHandler> _logger;

        public DeleteExamCommandHandler(
            IRepository<Exam> examRepository,
            ILogger<DeleteExamCommandHandler> logger)
        {
            _examRepository = examRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteExamCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exam = await _examRepository.Entity().FirstOrDefaultAsync(e=>e.ExamId==request.ExamId);
                if (exam == null)
                {
                    _logger.LogWarning("Exam not found for deletion: {ExamId}", request.ExamId);
                    return ResponseDto<bool>.Failure("Exam not found.");
                }

                  _examRepository.Remove(exam);
                await _examRepository.SaveChangesAsync();

                _logger.LogInformation("Exam deleted successfully: {ExamId}", request.ExamId);

                return ResponseDto<bool>.Success(true, "Exam deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting exam: {ExamId}", request.ExamId);
                return ResponseDto<bool>.Failure("An unexpected error occurred while deleting the exam.", new List<string> { ex.Message });
            }
        }
    }
}
