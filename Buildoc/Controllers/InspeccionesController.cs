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
    public class InspeccionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InspeccionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inspecciones
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Inspeccion.Include(i => i.Inspector).Include(i => i.Proyecto).Include(i => i.TipoInspeccion);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Inspecciones/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspeccion = await _context.Inspeccion
                .Include(i => i.Inspector)
                .Include(i => i.Proyecto)
                .Include(i => i.TipoInspeccion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspeccion == null)
            {
                return NotFound();
            }

            return View(inspeccion);
        }

        // GET: Inspecciones/Create
        public IActionResult Create()
        {
            ViewData["InspectorId"] = new SelectList(_context.Users, "Id", "Nombres");
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Nombre");
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Nombre");
            return View();
        }

        // POST: Inspecciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaInspeccion,Objetivo,Descripcion,TipoInspeccionId,ProyectoId,InspectorId,Resultado,Estado")] Inspeccion inspeccion)
        {
            if (ModelState.IsValid)
            {
                inspeccion.Id = Guid.NewGuid();
                inspeccion.Estado = "Programada";
                _context.Add(inspeccion);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡La inspección se ha creado exitosamente!";
                return Json(new { success = true });
            }
            ViewData["InspectorId"] = new SelectList(_context.Users, "Id", "Id", inspeccion.InspectorId);
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Departamento", inspeccion.ProyectoId);
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Descripcion", inspeccion.TipoInspeccionId);
            return PartialView("Create",inspeccion);
        }

        // GET: Inspecciones/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspeccion = await _context.Inspeccion.FindAsync(id);
            if (inspeccion == null)
            {
                return NotFound();
            }
            ViewData["InspectorId"] = new SelectList(_context.Users, "Id", "Id", inspeccion.InspectorId);
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Departamento", inspeccion.ProyectoId);
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Descripcion", inspeccion.TipoInspeccionId);
            return View(inspeccion);
        }

        // POST: Inspecciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FechaInspeccion,Objetivo,Descripcion,TipoInspeccionId,ProyectoId,InspectorId,Resultado,Estado")] Inspeccion inspeccion)
        {
            if (id != inspeccion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspeccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspeccionExists(inspeccion.Id))
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
            ViewData["InspectorId"] = new SelectList(_context.Users, "Id", "Id", inspeccion.InspectorId);
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Departamento", inspeccion.ProyectoId);
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Descripcion", inspeccion.TipoInspeccionId);
            return View(inspeccion);
        }

        // GET: Inspecciones/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspeccion = await _context.Inspeccion
                .Include(i => i.Inspector)
                .Include(i => i.Proyecto)
                .Include(i => i.TipoInspeccion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspeccion == null)
            {
                return NotFound();
            }

            return View(inspeccion);
        }

        // POST: Inspecciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inspeccion = await _context.Inspeccion.FindAsync(id);
            if (inspeccion != null)
            {
                _context.Inspeccion.Remove(inspeccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspeccionExists(Guid id)
        {
            return _context.Inspeccion.Any(e => e.Id == id);
        }
    }
}
