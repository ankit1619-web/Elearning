using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Sections;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Sections
{
    public class CreateSectionCommandHandler : IRequestHandler<CreateSectionCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<Section> _SectionRepository;

        public CreateSectionCommandHandler(IRepository<Section> context)
        {
            _SectionRepository = context;
        } 

        public async Task<ResponseDto<Guid>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool exists = await _SectionRepository.Entity()
                    .AnyAsync(s => s.SubjectId == request.SubjectId &&
                                  s.SectionName.ToLower() == request.SectionName.Trim().ToLower(), cancellationToken);
                if (exists)
                {
                    return ResponseDto<Guid>.Failure($"Section Name '{request.SectionName}' already exists under this Subject.");
                }

                var section = new Section
                {
                    SectionId = Guid.NewGuid(),
                    SubjectId = request.SubjectId,
                    SectionName = request.SectionName.Trim(),
                    IsActive = request.IsActive,
                    DisplayOrder = request.DisplayOrder
                };

                await _SectionRepository.AddAsync(section);
                await _SectionRepository.SaveChangesAsync();

                return ResponseDto<Guid>.Success(section.SectionId, "Section created successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<Guid>.Failure($"Failed to create section: {ex.Message}");
            }
        }
    }
}
