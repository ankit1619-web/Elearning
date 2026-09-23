using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subjects;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Subjects
{
    public class GetSubjectByIdQueryHandler : IRequestHandler<GetSubjectByIdQuery, ResponseDto<SubjectDto>>
    {
        private readonly IRepository<Subject> _subjectRepository;
        public GetSubjectByIdQueryHandler(IRepository<Subject> context)
        {
            _subjectRepository = context;
        }

        public async Task<ResponseDto<SubjectDto>> Handle(GetSubjectByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var subject = await _subjectRepository.Entity()
                    .AsNoTracking()
                    .Include(s => s.Paper)
                    .Where(s => s.SubjectId == request.SubjectId)
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
                    .FirstOrDefaultAsync(cancellationToken);

                if (subject == null)
                    return ResponseDto<SubjectDto>.Failure("Subject not found.");

                return ResponseDto<SubjectDto>.Success(subject, "Subject details retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<SubjectDto>.Failure($"Error fetching subject details: {ex.Message}");
            }
        }
    }
}
