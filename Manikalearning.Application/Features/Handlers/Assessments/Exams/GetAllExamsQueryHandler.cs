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
    public class GetAllExamsQueryHandler : IRequestHandler<GetAllExamsQuery, ResponseDto<List<ExamDto>>>
    {
        private readonly IRepository<Exam> _examRepository;
        private readonly ILogger<GetAllExamsQueryHandler> _logger;

        public GetAllExamsQueryHandler(
            IRepository<Exam> examRepository,
            ILogger<GetAllExamsQueryHandler> logger)
        {
            _examRepository = examRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<List<ExamDto>>> Handle(GetAllExamsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var exams = await _examRepository.Entity()
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
                        CreatedAt = x.CreatedAt
                    })
                    .ToListAsync();

                return ResponseDto<List<ExamDto>>.Success(exams, "Exams retrieved successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching exams list.");
                return ResponseDto<List<ExamDto>>.Failure("An unexpected error occurred while fetching exams.", new List<string> { ex.Message });
            }
        }
    }
}
