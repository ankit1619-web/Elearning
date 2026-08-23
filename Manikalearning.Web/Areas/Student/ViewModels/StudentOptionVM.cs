namespace Manikalearning.Web.Areas.Student.ViewModels
{
    public class StudentOptionVM
    {
        public Guid OptionId { get; set; }

        public string OptionLabel { get; set; } = string.Empty;

        public string OptionText { get; set; } = string.Empty;
         
        public bool IsCorrect { get; set; }
    }
}
