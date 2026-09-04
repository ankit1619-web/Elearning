using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Web
{
    public class GetExamDetailsQuery : IRequest<ExamDetailsDto?>
    {
        public Guid ExamId { get; set; }

        public GetExamDetailsQuery(Guid examId)
        {
            ExamId = examId;
        }
    }
}
