using Manikalearning.Application.Interface;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Manikalearning.Infrastructure.Payment
{
    public class RazorpayOptions
    {
        public string KeyId { get; set; } = "";
        public string KeySecret { get; set; } = "";        // keep in user-secrets / environment, NOT in git
        public string WebhookSecret { get; set; } = "";    // the secret you type when you create the webhook in the Razorpay dashboard
    }

    public class RazorpayClient : IRazorpayClient
    {
        private readonly HttpClient _http;
        private readonly RazorpayOptions _opt;

        public RazorpayClient(HttpClient http, IOptions<RazorpayOptions> options)
        {
            _http = http;
            _opt = options.Value;
            _http.BaseAddress = new Uri("https://api.razorpay.com/v1/");
            var token = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_opt.KeyId}:{_opt.KeySecret}"));
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        }

        public string KeyId => _opt.KeyId;

        public async Task<RazorpayOrder> CreateOrderAsync(long amountPaise, string receipt, IDictionary<string, string> notes, CancellationToken ct)
        {
            var body = JsonSerializer.Serialize(new { amount = amountPaise, currency = "INR", receipt, notes });
            using var res = await _http.PostAsync("orders", new StringContent(body, Encoding.UTF8, "application/json"), ct);
            var json = await res.Content.ReadAsStringAsync(ct);
            if (!res.IsSuccessStatusCode) throw new InvalidOperationException($"Razorpay order failed ({(int)res.StatusCode}).");
            return Parse(json);
        }

        public async Task<RazorpayOrder?> GetOrderAsync(string orderId, CancellationToken ct)
        {
            using var res = await _http.GetAsync($"orders/{Uri.EscapeDataString(orderId)}", ct);
            if (!res.IsSuccessStatusCode) return null;
            return Parse(await res.Content.ReadAsStringAsync(ct));
        }
         
        public bool VerifySignature(string orderId, string paymentId, string signature)
        {
            using var h = new HMACSHA256(Encoding.UTF8.GetBytes(_opt.KeySecret));
            var expected = Convert.ToHexString(h.ComputeHash(Encoding.UTF8.GetBytes($"{orderId}|{paymentId}"))).ToLowerInvariant();
            var a = Encoding.UTF8.GetBytes(expected);
            var b = Encoding.UTF8.GetBytes((signature ?? "").ToLowerInvariant());
            return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);  
        }
         
        public bool VerifyWebhookSignature(byte[] rawBody, string signature)
        {
            if (string.IsNullOrEmpty(_opt.WebhookSecret) || string.IsNullOrEmpty(signature)) return false;
            using var h = new HMACSHA256(Encoding.UTF8.GetBytes(_opt.WebhookSecret));
            var expected = Encoding.UTF8.GetBytes(Convert.ToHexString(h.ComputeHash(rawBody)).ToLowerInvariant());
            var given = Encoding.UTF8.GetBytes(signature.Trim().ToLowerInvariant());
            return expected.Length == given.Length && CryptographicOperations.FixedTimeEquals(expected, given);
        }

        private static RazorpayOrder Parse(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var r = doc.RootElement;
            var o = new RazorpayOrder
            {
                Id = r.GetProperty("id").GetString() ?? "",
                Amount = r.GetProperty("amount").GetInt64(),
                Currency = r.TryGetProperty("currency", out var c) ? c.GetString() ?? "INR" : "INR",
                Status = r.TryGetProperty("status", out var s) ? s.GetString() ?? "" : ""
            }; 
            if (r.TryGetProperty("notes", out var n) && n.ValueKind == JsonValueKind.Object)
                foreach (var p in n.EnumerateObject()) o.Notes[p.Name] = p.Value.ToString();
            return o;
        }
    }
}
