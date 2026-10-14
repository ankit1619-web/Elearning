using FluentValidation;
using FluentValidation.Results;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Doubts;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Doubts
{
    public class AnswerDoubtCommandHandler : IRequestHandler<AnswerDoubtCommand, Guid>
    {
        private readonly IRepository<DoubtEntity> _doubts;
        private readonly IRepository<DoubtReplyEntity> _replies;
        public AnswerDoubtCommandHandler(IRepository<DoubtEntity> doubts, IRepository<DoubtReplyEntity> replies) { _doubts = doubts; _replies = replies; }

        public async Task<Guid> Handle(AnswerDoubtCommand req, CancellationToken ct)
        {
            var d = await _doubts.Entity().FirstOrDefaultAsync(x => x.DoubtId == req.DoubtId && x.IsActive && !x.IsDeleted, ct)
                    ?? throw new ValidationException(new[] { new ValidationFailure("DoubtId", "Doubt not found.") });

            var reply = new DoubtReplyEntity
            {
                ReplyId = Guid.NewGuid(),
                DoubtId = d.DoubtId,
                UserId = req.StaffUserId,
                ReplyText = req.Text.Trim(),
                IsFromStaff = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            await _replies.AddAsync(reply);

            d.Status = req.Close ? DoubtStatus.Closed : DoubtStatus.Answered;
            await _doubts.SaveChangesAsync();
            return reply.ReplyId;
        }
    }
}
