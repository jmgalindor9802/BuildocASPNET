using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Buildoc.Models;
using File = Buildoc.Models.FileModel;

namespace Buildoc.Services
{
    public class FileService:IFileService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public FileService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> Upload(IFormFile file, string containerName)  // Cambié el nombre a Upload para coincidir con la interfaz
        {
            var containerInstance = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerInstance.CreateIfNotExistsAsync();

            var blobInstance = containerInstance.GetBlobClient(file.FileName);
            await blobInstance.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });

            return blobInstance.Uri.ToString(); // Devuelve la URL del archivo subido
        }

        public async Task<Stream> Get(string fileName)  // Cambié el nombre a Get para coincidir con la interfaz
        {
            var containerInstance = _blobServiceClient.GetBlobContainerClient("documents");
            var blobInstance = containerInstance.GetBlobClient(fileName);
            var downloadContent = await blobInstance.DownloadAsync();
            return downloadContent.Value.Content;
        }


    }
}
