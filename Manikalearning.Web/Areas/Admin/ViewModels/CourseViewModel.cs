using System.ComponentModel.DataAnnotations;
using Manikalearning.Application.Features.Commands.Assessments.Courses;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class CourseViewModel
    {
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Course name is required.")]
        [StringLength(150, ErrorMessage = "Course name cannot exceed 150 characters.")]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 999999, ErrorMessage = "Price must be a positive value.")]
        [Display(Name = "Price (₹)")]
        public decimal Price { get; set; }

        [Display(Name = "Publish Status")]
        public bool Ispublished { get; set; } = true;
         
        public bool IsEdit => CourseId != Guid.Empty;
         
        public CreateCourseCommand ToCreateCommand()
        {
            return new CreateCourseCommand(CourseName, Description ??"", Price);
        }

        public UpdateCourseCommand ToUpdateCommand()
        {
            return new UpdateCourseCommand(CourseId, CourseName, Description ?? "", Price, Ispublished);
        }
    }
}
