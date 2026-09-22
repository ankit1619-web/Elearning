using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Papers;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Papers
{
    public class UpdatePaperCommandHandler : IRequestHandler<UpdatePaperCommand, ResponseDto<bool>>
    {

        private readonly IRepository<Paper> _paperRepository;

        public UpdatePaperCommandHandler(IRepository<Paper> context)
        {
            _paperRepository = context;
        }

        public async Task<ResponseDto<bool>> Handle(UpdatePaperCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var paper = await _paperRepository.Entity().FirstOrDefaultAsync(x=>x.PaperId== request.PaperId );
                if (paper == null)
                {
                    return ResponseDto<bool>.Failure("Paper not found.");
                }

                bool exists = await _paperRepository.Entity().AnyAsync(p => p.ExamId == request.ExamId &&
                                                                 p.PaperId != request.PaperId &&
                                                                 p.PaperCode.ToLower() == request.PaperCode.Trim().ToLower(), cancellationToken);
                if (exists)
                {
                    return ResponseDto<bool>.Failure($"Paper Code '{request.PaperCode}' already exists for this Exam.");
                }

                paper.ExamId = request.ExamId;
                paper.PaperName = request.PaperName.Trim();
                paper.PaperCode = request.PaperCode.Trim().ToUpper();
                paper.Description = request.Description?.Trim() ?? "";
                paper.DisplayOrder = request.DisplayOrder;
                paper.IsActive = request.IsActive;

                _paperRepository.Update(paper);
                await _paperRepository.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Paper updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to update paper: {ex.Message}");
            }
        }
    }
}
