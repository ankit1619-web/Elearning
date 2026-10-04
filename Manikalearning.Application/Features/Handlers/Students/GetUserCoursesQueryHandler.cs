using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetUserCoursesQueryHandler : IRequestHandler<GetUserCoursesQuery, MyCoursesViewModel>
    {
        private readonly ICourseRepository _courseRepository;

        public GetUserCoursesQueryHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<MyCoursesViewModel> Handle(GetUserCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = (await _courseRepository.GetStudentEnrolledCoursesAsync(
                request.StudentId,
                request.Filter,
                request.SearchTerm,
                cancellationToken)).ToList();

            return new MyCoursesViewModel
            {
                Courses = courses,
                TotalEnrolled = courses.Count,
                InProgressCount = courses.Count(c => !c.IsCompleted),
                CompletedCount = courses.Count(c => c.IsCompleted),
                SelectedFilter = request.Filter,
                SearchQuery = request.SearchTerm
            };
        }
    }
}
