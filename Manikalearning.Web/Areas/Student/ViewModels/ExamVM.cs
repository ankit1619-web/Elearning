namespace Manikalearning.Web.Areas.Student.ViewModels
{
    public class ExamVM
    {
        public Guid ExamId { get; set; }

        public string ExamName { get; set; } = string.Empty;

        public string? ExamCode { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int TotalQuestions { get; set; }

        public int TotalPYQs { get; set; }

        public int TotalTests { get; set; }

        public bool IsActive { get; set; }

        // List
        public List<ExamVM> Items { get; set; } = new();

        public string? Search { get; set; }
    }
}
