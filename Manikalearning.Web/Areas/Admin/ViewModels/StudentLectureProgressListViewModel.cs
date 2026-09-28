using Manikalearning.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class StudentLectureProgressListViewModel
    {
        public List<StudentLectureProgressDto> ProgressRecords { get; set; } = new();

        public string? SearchTerm { get; set; }
        public Guid? CourseIdFilter { get; set; }
        public bool? IsCompletedFilter { get; set; }

        public List<SelectListItem> CourseOptions { get; set; } = new();

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int CompletedCount { get; set; }
        public int InProgressCount { get; set; }
        public decimal AverageCompletionRate { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
