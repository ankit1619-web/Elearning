using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Tests;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Tests
{
    public class GetAvailableQuestionsQueryHandler : IRequestHandler<GetAvailableQuestionsQuery, List<TestQuestionDto>>
    {
        private readonly IRepository<Question> _questionRepo;
        public GetAvailableQuestionsQueryHandler(IRepository<Question> questionRepo)
        {
            _questionRepo = questionRepo;
        } 

        public async Task<List<TestQuestionDto>> Handle(GetAvailableQuestionsQuery request, CancellationToken cancellationToken)
        {
            var query = _questionRepo.Entity()
                .AsNoTracking()
                .AsQueryable();

            if (request.SubjectId.HasValue && request.SubjectId.Value != Guid.Empty)
            {
                query = query.Where(q => q.SubjectId == request.SubjectId.Value);
            }

            return await query
                .Select(q => new TestQuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText, 
                    SubjectName = q.Subject != null ? q.Subject.SubjectName : null, 
                    Marks = q.Marks
                })
                .ToListAsync(cancellationToken);
        }
    }
}
