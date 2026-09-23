using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subjects;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Subjects
{
    public class GetSubjectsByPaperQueryHandler : IRequestHandler<GetSubjectsByPaperQuery, ResponseDto<List<SubjectDto>>>
    {
        private readonly IRepository<Subject> _subjectRepository; 
        public GetSubjectsByPaperQueryHandler(IRepository<Subject> context)
        {
            _subjectRepository = context;
        }

        public async Task<ResponseDto<List<SubjectDto>>> Handle(GetSubjectsByPaperQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _subjectRepository.Entity()
                    .AsNoTracking()
                    .Include(s => s.Paper)
                    .Where(s => s.PaperId == request.PaperId);

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    string search = request.SearchTerm.Trim().ToLower();
                    query = query.Where(s => s.SubjectName.ToLower().Contains(search));
                }

                var subjects = await query
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => new SubjectDto
                    {
                        SubjectId = s.SubjectId,
                        PaperId = s.PaperId,
                        PaperName = s.Paper != null ? s.Paper.PaperName : string.Empty,
                        SubjectName = s.SubjectName,
                        Description = s.Description,
                        DisplayOrder = s.DisplayOrder,
                        IsActive = s.IsActive 
                    })
                    .ToListAsync(cancellationToken);

                return ResponseDto<List<SubjectDto>>.Success(subjects, "Subjects fetched successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<List<SubjectDto>>.Failure($"Error fetching subjects: {ex.Message}");
            }
        }
    }
}
