using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Papers;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Papers
{
    public class GetPaperByIdQueryHandler : IRequestHandler<GetPaperByIdQuery, ResponseDto<PaperDto>>
    {
        private readonly IRepository<Paper> _paperRepository;

        public GetPaperByIdQueryHandler(IRepository<Paper> context)
        {
            _paperRepository = context;
        }

        public async Task<ResponseDto<PaperDto>> Handle(GetPaperByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var paper = await _paperRepository.Entity()
                    .AsNoTracking()
                    .Include(p => p.Exam)
                    .Where(p => p.PaperId == request.PaperId)
                    .Select(p => new PaperDto
                    {
                        PaperId = p.PaperId,
                        ExamId = p.ExamId,
                        ExamName = p.Exam != null ? p.Exam.ExamName : string.Empty,
                        PaperName = p.PaperName,
                        PaperCode = p.PaperCode,
                        Description = p.Description,
                        DisplayOrder = p.DisplayOrder,
                        IsActive = p.IsActive 
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (paper == null)
                    return ResponseDto<PaperDto>.Failure("Paper not found.");

                return ResponseDto<PaperDto>.Success(paper, "Paper retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<PaperDto>.Failure($"Error fetching paper details: {ex.Message}");
            }
        }
    }
}
