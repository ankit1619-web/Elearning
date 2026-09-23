using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Sections;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Sections
{
    public class UpdateSectionCommandHandler : IRequestHandler<UpdateSectionCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Section> _SectionRepository;

        public UpdateSectionCommandHandler(IRepository<Section> context)
        {
            _SectionRepository = context;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var section = await _SectionRepository.Entity()
                    .FirstOrDefaultAsync(s => s.SectionId == request.SectionId, cancellationToken);
                if (section == null)
                {
                    return ResponseDto<bool>.Failure("Section not found.");
                }

                bool exists = await _SectionRepository.Entity()
                    .AnyAsync(s => s.SubjectId == request.SubjectId &&
                                  s.SectionId != request.SectionId &&
                                  s.SectionName.ToLower() == request.SectionName.Trim().ToLower(), cancellationToken);
                if (exists)
                {
                    return ResponseDto<bool>.Failure($"Section Name '{request.SectionName}' already exists under this Subject.");
                }

                section.SubjectId = request.SubjectId;
                section.SectionName = request.SectionName.Trim();
                section.IsActive = request.IsActive;
                section.DisplayOrder = request.DisplayOrder;

                 _SectionRepository.Update(section);
                await _SectionRepository.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Section updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to update section: {ex.Message}");
            }
        }
    }
}
