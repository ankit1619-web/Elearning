using Manikalearning.Application.Interface;

namespace Manikalearning.Web.Helpers
{

    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _baseStoragePath;
        private readonly Dictionary<string, string[]> _allowedExtensions = new()
        {
            { "Pdfs", new[] { ".pdf" } },
            { "Videos", new[] { ".mp4", ".mkv", ".webm" } }
        };

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            _baseStoragePath = Path.Combine(env.ContentRootPath, "SecureUploads");
        }

        public async Task<string> SaveFileAsync(IFormFile file, string fileType)  
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be empty.");

            if (!_allowedExtensions.ContainsKey(fileType))
                throw new ArgumentException("Invalid file category.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions[fileType].Contains(ext))
                throw new InvalidOperationException($"Invalid extension {ext} for {fileType}.");
             
            var targetFolder = Path.Combine(_baseStoragePath, fileType);
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }
             
            var uniqueFileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(targetFolder, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;  
        }

        public byte[] GetFileBytes(string fileName, string fileType)
        {
            var fullPath = GetFilePath(fileName, fileType);
            if (!File.Exists(fullPath)) return null;

            return File.ReadAllBytes(fullPath);
        }

        public string GetFilePath(string fileName, string fileType)
        {
            fileName = fileName
        .Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
        .FirstOrDefault(s => s.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
                             s.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ||
                             s.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase) ||
                             s.EndsWith(".webm", StringComparison.OrdinalIgnoreCase));

            // Fallback agar extension match na ho
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Path.GetFileName(fileName.TrimEnd('/', '\\'));
            }
            return Path.Combine(_baseStoragePath, fileType, fileName);
        }
    }
}
