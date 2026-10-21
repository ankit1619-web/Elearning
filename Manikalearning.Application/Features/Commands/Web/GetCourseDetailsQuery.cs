using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Web
{
    public class GetCourseDetailsQuery : IRequest<WatchOverviewViewModel>
    {
        public Guid CourseId { get; set; }

        public GetCourseDetailsQuery(Guid courseId)
        {
            CourseId = courseId;
        }
    }
}
