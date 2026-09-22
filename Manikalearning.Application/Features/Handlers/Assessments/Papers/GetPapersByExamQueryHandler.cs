using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Papers;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Papers
{
    public class GetPapersByExamQueryHandler : IRequestHandler<GetPapersByExamQuery, ResponseDto<List<PaperDto>>>
    {
        private readonly IRepository<Paper> _paperRepository;

        public GetPapersByExamQueryHandler(IRepository<Paper> context)
        {
            _paperRepository = context;
        }

        public async Task<ResponseDto<List<PaperDto>>> Handle(GetPapersByExamQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _paperRepository.Entity()
                    .AsNoTracking()
                    .Include(p => p.Exam)
                    .Where(p => p.ExamId == request.ExamId);

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    string search = request.SearchTerm.Trim().ToLower();
                    query = query.Where(p => p.PaperName.ToLower().Contains(search) ||
                                             p.PaperCode.ToLower().Contains(search));
                }

                var papers = await query
                    .OrderBy(p => p.DisplayOrder)
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
                    .ToListAsync(cancellationToken);

                return ResponseDto<List<PaperDto>>.Success(papers, "Papers fetched successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<List<PaperDto>>.Failure($"Error fetching papers: {ex.Message}");
            }
        }
    }
}
