using Manikalearning.Domain.Entities;

namespace Manikalearning.Application.DTOs
{
    public class PyqTestState
    {
        public Guid SectionId { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public int DurationSeconds { get; set; }
        public List<PyqAnswerState> Answers { get; set; } = new();
    }

    public class PyqAnswerState
    {
        public Guid QuestionId { get; set; }
        public int? Selected { get; set; }
        public int Status { get; set; }          
        public bool Bookmarked { get; set; }
    }

    public class PyqQuestionVm
    {
        public string Controller { get; set; } = "PYQPractice";    
        public string ExitUrl { get; set; } = "";           
        public bool CanBookmark { get; set; } = true;          
        public string? BannerLink { get; set; }             
        public string? BannerLinkText { get; set; }
        public TopicPyqDto Question { get; set; } = default!;
        public int Index { get; set; }
        public int Total { get; set; }
        public int? Selected { get; set; }
        public bool Bookmarked { get; set; }
        public List<int> Statuses { get; set; } = new();
        public int SecondsLeft { get; set; }
        public bool IsPreviewOnly { get; set; }
        public string? BannerMessage { get; set; }
    }

    public class PyqReviewVm
    {
        public string Controller { get; set; } = "PYQPractice";
        public int Total { get; set; }
        public int Answered { get; set; }
        public int NotAnswered { get; set; }
        public int Marked { get; set; }
        public int SecondsLeft { get; set; }
    }

    public class PyqResultVm
    {
        public string ExitUrl { get; set; } = "";
        public string ExitLabel { get; set; } = "Back to PYQ Bank";
        public bool Saved { get; set; } = true;               
        public string? SaveNotice { get; set; }
        public string? SaveLink { get; set; }
        public string? SaveLinkText { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int SkippedQuestions { get; set; }
        public decimal Score { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal Accuracy { get; set; }
        public bool AutoSubmitted { get; set; }
    }

    public static class PyqYearRules
    {
        public const int MaxQuestionsPerTest = 100;   
        public static IQueryable<Question> Base(IQueryable<Question> q) =>
            q.Where(x => x.IsPYQ && x.IsActive && x.IsApproved && x.PYQYear != null
                         && x.PassageId == null);              
    } 
    public class ExamOptionDto { public Guid ExamId { get; set; } public string ExamName { get; set; } = ""; }

    public class PaperOptionDto
    {
        public Guid PaperId { get; set; }
        public Guid ExamId { get; set; }
        public string ExamName { get; set; } = "";
        public string PaperName { get; set; } = "";
    }

    public class PyqSessionDto { public string Session { get; set; } = ""; public int Count { get; set; } }

    public class PyqYearRowDto
    {
        public int Year { get; set; }
        public int Total { get; set; }
        public bool IsCut => Total > PyqYearRules.MaxQuestionsPerTest;
        public List<PyqSessionDto> Sessions { get; set; } = new();
    }

    public class PyqYearsDto
    {
        public Guid? ExamId { get; set; }
        public Guid? PaperId { get; set; }
        public List<ExamOptionDto> Exams { get; set; } = new();
        public List<PaperOptionDto> Papers { get; set; } = new();
        public List<PyqYearRowDto> Years { get; set; } = new();
    }
}
