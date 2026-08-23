using Manikalearning.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Manikalearning.Web.Areas.Student.ViewModels
{
    public class PYQVM
    { 

        public Guid? ExamId { get; set; }

        public Guid? PaperId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? TopicId { get; set; }

        public int? Year { get; set; }

        public string? Session { get; set; }

         

        public List<PYQQuestionVM> Questions { get; set; } = new();

         

        public List<SelectList> Exams { get; set; } = new();

        public List<SelectList> Papers { get; set; } = new();

        public List<SelectList> Subjects { get; set; } = new();

        public List<SelectList> Topics { get; set; } = new();

        public List<int> Years { get; set; } = new();

         

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }
    }


    public class PYQQuestionVM
    {
        public Guid QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public QuestionType QuestionType { get; set; }

        public string? Explanation { get; set; }

        public DifficultyLevel? Difficulty { get; set; }

        public int? Year { get; set; }

        public string? Session { get; set; }

        public string? SubjectName { get; set; }

        public string? TopicName { get; set; }

        public List<SelectList> Options { get; set; } = new();
    }
}
