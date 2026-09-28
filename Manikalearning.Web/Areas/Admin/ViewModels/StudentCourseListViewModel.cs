using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudentCourses;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class StudentCourseListViewModel
    {
        public List<StudentCourseDto> Enrollments { get; set; } = new();

        public string? SearchTerm { get; set; }
        public Guid? CourseIdFilter { get; set; }
        public bool? IsActiveFilter { get; set; }

        public List<SelectListItem> CourseFilterOptions { get; set; } = new();

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }

    public class EnrollStudentViewModel
    {
        [Required(ErrorMessage = "Please select a student.")]
        [Display(Name = "Student")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Please select a course.")]
        [Display(Name = "Course")]
        public Guid CourseId { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; } = DateTime.Now;

        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> UserOptions { get; set; } = new();
        public List<SelectListItem> CourseOptions { get; set; } = new();

        public EnrollStudentCommand ToCommand()
        {
            return new EnrollStudentCommand(UserId, CourseId, StartDate, ExpiryDate, IsActive);
        }
    }
}
