using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudnetSubscription;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class StudentSubscriptionListViewModel
    {
        public List<StudentSubscriptionDto> Subscriptions { get; set; } = new();

        public string? SearchTerm { get; set; }
        public string StatusFilter { get; set; } = "All"; 
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int ExpiredCount { get; set; } 
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }

    public class ExtendSubscriptionViewModel
    {
        [Required]
        public Guid SubscriptionId { get; set; }

        public string StudentName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public DateTime CurrentEndDate { get; set; }

        [Required(ErrorMessage = "Please enter days to extend.")]
        [Range(1, 365, ErrorMessage = "Days must be between 1 and 365.")]
        [Display(Name = "Extend Days")]
        public int AdditionalDays { get; set; } = 30;

        [StringLength(250, ErrorMessage = "Reason cannot exceed 250 characters.")]
        public string? Reason { get; set; }

        public ExtendSubscriptionCommand ToCommand()
        {
            return new ExtendSubscriptionCommand(SubscriptionId, AdditionalDays, Reason);
        }
    }
}
