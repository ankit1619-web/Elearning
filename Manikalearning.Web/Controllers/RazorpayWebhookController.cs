using FluentValidation;
using Manikalearning.Application.Features.Handlers.Students;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Manikalearning.Web.Controllers
{
    [ApiController]
    [AllowAnonymous]                        
    [IgnoreAntiforgeryToken]
    [Route("api/razorpay")]
    public class RazorpayWebhookController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRazorpayClient _razorpay;
        private readonly ILogger<RazorpayWebhookController> _log;

        public RazorpayWebhookController(IMediator mediator, IRazorpayClient razorpay, ILogger<RazorpayWebhookController> log)
        { _mediator = mediator; _razorpay = razorpay; _log = log; }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook(CancellationToken ct)
        { 
            using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms, ct);
            var raw = ms.ToArray();
            if (raw.Length == 0 || raw.Length > 1_000_000) return BadRequest();
             
            var signature = Request.Headers["X-Razorpay-Signature"].ToString();
            if (!_razorpay.VerifyWebhookSignature(raw, signature))
            {
                _log.LogWarning("Razorpay webhook rejected: bad signature.");
                return Unauthorized();
            }
             
            string? evt, orderId = null, paymentId = null;
            try
            {
                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;
                evt = root.TryGetProperty("event", out var e) ? e.GetString() : null;

                if (root.TryGetProperty("payload", out var payload) &&
                    payload.TryGetProperty("payment", out var pay) &&
                    pay.TryGetProperty("entity", out var ent))
                {
                    paymentId = ent.TryGetProperty("id", out var pid) ? pid.GetString() : null;
                    orderId = ent.TryGetProperty("order_id", out var oid) ? oid.GetString() : null;
                }
            }
            catch (JsonException) { return BadRequest(); }
             
            if (evt is not ("payment.captured" or "order.paid") || string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(paymentId))
                return Ok();

            try
            {
                var r = await _mediator.Send(new ConfirmPaymentCommand(orderId, paymentId, "", FromWebhook: true), ct);
                if (!r.Success) _log.LogWarning("Razorpay webhook for {Order}: {Message}", orderId, r.Message);
                return Ok();           
            }
            catch (ValidationException)
            {
                return Ok();
            } 
        }
    }
}
