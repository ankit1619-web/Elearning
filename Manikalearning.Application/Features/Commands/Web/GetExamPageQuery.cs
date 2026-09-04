using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Web
{
     
    public class GetExamPageQuery : IRequest<List<ExamListDto>>
    {
    }
}
