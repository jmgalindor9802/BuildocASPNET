using Buildoc.Models;

namespace Buildoc.Services
{
    public interface IFileService
    {
        Task<string> Upload(IFormFile file, string containerName);

        Task<Stream> Get(string fileName);
        string GetFilePath(string fileName);
    }
}
