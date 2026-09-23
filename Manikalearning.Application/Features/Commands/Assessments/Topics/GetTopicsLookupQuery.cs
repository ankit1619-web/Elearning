using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Topics
{
    public record GetTopicsLookupQuery(Guid? SectionId) : IRequest<ResponseDto<List<LookupDto>>>;
    
}
