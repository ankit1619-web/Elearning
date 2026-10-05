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
                .Include(m => m.Question)
                .Where(m => m.UserId == request.UserId)
                .OrderByDescending(m => m.LastWrongAt)
                .Select(m => new MistakeDto
                {
                    MistakeId = m.MistakeId,
                    QuestionId = m.QuestionId,
                    QuestionText = m.Question.QuestionText,
                    //SelectedAnswer = m.SelectedAnswer,
                    //CorrectAnswer = m.Question.CorrectAnswer,
                    Explanation = m.Question.Explanation,
                    //CreatedAt = m.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
