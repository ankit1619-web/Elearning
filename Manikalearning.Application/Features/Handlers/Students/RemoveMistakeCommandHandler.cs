using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class RemoveMistakeCommandHandler : IRequestHandler<RemoveMistakeCommand, bool>
    {
        private readonly IRepository<QuestionMistake> _mistakeRepo;

        public RemoveMistakeCommandHandler(IRepository<QuestionMistake> mistakeRepo)
        {
            _mistakeRepo = mistakeRepo;
        }

        public async Task<bool> Handle(RemoveMistakeCommand request, CancellationToken cancellationToken)
        {
            var mistake = await _mistakeRepo.Query()
                .FirstOrDefaultAsync(m => m.MistakeId == request.MistakeId && m.UserId == request.UserId, cancellationToken);

            if (mistake == null) return false;

              _mistakeRepo.Remove(mistake);
            await _mistakeRepo.SaveChangesAsync();
            return true;
        }
    }
}
