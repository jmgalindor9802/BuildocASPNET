using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Buildoc.Data;
using Buildoc.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Buildoc.Controllers
{
    public class TipoIncidentesController : Controller
    {
        private readonly ApplicationDbContext _context;
		private readonly UserManager<Usuario> _userManager;

		public TipoIncidentesController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
			_userManager = userManager;
		}

		// GET: TipoIncidentes
		[Authorize(Roles = "Administrador")]
		public async Task<IActionResult> Index()
		{

			// Obtener todos los tipos de incidentes
			var tipoIncidentes = await _context.TipoIncidentes.ToListAsync();

            // Filtrar tipos de incidentes activos para mostrar en la vista
            var tiposIncidentesActivosParaVista = tipoIncidentes
                .Where(ti => ti.Estado)
                .Select(ti => new TipoIncidenteViewModel
                {
                    Id = ti.Id,
                    Categoria = ti.Categoria.GetDescription(),
                    Titulo = ti.Titulo,
                    Descripcion = ti.Descripcion,
                    Gravedad = ti.Gravedad,
                    Estado = ti.Estado
                })
                .ToList();

            var tipoIncidentesVista = await _context.TipoIncidentes
              .Include(ti => ti.Incidentes)
              .ToListAsync();
            //Contadores
            var tipoIncidentesTotales = tipoIncidentes.Count;
            var tipoIncidentesActivos = tipoIncidentes.Count(ti => ti.Estado);
            var tipoIncidentesArchivados = tipoIncidentes.Count(ti => !ti.Estado);

            // Pasar los contadores a la vista mediante ViewBag
            ViewBag.tipoIncidentesTotales = tipoIncidentesTotales;
			ViewBag.tipoIncidentesActivos = tipoIncidentesActivos;
			ViewBag.tipoIncidentesArchivados = tipoIncidentesArchivados;

			// Retornar a la vista los tipos de incidentes activos
			return View(tipoIncidentesVista);
		}
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Activos()
        {
            // Obtener todos los tipos de incidentes
            var tipoIncidentes = await _context.TipoIncidentes
                .Include (ti => ti.Incidentes)
                .ToListAsync();
            // Filtrar tipos de incidentes activos para mostrar en la vista
            var tiposIncidentesActivosParaVista = tipoIncidentes
                .Where(ti => ti.Estado)
                .ToList();
            // Retornar a la vista los tipos de incidentes activos
            return View(tiposIncidentesActivosParaVista);
        }


        // GET: TipoIncidentes
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Archivados()
        {
            // Obtener todos los tipos de incidentes
            var tipoIncidentes = await _context.TipoIncidentes.ToListAsync();
            // Filtrar tipos de incidentes archivados para mostrar en la vista
            var tiposIncidentesActivosParaVista = tipoIncidentes.Where(ti => !ti.Estado).ToList();
            // Retornar a la vista los tipos de incidentes activos
            return View(tiposIncidentesActivosParaVista);
        }

        // GET: TipoIncidentes/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoIncidente = await _context.TipoIncidentes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoIncidente == null)
            {
                return NotFound();
            }

            return PartialView(tipoIncidente);
        }

        // GET: TipoIncidentes/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(Enum.GetValues(typeof(CategoriaEnum)).Cast<CategoriaEnum>().Select(e => new { Value = (int)e, Text = e.GetDescription() }), "Value", "Text"); 
            return PartialView();
        }

        // POST: TipoIncidentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Id,Categoria,Titulo,Descripcion,Gravedad")] TipoIncidente tipoIncidente)
        {
            if (!ModelState.IsValid)
            {
                // Obtener errores de validación
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Los datos están incompletos o inválidos. Inténtelo nuevamente", errors });
            }
            if (ModelState.IsValid)
            {
                tipoIncidente.Id = Guid.NewGuid();
                tipoIncidente.Estado = true;
				_context.Add(tipoIncidente);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡El tipo de incidente se ha creado exitosamente!";
                return Json(new { success = true });
            }
            ViewBag.Categorias = new SelectList(Enum.GetValues(typeof(CategoriaEnum)).Cast<CategoriaEnum>().Select(e => new { Value = (int)e, Text = e.GetDescription() }), "Value", "Text", tipoIncidente.Categoria);
            return PartialView(tipoIncidente);
        }

        // GET: TipoIncidentes/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoIncidente = await _context.TipoIncidentes.FindAsync(id);
            if (tipoIncidente == null)
            {
                return NotFound();
            }
            ViewBag.Categorias = new SelectList(Enum.GetValues(typeof(CategoriaEnum)).Cast<CategoriaEnum>().Select(e => new { Value = (int)e, Text = e.GetDescription() }), "Value", "Text", tipoIncidente.Categoria);
            return PartialView(tipoIncidente);
        }

        // POST: TipoIncidentes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Categoria,Titulo,Descripcion,Gravedad")] TipoIncidente tipoIncidente)
        {
            if (id != tipoIncidente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el tipo de incidente existente para mantener el estado original
                    var existingTipoIncidente = await _context.TipoIncidentes
                        .AsNoTracking() // No rastrear para evitar conflictos
                        .FirstOrDefaultAsync(ti => ti.Id == tipoIncidente.Id);
                    if (existingTipoIncidente == null)
                    {
                        return NotFound();
                    }

                    // Mantener el estado existente
                    tipoIncidente.Estado = existingTipoIncidente.Estado;

                    // Actualizar los valores del tipo de incidente
                    _context.Entry(tipoIncidente).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoIncidenteExists(tipoIncidente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "¡El tipo de incidente se ha editado exitosamente!";
                return Json(new { success = true });
            }
            ViewBag.Categorias = new SelectList(Enum.GetValues(typeof(CategoriaEnum)).Cast<CategoriaEnum>().Select(e => new { Value = (int)e, Text = e.GetDescription() }), "Value", "Text", tipoIncidente.Categoria);
            return PartialView(tipoIncidente);
        }


        // GET: TipoIncidentes/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Deactivate(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoIncidente = await _context.TipoIncidentes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoIncidente == null)
            {
                return NotFound();
            }

            return PartialView(tipoIncidente);
        }

        // POST: TipoIncidentes/Delete/5
        [HttpPost, ActionName("Deactivate")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(Guid id)
        {
            var tipoIncidente = await _context.TipoIncidentes.FindAsync(id);
            if (tipoIncidente == null)
            {
                return NotFound();
            }

            // Cambiar el estado a false en lugar de eliminar el registro
            tipoIncidente.Estado = false;
            _context.Update(tipoIncidente);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "¡El tipo de incidente se ha desactivado exitosamente!";
            return Json(new { success = true });
        }

        // GET: TipoIncidentes/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Restaurar(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoIncidente = await _context.TipoIncidentes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoIncidente == null)
            {
                return NotFound();
            }

            return PartialView(tipoIncidente);
        }

        // POST: TipoIncidentes/Delete/5
        [HttpPost, ActionName("Restaurar")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestaurarConfirm(Guid id)
        {
            var tipoIncidente = await _context.TipoIncidentes.FindAsync(id);
            if (tipoIncidente == null)
            {
                return NotFound();
            }

            // Cambiar el estado a true en lugar de eliminar el registro
            tipoIncidente.Estado = true;
            _context.Update(tipoIncidente);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "¡El tipo de incidente se ha restaurado exitosamente!";
            return Json(new { success = true });
        }

        // GET: TipoIncidentes/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoIncidente = await _context.TipoIncidentes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoIncidente == null)
            {
                return NotFound();
            }

            return PartialView(tipoIncidente);
        }

        // POST: TipoIncidentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tipoIncidente = await _context.TipoIncidentes.FindAsync(id);
            if (tipoIncidente == null)
            {
                return NotFound();
            }

            // Eliminar el tipo de incidente permanentemente
            _context.TipoIncidentes.Remove(tipoIncidente);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "¡El tipo de incidente se ha eliminado permanentemente!";
            return Json(new { success = true });
        }

        private bool TipoIncidenteExists(Guid id)
        {
            return _context.TipoIncidentes.Any(e => e.Id == id);
        }
    }
}
