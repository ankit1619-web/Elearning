
using Manikalearning.Application.Features.Commands.Assessments.Coursesection;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class CourseSectionViewModel
    {
        public Guid SectionId { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Section Title is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters.")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Display order must be 0 or greater.")]
        public int DisplayOrder { get; set; } = 1;

        public bool IsActive { get; set; } = true;
        public bool IsEdit => SectionId != Guid.Empty;

        // Command Mapping Helpers
        public CreateCourseSectionCommand ToCreateCommand()
        {
            return new CreateCourseSectionCommand(CourseId, Title, Description, DisplayOrder);
        }

        public UpdateCourseSectionCommand ToUpdateCommand()
        {
            return new UpdateCourseSectionCommand(SectionId, Title, Description, DisplayOrder, IsActive);
        }
    }
}
