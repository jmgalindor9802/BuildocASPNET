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
        private readonly ILogger<InspeccionesController> _logger;
        public FileController(IFileService fileService, ApplicationDbContext context, ILogger<InspeccionesController> logger)
        {
            _fileService = fileService;
            _context = context;
            _logger = logger;
        }

        // GET: File/Download/{fileName}
        public async Task<IActionResult> Download(string fileName)
        {
            var fileStream = await _fileService.Get(fileName); // Obtén el stream del archivo desde el servicio
            if (fileStream == null)
            {
                return NotFound(); // Maneja el caso en que el archivo no existe
            }

            var contentType = "application/octet-stream"; // Asume tipo genérico o puedes determinarlo dinámicamente
            return File(fileStream, contentType, fileName); // Devuelve el archivo para descarga
        }


        // GET: File/ListFiles/{containerName}
        [HttpGet]
        public async Task<IActionResult> ListFiles(string containerName)
        {
            // Implementar lógica para listar archivos si es necesario
            // Esta es una idea general, puede variar según cómo quieras mostrar los archivos

            // Ejemplo simplificado:
            // var files = await _fileService.ListFiles(containerName);
            // return View(files);

            return View(); // Retorna una vista con la lista de archivos
        }

        // POST: File/DeleteFile/{fileName}
        [HttpPost]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("El nombre del archivo no puede estar vacío.");
            }

            try
            {
                // Implementar la lógica para eliminar el archivo
                // await _fileService.Delete(fileName);

                return Json(new { success = true, message = "Archivo eliminado exitosamente." });
            }
            catch
            {
                return StatusCode(500, "Ocurrió un error al intentar eliminar el archivo.");
            }
        }
    }


}



