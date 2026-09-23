using Manikalearning.Application.Features.Commands.Assessments.Topics;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class TopicViewModel
    {
        public Guid TopicId { get; set; }

        [Required(ErrorMessage = "Section is required.")]
        public Guid SectionId { get; set; }

        [Required(ErrorMessage = "Topic name is required.")]
        [StringLength(150, ErrorMessage = "Topic name cannot exceed 150 characters.")]
        public string TopicName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public List<SelectListItem> SectionsList { get; set; } = new();

        public CreateTopicCommand ToCreateCommand() =>
            new CreateTopicCommand(SectionId, TopicName, Description,DisplayOrder, IsActive);

        public UpdateTopicCommand ToUpdateCommand() =>
            new UpdateTopicCommand(TopicId, SectionId, TopicName, Description, DisplayOrder, IsActive);
    }
}
