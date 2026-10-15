using Manikalearning.Application.Comman;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetStudentMistakesQueryHandler : IRequestHandler<GetStudentMistakesQuery, List<MistakeDto>>
    {
        private readonly IRepository<QuestionMistake> _mistakeRepo;

        public GetStudentMistakesQueryHandler(IRepository<QuestionMistake> mistakeRepo)
        {
            _mistakeRepo = mistakeRepo;
        }

        public async Task<List<MistakeDto>> Handle(GetStudentMistakesQuery request, CancellationToken cancellationToken)
        {
            return await _mistakeRepo.Query()
                .AsNoTracking()
                .Include(m => m.Question)
                    .ThenInclude(q => q.Options)
                .Where(m => m.UserId == request.UserId
                         && !m.IsResolved
                         && m.Question.IsActive)
                .OrderByDescending(m => m.LastWrongAt)
                .Select(m => new MistakeDto
                {
                    MistakeId = m.MistakeId,
                    QuestionId = m.QuestionId,
                    QuestionText = m.Question.QuestionText,
                     
                    Explanation = m.Question.Explanation,
                   
                    CorrectAnswer = m.Question.Options
                                        .Where(o => o.IsCorrect)
                                        .Select(o => o.OptionText)
                                        .FirstOrDefault() 
                })
                .ToListAsync(cancellationToken);
        }
    }
}
