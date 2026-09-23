using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subjects;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Subjects
{
    public class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Subject> _subjectRepository;
        public DeleteSubjectCommandHandler(IRepository<Subject> context)
        {
            _subjectRepository = context;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var subject = await _subjectRepository.Entity()
                    .FirstOrDefaultAsync(s => s.SubjectId == request.SubjectId, cancellationToken);
                if (subject == null)
                {
                    return ResponseDto<bool>.Failure("Subject not found.");
                } 
                bool hasQuestions = await _subjectRepository.Entity()
                    .AnyAsync(q => q.SubjectId == request.SubjectId, cancellationToken);
                if (hasQuestions)
                {
                    return ResponseDto<bool>.Failure("Cannot delete this Subject because Questions are mapped to it.");
                }

                  _subjectRepository.Remove(subject);
                await _subjectRepository.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Subject deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to delete subject: {ex.Message}");
            }
        }
    }
}
