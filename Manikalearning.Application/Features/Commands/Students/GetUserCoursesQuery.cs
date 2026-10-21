using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public record GetUserCoursesQuery(
        Guid StudentId,
        string Filter = "All",
        string SearchTerm = ""
    ) : IRequest<MyCoursesViewModel>;
}
