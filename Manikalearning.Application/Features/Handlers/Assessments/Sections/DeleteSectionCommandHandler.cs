using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Sections;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Sections
{
    public class DeleteSectionCommandHandler : IRequestHandler<DeleteSectionCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Section> _SectionRepository;
        private readonly IRepository<Topic> _TopicRepository;

        public DeleteSectionCommandHandler(IRepository<Section> context, IRepository<Topic> TopicRepository)
        {
            _SectionRepository = context;
            _TopicRepository = TopicRepository;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var section = await _SectionRepository.Entity()
                    .FirstOrDefaultAsync(s => s.SectionId == request.SectionId, cancellationToken);
                if (section == null)
                {
                    return ResponseDto<bool>.Failure("Section not found.");
                }

                bool hasTopics = await _TopicRepository.Entity()
                    .AnyAsync(t => t.SectionId == request.SectionId, cancellationToken);
                if (hasTopics)
                {
                    return ResponseDto<bool>.Failure("Cannot delete this Section because Topics exist under it.");
                }

                _SectionRepository.Remove(section);
                await _SectionRepository.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Section deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to delete section: {ex.Message}");
            }
        }
    }
}
