using Manikalearning.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manikalearning.Domain.Entities
{
    [Table("Questions")]
    public class Question
    {
        [Key]
        public Guid QuestionId { get; set; } = Guid.NewGuid();

        // Taxonomy Foreign Keys
        public Guid? ExamId { get; set; }
        [ForeignKey("ExamId")]
        public Exam? Exam { get; set; }

        public Guid? PaperId { get; set; }
        [ForeignKey("PaperId")]
        public Paper? Paper { get; set; }

        public Guid? SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public Subject? Subject { get; set; }

        public Guid? SectionId { get; set; }
        [ForeignKey("SectionId")]
        public Section? Section { get; set; }

        public Guid? TopicId { get; set; }
        [ForeignKey("TopicId")]
        public Topic? Topic { get; set; }

        public Guid? SubTopicId { get; set; }
        [ForeignKey("SubTopicId")]
        public SubTopic? SubTopic { get; set; }

        public Guid? PassageId { get; set; }
        [ForeignKey("PassageId")]
        public QuestionPassage? Passage { get; set; }

        // Question Attributes
        public QuestionType QuestionType { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        public string? Explanation { get; set; }

        public string? ShortExplanation { get; set; }

        [MaxLength(50)]
        public DifficultyLevel? Difficulty { get; set; }

        public decimal Marks { get; set; } = 2.00M; // Standard UGC NET mark weight

        public decimal NegativeMarks { get; set; } = 0.00M;

        // PYQ Information
        public bool IsPYQ { get; set; } = false;

        public int? PYQYear { get; set; }

        [MaxLength(50)]
        public string? PYQSession { get; set; } // e.g., 'Morning', 'Evening', 'Shift 1'

        [MaxLength(150)]
        public string? SourceName { get; set; }

        // Publishing & Audit
        public bool IsApproved { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public Guid? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
        public ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();
        public ICollection<TestAttemptAnswer> TestAttemptAnswers { get; set; } = new List<TestAttemptAnswer>();
        public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
        public ICollection<QuestionMistake> QuestionMistakes { get; set; } = new List<QuestionMistake>();
    }

    [Table("QuestionOptions")]
    public class QuestionOption
    {
        [Key]
        public Guid OptionId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        [Required, MaxLength(10)]
        public string OptionLabel { get; set; } // A, B, C, D

        [Required]
        public string OptionText { get; set; }

        public bool IsCorrect { get; set; } = false;

        public int DisplayOrder { get; set; }
         
        public ICollection<TestAttemptAnswer> TestAttemptAnswers { get; set; }
    }

    [Table("Tests")]
    public class Test
    {
        [Key]
        public Guid TestId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string TestName { get; set; }

        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public decimal TotalMarks { get; set; }
        public bool IsPublished { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
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
