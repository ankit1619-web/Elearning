using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Web
{
    public  class GetFilteredPYQsQueryHandler
    {
        private readonly IRepository<Question> _questionRepository;
        public GetFilteredPYQsQueryHandler(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;

        }
        public async Task<PYQTestSessionVM> Handle(GetFilteredPYQsQuery request, CancellationToken cancellationToken)
        {
            var query = _questionRepository.Entity()
                .Include(q => q.Options)
                .AsQueryable();
             
            if (request.PaperId.HasValue)
                query = query.Where(q => q.PaperId == request.PaperId.Value);

            if (request.SectionId.HasValue)
                query = query.Where(q => q.SectionId == request.SectionId.Value);

            if (request.TopicId.HasValue)
                query = query.Where(q => q.TopicId == request.TopicId.Value);

            var questionsList = await query
                .OrderBy(q => q.CreatedAt)
                .Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText, 
                    Explanation = q.Explanation, 
                    Options = q.Options.Select(o => new QuestionOptionDto
                    {
                        OptionId = o.OptionId,
                        OptionText = o.OptionText
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            return new PYQTestSessionVM
            {
                Title = "PYQ Practice Test",
                Questions = questionsList
            };
        }
    }
}
