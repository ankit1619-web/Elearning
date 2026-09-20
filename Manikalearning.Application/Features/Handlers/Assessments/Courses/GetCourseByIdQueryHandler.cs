using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Courses;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Courses
{
    public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, ResponseDto<CourseDto>>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly ILogger<GetCourseByIdQueryHandler> _logger;

        public GetCourseByIdQueryHandler(IRepository<Course> courseRepository, ILogger<GetCourseByIdQueryHandler> logger)
        {
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.Entity()
                .AsNoTracking()
                .Where(c => c.CourseId == request.CourseId)
                .Select(c => new CourseDto
                {
                    Id = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    Price = c.Price,
                    Ispublished = c.IsPublished
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (course == null)
            {
                return ResponseDto<CourseDto>.Failure("Course not found with the provided ID.");
            }

            return ResponseDto<CourseDto>.Success(course, "Course details fetched successfully.");
        }
    }
}
