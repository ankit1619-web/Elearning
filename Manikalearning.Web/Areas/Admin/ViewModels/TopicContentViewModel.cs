using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class TopicContentViewModel
    {
        public Guid Id { get; set; } 
        public Guid TopicId { get; set; } 
        public string TopicName { get; set; } = string.Empty;

        [Display(Name = "Reading Content ")]
        public string? ContentEn { get; set; }
    }
}
