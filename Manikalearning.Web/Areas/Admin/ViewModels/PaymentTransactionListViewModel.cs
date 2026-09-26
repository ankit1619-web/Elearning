using Manikalearning.Application.DTOs;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class PaymentTransactionListViewModel
    {
        public List<PaymentTransactionDto> Transactions { get; set; } = new();

        public string? SearchTerm { get; set; }
        public string StatusFilter { get; set; } = "All";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public decimal TotalRevenue { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
