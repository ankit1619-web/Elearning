using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Papers;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Papers
{
    public class CreatePaperCommandHandler : IRequestHandler<CreatePaperCommand, ResponseDto<Guid>>
    {

        private readonly IRepository<Paper> _paperRepository;

        public CreatePaperCommandHandler(IRepository<Paper> context)
        {
            _paperRepository = context;
        }

        public async Task<ResponseDto<Guid>> Handle(CreatePaperCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool exists = await _paperRepository.Entity().AnyAsync(p => p.ExamId == request.ExamId &&
                                                                 p.PaperCode.ToLower() == request.PaperCode.Trim().ToLower(), cancellationToken);
                if (exists)
                {
                    return ResponseDto<Guid>.Failure($"Paper Code '{request.PaperCode}' already exists for this Exam.");
                }

                var paper = new Paper
                {
                    PaperId = Guid.NewGuid(),
                    ExamId = request.ExamId,
                    PaperName = request.PaperName.Trim(),
                    PaperCode = request.PaperCode.Trim().ToUpper(),
                    Description = request.Description?.Trim() ?? "",
                    DisplayOrder = request.DisplayOrder,
                    IsActive = request.IsActive 
                };

                await _paperRepository.AddAsync(paper);
                await _paperRepository.SaveChangesAsync();

                return ResponseDto<Guid>.Success(paper.PaperId, "Paper created successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<Guid>.Failure($"Failed to create paper: {ex.Message}");
            }
        }
    }
}
