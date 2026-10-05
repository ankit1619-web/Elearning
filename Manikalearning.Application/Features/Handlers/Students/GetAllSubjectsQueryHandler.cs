using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetAllSubjectsQueryHandler : IRequestHandler<GetAllSubjectsQuery, List<SubjectDto>>
    {
        private readonly IRepository<Subject> _subjectRepo;

        public GetAllSubjectsQueryHandler(IRepository<Subject> subjectRepo)
        {
            _subjectRepo = subjectRepo;
        }

        public async Task<List<SubjectDto>> Handle(GetAllSubjectsQuery request, CancellationToken cancellationToken)
        {
            return await _subjectRepo.Query()
                .Where(s => s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new SubjectDto
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName
                })
                .ToListAsync(cancellationToken);
        }
    }
}
