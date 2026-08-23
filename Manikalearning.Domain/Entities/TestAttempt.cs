using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manikalearning.Domain.Entities
{
    [Table("TestAttempts")]
    public class TestAttempt
    {
        [Key]
        public Guid AttemptId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TestId { get; set; }
        [ForeignKey("TestId")]
        public Test Test { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Started";

        public int TotalQuestions { get; set; } = 0;
        public int AttemptedQuestions { get; set; } = 0;
        public int CorrectAnswers { get; set; } = 0;
        public int WrongAnswers { get; set; } = 0;
        public int SkippedQuestions { get; set; } = 0;

        public decimal TotalMarks { get; set; } = 0;
        public decimal Score { get; set; } = 0;
        public decimal Accuracy { get; set; } = 0;
        public int TimeTakenSeconds { get; set; } = 0;

        // Navigation Properties
        public ICollection<TestAttemptAnswer> TestAttemptAnswers { get; set; }
    }


    [Table("TestAttemptAnswers")]
    public class TestAttemptAnswer
    {
        [Key]
        public Guid AttemptAnswerId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid AttemptId { get; set; }
        [ForeignKey("AttemptId")]
        public TestAttempt TestAttempt { get; set; }

        [Required]
        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        public Guid? SelectedOptionId { get; set; }
        [ForeignKey("SelectedOptionId")]
        public QuestionOption SelectedOption { get; set; }

        public bool IsAnswered { get; set; } = false;
        public bool IsMarkedForReview { get; set; } = false;
        public bool? IsCorrect { get; set; }
        public decimal MarksObtained { get; set; } = 0;
        public DateTime? AnsweredAt { get; set; }
    }

    [Table("Bookmarks")]
    public class Bookmark
    {
        [Key]
        public Guid BookmarkId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("QuestionMistakes")]
    public class QuestionMistake
    {
        [Key]
        public Guid MistakeId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        public int WrongCount { get; set; } = 1;
        public DateTime LastWrongAt { get; set; } = DateTime.UtcNow;
        public bool IsResolved { get; set; } = false;
    }
}
