using Manikalearning.Application.Features.Commands.Assessments.Subtopics;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class SubTopicViewModel
    {
        public Guid SubTopicId { get; set; }

        [Required(ErrorMessage = "Please select a topic.")]
        [Display(Name = "Topic")]
        public Guid TopicId { get; set; }

        [Required(ErrorMessage = "SubTopic name is required.")]
        [StringLength(200, ErrorMessage = "SubTopic name cannot exceed 200 characters.")]
        [Display(Name = "SubTopic Name")]
        public string SubTopicName { get; set; } = string.Empty;       


        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> TopicsList { get; set; } = new List<SelectListItem>();

        public CreateSubTopicCommand ToCreateCommand()
        {
            return new CreateSubTopicCommand
            {
                TopicId = TopicId,
                Name = SubTopicName,
                IsActive = IsActive
            };
        }

        public UpdateSubTopicCommand ToUpdateCommand()
        {
            return new UpdateSubTopicCommand
            {
                Id = SubTopicId,
                TopicId = TopicId,
                Name = SubTopicName,
                IsActive = IsActive
            };
        }
    }
}
