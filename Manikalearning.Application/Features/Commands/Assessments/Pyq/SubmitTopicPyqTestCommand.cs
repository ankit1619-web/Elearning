using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Pyq
{
    
    public class UserAnswerDto
    {
        public Guid QuestionId { get; set; }
        public Guid? SelectedOptionId { get; set; }
        public int? SelectedOptionNumber { get; set; } // 1, 2, 3, ya 4
        public bool IsAnswered { get; set; }
        public bool IsMarkedForReview { get; set; }
    }
    public record SubmitTopicPyqTestCommand(
        Guid LectureId,
        Guid UserId,
        Guid? TestId, 
        int TimeTakenSeconds,
        List<UserAnswerDto> Answers
    ) : IRequest<ResponseDto<TopicTestResultDto>>;

    public class TopicTestResultDto
    {
        public int TotalQuestions { get; set; }
        public int AttemptedQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int SkippedQuestions { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal Score { get; set; }
        public decimal Accuracy { get; set; }
    }
}
