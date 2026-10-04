using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.DTOs
{
    public class PaymentTransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;

        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public string RazorpaySignature { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Currency { get; set; } = "INR";

        public string PaymentStatus { get; set; } = "Pending"; // Success, Failed, Pending, Refunded
        public string PaymentMethod { get; set; } = "N/A";     // UPI, Card, NetBanking, Wallet
        public string ItemType { get; set; } = "Plan";         // Plan / Course
        public string ItemName { get; set; } = string.Empty;
        public string? AppliedCouponCode { get; set; }

        public DateTime? PaidAt { get; set; }
    }
    public class PaymentTransactionPagedResultDto
    {
        public List<PaymentTransactionDto> Transactions { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
