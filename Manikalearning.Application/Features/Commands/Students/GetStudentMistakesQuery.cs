using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{ 
    public record GetStudentMistakesQuery(Guid UserId) : IRequest<List<MistakeDto>>;
}
