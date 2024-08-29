using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
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

  
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

            var blobInstance = containerInstance.GetBlobClient(uniqueFileName);
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
        public string GenerateDownloadLink(string fileName, string containerName)
        {
            var containerInstance = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobInstance = containerInstance.GetBlobClient(fileName);

            if (blobInstance.Exists())
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = fileName,
                    Resource = "b", // Tipo de recurso: 'b' para blob
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30) // Enlace válido por 30 minutos
                };

                // Permisos: permitir la lectura del archivo
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasUri = blobInstance.GenerateSasUri(sasBuilder);

                return sasUri.ToString(); // Devuelve la URL con el SAS token
            }

            return null; // Retorna null si el archivo no existe
        }

    }
}
