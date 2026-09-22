using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Papers
{
    public record GetPapersByExamQuery(Guid ExamId, string SearchTerm = "") : IRequest<ResponseDto<List<PaperDto>>>;
}
