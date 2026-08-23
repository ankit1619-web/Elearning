using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class PaperVM
    {
        public Guid PaperId { get; set; }

        [Required]
        public Guid ExamId { get; set; }

        public string? ExamName { get; set; }

        [Required]
        [StringLength(150)]
        public string PaperName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? PaperCode { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }

        // Dropdown
        public List<SelectList> Exams { get; set; } = new();


        // List
        public string? Search { get; set; }

        public List<PaperVM> Items { get; set; } = new();


        // Pagination
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }
    }
}
