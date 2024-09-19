using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Buildoc.Data;
using Buildoc.Models.Inspecciones;
using Buildoc.Models;
using Microsoft.AspNetCore.Identity;
using Buildoc.Services;

namespace Buildoc.Controllers
{
    public class NovedadInspeccionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IFileService _fileService;
        public NovedadInspeccionController(IFileService fileService, ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
            _fileService = fileService;
        }

        // GET: NovedadInspeccion
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.NovedadInspeccion.Include(n => n.Inspeccion).Include(n => n.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: NovedadInspeccion/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var novedadInspeccion = await _context.NovedadInspeccion
                .Include(n => n.Inspeccion)
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (novedadInspeccion == null)
            {
                return NotFound();
            }

            return View(novedadInspeccion);
        }

        // GET: NovedadInspeccion/Create
        public async Task<IActionResult> Create(Guid id)
        {
            // Recuperar la inspección y la respuesta de inspección asociada
            var inspeccion = await _context.Inspeccion
                .Include(i => i.Respuesta) // Asegúrate de incluir la relación si existe
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inspeccion == null)
            {
                return NotFound();
            }

            // Pasar la inspección y la respuesta de inspección a la vista
            ViewData["Inspeccion"] = inspeccion;
            ViewData["RespuestaInspeccion"] = inspeccion.Respuesta;
            ViewData["EstadoInspeccion"] = inspeccion.Estado;
            ViewData["InspeccionId"] = id;
            return PartialView();
        }



        // POST: NovedadInspeccion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Comentario")] NovedadInspeccion novedadInspeccion, Guid idInspeccion, string actionAprobacion, bool volverRevision, List<IFormFile> archivos)
        {
            if (!_context.Inspeccion.Any(i => i.Id == idInspeccion))
            {
                ModelState.AddModelError("", "La inspección no existe.");
                return View(novedadInspeccion);
            }
            // Validar que el comentario no esté vacío
            if (string.IsNullOrWhiteSpace(novedadInspeccion.Comentario))
            {
                return Json(new { success = false, message = "El comentario no puede estar vacío." });
            }
            if (ModelState.IsValid)
            {
                // Asignar automáticamente los valores de UsuarioId e InspeccionId
                novedadInspeccion.Id = Guid.NewGuid();
                novedadInspeccion.FechaCreacion=DateTime.Now;
                novedadInspeccion.InspeccionId = idInspeccion;
                novedadInspeccion.UsuarioId = _userManager.GetUserId(User); // Usuario autenticado      

                // Obtener la inspección para actualizar su estado
                var inspeccion = await _context.Inspeccion.FindAsync(idInspeccion);
               
                if (inspeccion == null)
                {
                    return NotFound();
                }

                if (actionAprobacion == "Devolver")
                {
                    inspeccion.Estado = EstadoInspeccion.PendientesDeRevision;
                    novedadInspeccion.Estado = EstadoInspeccion.PendientesDeRevision;
                }

                else if (actionAprobacion == "Aprobar")
                {
                    inspeccion.Estado = EstadoInspeccion.Aprobada;
                    novedadInspeccion.Estado = EstadoInspeccion.Aprobada;
                }
                else if (actionAprobacion == "Desaprobar")
                {
                    inspeccion.Estado = EstadoInspeccion.Desaprobada;
                    novedadInspeccion.Estado = EstadoInspeccion.Desaprobada;

                   
                }
                else
                {
                    // Si actionAprobacion es null o no coincide con ninguna acción,
                    // se asigna el estado actual de la inspección a la novedad
                    novedadInspeccion.Estado = inspeccion.Estado;
                }

                // Manejar la subida de archivos
                if (archivos != null && archivos.Count > 0)
                {
                    foreach (var archivo in archivos)
                    {
                        if (archivo.Length > 0)
                        {
                            // Aquí puedes manejar el archivo, por ejemplo, subiéndolo a Azure Blob Storage, o guardarlo en el sistema de archivos.
                            var filePath = await _fileService.Upload(archivo, "documents");

                            if (string.IsNullOrEmpty(filePath))
                            {
                                throw new Exception("El archivo no pudo ser subido.");
                            }

                            // Guardar los metadatos del archivo
                            var fileModel = new FileModel
                            {
                                Id = Guid.NewGuid(),
                                FileName = archivo.FileName,
                                FilePath = filePath,
                                ContentType = archivo.ContentType,
                                FileSize = archivo.Length,
                                NovedadInspeccionId = novedadInspeccion.Id
                            };

                            _context.FileModels.Add(fileModel);
                        }
                    }
                }
                _context.Add(novedadInspeccion);
                await _context.SaveChangesAsync();
				

				TempData["SuccessMessage"] = "¡La novedad se ha agregado exitosamente!";
                return Json(new { success = true });
            }

            // Si el modelo no es válido, devolver errores del modelo
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();
            return Json(new { success = false, message = string.Join(" ", errors) });
        }

        // GET: NovedadInspeccion/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var novedadInspeccion = await _context.NovedadInspeccion.FindAsync(id);
            if (novedadInspeccion == null)
            {
                return NotFound();
            }
            ViewData["InspeccionId"] = new SelectList(_context.Inspeccion, "Id", "Id", novedadInspeccion.InspeccionId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", novedadInspeccion.UsuarioId);
            return View(novedadInspeccion);
        }

        // POST: NovedadInspeccion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,InspeccionId,Comentario,FechaCreacion,UsuarioId")] NovedadInspeccion novedadInspeccion)
        {
            if (id != novedadInspeccion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(novedadInspeccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NovedadInspeccionExists(novedadInspeccion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["InspeccionId"] = new SelectList(_context.Inspeccion, "Id", "Id", novedadInspeccion.InspeccionId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", novedadInspeccion.UsuarioId);
            return View(novedadInspeccion);
        }

        // GET: NovedadInspeccion/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var novedadInspeccion = await _context.NovedadInspeccion
                .Include(n => n.Inspeccion)
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (novedadInspeccion == null)
            {
                return NotFound();
            }

            return View(novedadInspeccion);
        }

        // POST: NovedadInspeccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var novedadInspeccion = await _context.NovedadInspeccion.FindAsync(id);
            if (novedadInspeccion != null)
            {
                _context.NovedadInspeccion.Remove(novedadInspeccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NovedadInspeccionExists(Guid id)
        {
            return _context.NovedadInspeccion.Any(e => e.Id == id);
        }
    }
}
