using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class ExamVM
    {
        // Common
        public Guid ExamId { get; set; }

        [Required]
        [StringLength(150)]
        public string ExamName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ExamCode { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; }


        // List
        public string? Search { get; set; }

        public List<ExamVM> Items { get; set; } = new(); 

        // Pagination
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }
    }
}
