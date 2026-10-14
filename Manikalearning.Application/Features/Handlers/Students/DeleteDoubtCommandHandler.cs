using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record DeleteDoubtCommand(Guid UserId, Guid DoubtId) : IRequest<bool>;

    public class DeleteDoubtCommandValidator : AbstractValidator<DeleteDoubtCommand>
    {
        public DeleteDoubtCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.DoubtId).NotEmpty();
        }
    }

    public class DeleteDoubtCommandHandler : IRequestHandler<DeleteDoubtCommand, bool>
    {
        private readonly IRepository<DoubtEntity> _doubts;
        public DeleteDoubtCommandHandler(IRepository<DoubtEntity> doubts) => _doubts = doubts;

        public async Task<bool> Handle(DeleteDoubtCommand req, CancellationToken ct)
        { 
            var d = await _doubts.Entity().FirstOrDefaultAsync(x =>
                x.DoubtId == req.DoubtId && x.UserId == req.UserId && !x.IsDeleted && x.Status == DoubtStatus.Open, ct);
            if (d == null) return false;

            d.IsDeleted = true;
            await _doubts.SaveChangesAsync();
            return true;
        }
    }
}
