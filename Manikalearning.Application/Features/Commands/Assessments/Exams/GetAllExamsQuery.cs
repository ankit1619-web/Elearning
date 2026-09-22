using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Exams
{
    public record GetAllExamsQuery(string SearchTerm = null) : IRequest<ResponseDto<List<ExamDto>>>;
}
