using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Buildoc.Data;
using Buildoc.Models;

namespace Buildoc.Controllers
{
    public class TipoInspeccionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoInspeccionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoInspecciones
        public async Task<IActionResult> Index()
        {


            return View(await _context.TipoInspeccion.ToListAsync());
        }

        // GET: TipoInspecciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoInspeccion = await _context.TipoInspeccion
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
        public async Task<IActionResult> Create([Bind("Id,Nombre,Categoria,Descripcion")] TipoInspeccion tipoInspeccion)
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


            if (ModelState.IsValid)
            {

                _context.Add(tipoInspeccion);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡El tipo de inspección se ha creado exitosamente!";
                return Json(new { success = true });
            }
            return PartialView("Create",tipoInspeccion);
        }

        // GET: TipoInspecciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoInspeccion = await _context.TipoInspeccion.FindAsync(id);
            if (tipoInspeccion == null)
            {
                return NotFound();
            }
            return PartialView("Edit",tipoInspeccion);
        }

        // POST: TipoInspecciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Categoria,Descripcion")] TipoInspeccion tipoInspeccion)
        {
            if (id != tipoInspeccion.Id)
            {
                return NotFound();
            }
            // Verificar si ya existe un tipo de inspección con el mismo nombre, excepto el actual
            var existingTipoInspeccion = await _context.TipoInspeccion
                .FirstOrDefaultAsync(t => t.Nombre == tipoInspeccion.Nombre && t.Id != id);

            if (existingTipoInspeccion != null)
            {
                return Json(new { success = false, message = "Ya existe un tipo de inspección con este nombre." });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoInspeccion);
                    await _context.SaveChangesAsync();
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
                TempData["SuccessMessage"] = "¡El proyecto se ha editado exitosamente!";
                return Json(new { success = true });
            }
            return PartialView("Edit",tipoInspeccion);
        }

        // GET: TipoInspecciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoInspeccion = await _context.TipoInspeccion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoInspeccion == null)
            {
                return NotFound();
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
            TempData["SuccessMessage"] = "¡El proyecto se ha archivado exitosamente!"; 
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        private bool TipoInspeccionExists(int id)
        {
            return _context.TipoInspeccion.Any(e => e.Id == id);
        }
    }
}
