using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Courses;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Courses
{
    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<Course> _courseRepository; 

        public UpdateCourseCommandHandler(IRepository<Course> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<ResponseDto<Guid>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        { 
            var course = await _courseRepository.Entity()
                .FirstOrDefaultAsync(c => c.CourseId == request.CourseId, cancellationToken);

            if (course == null)
            {
                return ResponseDto<Guid>.Failure("Course not found or has already been deleted.");
            }
             
            course.CourseName = request.CourseName;
            course.Description = request.Description;
            course.Price = request.Price;
            course.IsPublished = request.IsPublished; 
             
              _courseRepository.Update(course);
             
              await _courseRepository.SaveChangesAsync();
               
            return ResponseDto<Guid>.Success(course.CourseId, "Course updated successfully.");
        }
    }
}
