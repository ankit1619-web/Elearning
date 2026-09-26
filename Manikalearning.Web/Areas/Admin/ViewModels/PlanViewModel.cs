using Manikalearning.Application.Features.Commands.Assessments.Plans;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class PlanViewModel
    {
        public Guid PlanId { get; set; }

        [Required(ErrorMessage = "Plan name is required.")]
        [StringLength(100, ErrorMessage = "Plan name cannot exceed 100 characters.")]
        [Display(Name = "Plan Name")]
        public string PlanName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 100000, ErrorMessage = "Please enter a valid price.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duration in days is required.")]
        [Range(1, 3650, ErrorMessage = "Duration must be at least 1 day.")]
        [Display(Name = "Duration (Days)")]
        public int DurationDays { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;
         
        public CreatePlanCommand ToCreateCommand()
        {
            return new CreatePlanCommand(PlanName, Description??"", Price, DurationDays,IsActive);
        }

        public UpdatePlanCommand ToUpdateCommand()
        {
            return new UpdatePlanCommand(PlanId, PlanName, Price, DurationDays, Description, IsActive);
        }
    }
}
