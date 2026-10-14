using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.DTOs
{
    public class DoubtDto
    {
        public Guid DoubtId { get; set; }
        public string QuestionText { get; set; } = "";
        public string Status { get; set; } = "";
        public string? CourseName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public static class DoubtStatus
    {
        public const string Open = "Open";
        public const string Answered = "Answered";
        public const string Closed = "Closed";
    }
     

    public class DoubtListDto
    {
        public List<DoubtDto> Items { get; set; } = new();
        public string? Status { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    }
}
