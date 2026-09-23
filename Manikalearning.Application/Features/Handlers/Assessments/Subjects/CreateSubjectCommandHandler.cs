using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Subjects;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Subjects
{
    public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<Subject> _subjectRepository;
        public CreateSubjectCommandHandler(IRepository<Subject> context)
        {
            _subjectRepository = context;
        }

        public async Task<ResponseDto<Guid>> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool exists = await _subjectRepository.Entity().AnyAsync(s => s.PaperId == request.PaperId &&
                                                                  s.SubjectName.ToLower() == request.SubjectName.Trim().ToLower(), cancellationToken);
                if (exists)
                {
                    return ResponseDto<Guid>.Failure($"Subject Name '{request.SubjectName}' already exists under this Paper.");
                }

                var subject = new Subject
                {
                    SubjectId = Guid.NewGuid(),
                    PaperId = request.PaperId,
                    SubjectName = request.SubjectName.Trim(),
                    Description = request.Description?.Trim() ?? "",
                    DisplayOrder = request.DisplayOrder,
                    IsActive = request.IsActive
                };

                await _subjectRepository.AddAsync(subject);
                await _subjectRepository.SaveChangesAsync();

                return ResponseDto<Guid>.Success(subject.SubjectId, "Subject created successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<Guid>.Failure($"Failed to create subject: {ex.Message}");
            }
        }
    }
}
