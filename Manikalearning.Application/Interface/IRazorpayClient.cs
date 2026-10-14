namespace Manikalearning.Application.Interface
{
    public class RazorpayOrder
    {
        public string Id { get; set; } = "";
        public long Amount { get; set; }                     
        public string Currency { get; set; } = "INR";
        public string Status { get; set; } = "";               // created | attempted | paid
        public Dictionary<string, string> Notes { get; set; } = new();
    }

    public interface IRazorpayClient
    {
        string KeyId { get; }
        Task<RazorpayOrder> CreateOrderAsync(long amountPaise, string receipt, IDictionary<string, string> notes, CancellationToken ct);
        Task<RazorpayOrder?> GetOrderAsync(string orderId, CancellationToken ct);
        bool VerifySignature(string orderId, string paymentId, string signature);
        bool VerifyWebhookSignature(byte[] rawBody, string signature); 
    }
}
