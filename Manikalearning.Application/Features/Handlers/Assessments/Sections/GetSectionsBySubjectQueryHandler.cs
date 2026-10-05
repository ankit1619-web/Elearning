using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Sections;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Sections
{
    public class GetSectionsBySubjectQueryHandler : IRequestHandler<GetSectionsBySubjectQuery, ResponseDto<List<SectionDto>>>
    {
        private readonly IRepository<Section> _SectionRepository;

        public GetSectionsBySubjectQueryHandler(IRepository<Section> context)
        {
            _SectionRepository = context;
        }

        public async Task<ResponseDto<List<SectionDto>>> Handle(GetSectionsBySubjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _SectionRepository.Entity()
                    .AsNoTracking()
                    .Include(s => s.Subject)
                    .AsQueryable();

                if (request.SubjectId.HasValue && request.SubjectId.Value != Guid.Empty)
                {
                    query = query.Where(s => s.SubjectId == request.SubjectId.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.SectionName))
                {
                    query = query.Where(s => s.SectionName == request.SectionName);
                }
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    string search = request.SearchTerm.Trim().ToLower();
                    query = query.Where(s => s.SectionName.ToLower().Contains(search) ||
                                             (s.Subject != null && s.Subject.SubjectName.ToLower().Contains(search)));
                }

                var sections = await query
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => new SectionDto
                    {
                        SectionId = s.SectionId,
                        SubjectId = s.SubjectId,
                        SubjectName = s.Subject != null ? s.Subject.SubjectName : string.Empty,
                        SectionName = s.SectionName,
                        IsActive = s.IsActive,
                        DisplayOrder = s.DisplayOrder
                    })
                    .ToListAsync(cancellationToken);

                return ResponseDto<List<SectionDto>>.Success(sections, "Sections fetched successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<List<SectionDto>>.Failure($"Error fetching sections: {ex.Message}");
            }
        }
    }
}
