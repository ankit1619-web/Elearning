 
using Manikalearning.Application.Features.Commands.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manikalearning.Web.Controllers
{ 
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Register()
        { 
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }
             
            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Login", "Account");
            }
             
            if (response.Errors != null && response.Errors.Any())
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }

            return View(command);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToLocal(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message ?? "Welcome back!";
                 
                if (string.IsNullOrEmpty(command.ReturnUrl) || !Url.IsLocalUrl(command.ReturnUrl))
                {
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    if (User.IsInRole("Student"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Student" });
                    }
                    if (User.IsInRole("Teacher"))
                    {
                        return RedirectToAction("Index", "Courses", new { area = "Teacher" });
                    }
                    return RedirectToAction("Index", "Home");
                }

                return Redirect(command.ReturnUrl);
            }

            TempData["ErrorMessage"] = response.Errors.Count > 0
                ? string.Join(" | ", response.Errors)
                : response.Message;

            ViewData["ReturnUrl"] = command.ReturnUrl;
            return View(command);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var response = await _mediator.Send(new LogoutCommand());
            TempData["SuccessMessage"] = response.Message ?? "Logged out successfully.";
            return RedirectToAction(nameof(Login));
        }
         
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
        {
            var response = await _mediator.Send(command);
             
            TempData["SuccessMessage"] = response.Message ?? "If an account exists with that email, a password reset link has been sent.";
            return RedirectToAction(nameof(Login));
        }
         
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid password reset token or email.";
                return RedirectToAction(nameof(Login));
            }

            var model = new ResetPasswordCommand(email, token, string.Empty, string.Empty);
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message ?? "Password reset successfully. Please log in with your new credentials.";
                return RedirectToAction(nameof(Login));
            }

            TempData["ErrorMessage"] = response.Errors.Count > 0
                ? string.Join(" | ", response.Errors)
                : response.Message;

            return View(command);
        }
         
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
        {
            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message ?? "Your password has been changed successfully.";
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = response.Errors.Count > 0
                ? string.Join(" | ", response.Errors)
                : response.Message;

            return View(command);
        }
         
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
         
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
    }
}