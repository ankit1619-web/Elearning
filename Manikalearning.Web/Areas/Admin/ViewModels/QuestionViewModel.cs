using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Questions;
using Manikalearning.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class QuestionViewModel
    {
        public Guid QuestionId { get; set; }
         
        public Guid? SubjectId { get; set; }
         
        public Guid? SectionId { get; set; }
         
        public Guid? TopicId { get; set; } 
        public Guid? SubTopicId { get; set; }
        public Guid? ExamId { get; set; }
        public Guid? PaperId { get; set; }
        public Guid? PassageId { get; set; }

        public QuestionType QuestionType { get; set; }
        public DifficultyLevel Difficulty { get; set; } 

        [Required(ErrorMessage = "Question text is required.")]
        public string QuestionText { get; set; } = string.Empty; 
        public string? QuestionTextHindi { get; set; }
        public string? Explanation { get; set; }
        public string? ExplanationHindi { get; set; }
        public string? ShortExplanation { get; set; }  
        public string? ShortExplanationHindi { get; set; }
         
        [Range(0, 100)]
        public decimal Marks { get; set; } = 1.0m;

        [Range(0, 100)]
        public decimal NegativeMarks { get; set; } = 0.25m;

        public bool IsPYQ { get; set; }
        public int? PYQYear { get; set; }
        public string? PYQSession { get; set; }
        public string? SourceName { get; set; }

        public bool IsApproved { get; set; } = true;
        public bool IsActive { get; set; } = true;

        // Dynamic Options
        public List<QuestionOptionViewModel> Options { get; set; } = new List<QuestionOptionViewModel>();

        // Dropdown Lists
        public List<SelectListItem> SubjectsList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SectionsList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TopicsList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SubTopicsList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PassagesList { get; set; } = new List<SelectListItem>();

        public CreateQuestionCommand ToCreateCommand() => new()
        {
            SubjectId = SubjectId,
            SectionId = SectionId,
            TopicId = TopicId,
            SubTopicId = SubTopicId,
            ExamId = ExamId,
            PaperId = PaperId,
            PassageId = PassageId,
            QuestionType = QuestionType,
            QuestionText = QuestionText,
            QuestionTextHindi = QuestionTextHindi,
            Explanation = Explanation,
            ExplanationHindi = ExplanationHindi,
            ShortExplanation = ShortExplanation,
            ShortExplanationHindi = ShortExplanationHindi,
            Difficulty = Difficulty,
            Marks = Marks,
            NegativeMarks = NegativeMarks,
            IsPYQ = IsPYQ,
            PYQYear = PYQYear,
            PYQSession = PYQSession,
            SourceName = SourceName,
            IsApproved = IsApproved,
            IsActive = IsActive,
            Options = Options.Select(o => new QuestionOptionDto
            {
                OptionId = o.OptionId,
                OptionLabel = o.OptionLabel,
                OptionText = o.OptionText,
                OptionTextHindi = o.OptionTextHindi,
                IsCorrect = o.IsCorrect,
                DisplayOrder = o.DisplayOrder
            }).ToList() 
        };

        public UpdateQuestionCommand ToUpdateCommand() => new()
        {
            QuestionId = QuestionId,
            SubjectId = SubjectId,
            SectionId = SectionId,
            TopicId = TopicId,
            SubTopicId = SubTopicId,
            ExamId = ExamId,
            PaperId = PaperId,
            PassageId = PassageId,
            QuestionType = QuestionType,
            QuestionText = QuestionText,
            QuestionTextHindi = QuestionTextHindi,
            Explanation = Explanation,
            ExplanationHindi = ExplanationHindi,
            ShortExplanation = ShortExplanation,
            ShortExplanationHindi = ShortExplanationHindi,
            Difficulty = Difficulty,
            Marks = Marks,
            NegativeMarks = NegativeMarks,
            IsPYQ = IsPYQ,
            PYQYear = PYQYear,
            PYQSession = PYQSession,
            SourceName = SourceName,
            IsApproved = IsApproved,
            IsActive = IsActive,
            Options = Options.Select(o => new QuestionOptionDto
            {
                OptionId = o.OptionId,
                OptionLabel = o.OptionLabel,
                OptionText = o.OptionText,
                OptionTextHindi = o.OptionTextHindi,
                IsCorrect = o.IsCorrect,
                DisplayOrder = o.DisplayOrder
            }).ToList()
        };
    }
    public class QuestionOptionViewModel
    {
        public Guid OptionId { get; set; }
        public string OptionLabel { get; set; } = "A";

        [Required(ErrorMessage = "Option text is required.")]
        public string OptionText { get; set; } = string.Empty;
        public string? OptionTextHindi { get; set; }
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; } = 1;
    }
}
