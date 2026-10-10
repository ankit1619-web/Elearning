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

        public GetSectionsBySubjectQueryHandler(IRepository<Section> sectionRepo)
        {
            _sectionRepo = sectionRepo;
        }

        public async Task<ResponseDto<List<SectionDto>>> Handle(GetSectionsBySubjectQuery request, CancellationToken cancellationToken)
        {
            var sections = await _sectionRepo.Query()
                .Where(s => s.SubjectId == request.SubjectId && s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new SectionDto
                {
                    SectionId = s.SectionId,
                    SectionName = s.SectionName,
                    TotalQuestions = s.Topics.SelectMany(t => t.Questions).Count()
                })
                .ToListAsync(cancellationToken);

            return ResponseDto<List<SectionDto>>.Success(sections, "Sections retrieved successfully.");
        }
    }
}
