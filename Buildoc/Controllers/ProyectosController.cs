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
    public class ProyectosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProyectosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Proyectos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Proyectos.ToListAsync());
        }

        // GET: Proyectos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyectos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (proyecto == null)
            {
                return NotFound();
            }

            return PartialView("Details",proyecto);
        }

        // GET: Proyectos/Create
        public IActionResult Create()
        {
            return PartialView();
        }

        // POST: Proyectos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Departamento,Municipio,Direccion,Cliente")] Proyecto proyecto)
        {
            // Verificar si ya existe un proyecto con el mismo nombre
            var existingProyecto = await _context.Proyectos.FirstOrDefaultAsync(p => p.Nombre == proyecto.Nombre);

            if (existingProyecto != null)
            {
                return Json(new { success = false, message = "Ya existe un proyecto con este nombre." });
            }
            if (ModelState.IsValid)
            {
                proyecto.Id = Guid.NewGuid();
                _context.Add(proyecto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡El proyecto se ha creado exitosamente!";
                return Json(new { success = true });
            }
            return PartialView("Create", proyecto);
        }

        // GET: Proyectos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return PartialView("Edit",proyecto);
        }

        // POST: Proyectos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nombre,Descripcion,Departamento,Municipio,Direccion,Cliente")] Proyecto proyecto)
        {
            if (id != proyecto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proyecto);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "¡El proyecto se ha editado exitosamente!";
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProyectoExists(proyecto.Id))
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
            return PartialView("Edit",proyecto);
        }

        // GET: Proyectos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyectos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (proyecto == null)
            {
                return NotFound();
            }

            return PartialView("Delete",proyecto);
        }

        // POST: Proyectos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto != null)
            {
                _context.Proyectos.Remove(proyecto);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "¡El proyecto se ha eliminado exitosamente!";
            return Json(new { success = true });
        }

        private bool ProyectoExists(Guid id)
        {
            return _context.Proyectos.Any(e => e.Id == id);
        }
    }
}
