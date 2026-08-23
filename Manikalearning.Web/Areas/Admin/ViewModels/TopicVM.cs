using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class TopicVM
    {
        public Guid TopicId { get; set; }

        [Required]
        public Guid SectionId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? PaperId { get; set; }

        public Guid? ExamId { get; set; }

        public string? SectionName { get; set; }

        [Required]
        [StringLength(200)]
        public string TopicName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }


        // Dropdowns
        public List<SelectList> Exams { get; set; } = new();

        public List<SelectList> Papers { get; set; } = new();

        public List<SelectList> Subjects { get; set; } = new();

        public List<SelectList> Sections { get; set; } = new();


        // List
        public string? Search { get; set; }

        public List<TopicVM> Items { get; set; } = new();

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }
    }
}
