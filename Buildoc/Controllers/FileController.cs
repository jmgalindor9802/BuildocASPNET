using Buildoc.Services;
using Microsoft.AspNetCore.Mvc;
using Buildoc.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Buildoc.Data;
using Microsoft.EntityFrameworkCore;

namespace Buildoc.Controllers
{
    public class FileController : Controller
    {
       private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;
        public FileController(IFileService fileService, ApplicationDbContext context)
        {
            _fileService = fileService;
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file, Guid inspeccionId)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "No se seleccionó ningún archivo." });
            }

            // Definir el nombre del contenedor
            string containerName = "inspecciones";

            // Subir el archivo a Azure Blob Storage y obtener la URL del archivo
            var fileUrl = await _fileService.Upload(file, containerName);

            // Crear un modelo de archivo para guardar en la base de datos
            var fileModel = new FileModel
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FilePath = fileUrl,
                ContentType = file.ContentType,
                FileSize = file.Length,
                InspeccionId = inspeccionId
            };


            // Guardar los metadatos del archivo en la base de datos
            _context.FileModels.Add(fileModel);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Archivo subido exitosamente." });
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            var fileStream = await _fileService.Get(name);

            // Determinar el tipo de contenido del archivo basándote en su extensión
            string fileType = "application/octet-stream"; // Tipo de archivo por defecto
            if (name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                fileType = "image/png";
            }
            else if (name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                fileType = "image/jpeg";
            }
            // Puedes agregar más tipos de contenido si es necesario

            return File(fileStream, fileType);
        }



    }


}
