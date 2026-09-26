using Manikalearning.Application.Features.Commands.Assessments.PaymentTransaction;
using Manikalearning.Web.Areas.Admin.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    [Route("Admin/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(
            string? search = null,
            string? status = "All",
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = new GetPaymentTransactionsQuery(search, status, fromDate, toDate, pageIndex, pageSize);
            var response = await _mediator.Send(query);

            if (!response.IsSuccess || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Unable to load transaction records.";
                return View(new PaymentTransactionListViewModel());
            }

            var viewModel = new PaymentTransactionListViewModel
            {
                Transactions = response.Data.Transactions,
                SearchTerm = search,
                StatusFilter = status ?? "All",
                FromDate = fromDate,
                ToDate = toDate,
                PageIndex = response.Data.PageIndex,
                PageSize = response.Data.PageSize,
                TotalCount = response.Data.TotalCount,
                TotalRevenue = response.Data.TotalRevenue
            };

            return View(viewModel);
        }

        [HttpGet("DetailsPartial/{id:guid}")]
        public async Task<IActionResult> DetailsPartial(Guid id)
        {
            var response = await _mediator.Send(new GetPaymentTransactionByIdQuery(id));
            if (!response.IsSuccess || response.Data == null)
                return BadRequest(response.Message ?? "Transaction not found.");

            return PartialView("_TransactionDetailsPartial", response.Data);
        }
    }
}
