using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Buildoc.Services
{
	public interface IBlobStorageService
	{
		Task<string> CargarArchivo(IFormFile file);
	}

	public class BlobStorageService : IBlobStorageService
	{
		private readonly BlobServiceClient _blobServiceClient;

		public BlobStorageService(BlobServiceClient blobServiceClient)
		{
			_blobServiceClient = blobServiceClient;
		}

		public async Task<string> CargarArchivo(IFormFile file)
		{
			string mensaje = "";
			try
			{
				BlobContainerClient contenedor = _blobServiceClient.GetBlobContainerClient("archivosasp");
				await contenedor.UploadBlobAsync(file.FileName, file.OpenReadStream());
				mensaje = "se cargo";
			}
			catch (Exception ex)
			{
				mensaje = ex.Message;
			}
			return mensaje;
		}
	}
}
