using Manikalearning.Application.Features.Commands.Assessments.Sections;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class SectionViewModel
    {
        public Guid SectionId { get; set; }

        [Required(ErrorMessage = "Subject selection is required.")]
        public Guid SubjectId { get; set; }

        public string? SubjectName { get; set; }

        [Required(ErrorMessage = "Section Name is required.")]
        [StringLength(150, ErrorMessage = "Section Name cannot exceed 150 characters.")]
        public string SectionName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; } = 1;

        public List<SelectListItem> SubjectsList { get; set; } = new List<SelectListItem>();

        public CreateSectionCommand ToCreateCommand()
        {
            return new CreateSectionCommand(
                SubjectId: this.SubjectId,
                SectionName: this.SectionName,
                IsActive: this.IsActive,
                DisplayOrder: this.DisplayOrder
            );
        }

        public UpdateSectionCommand ToUpdateCommand()
        {
            return new UpdateSectionCommand(
                SectionId: this.SectionId,
                SubjectId: this.SubjectId,
                SectionName: this.SectionName,
                IsActive: this.IsActive,
                DisplayOrder: this.DisplayOrder
            );
        }
    }
}
