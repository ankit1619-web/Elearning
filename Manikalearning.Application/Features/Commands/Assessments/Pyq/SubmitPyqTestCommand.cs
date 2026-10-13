using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Pyq
{
    public class SubmitPyqTestCommand : IRequest<PyqTestResultDto>
    {
        public Guid UserId { get; set; }
        public Guid SectionId { get; set; }
        public int TimeTakenSeconds { get; set; }
        public List<PyqAnswerDto> Answers { get; set; } = new();
    }
}
