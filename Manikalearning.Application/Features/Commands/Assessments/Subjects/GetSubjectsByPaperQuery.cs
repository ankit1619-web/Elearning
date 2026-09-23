using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Subjects
{
    public record GetSubjectsByPaperQuery(Guid PaperId, string SearchTerm = "") : IRequest<ResponseDto<List<SubjectDto>>>;
}
