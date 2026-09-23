using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Sections
{
    public record GetSectionsBySubjectQuery(Guid? SubjectId = null, string SearchTerm = "") : IRequest<ResponseDto<List<SectionDto>>>;
    
}
