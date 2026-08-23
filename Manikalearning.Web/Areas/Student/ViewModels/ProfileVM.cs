using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Student.ViewModels
{
    public class ProfileVM
    {
        public Guid UserId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        public string? Mobile { get; set; }

        public string? ProfileImage { get; set; }


        // Statistics

        public int TestsAttempted { get; set; }

        public int PYQsAttempted { get; set; }

        public int QuestionsSolved { get; set; }

        public decimal AverageScore { get; set; }

        public decimal BestScore { get; set; }
    }
}
