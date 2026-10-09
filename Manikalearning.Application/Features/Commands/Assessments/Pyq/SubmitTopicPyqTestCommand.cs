using Manikalearning.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Commands.Assessments.Pyq
{
    public class UserAnswerDto
    {
        public Guid QuestionId { get; set; }
        public Guid? SelectedOptionId { get; set; } // Option ki Foreign Key ID
        public bool IsAnswered { get; set; }
        public bool IsMarkedForReview { get; set; }
    }

    public record SubmitTopicPyqTestCommand(
        Guid LectureId,
        Guid UserId,
        Guid? TestId, // Mandatory FK in DB
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
