using Manikalearning.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Manikalearning.Web.Areas.Student.ViewModels
{
    public class PracticeVM
    {
        public Guid? ExamId { get; set; }

        public Guid? PaperId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? TopicId { get; set; }

        public Guid? SubTopicId { get; set; }

        public DifficultyLevel? Difficulty { get; set; }

        public int QuestionCount { get; set; } = 10;

        public bool IncludePYQ { get; set; }

         
        public List<PracticeQuestionVM> Questions { get; set; } = new();

         
        public List<SelectList> Exams { get; set; } = new();

        public List<SelectList> Papers { get; set; } = new();

        public List<SelectList> Subjects { get; set; } = new();

        public List<SelectList> Topics { get; set; } = new();

        public List<SelectList> SubTopics { get; set; } = new();
    }


    public class PracticeQuestionVM
    {
        public Guid QuestionId { get; set; }

        public int QuestionNumber { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public DifficultyLevel? Difficulty { get; set; }

        public decimal Marks { get; set; }

        public List<StudentOptionVM> Options { get; set; } = new();

        public Guid? SelectedOptionId { get; set; }

        public bool IsMarkedForReview { get; set; }
    }
}
