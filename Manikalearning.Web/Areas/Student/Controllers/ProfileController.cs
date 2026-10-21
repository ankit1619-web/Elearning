using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Users;
using Manikalearning.Application.Features.Handlers.Assessments.Users;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Manikalearning.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ProfileController : Controller
    {
        private const long MaxPhotoBytes = 2 * 1024 * 1024;
        private static readonly string[] AllowedExt = { ".jpg", ".jpeg", ".png", ".webp" };

        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _env;

        public ProfileController(IMediator mediator, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment env)
        { _mediator = mediator; _userManager = userManager; _signInManager = signInManager; _env = env; }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? status = null, string? type = "success", CancellationToken ct=default)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });
            var vm = await BuildAsync(userId.Value, null,status,type, ct); 
            return vm == null ? NotFound() : View(vm);
        }
         
        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([Bind(Prefix = "Input")] ProfileInput input, CancellationToken ct)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                await _mediator.Send(new UpdateProfileCommand(userId.Value, input.FullName ?? "", input.Mobile, input.TargetExamId, input.TargetExamDate, input.DailyGoal), ct);
                
                return RedirectToAction(nameof(Index), new { status = "Profile saved!", type = "success" }); 
            }
            catch (ValidationException ex)
            {
                foreach (var f in ex.Errors) ModelState.AddModelError($"Input.{f.PropertyName}", f.ErrorMessage);
                return View("Index", await BuildAsync(userId.Value, input, ex.Message, "error", ct));
            }
        }
         
        [HttpPost("Photo")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(3_000_000)]
        public async Task<IActionResult> Photo(IFormFile? photo, CancellationToken ct)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            if (photo == null || photo.Length == 0) return Fail("Please choose a photo.");
            if (photo.Length > MaxPhotoBytes) return Fail("The photo must be smaller than 2 MB.");

            var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
            if (!AllowedExt.Contains(ext)) return Fail("Only JPG, PNG or WEBP photos are allowed.");
             
            var head = new byte[12];
            await using (var s = photo.OpenReadStream()) { if (await s.ReadAsync(head, 0, 12, ct) < 12) return Fail("This file is not a valid photo."); }
            if (!LooksLikeImage(head)) return Fail("This file is not a valid photo.");

            var dir = Path.Combine(_env.WebRootPath, "uploads", "profile");
            Directory.CreateDirectory(dir);
            var fileName = $"{userId.Value:N}-{Guid.NewGuid():N}{ext}";                
            var full = Path.Combine(dir, fileName);

            await using (var fs = new FileStream(full, FileMode.CreateNew)) { await photo.CopyToAsync(fs, ct); }

            try
            {
                var old = await _mediator.Send(new UpdateProfileImageCommand(userId.Value, "/uploads/profile/" + fileName), ct);
                DeleteOldFile(old);
                TempData["SuccessMessage"] = "Photo updated.";
            }
            catch
            {
                System.IO.File.Delete(full);        
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Photo/Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoto(CancellationToken ct)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var old = await _mediator.Send(new UpdateProfileImageCommand(userId.Value, null), ct);
            DeleteOldFile(old); 
            return RedirectToAction(nameof(Index), new { status = "Photo removed !", type = "success" }); 
        }
         
        [HttpPost("Password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([Bind(Prefix = "Password")] ChangePasswordInput input, CancellationToken ct)
        {
            var userId = CurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var check = await new ChangePasswordInputValidator().ValidateAsync(input, ct);
            foreach (var e in check.Errors) ModelState.AddModelError($"Password.{e.PropertyName}", e.ErrorMessage);

            if (check.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userId.Value.ToString());
                if (user == null) return NotFound();

                var result = await _userManager.ChangePasswordAsync(user, input.CurrentPassword!, input.NewPassword!);
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);    
                    return RedirectToAction(nameof(Index), new { status = "Password changed !", type = "success" }); 
                }
                foreach (var e in result.Errors)
                    ModelState.AddModelError(e.Code.Contains("Mismatch") ? "Password.CurrentPassword" : "Password.NewPassword", e.Description);
            }
            return View("Index", await BuildAsync(userId.Value, null,check.Errors.FirstOrDefault().ErrorMessage,"error", ct));       
        }
         
        private IActionResult Fail(string message) { TempData["ErrorMessage"] = message; return RedirectToAction(nameof(Index)); }

        private async Task<ProfilePageVm?> BuildAsync(Guid userId, ProfileInput? input, string? status = null, string? type = "success", CancellationToken ct=default)
        {
            var p = await _mediator.Send(new GetProfileQuery(userId), ct);
            if (p == null) return null;
            return new ProfilePageVm
            {
                StatusMessage=status,
                StatusType=type,
                Profile = p,
                Input = input ?? new ProfileInput { FullName = p.FullName, Mobile = p.Mobile, TargetExamId = p.TargetExamId, TargetExamDate = p.TargetExamDate, DailyGoal = p.DailyGoal }
            };
        }

        private static bool LooksLikeImage(byte[] h) =>
            (h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF) ||                                                   
            (h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47) ||                                  
            (h[0] == 0x52 && h[1] == 0x49 && h[2] == 0x46 && h[3] == 0x46 && h[8] == 0x57 && h[9] == 0x45 && h[10] == 0x42 && h[11] == 0x50);    

        private void DeleteOldFile(string? webPath)
        {
            if (string.IsNullOrEmpty(webPath) || !webPath.StartsWith("/uploads/profile/")) return;       
            var full = Path.Combine(_env.WebRootPath, webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            try { if (System.IO.File.Exists(full)) System.IO.File.Delete(full); } catch {   }
        }

        private Guid? CurrentUserId() =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    }

    public class ProfileInput
    {
        public string? FullName { get; set; }
        public string? Mobile { get; set; }
        public Guid? TargetExamId { get; set; }
        public DateTime? TargetExamDate { get; set; }
        public int DailyGoal { get; set; } = 30;

    }

    public class ChangePasswordInput
    {
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }

    public class ChangePasswordInputValidator : AbstractValidator<ChangePasswordInput>
    {
        public ChangePasswordInputValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Enter your current password.");
            RuleFor(x => x.NewPassword).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Enter a new password.")
                .MinimumLength(8).WithMessage("The new password must have at least 8 characters.")
                .MaximumLength(100)
                .Must((x, n) => n != x.CurrentPassword).WithMessage("The new password must be different from the current one.");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword).WithMessage("The two passwords do not match.");
        }
    }

    public class ProfilePageVm
    {
        public string? StatusMessage { get; set; }
        public string? StatusType { get; set; }
        public ProfileDto Profile { get; set; } = new();
        public ProfileInput Input { get; set; } = new();
        public ChangePasswordInput Password { get; set; } = new();
    }
}
