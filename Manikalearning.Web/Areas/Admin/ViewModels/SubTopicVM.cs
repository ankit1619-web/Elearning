using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class SubTopicVM
    {
        public Guid SubTopicId { get; set; }

        [Required]
        public Guid TopicId { get; set; }

        public Guid? SectionId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? PaperId { get; set; }

        public Guid? ExamId { get; set; }

        public string? TopicName { get; set; }

        [Required]
        [StringLength(200)]
        public string SubTopicName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;


        // Dropdowns
        public List<SelectList> Exams { get; set; } = new();

        public List<SelectList> Papers { get; set; } = new();

        public List<SelectList> Subjects { get; set; } = new();

        public List<SelectList> Sections { get; set; } = new();
        public List<SelectList> Topics { get; set; } = new();


        // List
        public string? Search { get; set; }

        public List<SubTopicVM> Items { get; set; } = new();

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }
    }
}
