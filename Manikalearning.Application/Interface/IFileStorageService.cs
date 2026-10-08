using Microsoft.AspNetCore.Http;

namespace Manikalearning.Application.Interface
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string fileType);
        byte[] GetFileBytes(string fileName, string fileType);
        string GetFilePath(string fileName, string fileType);
    }
}
