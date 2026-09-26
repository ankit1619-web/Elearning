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
    public class ResetTestAttemptCommandHandler : IRequestHandler<ResetTestAttemptCommand, bool>
    {
        private readonly IRepository<TestAttempt> _attemptRepo;
        private readonly IRepository<TestAttemptAnswer> _answerRepo;

        public ResetTestAttemptCommandHandler(IRepository<TestAttempt> attemptRepo, IRepository<TestAttemptAnswer> answerRepo)
        {
            _attemptRepo = attemptRepo;
            _answerRepo = answerRepo;
        }

        public async Task<bool> Handle(ResetTestAttemptCommand request, CancellationToken cancellationToken)
        {
            var attempt = await _attemptRepo.Entity().FirstOrDefaultAsync(x => x.AttemptId == request.AttemptId, cancellationToken);
            if (attempt == null) return false;
             
            var answers = await _answerRepo.Entity().Where(x => x.AttemptId == request.AttemptId).ToListAsync(cancellationToken);
            foreach (var answer in answers)
            {
                  _answerRepo.Remove(answer);
            }
             
              _attemptRepo.Remove(attempt);
            await _attemptRepo.SaveChangesAsync();
            return true;
        }
    }
}
