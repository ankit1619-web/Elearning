using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Courses;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, ResponseDto<List<CourseDto>>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly ILogger<GetCoursesQueryHandler> _logger;

    public GetCoursesQueryHandler(
        IRepository<Course> courseRepository,
        ILogger<GetCoursesQueryHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<ResponseDto<List<CourseDto>>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _courseRepository.Entity().AsNoTracking();
             
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();

                query = query.Where(c => c.CourseName.ToLower().Contains(search)
                                      || (c.Description != null && c.Description.ToLower().Contains(search)));
            }
             
            var courses = await query
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CourseDto
                {
                    Id = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    Price = c.Price,
                    Ispublished = c.IsPublished
                })
                .ToListAsync(cancellationToken);

            return ResponseDto<List<CourseDto>>.Success(courses, "Courses fetched successfully.");
        }
        catch (OperationCanceledException)
        { 
            _logger.LogWarning("GetCourses query was canceled by the client.");
            throw;  
        }
        catch (Exception ex)
        { 
            _logger.LogError(ex, "An error occurred while fetching courses. SearchTerm: {SearchTerm}", request.SearchTerm);
             
            return ResponseDto<List<CourseDto>>.Failure("An unexpected error occurred while fetching courses. Please try again later.");
        }
    }
}