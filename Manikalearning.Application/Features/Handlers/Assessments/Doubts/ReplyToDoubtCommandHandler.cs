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
    public class ReplyToDoubtCommandHandler : IRequestHandler<ReplyToDoubtCommand, Guid>
    {
        private readonly IRepository<DoubtEntity> _doubts;
        private readonly IRepository<DoubtReplyEntity> _replies;
        public ReplyToDoubtCommandHandler(IRepository<DoubtEntity> doubts, IRepository<DoubtReplyEntity> replies) { _doubts = doubts; _replies = replies; }

        public async Task<Guid> Handle(ReplyToDoubtCommand req, CancellationToken ct)
        {
            var d = await _doubts.Entity().FirstOrDefaultAsync(x =>
                        x.DoubtId == req.DoubtId && x.UserId == req.UserId && x.IsActive && !x.IsDeleted, ct)
                    ?? throw new ValidationException(new[] { new ValidationFailure("DoubtId", "Doubt not found.") });

            if (d.Status == DoubtStatus.Closed)
                throw new ValidationException(new[] { new ValidationFailure("Text", "This doubt is closed. Please ask a new doubt.") });

            var reply = new DoubtReplyEntity
            {
                ReplyId = Guid.NewGuid(),
                DoubtId = d.DoubtId,
                UserId = req.UserId,
                ReplyText = req.Text.Trim(),
                IsFromStaff = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            await _replies.AddAsync(reply);

            if (d.Status == DoubtStatus.Answered) d.Status = DoubtStatus.Open;  
            await _doubts.SaveChangesAsync();
            return reply.ReplyId;
        }
    }
}
