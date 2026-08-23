using Manikalearning.Domain.Entities;
using Manikalearning.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class QuestionVM
    { 

        public Guid QuestionId { get; set; }

        public Guid? ExamId { get; set; }

        public Guid? PaperId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SectionId { get; set; }

        public Guid? TopicId { get; set; }

        public Guid? SubTopicId { get; set; }

        public Guid? PassageId { get; set; }

         

        [Required]
        public QuestionType QuestionType { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        public string? Explanation { get; set; }

        public string? ShortExplanation { get; set; }

        public DifficultyLevel? Difficulty { get; set; }

        public decimal Marks { get; set; } = 1;

        public decimal NegativeMarks { get; set; }

         

        public bool IsPYQ { get; set; }

        public int? PYQYear { get; set; }

        public string? PYQSession { get; set; }

        public string? SourceName { get; set; }
         

        public bool IsApproved { get; set; }

        public bool IsActive { get; set; } = true;
         

        public List<SelectList> Options { get; set; } = new();

         

        public List<SelectList> Exams { get; set; } = new();

        public List<SelectList> Papers { get; set; } = new();

        public List<SelectList> Subjects { get; set; } = new();

        public List<SelectList> Sections { get; set; } = new();
        public List<SelectList> Topics { get; set; } = new();

        public List<SelectList> SubTopics { get; set; } = new();
         

        public string? Search { get; set; }

        public List<QuestionVM> Items { get; set; } = new();

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }
    }
    public class QuestionOptionVM
    {
        public Guid OptionId { get; set; }

        public Guid QuestionId { get; set; }

        [Required]
        public string OptionLabel { get; set; } = string.Empty;

        [Required]
        public string OptionText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public int DisplayOrder { get; set; }
    }
}
