using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Buildoc.Data;
using Buildoc.Models;
using Microsoft.EntityFrameworkCore;
using File = Buildoc.Models.FileModel;

namespace Buildoc.Services
{
    public class FileService:IFileService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ApplicationDbContext _dbContext;
        public FileService(BlobServiceClient blobServiceClient, ApplicationDbContext applicationDbContext)
        {
            _blobServiceClient = blobServiceClient;
            _dbContext = applicationDbContext;
        }

        public async Task<string> Upload(IFormFile file, string containerName)
        {
            // Obtiene el cliente del contenedor de blobs
            var containerInstance = _blobServiceClient.GetBlobContainerClient(containerName);

            // Crea el contenedor si no existe
            await containerInstance.CreateIfNotExistsAsync(PublicAccessType.Blob); // Configura el contenedor para acceso público

            // Genera un nombre único para el archivo
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

            // Obtiene el cliente del blob
            var blobInstance = containerInstance.GetBlobClient(uniqueFileName);

            // Sube el archivo al blob
            await blobInstance.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });

            // Retorna la URL pública del archivo subido
            return blobInstance.Uri.ToString();
        }


        public async Task<Stream> Get(string fileName)  // Cambié el nombre a Get para coincidir con la interfaz
        {
            var containerInstance = _blobServiceClient.GetBlobContainerClient("documents");
            var blobInstance = containerInstance.GetBlobClient(fileName);
            var downloadContent = await blobInstance.DownloadAsync();
            return downloadContent.Value.Content;
        }
        public string GetFilePath(string fileName)
        {
            // Implementa la lógica para obtener el FilePath usando el fileName
            // Esto puede incluir buscar el archivo en una base de datos o algún otro repositorio

            // Ejemplo: Buscar en una base de datos o estructura que contenga el FilePath
            var file = _dbContext.FileModels.FirstOrDefault(f => f.FileName == fileName);

            return file?.FilePath; // Devuelve la ruta del archivo
        }


    }
}
