using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Web
{
    public record GetLecturePartialQuery(Guid LectureId) : IRequest<CourseLectureDto>;
}
