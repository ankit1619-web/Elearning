using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Sections;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Sections
{
    public class GetSubjectsLookupQueryHandler : IRequestHandler<GetSubjectsLookupQuery, ResponseDto<List<LookupDto>>>
    {
        private readonly IRepository<Subject> _SectionRepository;

        public GetSubjectsLookupQueryHandler(IRepository<Subject> context)
        {
            _SectionRepository = context;
        }

        public async Task<ResponseDto<List<LookupDto>>> Handle(GetSubjectsLookupQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var subjects = await _SectionRepository.Entity()
                    .AsNoTracking()  
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.SubjectName)
                    .Select(s => new LookupDto
                    {
                        Id = s.SubjectId,
                        Name = s.SubjectName
                    })
                    .ToListAsync(cancellationToken);

                return ResponseDto<List<LookupDto>>.Success(subjects, "Subjects retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<List<LookupDto>>.Failure($"Failed to fetch subjects lookup: {ex.Message}");
            }
        }
    }
}
