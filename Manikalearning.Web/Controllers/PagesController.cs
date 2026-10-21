using FluentValidation;
using Manikalearning.Application.Features.Handlers.Assessments.Users;
using Manikalearning.Application.Interface;
using Manikalearning.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{
    [AllowAnonymous]
    public class PagesController : Controller
    {
        private readonly IMediator _mediator;
        public PagesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("about-us")]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet("contact-us")]
        public IActionResult Contact() => View(new ContactFormVm());

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContact(ContactFormVm vm, CancellationToken ct)
        { 
            if (!string.IsNullOrEmpty(vm.Website))
            {
                TempData["ContactSuccess"] = "Thank you! Your message has been sent. We will get back to you soon.";
                return RedirectToAction(nameof(Contact));
            }

            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var result = await _mediator.Send(new SubmitContactCommand(vm.Name, vm.Email, vm.Subject, vm.Message, ip), ct);

                if (result.IsSuccess)
                {
                    TempData["ContactSuccess"] = result.Message;
                    return RedirectToAction(nameof(Contact)); 
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "We could not send your message.");
            }
            catch (ValidationException ex)
            {
                foreach (var e in ex.Errors) ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
            }

            return View("Contact", vm);   
        }

        [HttpGet("careers")]
        public IActionResult Careers()
        {
            return View();
        }

        [HttpGet("faqs")]
        public IActionResult Faq()
        {
            return View();
        }

        [HttpGet("privacy-policy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("terms-and-conditions")]
        public IActionResult Terms()
        {
            return View();
        }

        [HttpGet("refund-policy")]
        public IActionResult Refund()
        {
            return View();
        }
    }
}