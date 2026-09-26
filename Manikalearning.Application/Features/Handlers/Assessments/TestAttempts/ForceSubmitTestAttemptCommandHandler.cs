using Manikalearning.Application.Features.Commands.Assessments.TestAttempts;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.TestAttempts
{
    public class ForceSubmitTestAttemptCommandHandler : IRequestHandler<ForceSubmitTestAttemptCommand, bool>
    {
        private readonly IRepository<TestAttempt> _attemptRepo;

        public ForceSubmitTestAttemptCommandHandler(IRepository<TestAttempt> attemptRepo)
        {
            _attemptRepo = attemptRepo;
        }

        public async Task<bool> Handle(ForceSubmitTestAttemptCommand request, CancellationToken cancellationToken)
        {
            var attempt = await _attemptRepo.Entity().FirstOrDefaultAsync(x => x.AttemptId == request.AttemptId, cancellationToken);
            if (attempt == null || attempt.Status == "Completed") return false;

            attempt.Status = "Completed";
            attempt.SubmittedAt = DateTime.UtcNow;
             
            if (attempt.AttemptedQuestions > 0)
            {
                attempt.Accuracy = Math.Round(((decimal)attempt.CorrectAnswers / attempt.AttemptedQuestions) * 100, 2);
            }

              _attemptRepo.Update(attempt);
            await _attemptRepo.SaveChangesAsync();
            return true;
        }
    }
}
