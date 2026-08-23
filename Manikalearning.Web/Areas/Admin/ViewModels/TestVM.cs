using Manikalearning.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{

    public class TestVM
    {
        public Guid TestId { get; set; }

        public Guid? ExamId { get; set; }

        public Guid? PaperId { get; set; }

        public Guid? SubjectId { get; set; }


        [Required]
        [StringLength(200)]
        public string TestName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TestType TestType { get; set; }

        public int TotalQuestions { get; set; }

        public decimal TotalMarks { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsPublished { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

         
        public List<TestQuestionVM> Questions { get; set; } = new();

         
        public List<SelectList> Exams { get; set; } = new();

        public List<SelectList> Papers { get; set; } = new();

        public List<SelectList> Subjects { get; set; } = new();
         
        public string? Search { get; set; }

        public List<TestVM> Items { get; set; } = new();

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }
    }


    public class TestQuestionVM
    {
        public Guid TestQuestionId { get; set; }

        public Guid TestId { get; set; }

        public Guid QuestionId { get; set; }

        public int QuestionOrder { get; set; }

        public decimal Marks { get; set; }

        public decimal NegativeMarks { get; set; }

        public string? QuestionText { get; set; }
    }
}
