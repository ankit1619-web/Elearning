using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Papers;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Papers
{
    public class DeletePaperCommandHandler : IRequestHandler<DeletePaperCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Paper> _paperRepository;

        public DeletePaperCommandHandler(IRepository<Paper> context)
        {
            _paperRepository = context;
        }

        public async Task<ResponseDto<bool>> Handle(DeletePaperCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var paper = await _paperRepository.Entity().FirstOrDefaultAsync(x => x.PaperId == request.PaperId);
                if (paper == null)
                {
                    return ResponseDto<bool>.Failure("Paper not found.");
                }

                bool hasSubjects = await _paperRepository.Entity().AnyAsync(s => s.PaperId == request.PaperId, cancellationToken);
                if (hasSubjects)
                {
                    return ResponseDto<bool>.Failure("Cannot delete this Paper because it contains associated Subjects.");
                }

                _paperRepository.Remove(paper);
                await _paperRepository.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Paper deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to delete paper: {ex.Message}");
            }
        }
    }
}
