using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Exams;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Exams
{
    public class GetExamByIdQueryHandler : IRequestHandler<GetExamByIdQuery, ResponseDto<ExamDto>>
    {
        private readonly IRepository<Exam> _examRepository;
        private readonly ILogger<GetExamByIdQueryHandler> _logger;

        public GetExamByIdQueryHandler(
            IRepository<Exam> examRepository,
            ILogger<GetExamByIdQueryHandler> logger)
        {
            _examRepository = examRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<ExamDto>> Handle(GetExamByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var exam = await _examRepository.Entity()
                    .Where(x => x.ExamId == request.ExamId)
                    .Select(x => new ExamDto
                    {
                        ExamId = x.ExamId,
                        ExamName = x.ExamName,
                        ExamCode = x.ExamCode,
                        Description = x.Description,
                        ImageUrl = x.ImageUrl,
                        IsActive = x.IsActive,
                        DisplayOrder = x.DisplayOrder,
                        CreatedAt = x.CreatedAt
                    }).FirstOrDefaultAsync();
                      

                return ResponseDto<ExamDto>.Success(exam, "Exams retrieved successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching exams list.");
                return ResponseDto<ExamDto>.Failure("An unexpected error occurred while fetching exams.", new List<string> { ex.Message });
            }
        }
    }
}
