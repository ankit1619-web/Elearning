using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Sections;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Sections
{
    public class GetSectionByIdQueryHandler : IRequestHandler<GetSectionByIdQuery, ResponseDto<SectionDto>>
    {
        private readonly IRepository<Section> _SectionRepository;

        public GetSectionByIdQueryHandler(IRepository<Section> context)
        {
            _SectionRepository = context;
        }

        public async Task<ResponseDto<SectionDto>> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var section = await _SectionRepository.Entity()
                    .AsNoTracking()
                    .Include(s => s.Subject)
                    .Where(s => s.SectionId == request.SectionId)
                    .Select(s => new SectionDto
                    {
                        SectionId = s.SectionId,
                        SubjectId = s.SubjectId,
                        SubjectName = s.Subject != null ? s.Subject.SubjectName : string.Empty,
                        SectionName = s.SectionName,
                        IsActive = s.IsActive,
                        DisplayOrder = s.DisplayOrder
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (section == null)
                    return ResponseDto<SectionDto>.Failure("Section not found.");

                return ResponseDto<SectionDto>.Success(section, "Section retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<SectionDto>.Failure($"Error fetching section details: {ex.Message}");
            }
        }
    }
}
