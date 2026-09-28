using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudentCourses;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudentCourses
{
    public class GetEnrollmentDropdownsQueryHandler : IRequestHandler<GetEnrollmentDropdownsQuery, ResponseDto<(List<UserSelectOptionDto> Users, List<CourseSelectOptionDto> Courses)>>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Course> _courseRepo;

        public GetEnrollmentDropdownsQueryHandler(IRepository<User> userRepo, IRepository<Course> courseRepo)
        {
            _userRepo = userRepo;
            _courseRepo = courseRepo;
        }

        public async Task<ResponseDto<(List<UserSelectOptionDto> Users, List<CourseSelectOptionDto> Courses)>> Handle(GetEnrollmentDropdownsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepo.Entity()
                    .AsNoTracking()
                    .OrderBy(u => u.FullName)
                    .Select(u => new UserSelectOptionDto
                    {
                        UserId = u.Id,
                        DisplayText = $"{u.FullName} ({u.Email})"
                    }).ToListAsync(cancellationToken);

                var courses = await _courseRepo.Entity()
                    .AsNoTracking()
                    .Where(c => c.IsPublished)
                    .OrderBy(c => c.CourseName)
                    .Select(c => new CourseSelectOptionDto
                    {
                        CourseId = c.CourseId,
                        Title = c.CourseName
                    }).ToListAsync(cancellationToken);

                return ResponseDto<(List<UserSelectOptionDto> Users, List<CourseSelectOptionDto> Courses)>.Success((users, courses), "Dropdown options retrieved.");
            }
            catch (Exception ex)
            {
                return ResponseDto<(List<UserSelectOptionDto> Users, List<CourseSelectOptionDto> Courses)>.Failure($"Failed to load dropdown options: {ex.Message}");
            }
        }
    }
}
