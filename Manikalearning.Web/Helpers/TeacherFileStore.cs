namespace Manikalearning.Web.Helpers
{
    
    public static class TeacherFileStore
    {
        public static readonly string[] Images = { ".jpg", ".jpeg", ".png", ".webp" };
        public static readonly string[] Videos = { ".mp4", ".webm" };
        public static readonly string[] Docs = { ".pdf" };

        public const long MaxImage = 2L * 1024 * 1024;
        public const long MaxNotes = 25L * 1024 * 1024;
        public const long MaxVideo = 500L * 1024 * 1024;
         
        public static async Task<(string? Url, string? Error)> SaveAsync(
            IWebHostEnvironment env, IFormFile? file, string folder, string[] allowedExt, long maxBytes)
        {
            if (file == null || file.Length == 0) return (null, null);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExt.Contains(ext))
                return (null, $"Sirf {string.Join(", ", allowedExt)} files allowed hain.");
            if (file.Length > maxBytes)
                return (null, $"File {maxBytes / 1024 / 1024} MB se badi hai.");

            var root = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
            var dir = Path.Combine(root, "uploads", folder);
            Directory.CreateDirectory(dir);

            var name = $"{Guid.NewGuid():N}{ext}";
            await using (var fs = new FileStream(Path.Combine(dir, name), FileMode.Create))
                await file.CopyToAsync(fs);

            return ($"/uploads/{folder}/{name}", null);
        }
    }
}
