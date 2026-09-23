using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Passages;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Passages
{
    public class GetPassagesLookupQueryHandler : IRequestHandler<GetPassagesLookupQuery, ResponseDto<List<LookupDto>>>
    {
        private readonly IRepository<QuestionPassage> _passageRepo;

        public GetPassagesLookupQueryHandler(IRepository<QuestionPassage> passageRepo)
        {
            _passageRepo = passageRepo;
        }

        public async Task<ResponseDto<List<LookupDto>>> Handle(GetPassagesLookupQuery request, CancellationToken cancellationToken)
        {
            var query = _passageRepo.Entity().AsNoTracking();
             
            
            var passages = await query 
                .OrderBy(p => p.Title)
                .Select(p => new LookupDto
                {
                    Id = p.PassageId,
                    Name = p.Title  
                })
                .ToListAsync(cancellationToken);

            return ResponseDto<List<LookupDto>>.Success(passages);
        }
    }
}
