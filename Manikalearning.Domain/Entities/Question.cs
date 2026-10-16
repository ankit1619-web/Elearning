using Manikalearning.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manikalearning.Domain.Entities
{
    [Table("Questions")]
    public class Question
    {
        public Guid QuestionId { get; set; }

        public Guid? ExamId { get; set; }

        public Guid? PaperId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SectionId { get; set; }

        public Guid? TopicId { get; set; }

        public Guid? SubTopicId { get; set; }

        public Guid? PassageId { get; set; }

        public Guid? CreatedBy { get; set; }  

        public string QuestionText { get; set; } = string.Empty;
        public string? QuestionTextHindi { get; set; } = string.Empty;

        public QuestionType QuestionType { get; set; }

        public DifficultyLevel Difficulty { get; set; }

        public string? Explanation { get; set; }
        public string? ExplanationHindi { get; set; }

        public string? ShortExplanation { get; set; }
        public string? ShortExplanationHindi { get; set; }

        public decimal Marks { get; set; }

        public decimal NegativeMarks { get; set; }

        public bool IsPYQ { get; set; }

        public int? PYQYear { get; set; }

        public string? PYQSession { get; set; }

        public string? SourceName { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("PassageId")]
        public QuestionPassage? Passage { get; set; }
        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }

        public virtual Exam? Exam { get; set; } 

        public virtual Paper? Paper { get; set; }

        public virtual Subject? Subject { get; set; }

        public virtual Section? Section { get; set; }

        public virtual Topic? Topic { get; set; }

        public virtual SubTopic? SubTopic { get; set; }
           
        public ICollection<QuestionOption> Options { get; set; }
            = new List<QuestionOption>();
          
        public bool IsApproved { get; set; } = true;
        public bool IsActive { get; set; } = true;
         public DateTime? UpdatedAt { get; set; } 
    }

    [Table("QuestionOptions")]
    public class QuestionOption
    {
        [Key]
        public Guid OptionId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid QuestionId { get; set; }

        [ForeignKey(nameof(QuestionId))]
        public virtual Question Question { get; set; } = null!;

        [Required, MaxLength(10)]
        public string OptionLabel { get; set; } = string.Empty; // A, B, C, D

        [Required]
        public string OptionText { get; set; } = string.Empty;
        public string? OptionTextHindi { get; set; }

        public bool IsCorrect { get; set; } = false;
        public int DisplayOrder { get; set; } = 1;
         
    }

    [Table("Tests")]
    public class Test
    {
        [Key]
        public Guid TestId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string TestName { get; set; }

        public Guid? ExamId { get; set; }
        public Guid? PaperId { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? TopicId { get; set; }

        [MaxLength(50)]
        public string TestType { get; set; }
        public int TotalQuestions { get; set; }
        public int? DurationMinutes { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal NegativeMarking { get; set; }
        public bool IsRandom { get; set; } = false;
        public bool ShowResultImmediately { get; set; } = true;
        public bool IsPublished { get; set; } = false;

        [MaxLength(450)]
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
         
        public ICollection<TestQuestion> TestQuestions { get; set; }
        public ICollection<TestAttempt> TestAttempts { get; set; }
    }


    [Table("TestQuestions")]
    public class TestQuestion
    {
        [Key]
        public Guid TestQuestionId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TestId { get; set; }
        [ForeignKey("TestId")]
        public Test Test { get; set; }

        [Required]
        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        public int QuestionOrder { get; set; }
        public decimal Marks { get; set; } = 2.00M;
        public decimal NegativeMarks { get; set; } = 0.50M;
    }

    [Table("QuestionPassages")]
    public class QuestionPassage
    {
        [Key]
        public Guid PassageId { get; set; } = Guid.NewGuid(); 
        public string? Title { get; set; }

        public string PassageText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Question> Questions { get; set; }
            = new List<Question>();
    }

   
}
