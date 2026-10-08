using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;

namespace Manikalearning.Application.Features.Handlers.Students
{
     
    public class SaveAskDoubtCommandHandler : IRequestHandler<SaveAskDoubtCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<DoubtEntity> _doubtRepo;

        public SaveAskDoubtCommandHandler(IRepository<DoubtEntity> doubtRepo)
        {
            _doubtRepo = doubtRepo;
        }

        public async Task<ResponseDto<Guid>> Handle(SaveAskDoubtCommand request, CancellationToken cancellationToken)
        {
            var doubt = new DoubtEntity
            {
                DoubtId = Guid.NewGuid(),
                UserId = request.UserId,
                LectureId = request.LectureId, 
                QuestionText = request.QuestionText.Trim(),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _doubtRepo.AddAsync(doubt);
            await _doubtRepo.SaveChangesAsync();
            return ResponseDto<Guid>.Success(doubt.DoubtId, "Doubt submitted successfully.");
        }
    }
}
