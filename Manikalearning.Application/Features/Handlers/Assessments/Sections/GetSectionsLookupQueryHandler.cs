using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Sections;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Sections
{
    public class GetSectionsLookupQueryHandler : IRequestHandler<GetSectionsLookupQuery, ResponseDto<List<LookupDto>>>
    {
        private readonly IRepository<Section> _SectionRepository;

        public GetSectionsLookupQueryHandler(IRepository<Section> context)
        {
            _SectionRepository = context;
        }

        public async Task<ResponseDto<List<LookupDto>>> Handle(GetSectionsLookupQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sections = await _SectionRepository.Entity()
                    .AsNoTracking()  
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.SectionName)
                    .Select(s => new LookupDto
                    {
                        Id = s.SectionId,
                        Name = s.SectionName    
                    })
                    .ToListAsync(cancellationToken);

                return ResponseDto<List<LookupDto>>.Success(sections, "Sections retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<List<LookupDto>>.Failure($"Failed to fetch sections lookup: {ex.Message}");
            }
        }
    }
}
