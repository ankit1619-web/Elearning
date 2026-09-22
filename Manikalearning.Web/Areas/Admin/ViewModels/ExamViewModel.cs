using Manikalearning.Application.Features.Commands.Assessments.Exams;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class ExamViewModel
    {
        public Guid ExamId { get; set; }

        [Required(ErrorMessage = "Exam Name is required.")]
        [StringLength(150, ErrorMessage = "Exam Name cannot exceed 150 characters.")]
        public string ExamName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Exam Code is required.")]
        [StringLength(50, ErrorMessage = "Exam Code cannot exceed 50 characters.")]
        public string ExamCode { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public CreateExamCommand ToCreateCommand()
        {
            return new CreateExamCommand
            {
                ExamName = this.ExamName,
                ExamCode = this.ExamCode,
                Description = this.Description,
                DisplayOrder = this.DisplayOrder,
                IsActive = this.IsActive
            };
        }

        public UpdateExamCommand ToUpdateCommand()
        {
            return new UpdateExamCommand(
                ExamId: this.ExamId,
                ExamName: this.ExamName,
                ExamCode: this.ExamCode,
                Description: this.Description, 
                IsActive: this.IsActive,
                DisplayOrder: this.DisplayOrder
            );
        }
    }
}
