namespace Manikalearning.Application.DTOs
{
    public class TestQuestionDto
    {
        public Guid TestQuestionId { get; set; }

        public Guid TestId { get; set; }

        public Guid QuestionId { get; set; }

        public int QuestionOrder { get; set; }

        public decimal Marks { get; set; }

        public decimal NegativeMarks { get; set; }
    }
}
