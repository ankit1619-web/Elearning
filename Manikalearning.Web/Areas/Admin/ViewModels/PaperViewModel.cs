using Manikalearning.Application.Features.Commands.Assessments.Papers;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class PaperViewModel
    {
        public Guid PaperId { get; set; }

        [Required(ErrorMessage = "Exam is required.")]
        public Guid ExamId { get; set; }

        public string? ExamName { get; set; }

        [Required(ErrorMessage = "Paper Name is required.")]
        [StringLength(150, ErrorMessage = "Paper Name cannot exceed 150 characters.")]
        public string PaperName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Paper Code is required.")]
        [StringLength(50, ErrorMessage = "Paper Code cannot exceed 50 characters.")]
        public string PaperCode { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public CreatePaperCommand ToCreateCommand()
        {
            return new CreatePaperCommand(
                ExamId: this.ExamId,
                PaperName: this.PaperName,
                PaperCode: this.PaperCode,
                Description: this.Description,
                DisplayOrder: this.DisplayOrder,
                IsActive: this.IsActive
            );
        }

        public UpdatePaperCommand ToUpdateCommand()
        {
            return new UpdatePaperCommand(
                PaperId: this.PaperId,
                ExamId: this.ExamId,
                PaperName: this.PaperName,
                PaperCode: this.PaperCode,
                Description: this.Description,
                DisplayOrder: this.DisplayOrder,
                IsActive: this.IsActive
            );
        }
    }
}
