using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Buildoc.Data;
using Buildoc.Models;
using Buildoc.Utilities;
using Buildoc.Services;

namespace Buildoc.Controllers
{
    public class TipoInspeccionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly ILogger<InspeccionesController> _logger;
        public TipoInspeccionesController(IFileService fileService, ApplicationDbContext context,ILogger<InspeccionesController> logger)
        {
            _context = context;
            _fileService = fileService;
            _logger = logger;
        }

        // GET: TipoInspecciones
        public async Task<IActionResult> Index()
        {


            return View(await _context.TipoInspeccion.ToListAsync());
        }
        public async Task<IActionResult> GetTipoInspeccionDetails(int id)
        {
            var tipoInspeccion = await _context.TipoInspeccion
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Nombre,
                    Categoria = ((CategoriaInspeccion)t.Categoria).GetDisplayName(), // Usa el método de extensión para obtener el nombre de visualización
                    t.Descripcion
                })
                .FirstOrDefaultAsync();

            Console.WriteLine("TipoInspeccion Details: " + tipoInspeccion); // Para verificar en la consola del servidor

            if (tipoInspeccion == null)
            {
                return NotFound();
            }

            return Json(tipoInspeccion);
        }


        // GET: TipoInspecciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoInspeccion = await _context.TipoInspeccion
                     .Include(t => t.Archivos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoInspeccion == null)
            {
                return NotFound();
            }

            return PartialView(tipoInspeccion);
        }

        // GET: TipoInspecciones/Create
        public IActionResult Create()
        {
            return PartialView();
        }

        // POST: TipoInspecciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Categoria,Descripcion")] TipoInspeccion tipoInspeccion, List<IFormFile> UploadedFiles)
        {
            // Verificar si ya existe un tipo de inspección con el mismo nombre
            var existingTipoInspeccion = await _context.TipoInspeccion
                .FirstOrDefaultAsync(t => t.Nombre == tipoInspeccion.Nombre);

            if (existingTipoInspeccion != null)
            {
                return Json(new { success = false, message = "Ya existe un tipo de inspección con este nombre." });
            }

            if (!ModelState.IsValid)
            {
                // Obtener errores de validación
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Los datos están incompletos o inválidos. Inténtelo nuevamente", errors });
            }

            try
            {
                // Agregar el TipoInspeccion
                _context.Add(tipoInspeccion);
                await _context.SaveChangesAsync();

                // Manejo de archivos subidos
                if (UploadedFiles != null && UploadedFiles.Count > 0)
                {
                    foreach (var file in UploadedFiles)
                    {
                        if (file.Length > 0)
                        {
                            // Cargar el archivo a Azure Blob Storage
                            var fileUrl = await _fileService.Upload(file, "documents");

                            _logger.LogInformation($"Archivo: {file.FileName}, URL: {fileUrl}");

                            // Crear el modelo de archivo y agregarlo al contexto
                            var fileModel = new FileModel
                            {
                                Id = Guid.NewGuid(),
                                TipoInspeccionId = tipoInspeccion.Id,
                                FileName = Path.GetFileName(file.FileName),
                                FilePath = fileUrl,
                                ContentType = file.ContentType,
                                FileSize = file.Length
                            };

                            _context.FileModels.Add(fileModel);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "¡El tipo de inspección se ha creado exitosamente!";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Registrar el error
                Console.WriteLine(ex.Message); // O usa un servicio de logging si lo tienes configurado
                                               // Si hubo un error en la creación de archivos, revertir el cambio en TipoInspeccion
                _context.Remove(tipoInspeccion);
                await _context.SaveChangesAsync();
                // Manejar errores y retornar un mensaje adecuado
                return Json(new { success = false, message = "Hubo un error al crear el tipo de inspección. Inténtelo nuevamente." });
            }
        }


        // GET: TipoInspecciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoInspeccion = await _context.TipoInspeccion
                   .Include(t => t.Archivos) // Incluir los archivos relacionados
                   .FirstOrDefaultAsync(t => t.Id == id);

            if (tipoInspeccion == null)
            {
                return NotFound();
            }

            // Obtener las categorías desde el enum
            var categorias = Enum.GetValues(typeof(CategoriaInspeccion))
                .Cast<CategoriaInspeccion>()
                .Select(c => new SelectListItem
                {
                    Value = ((int)c).ToString(),
                    Text = c.GetDisplayName() // Suponiendo que usas un método para obtener el nombre de visualización
                }).ToList();

            ViewBag.Categorias = categorias;

            return PartialView("Edit",tipoInspeccion);
        }

        // POST: TipoInspecciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Categoria,Descripcion")] TipoInspeccion tipoInspeccion, List<IFormFile> UploadedFiles)
        {
            if (id != tipoInspeccion.Id)
            {
                return NotFound();
            }

            // Verificar si ya existe un tipo de inspección con el mismo nombre, excepto el actual
            var existingTipoInspeccion = await _context.TipoInspeccion
                .Include(t => t.Archivos) // Incluir los archivos existentes
                .FirstOrDefaultAsync(t => t.Nombre == tipoInspeccion.Nombre && t.Id != id);

            if (existingTipoInspeccion != null)
            {
                return Json(new { success = false, message = "Ya existe un tipo de inspección con este nombre." });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar solo los datos del TipoInspeccion, no los archivos
                    _context.Entry(tipoInspeccion).Property(t => t.Nombre).IsModified = true;
                    _context.Entry(tipoInspeccion).Property(t => t.Categoria).IsModified = true;
                    _context.Entry(tipoInspeccion).Property(t => t.Descripcion).IsModified = true;
                    await _context.SaveChangesAsync();

                    // Manejo de archivos subidos
                    if (UploadedFiles != null && UploadedFiles.Count > 0)
                    {
                        foreach (var file in UploadedFiles)
                        {
                            if (file.Length > 0)
                            {
                                // Cargar el archivo a Azure Blob Storage (solo para nuevos archivos)
                                var fileUrl = await _fileService.Upload(file, "documents");

                                _logger.LogInformation($"Archivo: {file.FileName}, URL: {fileUrl}");

                                // Crear el modelo de archivo para el nuevo archivo
                                var fileModel = new FileModel
                                {
                                    Id = Guid.NewGuid(),
                                    TipoInspeccionId = tipoInspeccion.Id,
                                    FileName = Path.GetFileName(file.FileName),
                                    FilePath = fileUrl,
                                    ContentType = file.ContentType,
                                    FileSize = file.Length
                                };

                                // Agregar el nuevo archivo al contexto
                                _context.FileModels.Add(fileModel);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "¡El tipo de inspección se ha editado exitosamente!";
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoInspeccionExists(tipoInspeccion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return PartialView("Edit", tipoInspeccion);
        }


        // GET: TipoInspecciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoInspeccion = await _context.TipoInspeccion
                .Include(t => t.Inspecciones) // Asegúrate de incluir las inspecciones relacionadas
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tipoInspeccion == null)
            {
                return NotFound();
            }

            // Verificar si tiene inspecciones relacionadas
            if (tipoInspeccion.Inspecciones.Any())
            {
                TempData["DeleteMessage"] = "No se puede eliminar este tipo de inspección porque tiene inspecciones relacionadas.";
                ViewBag.PuedeEliminarse = false; // No se puede eliminar
            }
            else
            {
                TempData["DeleteMessage"] = "¿Está seguro de que desea eliminar este tipo de inspección?";
                ViewBag.PuedeEliminarse = true; // Se puede eliminar
            }

            return PartialView(tipoInspeccion);
        }



        // POST: TipoInspecciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoInspeccion = await _context.TipoInspeccion.FindAsync(id);
            if (tipoInspeccion != null)
            {
                _context.TipoInspeccion.Remove(tipoInspeccion);
            }
            TempData["SuccessMessage"] = "¡El tipo de inspección se ha eliminado exitosamente!"; 
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        private bool TipoInspeccionExists(int id)
        {
            return _context.TipoInspeccion.Any(e => e.Id == id);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteFile(Guid fileId)
        {
            var file = await _context.FileModels.FindAsync(fileId);

            if (file == null)
            {
                return Json(new { success = false, message = "Archivo no encontrado." });
            }

            try
            {
                // Eliminar el archivo del almacenamiento en la nube (Azure Blob Storage)
                await _fileService.Delete(file.FilePath);

                // Eliminar el archivo de la base de datos
                _context.FileModels.Remove(file);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Manejar el error y devolver una respuesta adecuada
                _logger.LogError($"Error al eliminar archivo: {ex.Message}");
                return Json(new { success = false, message = "Hubo un error al eliminar el archivo." });
            }
        }

    }
}
