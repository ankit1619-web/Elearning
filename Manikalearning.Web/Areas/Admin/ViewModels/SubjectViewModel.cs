using Manikalearning.Application.Features.Commands.Assessments.Subjects;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class SubjectViewModel
    {
        public Guid SubjectId { get; set; }

        [Required(ErrorMessage = "Paper selection is required.")]
        public Guid PaperId { get; set; }

        public string? PaperName { get; set; }

        [Required(ErrorMessage = "Subject Name is required.")]
        [StringLength(150, ErrorMessage = "Subject Name cannot exceed 150 characters.")]
        public string SubjectName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public CreateSubjectCommand ToCreateCommand()
        {
            return new CreateSubjectCommand(
                PaperId: this.PaperId,
                SubjectName: this.SubjectName,
                Description: this.Description,
                DisplayOrder: this.DisplayOrder,
                IsActive: this.IsActive
            );
        }

        public UpdateSubjectCommand ToUpdateCommand()
        {
            return new UpdateSubjectCommand(
                SubjectId: this.SubjectId,
                PaperId: this.PaperId,
                SubjectName: this.SubjectName,
                Description: this.Description,
                DisplayOrder: this.DisplayOrder,
                IsActive: this.IsActive
            );
        }
    }
}
