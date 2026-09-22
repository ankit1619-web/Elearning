using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Exams;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Exams
{
    public class UpdateExamCommandHandler : IRequestHandler<UpdateExamCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Exam> _examRepository;
        private readonly ILogger<UpdateExamCommandHandler> _logger;

        public UpdateExamCommandHandler(
            IRepository<Exam> examRepository,
            ILogger<UpdateExamCommandHandler> logger)
        {
            _examRepository = examRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateExamCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exam = await _examRepository.Entity().FirstOrDefaultAsync(e=>e.ExamId==request.ExamId);
                if (exam == null)
                {
                    _logger.LogWarning("Exam not found for update: {ExamId}", request.ExamId);
                    return ResponseDto<bool>.Failure("Exam not found.");
                }

                exam.ExamName = request.ExamName;
                exam.ExamCode = request.ExamCode ?? "";
                exam.Description = request.Description; 
                exam.IsActive = request.IsActive;
                exam.DisplayOrder = request.DisplayOrder;

                  _examRepository.Update(exam);
                await _examRepository.SaveChangesAsync();

                _logger.LogInformation("Exam updated successfully: {ExamId}", exam.ExamId);

                return ResponseDto<bool>.Success(true, "Exam updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating exam: {ExamId}", request.ExamId);
                return ResponseDto<bool>.Failure("An unexpected error occurred while updating the exam.", new List<string> { ex.Message });
            }
        }
    }
}
