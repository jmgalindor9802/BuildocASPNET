using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildoc.Controllers
{
	public class FileblobStorageController : Controller
	{
		public readonly BlobServiceClient _blob;

		public FileblobStorageController(BlobServiceClient blob)
		{
			_blob = blob;
		}


		// GET: FileblobStorage
		public ActionResult Index()
		{
			return View();
		}

		public async Task<string> CargarArchivo(IFormFile file)
		{
			string mensaje = "";
			try
			{
				BlobContainerClient contenedor = _blob.GetBlobContainerClient("archivosasp");
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
