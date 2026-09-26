using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Plans;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class PlanCourseMappingViewModel
    {
        [Required]
        public Guid PlanId { get; set; }

        public string PlanName { get; set; } = string.Empty;

        public List<Guid> SelectedCourseIds { get; set; } = new();

        public List<CourseOptionDto> AvailableCourses { get; set; } = new();

        // Command Conversion Helper
        public SavePlanCourseMappingCommand ToSaveCommand()
        {
            return new SavePlanCourseMappingCommand(PlanId, SelectedCourseIds ?? new List<Guid>());
        }
    }
}
