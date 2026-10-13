using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetSectionsBySubjectQueryHandler : IRequestHandler<GetSectionsBySubjectQuery, ResponseDto<List<SectionDto>>>
    {
        private readonly IRepository<Section> _sectionRepo;
        private readonly IRepository<Question> _questionRepo;

        public GetSectionsBySubjectQueryHandler(IRepository<Section> sectionRepo, IRepository<Question> questionRepo)
        {
            _sectionRepo = sectionRepo;
            _questionRepo = questionRepo;
        }

        public async Task<ResponseDto<List<SectionDto>>> Handle(GetSectionsBySubjectQuery request, CancellationToken cancellationToken)
        {
            var sectionsQuery = await _sectionRepo.Query()
                .AsNoTracking()
                .Where(s => s.SubjectId == request.SubjectId && s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync(cancellationToken);
            var sectionIds = sectionsQuery.Select(s => s.SectionId).ToList();
            var questionCounts = await _questionRepo.Query()
                .AsNoTracking()
                .Where(q => q.SectionId.HasValue
                         && sectionIds.Contains(q.SectionId.Value)
                         && q.IsPYQ
                         && q.IsActive
                         && q.IsApproved)
                .GroupBy(q => q.SectionId.Value)
                .Select(g => new { SectionId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SectionId, x => x.Count, cancellationToken);
            var result = sectionsQuery
                .Where(s => questionCounts.ContainsKey(s.SectionId) && questionCounts[s.SectionId] > 0)
                .Select(s => new SectionDto
                {
                    SectionId = s.SectionId,
                    SectionName = s.SectionName,
                    TotalQuestions = questionCounts[s.SectionId]
                })
                .ToList();
            return ResponseDto<List<SectionDto>>.Success(result, "Sections retrieved successfully.");
        }
    }
}
