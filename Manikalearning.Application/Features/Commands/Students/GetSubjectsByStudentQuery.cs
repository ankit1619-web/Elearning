using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public class GetSubjectsByStudentQuery : IRequest<List<SubjectDto>>
    {
        public Guid UserId { get; set; }
    }
}
