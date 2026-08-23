using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manikalearning.Domain.Entities
{
    [Table("Questions")]
    public class Question
    {
        [Key]
        public Guid QuestionId { get; set; } = Guid.NewGuid();

        public Guid? ExamId { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? TopicId { get; set; }

        [Required]
        public string QuestionText { get; set; }

        public string Explanation { get; set; }

        [MaxLength(50)]
        public string Difficulty { get; set; } // Easy, Medium, Hard

        public bool IsPYQ { get; set; } = false;
        public int? PYQYear { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<QuestionOption> QuestionOptions { get; set; }
        public ICollection<TestQuestion> TestQuestions { get; set; }
        public ICollection<TestAttemptAnswer> TestAttemptAnswers { get; set; }
        public ICollection<Bookmark> Bookmarks { get; set; }
        public ICollection<QuestionMistake> QuestionMistakes { get; set; }
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

        // Navigation Properties
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
}
