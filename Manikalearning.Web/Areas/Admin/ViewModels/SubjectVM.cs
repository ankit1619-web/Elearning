using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class SubjectVM
    {
        public Guid SubjectId { get; set; }

        [Required]
        public Guid PaperId { get; set; }

        public Guid? ExamId { get; set; }

        public string? PaperName { get; set; }

        [Required]
        [StringLength(150)]
        public string SubjectName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }


        // Dropdowns
        public List<SelectList> Exams { get; set; } = new();

        public List<SelectList> Papers { get; set; } = new();


        // List
        public string? Search { get; set; }

        public List<SubjectVM> Items { get; set; } = new();

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }
    }
}
