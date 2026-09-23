using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Domain.Enums
{
    public enum QuestionType
    {
        [Display(Name = "Single Choice")]
        SingleChoice = 1,

        [Display(Name = "Multiple Choice")]
        MultipleChoice = 2,

        [Display(Name = "True / False")]
        TrueFalse = 3,

        [Display(Name = "Assertion & Reason")]
        AssertionReason = 4,

        [Display(Name = "Match Following")]
        MatchFollowing = 5,

        [Display(Name = "Statement Based")]
        StatementBased = 6,

        [Display(Name = "Chronology")]
        Chronology = 7,

        [Display(Name = "Comprehension")]
        Comprehension = 8,

        [Display(Name = "Fill in Blank")]
        FillBlank = 9,

        [Display(Name = "Numerical")]
        Numerical = 10
    }
}
