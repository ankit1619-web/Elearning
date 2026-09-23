using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subjects;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Subjects
{
    public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Subject> _subjectRepository;
        public UpdateSubjectCommandHandler(IRepository<Subject> context)
        {
            _subjectRepository = context;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var subject = await _subjectRepository.Entity()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.SubjectId == request.SubjectId, cancellationToken);
                if (subject == null)
                {
                    return ResponseDto<bool>.Failure("Subject not found.");
                }

                bool exists = await _subjectRepository.Entity().AnyAsync(s => s.PaperId == request.PaperId &&
                                                                  s.SubjectId != request.SubjectId &&
                                                                  s.SubjectName.ToLower() == request.SubjectName.Trim().ToLower(), cancellationToken);
                if (exists)
                {
                    return ResponseDto<bool>.Failure($"Subject Name '{request.SubjectName}' already exists under this Paper.");
                }

                subject.PaperId = request.PaperId;
                subject.SubjectName = request.SubjectName.Trim();
                subject.Description = request.Description?.Trim() ?? "";
                subject.DisplayOrder = request.DisplayOrder;
                subject.IsActive = request.IsActive;

                  _subjectRepository.Update(subject);
                await _subjectRepository.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Subject updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to update subject: {ex.Message}");
            }
        }
    }
}
